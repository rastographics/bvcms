/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using System.Xml;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Search.Controllers
{
    [SessionExpire]
    [RouteArea("Search", AreaUrl = "Query")]
    public class QueryController : CmsStaffAsyncController
    {
        [GET("Query/{id:guid?}")]
        public ActionResult Index(Guid? id)
        {
            ViewBag.Title = "QueryBuilder";
            //var last5 = DbUtil.Db.FetchLastFiveQueries();
            var m = new QueryModel2();
            m.LoadQuery(id);

            InitToolbar(m);
            var newsearchid = (Guid?)TempData["newsearch"];
            if (m.TopClause.NewMatchAnyId.HasValue)
                newsearchid = m.TopClause.NewMatchAnyId;
            if (newsearchid.HasValue)
                ViewBag.NewSearchId = newsearchid.Value;
            else
                ViewBag.AutoRun = true;
            m.TopClause.IncrementLastRun();
            DbUtil.Db.SubmitChanges();
            return View(m);
        }

        private void InitToolbar(QueryModel2 m)
        {
            ViewBag.OnQueryBuilder = "true";
            ViewBag.TagAction = "/Query/TagAll/";
            ViewBag.UnTagAction = "/Query/UnTagAll/";
            ViewBag.Contact = "/Query/AddContact/";
            ViewBag.Tasks = "/Query/AddTasks/";
            ViewBag.GearSpan = "span12";
            ViewBag.queryid = m.TopClause.Id;
        }

        [POST("Query/Cut/{id}")]
        public ActionResult Cut(Guid id)
        {
            var m = new QueryModel2();
            m.LoadQuery();
            m.SelectedId = id;
            Session["QueryClipboard"] = 
                new QueryModel2.ClipboardItem( "cut", m.TopClause.Id, m.Current.ToXml() );
            return Content("ok");
        }
        [POST("Query/Copy/{id}")]
        public ActionResult Copy(Guid id)
        {
            var m = new QueryModel2();
            m.LoadQuery();
            m.SelectedId = id;
            Session["QueryClipboard"] = 
                new QueryModel2.ClipboardItem( "copy", m.TopClause.Id, m.Current.ToXml() );
            return Content("ok");
        }
        [POST("Query/Paste/{id}")]
        public ActionResult Paste(Guid id)
        {
            var m = new QueryModel2();
            m.LoadQuery();
            m.SelectedId = id;
            m.Paste(id);
            return View("Conditions", m);
        }
        [POST("Query/InsGroupAbove/{id:guid}")]
        public ActionResult InsGroupAbove(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.InsertGroupAbove();
            return View("Conditions", m);
        }
        [POST("Query/CodesDropdown/")]
        public ActionResult CodesDropdown(QueryModel2 m)
        {
            m.SetCodes();
            return View(m);
        }
        [POST("Query/SelectCondition/{id:guid}")]
        public ActionResult SelectCondition(Guid id, string conditionName)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.ConditionName = conditionName;
            m.SetVisibility();
            m.TextValue = "";
            m.IntegerValue = "";
            m.DateValue = "";
            m.CodeValue = "";
            m.CodeValues = new string[0];
            m.Days = "";
            m.Age = "";
            m.Program = 0;
            m.Quarters = "";
            m.StartDate = "";
            m.EndDate = "";
            m.Comparison = "Equal";
            m.UpdateCondition();
            m.SelectedId = id;
            return View("EditCondition", m);
        }
        [POST("Query/EditCondition/{id:guid}")]
        public ActionResult EditCondition(Guid id)
        {
            Response.NoCache();
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.EditCondition();
            return View(m);
        }

        [POST("Query/AddNewCondition/{id:guid}")]
        public ActionResult AddNewCondition(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.EditCondition();
            ViewBag.NewId = m.AddConditionToGroup();
            return View("Conditions", m);
        }
        [POST("Query/AddNewGroup/{id:guid}")]
        public ActionResult AddNewGroup(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.EditCondition();
            ViewBag.NewId = m.AddGroupToGroup();
            return View("Conditions", m);
        }
        [POST("Query/SaveCondition/{id:guid}")]
        public ActionResult ChangeGroup(Guid id, string comparison)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.Current.Comparison = comparison;
            m.TopClause.Save(DbUtil.Db);
            return Content("ok");
        }
        [POST("Query/SaveCondition")]
        public ActionResult SaveCondition(QueryModel2 m)
        {
            m.LoadQuery();
            if (m.Validate(ModelState))
                m.UpdateCondition();
            if (ModelState.IsValid)
                return View("Conditions", m);
            return View("EditCondition", m);
        }
        [POST("Query/Reload/")]
        public ActionResult Reload()
        {
            var m = new QueryModel2();
            m.LoadQuery();
            return View("Conditions", m);
        }
        [POST("Query/RemoveCondition/{id:guid}")]
        public ActionResult RemoveCondition(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.LoadQuery();
            m.DeleteCondition();
            m.SelectedId = null;
            return View("Conditions", m);
        }
        [POST("Query/Conditions")]
        public ActionResult Conditions()
        {
            var m = new QueryModel2();
            return View("Conditions", m);
        }
        [POST("Query/Divisions/{id:int}")]
        public ActionResult Divisions(int id)
        {
            return View(id);
        }
        [HttpPost]
        [POST("Query/Organizations/{id:int}")]
        public ActionResult Organizations(int id)
        {
            return View(id);
        }
        [POST("Query/SavedQueries")]
        public JsonResult SavedQueries()
        {
            var m = new QueryModel2();
            return Json(m.SavedQueries());
        }
        [POST("Query/SaveQueryDescription")]
        public ActionResult SaveQueryDescription(string name, string value)
        {
            var m = new QueryModel2();
            m.LoadQuery();
            switch (name)
            {
                case "SavedQueryDesc":
                    m.SavedQueryDesc = value;
                    m.TopClause.Description = value;
                    break;
            }
            m.TopClause.Save(DbUtil.Db);
            return new EmptyResult();
        }
        public void Results2Async()
        {
            AsyncManager.OutstandingOperations.Increment();
            string host = Util.Host;
            ThreadPool.QueueUserWorkItem((e) =>
            {
                var Db = new CMSDataContext(Util.GetConnectionString(host));
                Db.DeleteQueryBitTags();
                foreach (var a in Db.StatusFlags())
                {
                    var t = Db.FetchOrCreateSystemTag(a[0]);
                    Db.TagAll(Db.PeopleQuery(a[0] + ":" + a[1]), t);
                    Db.SubmitChanges();
                }
                AsyncManager.OutstandingOperations.Decrement();
            });
        }
        public ActionResult Results2Completed()
        {
            return null;
        }

        [POST("Query/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir)
        {
            var cb = new SqlConnectionStringBuilder(Util.ConnectionString);
            cb.ApplicationName = "qb";
            DbUtil.Db = new CMSDataContext(cb.ConnectionString);
            var m = new QueryModel2();
            m.Pager.Set("/Query/Results", page, size, sort, dir);
            try
            {
                UpdateModel(m);
            }
            catch (Exception ex)
            {
                return Content("Something went wrong<br><p>" + ex.Message + "</p>");
            }
            m.LoadQuery();

            var starttime = DateTime.Now;
            m.PopulateResults();
            DbUtil.LogActivity("QB Results ({0:N1}, {1})".Fmt(DateTime.Now.Subtract(starttime).TotalSeconds, m.TopClause.Id));
            InitToolbar(m);
            return View(m);
        }
        [GET("Query/NewQuery")]
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.ScratchPadCondition();
            var ncid = qb.CleanSlate2(DbUtil.Db);
            TempData["newsearch"] = ncid;
            return Redirect("/Query");
        }
        [GET("Query/Help/{name}")]
        public ActionResult Help(string name)
        {
            var wc = new WebClient();
            var s = wc.DownloadString("https://www.bvcms.com/DocDialog2/" + name);
            return Content(s);
        }
        [POST("Query/ToggleTag/{id:int}")]
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
        [POST("Query/TagAll/{tagname}/{cleartagfirst:bool?}")]
        public ContentResult TagAll(string tagname, bool? cleartagfirst)
        {
            if (!tagname.HasValue())
                return Content("error: no tag name");
            var m = new QueryModel2();
            m.LoadQuery();
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
        public ContentResult UnTagAll()
        {
            var m = new QueryModel2();
            m.LoadQuery();
            m.UnTagAll();
            return Content("Add");
        }
        [HttpPost]
        public ContentResult AddContact()
        {
            var m = new QueryModel2();
            m.LoadQuery();
            var cid = Contact.AddContact(m.TopClause.Id);
            return Content("/Contact/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks()
        {
            var m = new QueryModel2();
            m.LoadQuery();
            var c = new ContentResult();
            c.Content = Task.AddTasks(m.TopClause.Id).ToString();
            return c;
        }

        public ActionResult Export()
        {
            var m = new QueryModel2();
            m.LoadQuery();
            Response.ContentType = "text/xml";
            var settings = new XmlWriterSettings {Indent = true, Encoding = new System.Text.UTF8Encoding(false)};
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
