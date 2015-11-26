using System;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;
using Dapper;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "SavedQuery"), Route("{action}/{id?}")]
    public class SavedQueryController : CmsStaffController
    {
        [HttpGet, Route("~/SavedQueryList")]
        public ActionResult Index()
        {
            var m = new SavedQueryModel
            {
                OnlyMine = DbUtil.Db.UserPreference("SavedQueryOnlyMine", "false").ToBool()
            };
            //m.Pager.Set("/SavedQuery/Results", 1, null, "Last Run", "desc");
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SavedQueryModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult Edit(Guid id)
        {
            var m = new SavedQueryInfo(id);
            if (m.Name.Equals(Util.ScratchPad2))
                m.Name = "copy of scratchpad";
            return View(m);
        }
        [HttpPost]
        public ActionResult Update(SavedQueryInfo m)
        {
            if (m.Name.Equal(Util.ScratchPad2))
                m.Ispublic = false;
            m.CanDelete = true; // must be true since they can edit if they got here
            m.UpdateModel();
            return View("Row", m);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var q = DbUtil.Db.LoadQueryById2(id);
            DbUtil.Db.Queries.DeleteOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }

        const string SqlSavedqueries = @"
SELECT 
	QueryId,
    owner ,
    name
FROM dbo.Query
WHERE name IS NOT NULL
AND name <> 'scratchpad'
AND text not like '%AnyFalse%'
ORDER BY lastRun DESC
";
        [HttpGet]
        public ActionResult UpdateAll()
        {
            var db = DbUtil.Db;
            var list = db.Connection.Query(SqlSavedqueries).ToList();
            foreach (var sq in list)
            {
                var g = sq.QueryId as Guid?;
                if (!g.HasValue)
                    continue;
                DbUtil.DbDispose();
                var c = DbUtil.Db.LoadExistingQuery(g.Value);
                var s = ExportQuery.ToString(c);
                if(s.HasValue())
                    UpdateQueryConditions.Run(sq.QueryId);
            }
            return RedirectToAction("Code");
        }
        [HttpGet]
        public ActionResult Code()
        {
            var q = DbUtil.Db.Connection.Query(SqlSavedqueries);
            ViewBag.Count = q.Count();
            return View(q);
        }

    }
}
