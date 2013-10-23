/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Elmah;
using UtilityExtensions;
using CmsWeb.Models;
using CmsData;
using System.Xml.Linq;

namespace CmsWeb.Areas.Main.Controllers
{
    [SessionExpire]
    public class QueryBuilderController : CmsStaffAsyncController
    {
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);
            return RedirectToAction("Main");
        }
        public ActionResult Main(int? id, int? run)
        {
            if (DbUtil.Db.UserPreference("UseNewLook", "false").ToBool())
                return Redirect("/Query");

            ViewData["Title"] = "QueryBuilder";
            ViewData["OnQueryBuilder"] = "true";
            ViewData["TagAction"] = "/QueryBuilder/TagAll/";
            ViewData["UnTagAction"] = "/QueryBuilder/UnTagAll/";
            ViewData["AddContact"] = "/QueryBuilder/AddContact/";
            ViewData["AddTasks"] = "/QueryBuilder/AddTasks/";
            var m = new QueryModel { QueryId = id };
            DbUtil.LogActivity("QueryBuilder");
            if (run.HasValue)
                m.ShowResults = true;
            m.LoadScratchPad();
            ViewData["queryid"] = m.QueryId;
            ViewBag.AutoRun = (bool?)(TempData["AutoRun"]) == true;
            return View(m);
        }
        [HttpPost]
        public JsonResult SelectCondition(int id, string ConditionName)
        {
            var m = new QueryModel { ConditionName = ConditionName, SelectedId = id };
            m.LoadScratchPad();
            m.SetVisibility();

            m.TextValue = "";
            m.Comparison = "";
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

            return Json(m);
        }
        [HttpPost]
        public JsonResult GetCodes(string Comparison, string ConditionName)
        {
            var m = new QueryModel { Comparison = Comparison, ConditionName = ConditionName };
            m.SetCodes();
            return Json(new
            {
                CodesVisible = m.CodesVisible,
                CodeVisible = m.CodeVisible,
                CodeData = m.CodeData,
                SelectMultiple = m.SelectMultiple
            });
        }
        [HttpPost]
        public JsonResult EditCondition(int id)
        {
            var m = new QueryModel { SelectedId = id };
            m.LoadScratchPad();
            m.EditCondition();
            return Json(m);
        }

        [HttpPost]
        public ActionResult AddToGroup()
        {
            var m = new QueryModel();
            UpdateModel<IQBUpdateable>(m);
            m.LoadScratchPad();
            if (Validate(m))
                m.AddConditionToGroup();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Add()
        {
            var m = new QueryModel();
            UpdateModel<IQBUpdateable>(m);
            m.LoadScratchPad();
            if (Validate(m))
                m.AddConditionAfterCurrent();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Update()
        {
            var m = new QueryModel();
            UpdateModel<IQBUpdateable>(m);
            m.LoadScratchPad();
            if (Validate(m))
                m.UpdateCondition();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public JsonResult Remove()
        {
            var m = new QueryModel();
            UpdateModel<IQBUpdateable>(m);
            m.LoadScratchPad();
            m.DeleteCondition();
            return Json(m);
        }
        [HttpPost]
        public ActionResult InsGroupAbove(int id)
        {
            var m = new QueryModel { SelectedId = id };
            m.LoadScratchPad();
            m.InsertGroupAbove();
            var c = new ContentResult();
            c.Content = m.QueryId.ToString();
            return c;
        }
        [HttpPost]
        public ActionResult CopyAsNew(int id)
        {
            var m = new QueryModel { SelectedId = id };
            m.LoadScratchPad();
            m.CopyAsNew();
            var c = new ContentResult();
            c.Content = m.QueryId.ToString();
            return c;
        }
        public ActionResult Conditions()
        {
            var m = new QueryModel();
            return View(m);
        }
        [HttpPost]
        public JsonResult GetDivisions(int id)
        {
            var m = new QueryModel();
            return Json(new { Divisions = m.Divisions(id), Organizations = m.Organizations(0) });
        }
        [HttpPost]
        public JsonResult GetOrganizations(int id)
        {
            var m = new QueryModel();
            return Json(m.Organizations(id));
        }
        [HttpPost]
        public JsonResult SavedQueries()
        {
            var m = new QueryModel();
            return Json(m.SavedQueries()); ;
        }
        [HttpPost]
        public ActionResult SaveQuery()
        {
            var m = new QueryModel();
            UpdateModel(m);
            m.LoadScratchPad();
            var ret = m.SaveQuery();
            if (ret.HasValue())
                return Content(ret);
            return Content(m.Description);
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

        [HttpPost]
        public ActionResult Results()
        {
            var cb = new SqlConnectionStringBuilder(Util.ConnectionString);
            cb.ApplicationName = "qb";
            DbUtil.Db = new CMSDataContext(cb.ConnectionString);
            var m = new QueryModel();
            try
            {
                UpdateModel<IQBUpdateable>(m);
            }
            catch (Exception ex)
            {
                return Content("Something went wrong<br><p>" + ex.Message + "</p>");
            }
            m.LoadScratchPad();
            var starttime = DateTime.Now;
            try
            {
                m.PopulateResults();
            }
            catch (Exception ex)
            {
                var ex2 = new Exception("SearchBuilder error:\n" + m.GetTopClause().ToXml("exception", m.QueryId.Value), ex);
                ErrorLog errorLog = ErrorLog.GetDefault(null);
                errorLog.Log(new Error(ex2));
                return Content("Something went wrong<br><p>" + ex.Message + "</p>");
            }
            DbUtil.LogActivity("QB Results ({0:N1}, {1})".Fmt(DateTime.Now.Subtract(starttime).TotalSeconds, m.QueryId));
            return View(m);
        }
        [HttpPost]
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
        [HttpPost]
        public ContentResult TagAll(string tagname, bool? cleartagfirst)
        {
            if (!tagname.HasValue())
                return Content("error: no tag name");
            var m = new QueryModel();
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
            var m = new QueryModel();
            m.LoadScratchPad();
            m.UnTagAll();
            var c = new ContentResult();
            c.Content = "Add";
            return c;
        }
        [HttpPost]
        public ContentResult AddContact()
        {
            var m = new QueryModel();
            m.LoadScratchPad();
            var cid = CmsData.Contact.AddContact(m.QueryId.Value);
            return Content("/Contact/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks()
        {
            var m = new QueryModel();
            m.LoadScratchPad();
            var c = new ContentResult();
            c.Content = Task.AddTasks(m.QueryId.Value).ToString();
            return c;
        }

        private bool Validate(QueryModel m)
        {
            m.SetVisibility();
            DateTime dt = DateTime.MinValue;
            if (m.StartDateVisible)
                if (!DateTime.TryParse(m.StartDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.Errors.Add("StartDate", "invalid");
            if (m.EndDateVisible && m.EndDate.HasValue())
                if (!DateTime.TryParse(m.EndDate, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.Errors.Add("EndDate", "invalid");
            int i = 0;
            if (m.DaysVisible && !int.TryParse(m.Days, out i))
                m.Errors.Add("Days", "must be integer");
            if (i > 10000)
                m.Errors.Add("Days", "days > 1000");
            if (m.AgeVisible && !int.TryParse(m.Age, out i))
                m.Errors.Add("Age", "must be integer");


            if (m.TagsVisible && string.Join(",", m.Tags).Length > 500)
                m.Errors.Add("tagvalues", "too many tags selected");

            if (m.CodesVisible && m.CodeValues.Length == 0)
                m.Errors.Add("CodeValues", "must select item(s)");

            if (m.NumberVisible && !m.NumberValue.HasValue())
                m.Errors.Add("NumberValue", "must have a number value");
            else
            {
                float f;
                if (m.NumberVisible && m.NumberValue.HasValue())
                    if (!float.TryParse(m.NumberValue, out f))
                        m.Errors.Add("NumberValue", "must have a valid number value (no decoration)");
            }

            if (m.DateVisible && !m.Comparison.EndsWith("Equal"))
                if (!DateTime.TryParse(m.DateValue, out dt) || dt.Year <= 1900 || dt.Year >= 2200)
                    m.Errors.Add("DateValue", "need valid date");

            if (m.Comparison == "Contains")
                if (!m.TextValue.HasValue())
                    m.Errors.Add("TextValue", "cannot be empty");

            return m.Errors.Count == 0;
        }
        public ActionResult Export()
        {
            var m = new QueryModel();
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
            var ret = QueryBuilderClause.Import(DbUtil.Db, text, name);
            return Redirect("/QueryBuilder/Main/" + ret.newid);
        }
    }
}
