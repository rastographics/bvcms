using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using System;
using System.Data.SqlClient;
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

#if DEBUG
        [HttpGet, Route("~/Test/{id?}")]
        public ActionResult Test(int? id)
        {
            EmailReplacements.ReCodes();
            return Content("no test");
        }
        [HttpGet, Route("~/Warmup")]
        public ActionResult Warmup()
        {
            return View();
        }
#endif

        public ActionResult RecordTest(int id, string v)
        {
            var o = CurrentDatabase.LoadOrganizationById(id);
            o.AddEditExtra(CurrentDatabase, "tested", v);
            CurrentDatabase.SubmitChanges();
            return Content(v);
        }

        //todo: use injection
#if DEBUG
        [HttpGet, Route("~/TestScript")]
        [Authorize(Roles = "Developer")]
        public ActionResult TestScript()
        {
            //var id = DbUtil.Db.ScratchPadQuery(@"MemberStatusId = 10[Member] AND LastName = 'C*'");

            var file = Server.MapPath("~/test.py");
            var logFile = $"RunPythonScriptInBackground.{DateTime.Now:yyyyMMddHHmmss}";
            string host = Util.Host;

#if false
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var pe = new PythonModel(host);
                //pe.DictionaryAdd("OrgId", "89658");
                pe.DictionaryAdd("LogFile", logFile);
                PythonModel.ExecutePythonFile(file, pe);
            });
            return View("RunPythonScriptProgress");
#else
            var pe = new PythonModel(host);
            pe.DictionaryAdd("LogFile", logFile);
            ViewBag.Text = PythonModel.ExecutePython(file, pe, fromFile: true);
            return View("Test");
#endif
        }
        [HttpPost, Route("~/TestScript")]
        [ValidateInput(false)]
        [Authorize(Roles = "Developer")]
        public ActionResult TestScript(string script)
        {
            return Content(PythonModel.RunScript(Util.Host, script));
        }
#endif


        [HttpGet, Route("~/RunScript/{name}/{parameter?}/{title?}")]
        public ActionResult RunScript(string name, string parameter = null, string title = null)
        {
            var content = CurrentDatabase.ContentOfTypeSql(name);
            if (content == null)
            {
                return Content("no content");
            }

            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
            {
                p.Add("@" + kv.Key, kv.Value);
            }

            string script = ScriptModel.RunScriptSql(parameter, content, p, ViewBag);

            if (script.StartsWith("Not Authorized"))
            {
                return Message(script);
            }

            ViewBag.Report = name;
            ViewBag.Name = title ?? $"{name.SpaceCamelCase()} {parameter}";
            if (script.Contains("pagebreak"))
            {
                ViewBag.report = PythonModel.PageBreakTables(CurrentDatabase, script, p);
                return View("RunScriptPageBreaks");
            }
            ViewBag.Url = Request.Url?.PathAndQuery;
            var rd = cn.ExecuteReader(script, p, commandTimeout: 1200);
            ViewBag.ExcelUrl = Request.Url?.AbsoluteUri.Replace("RunScript/", "RunScriptExcel/");
            return View(rd);
        }

        [HttpGet, Route("~/RunScriptExcel/{scriptname}/{parameter?}")]
        public ActionResult RunScriptExcel(string scriptname, string parameter = null)
        {
            var content = CurrentDatabase.ContentOfTypeSql(scriptname);
            if (content == null)
            {
                return Message("no content");
            }

            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
            {
                p.Add("@" + kv.Key, kv.Value);
            }

            string script = ScriptModel.RunScriptSql(parameter, content, p, ViewBag);
            if (script.StartsWith("Not Authorized"))
            {
                return Message(script);
            }

            return cn.ExecuteReader(script, p, commandTimeout: 1200).ToExcel("RunScript.xlsx", fromSql: true);
        }

        [HttpGet, Route("~/PyScript/{name}")]
        public ActionResult PyScript(string name, string p1, string p2, string v1, string v2)
        {
#if DEBUG
#else
            try
            {
#endif
            var script = CurrentDatabase.ContentOfTypePythonScript(name);
            if (!script.HasValue())
            {
                return Message("no script named " + name);
            }

            if (!ScriptModel.CanRunScript(script))
            {
                return Message("Not Authorized to run this script");
            }

            if (Regex.IsMatch(script, @"model\.Form\b"))
            {
                return Redirect("/PyScriptForm/" + name);
            }

            script = script.Replace("@P1", p1 ?? "NULL")
                    .Replace("@P2", p2 ?? "NULL")
                    .Replace("V1", v1 ?? "None")
                    .Replace("V2", v2 ?? "None");
            if (script.Contains("@qtagid"))
            {
                var id = CurrentDatabase.FetchLastQuery().Id;
                var tag = CurrentDatabase.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                script = script.Replace("@qtagid", tag.Id.ToString());
            }

            ViewBag.report = name;
            ViewBag.url = Request.Url?.PathAndQuery;
            if (script.Contains("Background Process Completed"))
            {
                var logFile = $"RunPythonScriptInBackground.{DateTime.Now:yyyyMMddHHmmss}";
                ViewBag.LogFile = logFile;
                var qs = Request.Url?.Query;
                var host = Util.Host;

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
            var pe = new PythonModel(CurrentDatabase); 
            if (script.Contains("@BlueToolbarTagId"))
            {
                var id = CurrentDatabase.FetchLastQuery().Id;
                pe.DictionaryAdd("BlueToolbarGuid", id.ToCode());
            }

            foreach (var key in Request.QueryString.AllKeys)
            {
                pe.DictionaryAdd(key, Request.QueryString[key]);
            }

            pe.Output = ScriptModel.Run(name, pe);
            if (pe.Output.StartsWith("REDIRECT="))
            {
                var a = pe.Output.SplitStr("=", 2);
                return Redirect(a[1].TrimEnd());
            }

            return View(pe);
#if DEBUG
#else
        }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
#endif
        }
        [HttpPost, Route("~/RunPythonScriptProgress2")]
        public ActionResult RunPythonScriptProgress2(string logfile)
        {
            var txt = CurrentDatabase.ContentOfTypeText(logfile);
            return Content(txt);
        }
        [Route("~/PyScriptForm/{name}")]
        public ActionResult PyScriptForm(string name)
        {
            return Request.HttpMethod.ToUpper() == "GET"
                ? PyScriptFormGet(name)
                : PyScriptFormPost(name);
        }

        private ActionResult PyScriptFormGet(string name)
        {
            try
            {
                var pe = new PythonModel(Util.Host);
                foreach (var key in Request.QueryString.AllKeys)
                {
                    pe.DictionaryAdd(key, Request.QueryString[key]);
                }

                pe.Data.pyscript = name;
                pe.HttpMethod = "get";
                ScriptModel.Run(name, pe);
                return View(pe);
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

        private ActionResult PyScriptFormPost(string name)
        {
            try
            {
                var pe = new PythonModel(Util.Host);
                ScriptModel.GetFilesContent(pe);
                foreach (var key in Request.Form.AllKeys)
                {
                    pe.DictionaryAdd(key, Request.Form[key]);
                }

                pe.HttpMethod = "post";

                var ret = ScriptModel.Run(name, pe);
                if (ret.StartsWith("REDIRECT="))
                {
                    return Redirect(ret.Substring(9).Trim());
                }

                return Content(ret);
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

        [HttpPost, Route("~/PyScriptForm")]
        public ActionResult PyScriptForm()
        {
            try
            {
                var pe = new PythonModel(Util.Host);
                foreach (var key in Request.Form.AllKeys)
                {
                    pe.DictionaryAdd(key, Request.Form[key]);
                }

                pe.HttpMethod = "post";

                var script = CurrentDatabase.ContentOfTypePythonScript(pe.Data.pyscript);
                return Content(pe.RunScript(script));
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

    }

}
