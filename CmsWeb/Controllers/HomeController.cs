using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CmsData;
using CmsData.OnlineRegSummaryText;
using CmsData.Registration;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Areas.People.Models;
using CmsWeb.Areas.Search.Models;
using Dapper;
using Newtonsoft.Json;
using UtilityExtensions;
using CmsWeb.Models;
using HandlebarsDotNet;
using HtmlAgilityPack;

namespace CmsWeb.Controllers
{
    public class HomeController : CmsStaffController
    {
        public ActionResult Index()
        {
            if (!Util2.OrgLeadersOnly && User.IsInRole("OrgLeadersOnly"))
            {
                Util2.OrgLeadersOnly = true;
                DbUtil.Db.SetOrgLeadersOnly();
            }
            var m = new HomeModel();
            return View(m);
        }

        [ValidateInput(false)]
        public ActionResult ShowError(string error, string url)
        {
            ViewData["error"] = Server.UrlDecode(error);
            ViewData["url"] = url;
            return View();
        }

        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset();
            qb.Save(DbUtil.Db);
            return Redirect("/Query");
        }

#if DEBUG
        [HttpGet, Route("~/Test2")]
        public ActionResult Test2(string id)
        {
            var xml = @"
<Condition Id=""ed6b4ad9-2933-45ce-943b-e008235743b6"" Order=""0"" Field=""Group"" Comparison=""AllTrue"" Description=""scratchpad"" PreviousName=""scratchpad"" OnlineReg=""0"" OrgStatus=""0"" OrgType2=""0"">
<Condition Id=""2d5abc6c-3f02-4721-9e27-a629f3c4e16d"" Order=""2"" Field=""StatusFlag"" Comparison=""Equal"" CodeIdValue=""F40"" OnlineReg=""0"" OrgStatus=""0"" OrgType2=""0""/>
<Condition Id=""b857b40f-0662-45d6-b16c-746c5b7966f1"" Order=""4"" Field=""VolunteerProcessedDateMonthsAgo"" Comparison=""GreaterEqual"" TextValue=""23"" OnlineReg=""0"" OrgStatus=""0"" OrgType2=""0""/>
</Condition>";
            var c = Condition.Import(xml);
            var s = c.ToCode();
            var cc = QueryParser.Parse(s);

            return Content("done");
        }

        [HttpGet, Route("~/Test")]
        public ActionResult Test(string id)
        {
            var m = DbUtil.Db.Connection.Query(CmsWeb.Areas.Search.Controllers.SavedQueryController.SqlSavedqueries);
            foreach (var q in m)
            {
                var g = q.QueryId as Guid?;
                if (!g.HasValue)
                    continue;
                var c = DbUtil.Db.LoadExistingQuery(g.Value);
                var s = c.ToCode();
                if (!s.HasValue())
                    continue;
                try
                {
                    QueryParser.Parse(s);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{g.Value}, {ex.Message}");
                }
                break;
            }
            return Content("done");
        }
#endif

        public ActionResult RecordTest(int id, string v)
        {
            var o = DbUtil.Db.LoadOrganizationById(id);
            o.AddEditExtra(DbUtil.Db, "tested", v);
            DbUtil.Db.SubmitChanges();
            return Content(v);
        }

        public ActionResult NthTimeAttenders(int id)
        {
            var name = "VisitNumber-" + id;
            var q = DbUtil.Db.Queries.FirstOrDefault(qq => qq.Owner == "System" && qq.Name == name);
            if (q != null)
                return Redirect("/Query/" + q.QueryId);

            const CompareType comp = CompareType.Equal;
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();
            Condition c;
            switch (id)
            {
                case 1:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,True");
                    c.Quarters = "1";
                    c.Days = 7;
                    break;
                case 2:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,True");
                    c.Quarters = "2";
                    c.Days = 7;
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "0,False");
                    c.Quarters = "1";
                    c.Days = 7;
                    break;
                case 3:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,True");
                    c.Quarters = "3";
                    c.Days = 7;
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "0,False");
                    c.Quarters = "2";
                    c.Days = 7;
                    break;
            }
            cc.Description = name;
            cc.Save(DbUtil.Db, owner: "System");
            TempData["autorun"] = true;
            return Redirect("/Query/" + cc.Id);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ActiveRecords(DateTime? dt)
        {
            if (dt.HasValue)
            {
                TempData["ActiveRecords"] = DbUtil.Db.ActiveRecordsdt(dt.Value);
                TempData["ActiveRecords2"] = DbUtil.Db.ActiveRecords2dt(dt.Value);
            }
            else
            {
                TempData["ActiveRecords"] = DbUtil.Db.ActiveRecords();
                TempData["ActiveRecords2"] = DbUtil.Db.ActiveRecords2();
            }
            return View("Support2");
        }

        public ActionResult TargetPerson(bool id)
        {
            DbUtil.Db.SetUserPreference("TargetLinkPeople", id ? "false" : "true");
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }
        public ActionResult TargetOrg(bool id)
        {
            DbUtil.Db.SetUserPreference("TargetLinkOrg", id ? "false" : "true");
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }
        public ActionResult UseNewFeature(bool id)
        {
            Util2.UseNewFeature = !id;
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }
        public ActionResult UseNewDetails(bool id)
        {
            Util2.UseNewDetails = !id;
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }
        public ActionResult UseNewEditor(bool id)
        {
            DbUtil.Db.SetUserPreference("UseNewEditor3", id ? "false" : "true");
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }

        public ActionResult Names(string term)
        {
            var q = HomeModel.Names(term).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("~/FastSearch")]
        public ActionResult FastSearch(string q)
        {
            var qq = HomeModel.FastSearch(q).ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        [HttpGet, Route("~/FastSearchPrefetch")]
        public ActionResult FastSearchPrefetch()
        {
            Response.NoCache();
            var qq = HomeModel.PrefetchSearch().ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        public ActionResult SwitchTag(string tag)
        {
            var m = new TagsModel { tag = tag };
            m.SetCurrentTag();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            return Redirect("/");
        }

        [HttpGet, Route("~/TestScript")]
        [Authorize(Roles = "Developer")]
        public ActionResult TestScript()
        {
            return View();
        }

        [HttpPost, Route("~/TestScript")]
        [ValidateInput(false)]
        [Authorize(Roles = "Developer")]
        public ActionResult TestScript(string script)
        {
            return Content(PythonEvents.RunScript(Util.Host, script));
        }

        private string RunScriptSql(CMSDataContext db, string parameter, string body, DynamicParameters p)
        {
            if (!CanRunScript(body))
                return "Not Authorized to run this script";
            if (body.Contains("@qtagid"))
            {
                var id = db.FetchLastQuery().Id;
                var tag = db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                int? qtagid = tag.Id;
                p.Add("@qtagid", qtagid);
            }
            p.Add("@p1", parameter ?? "");
            return body;
        }

        [HttpGet, Route("~/RunScript/{name}/{parameter?}/{title?}")]
        public ActionResult RunScript(string name, string parameter = null, string title = null)
        {
            var content = DbUtil.Db.ContentOfTypeSql(name);
            if (content == null)
                return Content("no content");
            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
                p.Add("@" + kv.Key, kv.Value);
            var script = RunScriptSql(DbUtil.Db, parameter, content.Body, p);

            if (script.StartsWith("Not Authorized"))
                return Message(script);
            ViewBag.name = title ?? $"Run Script {name} {parameter}";
            var rd = cn.ExecuteReader(script, p, commandTimeout: 1200);
            return View(rd);
        }

        private bool CanRunScript(string script)
        {
            if (!script.StartsWith("--Roles="))
                return true;
            var re = new Regex("--Roles=(?<roles>.*)");
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim());
            if (!roles.Any(rr => User.IsInRole(rr)))
                return false;
            return true;
        }

        [HttpGet, Route("~/RunScriptExcel/{scriptname}/{parameter?}")]
        public ActionResult RunScriptExcel(string scriptname, string parameter = null)
        {
            var content = DbUtil.Db.ContentOfTypeSql(scriptname);
            if (content == null)
                return Message("no content");
            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
                p.Add("@" + kv.Key, kv.Value);
            var script = RunScriptSql(DbUtil.Db, parameter, content.Body, p);
            if (script.StartsWith("Not Authorized"))
                return Message(script);
            return cn.ExecuteReader(script, p).ToExcel("RunScript.xlsx");
        }

        [HttpGet, Route("~/PyScript/{name}")]
        public ActionResult PyScript(string name, string p1, string p2, string v1, string v2)
        {
            try
            {
                var script = DbUtil.Db.ContentOfTypePythonScript(name);
                if (!script.HasValue())
                    return Message("no script named " + name);

                if (script.Contains("model.Form"))
                    return Redirect("/PyScriptForm/" + name);
                script = script.Replace("@P1", p1 ?? "NULL")
                    .Replace("@P2", p2 ?? "NULL")
                    .Replace("V1", v1 ?? "None")
                    .Replace("V2", v2 ?? "None");
                if (script.Contains("@qtagid"))
                {
                    var id = DbUtil.Db.FetchLastQuery().Id;
                    var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                    script = script.Replace("@qtagid", tag.Id.ToString());
                }

                var pe = new PythonEvents(Util.Host);

                foreach (var key in Request.QueryString.AllKeys)
                    pe.DictionaryAdd(key, Request.QueryString[key]);

                pe.RunScript(script);

                return View(pe);
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }
        private string FetchPyScriptForm(string name)
        {
#if DEBUG
            if (name == "test")
                return System.IO.File.ReadAllText(Server.MapPath("~/test.py"));
#endif
            return DbUtil.Db.ContentOfTypePythonScript(name);
        }
        [HttpGet, Route("~/PyScriptForm/{name}")]
        public ActionResult PyScriptForm(string name)
        {
            try
            {
                var script = FetchPyScriptForm(name);

                if (!script.HasValue())
                    return Message("no script named " + name);
                var pe = new PythonEvents(Util.Host);
                foreach (var key in Request.QueryString.AllKeys)
                    pe.DictionaryAdd(key, Request.QueryString[key]);
                pe.Data.pyscript = name;
                pe.HttpMethod = "get";
                pe.RunScript(script);
                return View(pe);
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
                var pe = new PythonEvents(Util.Host);
                foreach (var key in Request.Form.AllKeys)
                    pe.DictionaryAdd(key, Request.Form[key]);
                pe.HttpMethod = "post";

                var script = FetchPyScriptForm(pe.Data.pyscript);
                return Content(pe.RunScript(script));
            }
            catch (Exception ex)
            {
                return RedirectShowError(ex.Message);
            }
        }

        [HttpGet, Route("~/Preferences")]
        public ActionResult UserPreferences()
        {
            return View(DbUtil.Db.CurrentUser);
        }

        [HttpGet, Route("~/Home/Support2")]
        public ActionResult Support2(string helplink)
        {
            if (helplink.HasValue())
                TempData["HelpLink"] = HttpUtility.UrlDecode(helplink);
            return View();
        }
    }

    public class Home2Controller : CmsController
    {
        [HttpGet, Route("~/Home/MyDataSupport")]
        public ActionResult MyDataSupport()
        {
            return View("../Home/MyDataSupport");
        }

        [HttpPost, Route("~/HideTip")]
        public ActionResult HideTip(string tip)
        {
            DbUtil.Db.SetUserPreference("hide-tip-" + tip, "true");
            return new EmptyResult();
        }

        [HttpGet, Route("~/ResetTips")]
        public ActionResult ResetTips()
        {
            DbUtil.Db.ExecuteCommand("DELETE dbo.Preferences WHERE Preference LIKE 'hide-tip-%' AND UserId = {0}",
                Util.UserId);
            var d = Session["preferences"] as Dictionary<string, string>;
            var keys = d.Keys.Where(kk => kk.StartsWith("hide-tip-")).ToList();
            foreach (var k in keys)
                d.Remove(k);

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            return Redirect("/");
        }

        [HttpGet]
        [Route("~/Person/TinyImage/{id}")]
        [Route("~/Person2/TinyImage/{id}")]
        [Route("~/TinyImage/{id}")]
        public ActionResult TinyImage(int id)
        {
            return new PictureResult(id, portrait: true, tiny: true);
        }

        [HttpGet]
        [Route("~/Person/Image/{id:int}/{w:int?}/{h:int?}")]
        [Route("~/Person2/Image/{id:int}/{w:int?}/{h:int?}")]
        [Route("~/Image/{id:int}/{w:int?}/{h:int?}")]
        public ActionResult Image(int id, int? w, int? h, string mode)
        {
            return new PictureResult(id);
        }

        [HttpGet, Route("~/ImageSized/{id:int}/{w:int}/{h:int}/{mode}")]
        public ActionResult ImageSized(int id, int w, int h, string mode)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            return new PictureResult(p.Picture.LargeId ?? 0, w, h, portrait: true, mode: mode);
        }
    }
}
