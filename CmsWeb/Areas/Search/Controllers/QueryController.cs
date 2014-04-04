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
using System.Xml;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.Search.Controllers
{
    [SessionExpire]
    public class QueryController : CmsStaffController
    {
        [HttpGet, Route("Query/{id:guid?}")]
        public ActionResult Index(Guid? id)
        {
            if (!ViewExtensions2.UseNewLook())
                return Redirect("/QueryBuilder2/Main/" + id);
            ViewBag.Title = "QueryBuilder";
            var m = new QueryModel(id);
            m.Pager.Set("/Query/Results/");

            InitToolbar(m);
            var newsearchid = (Guid?)TempData["newsearch"];
            if (m.TopClause.NewMatchAnyId.HasValue)
                newsearchid = m.TopClause.NewMatchAnyId;
            if (newsearchid.HasValue)
                ViewBag.NewSearchId = newsearchid.Value;
            m.TopClause.IncrementLastRun();
            DbUtil.Db.SubmitChanges();
            m.QueryId = m.TopClause.Id;
            ViewBag.xml = m.TopClause.ToXml();
            var sb = new StringBuilder();
            foreach (var c in m.TopClause.AllConditions)
                sb.AppendLine(c.Key.ToString());
            ViewBag.ConditionList = sb.ToString();
            return View(m);
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

        [HttpPost, Route("Query/Cut")]
        public ActionResult Cut(QueryModel m)
        {
            m.Cut();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/Copy")]
        public ActionResult Copy(QueryModel m)
        {
            m.Copy();
            return Content("ok");
        }
        [HttpPost, Route("Query/Paste")]
        public ActionResult Paste(QueryModel m)
        {
            m.Paste();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/InsGroupAbove")]
        public ActionResult InsGroupAbove(QueryModel m)
        {
            m.InsertGroupAbove();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/MakeTopGroup")]
        public ActionResult MakeTopGroup(QueryModel m)
        {
            m.MakeTopGroup();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/CodeSelect")]
        public ActionResult CodeSelect(QueryModel m)
        {
            return View("EditorTemplates/CodeSelect", m);
        }
        [HttpPost, Route("Query/SelectCondition")]
        public ActionResult SelectCondition(QueryModel m)
        {
            m.Comparison = "Equal";
            m.UpdateCondition();
            return View("EditCondition", m);
        }
        [HttpPost, Route("Query/EditCondition")]
        public ActionResult EditCondition(QueryModel m)
        {
            Response.NoCache();
            m.EditCondition();
            return View(m);
        }

        [HttpPost, Route("Query/AddNewCondition")]
        public ActionResult AddNewCondition(QueryModel m)
        {
            m.EditCondition();
            ViewBag.NewId = m.AddConditionToGroup();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/AddNewGroup")]
        public ActionResult AddNewGroup(QueryModel m)
        {
            m.EditCondition();
            ViewBag.NewId = m.AddGroupToGroup();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/ChangeGroup/{comparison}")]
        public ActionResult ChangeGroup(string comparison, QueryModel m)
        {
            m.Selected.Comparison = comparison;
            m.TopClause.Save(DbUtil.Db);
            return Content("ok");
        }
        [HttpPost, Route("Query/SaveCondition")]
        public ActionResult SaveCondition(QueryModel m)
        {
            if (m.Validate(ModelState))
                m.UpdateCondition();
            if (ModelState.IsValid)
                return View("Conditions", m);
            return View("EditCondition", m);
        }
        [HttpPost, Route("Query/Reload/")]
        public ActionResult Reload()
        {
            var m = new QueryModel();
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/RemoveCondition")]
        public ActionResult RemoveCondition(QueryModel m)
        {
            m.DeleteCondition();
            m.SelectedId = null;
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/Conditions")]
        public ActionResult Conditions(QueryModel m)
        {
            return View("Conditions", m);
        }
        [HttpPost, Route("Query/Divisions/{id:int}")]
        public ActionResult Divisions(int id)
        {
            return View(QueryModel.Divisions(id));
        }
        [HttpPost, Route("Query/Organizations/{id:int}")]
        public ActionResult Organizations(int id)
        {
            return View(QueryModel.Organizations(id));
        }
        [HttpPost, Route("Query/SavedQueries")]
        public JsonResult SavedQueries(QueryModel m)
        {
            return Json(m.SavedQueries());
        }
        [HttpPost, Route("Query/SaveAs")]
        public ActionResult SaveAs(Guid id, string nametosaveas)
        {
            if (nametosaveas.Equals(Util.ScratchPad2))
                nametosaveas = "copy of scratchpad";
            var m2 = new SavedQueryInfo(id) { Name = nametosaveas };
            return View(m2);
        }
        [HttpPost, Route("Query/Save")]
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

        [HttpPost, Route("Query/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, QueryModel m)
        {
            m.Pager.Set("/Query/Results", page, size, sort, dir);
            var starttime = DateTime.Now;
            DbUtil.LogActivity("QB Results ({0:N1}, {1})".Fmt(DateTime.Now.Subtract(starttime).TotalSeconds, m.TopClause.Id));
            InitToolbar(m);
            ViewBag.xml = m.TopClause.ToXml();
            return View(m);
        }
        [HttpGet, Route("Query/NewQuery")]
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset(DbUtil.Db);
            var nc = qb.AddNewClause();
            qb.Description = Util.ScratchPad2;
            qb.Save(DbUtil.Db);
            TempData["newsearch"] = nc.Id;
            return Redirect("/Query");
        }
        [HttpGet, Route("Query/Help/{name}")]
        public ActionResult Help(string name)
        {
            var wc = new WebClient();
            var s = wc.DownloadString("https://www.bvcms.com/DocDialog2/" + name);
            return Content(s);
        }
        [HttpPost, Route("Query/ToggleTag/{id:int}")]
        public JsonResult ToggleTag(int id)
        {
            try
            {
                var r = Person.ToggleTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                DbUtil.Db.SubmitChanges();
                return Json(new { HasTag = r });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message + ". Please report this to support@bvcms.com" });
            }
        }
        [HttpPost, Route("Query/SetAutoRun/")]
        public ActionResult ToggleAutoRun(bool setting)
        {
            DbUtil.Db.SetUserPreference("QueryAutoRun", setting ? "true": "false");
            return Content(setting.ToString().ToLower());
        }
        [HttpPost, Route("Query/TagAll/{tagname}/{cleartagfirst:bool?}")]
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
        [HttpPost, Route("Query/UnTagAll")]
        public ContentResult UnTagAll(QueryModel m)
        {
            m.UnTagAll();
            return Content("Add");
        }
        [HttpPost, Route("Query/AddContact")]
        public ContentResult AddContact(QueryModel m)
        {
            var cid = Contact.AddContact(m.TopClause.Id);
            return Content("/Contact/" + cid);
        }
        [HttpPost, Route("Query/AddTasks")]
        public ActionResult AddTasks(QueryModel m)
        {
            return Content(Task.AddTasks(m.TopClause.Id).ToString());
        }

        public ActionResult Export()
        {
            var m = new QueryModel();
            Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings { Indent = true, Encoding = new System.Text.UTF8Encoding(false) };
            using (var w = XmlWriter.Create(Response.OutputStream, settings))
                m.TopClause.SendToWriter(w);
            return new EmptyResult();
        }
        [HttpGet]
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Import(string text, string name)
        {
            var ret = Condition.Import(text, name, newGuids: true);
            return Redirect("/Query/" + ret.Id);
        }
    }
}
