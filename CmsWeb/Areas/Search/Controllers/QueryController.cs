/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using Elmah;
using UtilityExtensions;
using CmsData;
using CmsWeb.Code;
using Dapper;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "Query"), Route("{action}/{id?}")]
    [SessionExpire]
    public class QueryController : CmsStaffController
    {
        [HttpGet, Route("~/Query/{id:guid?}")]
        public ActionResult Index(Guid? id)
        {
            ViewBag.Title = "QueryBuilder";
            ViewBag.OrigQueryId = id;
            var m = new QueryModel(id);
            ViewBag.ForceAutoRun = TempData["autorun"];
            return ViewQuery(m);
        }

        [HttpGet, Route("~/Query/{name}")]
        public ActionResult NamedQuery(string name)
        {
            ViewBag.Title = "QueryBuilder";
            if (name == "-All-")
            {
                var cc = DbUtil.Db.ScratchPadCondition();
                cc.Reset();
                cc.Save(DbUtil.Db);
                TempData["autorun"] = true;
                return Redirect("/Query");
            }
            var id = DbUtil.Db.QueryIdByName(name);
            var m = new QueryModel(id);
            return ViewQuery(m);
        }

        private ActionResult ViewQuery(QueryModel m)
        {
            InitToolbar(m);
            m.TopClause.IncrementLastRun();
            DbUtil.Db.SubmitChanges();
            m.QueryId = m.TopClause.Id;
            ViewBag.xml = m.TopClause.ToXml();
            var sb = new StringBuilder();
            foreach (var c in m.TopClause.AllConditions)
            {
                sb.AppendLine(c.Key.ToString());
                if (c.Value.FieldInfo == null)
                    return NewQuery();
            }
            ViewBag.ConditionList = sb.ToString();
            return View("Index", m);
        }

        private void InitToolbar(QueryModel m)
        {
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
            m.Copy();
            return Content("ok");
        }

        [HttpPost]
        public ActionResult Paste(QueryModel m)
        {
            m.Paste();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult InsGroupAbove(QueryModel m)
        {
            m.InsertGroupAbove();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult MakeTopGroup(QueryModel m)
        {
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
            m.Comparison = "Equal";
            m.UpdateCondition();
            return View("EditCondition", m);
        }

        [HttpPost]
        public ActionResult EditCondition(QueryModel m)
        {
            Response.NoCache();
            m.EditCondition();
            return View(m);
        }

        [HttpPost]
        public ActionResult AddNewCondition(QueryModel m)
        {
            m.EditCondition();
            ViewBag.NewId = m.AddConditionToGroup();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult AddNewGroup(QueryModel m)
        {
            m.EditCondition();
            ViewBag.NewId = m.AddGroupToGroup();
            return View("Conditions", m);
        }

        [HttpPost, Route("ChangeGroup/{comparison}")]
        public ActionResult ChangeGroup(string comparison, QueryModel m)
        {
            m.Selected.Comparison = comparison;
            m.TopClause.Save(DbUtil.Db);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult SaveCondition(QueryModel m)
        {
            if (m.Validate(ModelState))
                m.UpdateCondition();
            if (ModelState.IsValid)
                return View("Conditions", m);
            return View("EditCondition", m);
        }

        [HttpPost]
        public ActionResult Reload()
        {
            var m = new QueryModel();
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult RemoveCondition(QueryModel m)
        {
            m.DeleteCondition();
            m.SelectedId = null;
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult Conditions(QueryModel m)
        {
            return View("Conditions", m);
        }

        [HttpPost]
        public ActionResult Divisions(string id)
        {
            var m = new QueryModel();
            return View(m.Divisions(id));
        }

        [HttpPost]
        public ActionResult Organizations(string id)
        {
            var m = new QueryModel();
            return View(m.Organizations(id));
        }

        [HttpPost]
        public JsonResult SavedQueries(QueryModel m)
        {
            return Json(m.SavedQueries());
        }

        [HttpPost]
        public ActionResult SaveAs(Guid id, string nametosaveas)
        {
            if (nametosaveas.Equals(Util.ScratchPad2))
                nametosaveas = "copy of scratchpad";
            var m2 = new SavedQueryInfo(id) {Name = nametosaveas};
            return View(m2);
        }

        [HttpPost]
        public ActionResult Save(string name, string value, SavedQueryInfo m)
        {
            var query = DbUtil.Db.LoadQueryById2(m.QueryId);
            var previous = (from p in DbUtil.Db.Queries
                where p.Owner == m.Owner
                where p.Name == name
                orderby p.LastRun
                select p).FirstOrDefault();
            if (previous != null)
            {
                // copying over a previous query with same name and owner
                m.CopyPropertiesTo(previous);
                previous.Text = query.Text;
                if (previous.Name.Equal(Util.ScratchPad2))
                    previous.Ispublic = false;
                DbUtil.Db.SubmitChanges();
                return Redirect("/Query/" + previous.QueryId);

                //                m.CopyPropertiesTo(previous);
                //                var pc = previous.ToClause();
                //                pc.Reset(DbUtil.Db);
                //                pc = Condition.Import(query.Text, name, newGuids: true, topguid: previous.QueryId);
                //                previous.Text = pc.ToXml();
                //                DbUtil.Db.SubmitChanges();
                //                return Redirect("/Query/" + previous.QueryId);
            }
            // saving to a new query
            m.CopyPropertiesTo(query);
            if (query.Name.Equal(Util.ScratchPad2))
                query.Ispublic = false;
            DbUtil.Db.SubmitChanges();
            return Redirect("/Query/" + m.QueryId);
        }

        [HttpPost]
        public ActionResult Results(QueryModel m)
        {
            var starttime = DateTime.Now;
            DbUtil.LogActivity($"QB Results ({DateTime.Now.Subtract(starttime).TotalSeconds:N1}, {m.TopClause.Id})");
            InitToolbar(m);
            ViewBag.xml = m.TopClause.ToXml();
            return View(m);
        }

        [HttpGet, Route("~/NewQuery")]
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause();
            qb.Description = Util.ScratchPad2;
            qb.Save(DbUtil.Db, increment: true);
            return Redirect("/Query");
        }

        [HttpGet, Route("Help/{name}")]
        public ActionResult Help(string name)
        {
            var wc = new WebClient();
            var s = wc.DownloadString("http://docs.touchpointsoftware.com/SearchBuilder/" + name + ".html");
            return Content(s);
        }

        [HttpPost]
        public JsonResult ToggleTag(int id)
        {
            try
            {
                var r = Person.ToggleTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                DbUtil.Db.SubmitChanges();
                return Json(new {HasTag = r});
            }
            catch (Exception ex)
            {
                return Json(new {error = ex.Message + ". Please report this to support@touchpointsoftware.com"});
            }
        }

        [HttpPost]
        public ActionResult SetAutoRun(bool setting)
        {
            DbUtil.Db.SetUserPreference("QueryAutoRun", setting ? "true" : "false");
            return Content(setting.ToString().ToLower());
        }

        [HttpPost]
        public ContentResult TagAll(string tagname, bool? cleartagfirst, QueryModel m)
        {
            if (!tagname.HasValue())
                return Content("error: no tag name");
            if (Util2.CurrentTagName == tagname && !(cleartagfirst ?? false))
            {
                m.TagAll();
                return Content("Remove");
            }
            var tag = DbUtil.Db.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            if (cleartagfirst ?? false)
                DbUtil.Db.ClearTag(tag);
            m.TagAll(tag);
            Util2.CurrentTag = tagname;
            DbUtil.Db.TagCurrent();
            return Content("Manage");
        }

        [HttpPost]
        public ContentResult UnTagAll(QueryModel m)
        {
            m.UnTagAll();
            return Content("Add");
        }

        [HttpPost]
        public ContentResult AddContact(QueryModel m)
        {
            var cid = Contact.AddContact(m.TopClause.Id);
            return Content("/Contact2/" + cid);
        }

        [HttpPost]
        public ActionResult AddTasks(QueryModel m)
        {
            return Content(Task.AddTasks(DbUtil.Db, m.TopClause.Id).ToString());
        }

        [HttpGet]
        [Route("~/Query/Export")]
        [Route("~/Query/Export/{id?}")]
        public ActionResult Export(Guid? id)
        {
            var m = new QueryModel(id);
            return Content(m.TopClause.ToCode(), "text/plain");
        }

        [HttpGet]
        [Route("~/Query/ExportSql")]
        [Route("~/Query/ExportSql/{id?}")]
        public ActionResult ExportSql(Guid? id)
        {
            var m = new QueryModel(id);
            var q = DbUtil.Db.PeopleQueryCondition(m.TopClause);
            return Content(DbUtil.Db.GetWhereClause(q), "text/plain");
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
            ret.Save(DbUtil.Db);
            return Redirect("/Query/" + ret.Id);
        }

        [HttpGet]
        [Route("~/Query/Parse")]
        [Route("~/Query/Parse/{id?}")]
        public ActionResult Parse(Guid? id)
        {
            var m = new QueryModel(id);
            var s = m.TopClause.ToCode();

            var q2 = DbUtil.Db.PeopleQueryCode(s);
            var q1 = DbUtil.Db.PeopleQueryCondition(m.TopClause);

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

            DbUtil.Db.Connection.Execute(@"
UPDATE QueryAnalysis 
set seconds = @seconds, 
    Message = @Error, 
    OriginalCount = @cnt1, 
    ParsedCount=@cnt2 
where Id = @id", new {id, seconds, Error = error, cnt1, cnt2});

            return Content($"original={cnt1:N0} parsed={cnt2:N0}");
        }

        [HttpGet]
        [Route("~/Query/ToXml")]
        [Route("~/Query/ToXml/{id?}")]
        public ActionResult ToXml(Guid? id)
        {
            var m = new QueryModel(id);
            var xml1 = m.TopClause.ToXml();
            return Content(xml1, "text/xml");
        }

        [HttpGet]
        [Route("~/Query/ParseToXml")]
        [Route("~/Query/ParseToXml/{id?}")]
        public ActionResult ParseToXml(Guid? id)
        {
            var m = new QueryModel(id);
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