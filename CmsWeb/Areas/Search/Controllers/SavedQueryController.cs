using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;
using CmsData;

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

        [HttpPost]
        public ActionResult Code(SavedQueryModel m)
        {
            var qlist = from q in m.DefineModelList()
                       select q.QueryId;
            var list = string.Join(",", qlist);
            TempData["codelist"] = list;
            return Content("ok");
        }
        [HttpGet]
        public ActionResult Code()
        {
            var list = TempData["codelist"] as string;
            if (list == null)
                return Content("no data");
            var guids = list.Split(',').ToList().ConvertAll(vv => new Guid(vv));
            return View(new QueryCodeModel(CodeSql.Queries, guids));
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
        [HttpGet]
        public ActionResult UpdateAll()
        {
            var list = QueryCodeModel.DatabaseList();
            foreach (var db in list)
                QueryCodeModel.UpdateAll(db);
            return Content("done");
        }
#endif
    }
}
