using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaUrl = "SavedQueryList")]
    public class SavedQueryController : CmsStaffController
    {
        [GET("SavedQueryList/")]
        public ActionResult Index()
        {
            if (!Fingerprint.UseNewLook())
                return Redirect("/SavedQuery");
            var m = new SavedQueryModel { OnlyMine = DbUtil.Db.UserPreference("SavedQueryOnlyMine", "false").ToBool() };
            m.Pager.Set("/SavedQueryList/Results", 1, null, "Last Run", "desc");
            return View(m);
        }
        [POST("SavedQueryList/Results/{page?}/{size?}/{sort=Last Run}/{dir=desc}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, SavedQueryModel m)
        {
            m.Pager.Set("/SavedQueryList/Results", page, size, sort, dir);
            return View(m);
        }
        [POST("SavedQueryList/Edit/{id:guid}")]
        public ActionResult Edit(Guid id)
        {
            var m = new SavedQueryInfo(id);
            return View(m);
        }
        [POST("SavedQueryList/Update")]
        public ActionResult Update(SavedQueryInfo m)
        {
            m.UpdateModel();
            return View("Row", m);
        }
        [POST("SavedQueryList/Delete/{id:guid}")]
        public ActionResult Delete(Guid id)
        {
            var q = DbUtil.Db.LoadQueryById2(id);
            DbUtil.Db.Queries.DeleteOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
        [GET("SavedQueryList/ExportToOld/{id:guid}")]
        public ActionResult ExportToOld(Guid id)
        {
            var q = DbUtil.Db.LoadQueryById2(id);
            var ret = QueryBuilderClause.Import(DbUtil.Db, q.ToClause().ToXml(), q.Name);
            return Redirect("/QueryBuilder/Main/" + ret.newid);
        }
//        [POST("SavedQueryList/PostPublic")]
//        public ActionResult PostPublic(int pk, string value)
//        {
//            var c = DbUtil.Db.QueryBuilderClauses.SingleOrDefault(cc => cc.QueryId == pk);
//            c.IsPublic = value == "1";
//            DbUtil.Db.SubmitChanges();
//            return Content(value);
//        }
    }
}
