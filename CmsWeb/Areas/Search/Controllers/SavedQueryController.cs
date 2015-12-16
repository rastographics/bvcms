using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
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

        internal const string SqlSavedqueries = @"
SELECT 
	QueryId,
    owner ,
    name,
    text
FROM dbo.Query q
join QueryAnalysis qa ON qa.Id = q.QueryId
where name >= 'YS-SR Teachers, Co-Teachers, Asst Teachers, Outreach Leaders'
ORDER BY name
/*
SELECT 
	QueryId,
    owner ,
    name,
    text
FROM dbo.Query q
WHERE name IS NOT NULL
AND name <> 'scratchpad'
AND text not like '%AnyFalse%'
*/
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
                Debug.WriteLine($"{sq.name}");
                DbUtil.DbDispose();
                try
                {
                    var c = DbUtil.Db.LoadExistingQuery(g.Value);
                    var s = c.ToCode();
                    if (s.HasValue())
                    {
                        UpdateQueryConditions.Run(sq.QueryId);
                        DbUtil.Db.Connection.Execute(@"UPDATE QueryAnalysis set Updated = 1 where Id = @id", new { id=sq.QueryId });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return RedirectToAction("Code");
        }
        [HttpGet]
        public ActionResult Code()
        {
            return View(new QueryCodeModel(CodeSql.Queries));
        }
#if DEBUG
        [HttpGet]
        public ActionResult CodeErrors()
        {
            return View("Code", new QueryCodeModel(CodeSql.Errors));
        }
        [HttpGet]
        public ActionResult CodeAnalysis()
        {
            QueryCodeModel.DoAnalysis();
            return Redirect("Code");
        }
        [HttpGet]
        public ActionResult CodeAnalysisAll()
        {
            var list = QueryCodeModel.DatabaseList();
            var sb = new StringBuilder();
            foreach (var db in list)
                sb.Append(QueryCodeModel.DoAnalysis(db));
            return Content(sb.ToString(), "text/plain");
        }
#endif
    }
}
