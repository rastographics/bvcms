/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Data.SqlClient;
using System.Threading;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using DocumentFormat.OpenXml.EMMA;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Search.Controllers
{
	[SessionExpire]
    [RouteArea("Search", AreaUrl = "Query")]
    public class QueryController : CmsStaffAsyncController
    {
        [GET("Query/{id:int?}")]
        public ActionResult Query(int? id)
        {
            ViewBag.Title = "QueryBuilder";
            ViewBag.OnQueryBuilder = "true";
            ViewBag.TagAction = "/Query/TagAll/";
            ViewBag.TagAction = "/Query/UnTagAll/";
            ViewBag.Contact = "/Query/AddContact/";
            ViewBag.Tasks = "/Query/AddTasks/";
            ViewBag.GearSpan = "span12";
            var m = new AdvancedModel { QueryId = id };
            DbUtil.LogActivity("QueryBuilder");
            m.LoadScratchPad();
            ViewData["queryid"] = m.QueryId;
            var newsearchid = (int?)TempData["newsearch"];
            if (newsearchid.HasValue)
                ViewBag.NewSearchId = newsearchid.Value;
#if DEBUG
            else
            {
                ViewBag.AutoRun = true;
            }
#endif
            return View(m);
        }
        [POST("Query/CodesDropdown/")]
        public ActionResult CodesDropdown(AdvancedModel m)
        {
            m.SetCodes();
            return View(m);
        }
        [POST("Query/SelectCondition/{id:int}")]
        public ActionResult SelectCondition(int id, string conditionName)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
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
        [POST("Query/EditCondition/{id:int}")]
        public ActionResult EditCondition(int id)
        {
            Response.NoCache();
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.EditCondition();
            return View(m);
        }

        [POST("Query/AddNewCondition/{id:int}")]
        public ActionResult AddNewCondition(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.EditCondition();
            ViewBag.NewId = m.AddConditionToGroup();
            return View("Conditions2", m);
        }
        [POST("Query/AddNewGroup/{id:int}")]
        public ActionResult AddNewGroup(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.EditCondition();
            ViewBag.NewId = m.AddGroupToGroup();
            return View("Conditions2", m);
        }
        [POST("Query/DuplicateCondition/{id:int}")]
        public ActionResult DuplicateCondition(int id)
        {
            var m = new AdvancedModel();
            m.LoadScratchPad();
            m.EditCondition();
            ViewBag.NewId = m.CopyCurrentCondition(id);
            return View("Conditions2", m);
        }
        [POST("Query/SaveCondition/{id:int}")]
        public ActionResult ChangeGroup(int id, string comparison)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.ChangeGroup(comparison);
            return Content("ok");
        }
        [POST("Query/SaveCondition")]
        public ActionResult SaveCondition(AdvancedModel m)
        {
            m.LoadScratchPad();
            if (m.Validate(ModelState))
            {
                m.UpdateCondition();
                DbUtil.Db.SubmitChanges();
            }
            if(ModelState.IsValid)
                return View("Conditions2", m);
            return View("EditCondition", m);
        }
        [POST("Query/Reload/")]
        public ActionResult Reload()
        {
            var m = new AdvancedModel();
            m.LoadScratchPad();
            return View("Conditions2", m);
        }
        [POST("Query/RemoveCondition/{id:int}")]
        public ActionResult RemoveCondition(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.DeleteCondition();
            m.SelectedId = null;
            return View("Conditions2", m);
        }
        [POST("Query/InsGroupAbove/{id:int}")]
        public ActionResult InsGroupAbove(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.InsertGroupAbove();
            return View("Conditions2", m);
        }
        [POST("Query/MoveToPreviousGroup/{id:int}")]
        public ActionResult MoveToPreviousGroup(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.MoveToPreviousGroup();
            return View("Conditions2", m);
        }
        [POST("Query/MoveToGroupBelow/{id:int}")]
        public ActionResult MoveToGroupBelow(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.MoveToGroupBelow();
            return View("Conditions2", m);
        }
        [POST("Query/CopyAsNew/{id:int}")]
        public ActionResult CopyAsNew(int id)
        {
            var m = new AdvancedModel { SelectedId = id };
            m.LoadScratchPad();
            m.CopyAsNew();
            var c = new ContentResult();
            c.Content = m.QueryId.ToString();
            return c;
        }
        [POST("Query/Conditions")]
        public ActionResult Conditions()
        {
            var m = new AdvancedModel();
            return View("Conditions2", m);
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
            var m = new AdvancedModel();
            return Json(m.SavedQueries()); ;
        }
        [POST("Query/SaveQuery")]
        public ActionResult SaveQuery()
        {
            var m = new AdvancedModel();
            UpdateModel(m);
            m.LoadScratchPad();
            m.SaveQuery();
            var c = new ContentResult();
            c.Content = m.Description;
            return c;
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
            var m = new AdvancedModel();
            m.Pager.Set("/Query/Results", page, size, sort, dir);
			try
			{
	            UpdateModel(m);
			}
			catch (Exception ex)
			{
				return Content("Something went wrong<br><p>" + ex.Message + "</p>");
			}
            m.LoadScratchPad();

			var starttime = DateTime.Now;
			m.PopulateResults();
			DbUtil.LogActivity("QB Results ({0:N1}, {1})".Fmt(DateTime.Now.Subtract(starttime).TotalSeconds, m.QueryId));
            return View(m);
        }
        [GET("Query/NewQuery")]
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            var ncid = qb.CleanSlate2(DbUtil.Db);
            TempData["newsearch"] = ncid;
            return Redirect("/Query");
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
            var m = new AdvancedModel();
            m.LoadScratchPad();
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
            var m = new AdvancedModel();
            m.LoadScratchPad();
            m.UnTagAll();
            var c = new ContentResult();
            c.Content = "Add";
            return c;
        }
        [HttpPost]
        public ContentResult AddContact()
        {
            var m = new AdvancedModel();
            m.LoadScratchPad();
            var cid = CmsData.Contact.AddContact(m.QueryId.Value);
            return Content("/Contact/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks()
        {
            var m = new AdvancedModel();
            m.LoadScratchPad();
            var c = new ContentResult();
            c.Content = Task.AddTasks(m.QueryId.Value).ToString();
            return c;
        }

        public ActionResult Export()
        {
            var m = new AdvancedModel();
            m.LoadScratchPad();
            return new QBExportResult(m.QueryId.Value);
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
			int id = QueryFunctions.Import(DbUtil.Db, text, name);
			return Redirect("/Query/" + id);
		}
    }
}
