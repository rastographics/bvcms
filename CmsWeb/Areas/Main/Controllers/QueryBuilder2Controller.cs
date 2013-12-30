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
using Elmah;
using Newtonsoft.Json;
using UtilityExtensions;
using CmsWeb.Models;
using CmsData;

namespace CmsWeb.Areas.Main.Controllers
{
    [SessionExpire]
    public class QueryBuilder2Controller : CmsStaffController
    {
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset(DbUtil.Db);
            qb.Save(DbUtil.Db);
            return RedirectToAction("Main");
        }

        public ActionResult Main(Guid? id, int? run)
        {
            if (ViewExtensions2.UseNewLook())
                return Redirect("/Query");
            if(!ViewExtensions2.TestSb2())
                return Redirect("/QueryBuilder");

            var newsearchid = (Guid?)TempData["newsearch"];
            var m = new QueryModel2 { QueryId = id };
            id = m.TopClause.Id;
            m.SelectedId = id;
            if (m.TopClause.NewMatchAnyId.HasValue)
                newsearchid = m.TopClause.NewMatchAnyId;
            if (newsearchid.HasValue)
                ViewBag.NewSearchId = newsearchid.Value;

            DbUtil.LogActivity("QueryBuilder");
            if (run.HasValue)
                m.ShowResults = true;
            ViewBag.queryid = id;
            ViewBag.Title = "QueryBuilder";
            ViewBag.OnQueryBuilder = "true";
            ViewBag.TagAction = "/QueryBuilder2/TagAll/" + id;
            ViewBag.UnTagAction = "/QueryBuilder2/UnTagAll/" + id;
            ViewBag.AddContact = "/QueryBuilder2/AddContact/" + id;
            ViewBag.AddTasks = "/QueryBuilder2/AddTasks/" + id;
            ViewBag.AutoRun = (bool?)(TempData["AutoRun"]) == true;
            return View(m);
        }
        [HttpPost]
        public ActionResult SelectCondition(Guid queryid, Guid selectedid, string conditionName)
        {
            var m = new QueryModel2
            {
                QueryId = queryid,
                SelectedId = selectedid,
                ConditionName = conditionName,
                CodeValue = "",
                CodeValues = new string[0],
                Program = 0,
                Comparison = "Equal"
            };
            m.SetVisibility();

            var j = JsonConvert.SerializeObject(m, Formatting.Indented);
            return Content(j);
        }
        [HttpPost]
        public ActionResult GetCodes(Guid queryid, Guid selectedid, string Comparison, string ConditionName)
        {
            var m = new QueryModel2
            {
                QueryId = queryid,
                SelectedId = selectedid,
                Comparison = Comparison, 
                ConditionName = ConditionName
            };
            m.SetVisibility();
            var j = new
            {
                m.CodesVisible,
                m.CodeVisible,
                m.CodeData,
                m.SelectMultiple
            };
            return Content(JsonConvert.SerializeObject(j, Formatting.Indented));
        }
        [HttpPost]
        public ActionResult EditCondition(Guid queryid, Guid selectedid)
        {
            var m = new QueryModel2 { QueryId = queryid, SelectedId = selectedid };
            m.EditCondition();
            var j = JsonConvert.SerializeObject(m, Formatting.Indented);
            return Content(j);
        }

        [HttpPost]
        public ActionResult AddToGroup(QueryModel2 m)
        {
            if (m.Validate())
                m.AddConditionToGroup();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Add(QueryModel2 m)
        {
            if (m.Validate())
                m.AddConditionAfterCurrent();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Update(QueryModel2 m)
        {
            if (m.Validate())
                m.UpdateCondition();
            return PartialView("TryConditions", m);
        }
        [HttpPost]
        public ActionResult Remove(QueryModel2 m)
        {
            m.DeleteCondition();
            return Content(JsonConvert.SerializeObject(m));
        }
        [HttpPost]
        public ActionResult InsGroupAbove(Guid id)
        {
            var m = new QueryModel2 { SelectedId = id };
            m.InsertGroupAbove();
            return Content(m.QueryId.ToString());
        }
        [HttpPost]
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
        public ActionResult SaveQuery(string SavedQueryDesc, bool IsPublic)
        {
            var m = new QueryModel2() { SavedQueryDesc = SavedQueryDesc, IsPublic = IsPublic };
            var ret = m.SaveQuery();
            if (ret.HasValue())
                return Content(ret);
            return Content(m.Description);
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
        public ContentResult TagAll(Guid id, string tagname, bool? cleartagfirst)
        {
            if (!tagname.HasValue())
                return Content("error: no tag name");
            var m = new QueryModel2() { QueryId = id };
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
        public ContentResult UnTagAll(Guid id)
        {
            var m = new QueryModel2() { QueryId = id };
            m.UnTagAll();
            return Content("Add");
        }
        [HttpPost]
        public ContentResult AddContact(Guid id)
        {
            var cid = CmsData.Contact.AddContact(id);
            return Content("/Contact/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks(Guid id)
        {
            return Content(Task.AddTasks(id).ToString());
        }

    }
}
