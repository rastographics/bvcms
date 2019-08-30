using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class ScriptController : CmsStaffController
    {
        public ScriptController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/PythonSearch/Names")]
        public ActionResult PythonSearchNames(string term)
        {
            var m = new PythonScriptModel(CurrentDatabase);
            var n = m.PythonSearch(term, 10).ToArray();
            return Json(n, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("~/RunScript/{name}/{parameter?}/{title?}")]
        public ActionResult RunScript(string name, string parameter = null, string title = null)
        {
            var m = new SqlScriptModel(CurrentDatabase);
            var sql = m.FetchScript(name);
            if (sql == null)
            {
                return Message("no sql script named " + name);
            }
            if (!SqlScriptModel.CanRunScript(sql))
            {
                return Message("Not Authorized to run this script");
            }
            var p = m.FetchParameters();

            ViewBag.Report = name;
            ViewBag.Name = title ?? $"{name.SpaceCamelCase()} {parameter}";
            if (sql.Contains("pagebreak"))
            {
                ViewBag.report = PythonModel.PageBreakTables(CurrentDatabase, sql, p);
                return View("RunScriptPageBreaks");
            }
            ViewBag.Url = Request.Url?.PathAndQuery;

            string html;

            using (var cn = CurrentDatabase.ReadonlyConnection())
            {
                cn.Open();
                var rd = cn.ExecuteReader(sql, p, commandTimeout: 1200);
                ViewBag.ExcelUrl = Request.Url?.AbsoluteUri.Replace("RunScript/", "RunScriptExcel/");
                html = GridResult.Table(rd, ViewBag.Name2);
            }
            return View(new HtmlHolder { html = html });
        }

        [HttpGet, Route("~/RunScriptExcel/{scriptname}/{parameter?}")]
        public ActionResult RunScriptExcel(string scriptname, string parameter = null)
        {
            var model = new SqlScriptModel(CurrentDatabase);
            var content = CurrentDatabase.ContentOfTypeSql(scriptname);
            if (content == null)
            {
                return Message("no content");
            }

            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
            {
                p.Add("@" + kv.Key, kv.Value);
            }

            string script = model.AddParametersForSql(parameter, content, p, ViewBag);
            if (script.StartsWith("Not Authorized"))
            {
                return Message(script);
            }

            using (var cn = CurrentDatabase.ReadonlyConnection())
            {
                cn.Open();
                return cn.ExecuteReader(script, p, commandTimeout: 1200).ToExcel("RunScript.xlsx", fromSql: true);
            }
        }

        [HttpGet, Route("~/PyScript/{name}/{p1?}/{p2?}")]
        public ActionResult PyScript(string name, string p1, string p2, string v1, string v2)
        {
            var m = new PythonScriptModel(CurrentDatabase);
            var script = m.FetchScript(name);

            if (script == null)
            {
                return Message("no python script named " + name);
            }
            if (!PythonScriptModel.CanRunScript(script))
            {
                return Message("Not Authorized to run this script");
            }
            if (Regex.IsMatch(script, @"model\.Form\b"))
            {
                return Redirect("/PyScriptForm/" + name);
            }
            script = m.ReplaceParametersInScript(script, p1, p2, v1, v2);

            ViewBag.report = name;
            ViewBag.url = Request.Url?.PathAndQuery;

            if (script.Contains("Background Process Completed"))
            {
                return RunProgressInBackground(script);
            }
#if DEBUG
#else
            try
            {
#endif
                var ret = m.RunPythonScript(script, p1, p2);
                m.pythonModel.Output = ret;
                if (m.pythonModel.Output.StartsWith("REDIRECT="))
                {
                    var a = m.pythonModel.Output.SplitStr("=", 2);
                    return Redirect(a[1].TrimEnd());
                }
                return View(m.pythonModel);
#if DEBUG
#else
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
#endif
        }

        private ActionResult RunProgressInBackground(string script)
        {
            var logFile = $"RunPythonScriptInBackground.{DateTime.Now:yyyyMMddHHmmss}";
            ViewBag.LogFile = logFile;
            var qs = Request.Url?.Query;
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var qsa = HttpUtility.ParseQueryString(qs ?? "");
                var pm = new PythonModel(CurrentDatabase);
                pm.DictionaryAdd("LogFile", logFile);
                foreach (string key in qsa)
                {
                    pm.DictionaryAdd(key, qsa[key]);
                }

                string result = pm.RunScript(script);
                if (result.HasValue())
                {
                    pm.LogToContent(logFile, result);
                }
            });
            return View("RunPythonScriptProgress");
        }

        [HttpPost, Route("~/RunPythonScriptProgress2")]
        public ActionResult RunPythonScriptProgress2(string logfile)
        {
            var txt = CurrentDatabase.ContentOfTypeText(logfile);
            return Content(txt);
        }

        [HttpGet, Route("~/PyScriptForm/{name}/{p1?}/{p2?}")]
        public ActionResult PyScriptForm(string name, string p1 = null, string p2 = null)
        {
#if DEBUG
#else
            try
            {
#endif
                var m = new PythonScriptModel(CurrentDatabase);
                var script = m.FetchScript(name);
                m.pythonModel.HttpMethod = "get";
                m.RunPythonScript(script, p1, p2);
                return View(m.pythonModel);
#if DEBUG
#else
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
#endif
        }

        [HttpPost, Route("~/PyScriptForm/{name?}")]
        public ActionResult PyScriptForm(string name)
        {
            try
            {
                var model = new PythonScriptModel(CurrentDatabase);
                var script = model.FetchScript(name ?? Request.Form["pyscript"]);
                model.PrepareHttpPost();

                var ret = model.RunPythonScript(script);
                if (ret.StartsWith("REDIRECT="))
                {
                    return Redirect(ret.Substring(9).Trim());
                }
                return Content(ret);
            }
            catch (Exception ex)
            {
                return Content($@"<div class='alert alert-danger'>{ex.Message}</div></div>");
            }
        }
    }

}
