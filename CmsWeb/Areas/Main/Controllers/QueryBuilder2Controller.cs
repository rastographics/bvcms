/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json;
using UtilityExtensions;
using CmsWeb.Models;
using CmsData;

namespace CmsWeb.Areas.Main.Controllers
{
    [SessionExpire]
    public class QueryBuilder2Controller : CmsStaffAsyncController
    {
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.ScratchPadCondition();
            var ncid = qb.CleanSlate2(DbUtil.Db);
            TempData["newsearch"] = ncid;
            return RedirectToAction("Main");
        }
        public ActionResult Main(Guid? id, int? run)
        {
            if (Fingerprint.UseNewLook())
                return Redirect("/Query");

            ViewData["Title"] = "QueryBuilder";
            ViewData["OnQueryBuilder"] = "true";
            ViewData["TagAction"] = "/QueryBuilder2/TagAll/";
            ViewData["UnTagAction"] = "/QueryBuilder2/UnTagAll/";
            ViewData["AddContact"] = "/QueryBuilder2/AddContact/";
            ViewData["AddTasks"] = "/QueryBuilder2/AddTasks/";

            var newsearchid = (Guid?)TempData["newsearch"];
            var m = new QueryModel2 { QueryId = id };
            m.SelectedId = m.TopClause.Id;
            if (m.TopClause.NewMatchAnyId.HasValue)
                newsearchid = m.TopClause.NewMatchAnyId;
            if (newsearchid.HasValue)
                ViewBag.NewSearchId = newsearchid.Value;

            DbUtil.LogActivity("QueryBuilder");
            if (run.HasValue)
                m.ShowResults = true;
            ViewBag.queryid = m.TopClause.Id;
            ViewBag.AutoRun = (bool?)(TempData["AutoRun"]) == true;
            return View(m);
        }
        [HttpPost]
        public JsonResult SelectCondition(Guid id, string ConditionName)
        {
            var m = new QueryModel2 { ConditionName = ConditionName, SelectedId = id };

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
            var m = new QueryModel2 { Comparison = Comparison, ConditionName = ConditionName };
            return Json(new
            {
                CodesVisible = m.CodesVisible,
                CodeVisible = m.CodeVisible,
                CodeData = m.CodeData,
                SelectMultiple = m.SelectMultiple
            });
        }
        [HttpPost]
        public ActionResult EditCondition(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.EditCondition();
            var j = JsonConvert.SerializeObject(m, Formatting.Indented);
            return Content(j);
        }

        [HttpPost]
        public ActionResult AddToGroup(QueryModel2 m)
        {
            if (Validate(m))
                m.AddConditionToGroup();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Add(QueryModel2 m)
        {
            if (Validate(m))
                m.AddConditionAfterCurrent();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Update(QueryModel2 m)
        {
            if (Validate(m))
                m.UpdateCondition();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public JsonResult Remove(QueryModel2 m)
        {
            m.DeleteCondition();
            return Json(m);
        }
        [HttpPost]
        public ActionResult InsGroupAbove(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            //m.InsertGroupAbove();
            var c = new ContentResult();
            c.Content = m.QueryId.ToString();
            return c;
        }
        [HttpPost]
        public ActionResult CopyAsNew(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            //m.CopyAsNew();
            var c = new ContentResult();
            c.Content = m.QueryId.ToString();
            return c;
        }
        public ActionResult Conditions()
        {
            var m = new QueryModel2();
            return View(m);
        }
        [HttpPost]
        public JsonResult GetDivisions(int id)
        {
            var m = new QueryModel2();
            return Json(new { Divisions = m.Divisions(id), Organizations = m.Organizations(0) });
        }
        [HttpPost]
        public JsonResult GetOrganizations(int id)
        {
            var m = new QueryModel2();
            return Json(m.Organizations(id));
        }
        [HttpPost]
        public JsonResult SavedQueries()
        {
            var m = new QueryModel2();
            return Json(m.SavedQueries()); ;
        }
        [HttpPost]
        public ActionResult SaveQuery(Guid QueryId, string SavedQueryDesc, bool IsPublic)
        {
            var m = new QueryModel2() {QueryId = QueryId};
//            var ret = m.SaveQuery();
//            if (ret.HasValue())
//                return Content(ret);
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
        public ActionResult Results(QueryModel2 m)
        {
            var cb = new SqlConnectionStringBuilder(Util.ConnectionString);
            cb.ApplicationName = "qb";
            var starttime = DateTime.Now;
#if DEBUG
            m.PopulateResults();
#else
            try
            {
                m.PopulateResults();
            }
            catch (Exception ex)
            {
                var errorLog = ErrorLog.GetDefault(null);
                errorLog.Log(new Error(ex));
                return Content("Something went wrong<br><p>" + ex.Message + "</p>");
            }
#endif
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
            var m = new QueryModel2();
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
            m.UnTagAll();
            var c = new ContentResult();
            c.Content = "Add";
            return c;
        }
        [HttpPost]
        public ContentResult AddContact()
        {
            var m = new QueryModel2();
            var cid = CmsData.Contact.AddContact(m.QueryId.Value);
            return Content("/Contact/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks()
        {
            var m = new QueryModel2();
            var c = new ContentResult();
            c.Content = Task.AddTasks(m.QueryId.Value).ToString();
            return c;
        }

        private bool Validate(QueryModel2 m)
        {
            m.Errors = new Dictionary<string, string>();
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
    }
}
