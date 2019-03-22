/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using Dapper;
using Elmah;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "Query"), Route("{action}/{id?}")]
    [SessionExpire]
    public class QueryController : CmsStaffController
    {
        public QueryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/Query/{id:guid?}")]
        public ActionResult Index(Guid? id)
        {
            ViewBag.Title = "QueryBuilder";
            ViewBag.OrigQueryId = id;
            var m = new QueryModel(id, CurrentDatabase);
            ViewBag.ForceAutoRun = TempData["autorun"];
            return ViewQuery(m);
        }

        [HttpGet, Route("~/Query/{name}")]
        public ActionResult NamedQuery(string name)
        {
            ViewBag.Title = "QueryBuilder";
            if (name == "-All-")
            {
                var cc = CurrentDatabase.ScratchPadCondition();
                cc.Reset();
                cc.Save(CurrentDatabase);
                TempData["autorun"] = true;
                return Redirect("/Query");
            }
            var id = CurrentDatabase.QueryIdByName(name);
            var m = new QueryModel(id, CurrentDatabase);
            return ViewQuery(m);
        }
        [HttpGet, Route("~/QueryCode")]
        public ActionResult QueryCode(string code)
        {
            ViewBag.Title = "QueryBuilder";
            var m = QueryModel.QueryCode(CurrentDatabase, code);
            return ViewQuery(m);
        }
        private ActionResult ViewQuery(QueryModel m)
        {
            m.Db = CurrentDatabase;
            InitToolbar(m);
            m.TopClause.IncrementLastRun();
            CurrentDatabase.SubmitChanges();
            m.QueryId = m.TopClause.Id;
            ViewBag.xml = m.TopClause.ToXml();
            var sb = new StringBuilder();
            foreach (var c in m.TopClause.AllConditions)
            {
                sb.AppendLine(c.Key.ToString());
                if (c.Value.FieldInfo == null)
                {
                    return NewQuery();
                }
            }
            ViewBag.ConditionList = sb.ToString();
            return View("Index", m);
        }

        private void InitToolbar(QueryModel m)
        {
            m.Db = CurrentDatabase;
            ViewBag.OnQueryBuilder = "true";
            ViewBag.TagAction = "/Query/TagAll/";
            ViewBag.UnTagAction = "/Query/UnTagAll/";
            ViewBag.AddContact = "/Query/AddContact/";
            ViewBag.AddTasks = "/Query/AddTasks/";
            ViewBag.GearSpan = "span12";
            ViewBag.queryid = m.TopClause.Id;
        }

        [HttpPost]
        public ActionResult Cut(QueryModel m)
        {
            try
            {
                m.Db = CurrentDatabase;
                m.Cut();
            }
            catch (Exception ex)
            {
                var errorLog = ErrorLog.GetDefault(null);
                errorLog.Log(new Error(ex));
            }
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult Copy(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.Copy();
            return Content("ok");
        }

        [HttpPost]
        public ActionResult Paste(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.Paste();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult InsGroupAbove(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.InsertGroupAbove();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult MakeTopGroup(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.MakeTopGroup();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult CodeSelect(QueryModel m)
        {
            return View("EditorTemplates/CodeSelect", m);
        }

        [HttpPost]
        public ActionResult SelectCondition(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.Comparison = "Equal";
            m.UpdateCondition();
            return View("EditCondition", m);
        }

        [HttpPost]
        public ActionResult EditCondition(QueryModel m)
        {
            m.Db = CurrentDatabase;
            Response.NoCache();
            m.EditCondition();
            return View(m);
        }

        [HttpPost]
        public ActionResult AddNewCondition(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.EditCondition();
            ViewBag.NewId = m.AddConditionToGroup();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult AddNewGroup(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.EditCondition();
            ViewBag.NewId = m.AddGroupToGroup();
            return View("Conditions", m);
        }

        [HttpPost, Route("ChangeGroup/{comparison}")]
        public ActionResult ChangeGroup(string comparison, QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.Selected.Comparison = comparison;
            m.TopClause.Save(CurrentDatabase);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult SaveCondition(QueryModel m)
        {
            m.Db = CurrentDatabase;
            if (m.Validate(ModelState))
            {
                m.UpdateCondition();
            }

            if (ModelState.IsValid)
            {
                return View("Conditions", m);
            }

            return View("EditCondition", m);
        }

        [HttpPost]
        public ActionResult Reload()
        {
            var m = new QueryModel(CurrentDatabase);
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult RemoveCondition(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.DeleteCondition();
            m.SelectedId = null;
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult ToggleConditionEnabled(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.ToggleConditionEnabled();
            m.SelectedId = null;
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult Conditions(QueryModel m)
        {
            m.Db = CurrentDatabase;
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult Divisions(string id)
        {
            var m = new QueryModel(CurrentDatabase);
            return View(m.Divisions(id));
        }

        [HttpPost]
        public ActionResult Organizations(string id)
        {
            var m = new QueryModel(CurrentDatabase);
            return View(m.Organizations(id));
        }

        [HttpPost]
        public JsonResult SavedQueries(QueryModel m)
        {
            m.Db = CurrentDatabase;
            return Json(m.SavedQueries());
        }

        [HttpPost]
        public ActionResult SaveAs(Guid id, string nametosaveas)
        {
            if (nametosaveas.Equals(Util.ScratchPad2))
            {
                nametosaveas = "copy of scratchpad";
            }

            var m2 = new SavedQueryInfo(id, CurrentDatabase) { Name = nametosaveas };
            return View(m2);
        }

        [HttpPost]
        public ActionResult Save(string name, string value, SavedQueryInfo m)
        {
            var query = CurrentDatabase.LoadQueryById2(m.QueryId);
            var previous = (from p in CurrentDatabase.Queries
                            where p.Owner == m.Owner
                            where p.Name == name
                            orderby p.LastRun
                            select p).FirstOrDefault();
            if (previous != null)
            {
                // copying over a previous query with same name and owner
                m.CopyPropertiesTo(previous);
                if (previous.Name.Equal(Util.ScratchPad2))
                {
                    previous.Text = query.Text;
                    previous.Ispublic = false;
                }
                else // saved search, not a scratchpad query
                {
                    // remove DisableOnScratchpad attributes from saved search
                    previous.Text = query.Text.Replace(" DisableOnScratchpad=\"True\"", "");
                }
                CurrentDatabase.SubmitChanges();
                return Redirect("/Query/" + previous.QueryId);
            }
            // saving to a new query
            m.CopyPropertiesTo(query);
            if (query.Name.Equal(Util.ScratchPad2))
            {
                query.Ispublic = false;
            }

            CurrentDatabase.SubmitChanges();
            return Redirect("/Query/" + m.QueryId);
        }

        [HttpPost]
        public ActionResult Results(QueryModel m)
        {
            m.Db = CurrentDatabase;
            var starttime = DateTime.Now;
            DbUtil.LogActivity($"QB Results ({DateTime.Now.Subtract(starttime).TotalSeconds:N1}, {m.TopClause.Id})");
            InitToolbar(m);
            ViewBag.xml = m.TopClause.ToXml();
            return View(m);
        }

        [HttpGet, Route("~/NewQuery")]
        public ActionResult NewQuery()
        {
            var qb = CurrentDatabase.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause();
            qb.Description = Util.ScratchPad2;
            qb.Save(CurrentDatabase, increment: true);
            return Redirect("/Query");
        }

        [HttpGet, Route("Help/{name}")]
        public ActionResult Help(string name)
        {
            var wc = new WebClient();
            var s = wc.DownloadString("https://docs.touchpointsoftware.com/SearchBuilder/" + name + ".html");
            return Content(s);
        }

        [HttpPost]
        public JsonResult ToggleTag(int id)
        {
            try
            {
                var r = Person.ToggleTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                CurrentDatabase.SubmitChanges();
                return Json(new { HasTag = r });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message + $". Please report this to {ConfigurationManager.AppSettings["supportemail"]}" });
            }
        }

        [HttpPost]
        public ActionResult SetAutoRun(bool setting)
        {
            CurrentDatabase.SetUserPreference("QueryAutoRun", setting ? "true" : "false");
            return Content(setting.ToString().ToLower());
        }

        [HttpPost]
        public ContentResult TagAll(string tagname, bool? cleartagfirst, QueryModel m)
        {
            // take a query, add all people from the result of that query to the tag specified
            // empty the tag first if requested
            string resultMessage = string.Empty;
            bool shouldContinue = true;

            if (!tagname.HasValue())
            {
                resultMessage = "error: no tag name";
                shouldContinue = false;
            }

            if (shouldContinue)
            {
                var usingActiveTag = Util2.CurrentTagName == tagname;
                var workingTag = CurrentDatabase.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
                var shouldEmptyTag = cleartagfirst ?? false;

                if (shouldEmptyTag)
                {
                    CurrentDatabase.ClearTag(workingTag);
                }

                m.Db = CurrentDatabase;
                m.TagAll(workingTag);

                Util2.CurrentTag = workingTag.Name;

                resultMessage = "Manage";
                shouldContinue = false;
            }

            return Content(resultMessage);
        }

        [HttpPost]
        public ContentResult UnTagAll(QueryModel m)
        {
            m.Db = CurrentDatabase;
            m.UntagAll();
            return Content("Add");
        }

        [HttpPost]
        public ContentResult AddContact(QueryModel m)
        {
            m.Db = CurrentDatabase;
            var cid = Contact.AddContact(m.TopClause.Id);
            return Content("/Contact2/" + cid);
        }

        [HttpPost]
        public ActionResult AddTasks(QueryModel m)
        {
            m.Db = CurrentDatabase;
            return Content(Task.AddTasks(CurrentDatabase, m.TopClause.Id).ToString());
        }

        [HttpGet]
        [Route("~/Query/Export")]
        [Route("~/Query/Export/{id?}")]
        public ActionResult Export(Guid? id)
        {
            var m = new QueryModel(id, CurrentDatabase);
            return Content(m.TopClause.ToCode(), "text/plain");
        }

        [HttpGet]
        [Route("~/Query/ExportSql")]
        [Route("~/Query/ExportSql/{id?}")]
        public ActionResult ExportSql(Guid? id)
        {
            var m = new QueryModel(id, CurrentDatabase);
            var q = CurrentDatabase.PeopleQueryCondition(m.TopClause);
            return Content(CurrentDatabase.GetWhereClause(q), "text/plain");
        }

        [HttpGet, Route("~/Query/Import")]
        public ActionResult Import()
        {
            return View();
        }

        [HttpPost, Route("~/Query/Import")]
        [ValidateInput(false)]
        public ActionResult Import(string text, string name)
        {
            var ret = Condition.Import(text, name, newGuids: true);
            ret.Save(CurrentDatabase);
            return Redirect("/Query/" + ret.Id);
        }

        [HttpGet]
        [Route("~/Query/Parse")]
        [Route("~/Query/Parse/{id?}")]
        public ActionResult Parse(Guid? id)
        {
            var m = new QueryModel(id, CurrentDatabase);
            var s = m.TopClause.ToCode();

            var q2 = CurrentDatabase.PeopleQueryCode(s);
            var q1 = CurrentDatabase.PeopleQueryCondition(m.TopClause);

            int cnt1 = 0, cnt2 = 0;
            int seconds = 0;
            string error = null;

            try
            {
                var dt = DateTime.Now;
                cnt1 = q1.Count();
                seconds = DateTime.Now.Subtract(dt).TotalSeconds.ToInt();
                cnt2 = q2.Count();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            CurrentDatabase.Connection.Execute(@"
UPDATE QueryAnalysis 
set seconds = @seconds, 
    Message = @Error, 
    OriginalCount = @cnt1, 
    ParsedCount=@cnt2 
where Id = @id", new { id, seconds, Error = error, cnt1, cnt2 });

            return Content($"original={cnt1:N0} parsed={cnt2:N0}");
        }

        [HttpGet]
        [Route("~/Query/ToXml")]
        [Route("~/Query/ToXml/{id?}")]
        public ActionResult ToXml(Guid? id)
        {
            var m = new QueryModel(id, CurrentDatabase);
            var xml1 = m.TopClause.ToXml();
            return Content(xml1, "text/xml");
        }

        [HttpGet]
        [Route("~/Query/ParseToXml")]
        [Route("~/Query/ParseToXml/{id?}")]
        public ActionResult ParseToXml(Guid? id)
        {
            var m = new QueryModel(id, CurrentDatabase);
            var xml1 = m.TopClause.ToXml();
            var s = m.TopClause.ToCode();
            var c = Condition.Parse(s);
            var xml2 = c.ToXml();
            var content = $@"
ORIGINAL
{xml1}

PARSED
{xml2}
";
            return Content(content, "text/plain");
        }
    }
}
