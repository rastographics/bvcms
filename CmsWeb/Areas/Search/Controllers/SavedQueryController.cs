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
    [RouteArea("Search", AreaUrl = "SavedQuery2")]
    public class SavedQueryController : CmsStaffController
    {
        [GET("SavedQuery2/")]
        public ActionResult Index()
        {
            var m = new SavedQueryModel();
            m.Pager.Set("/SavedQuery2/Results", 1, null, "Last Updated", "desc");
            return View(m);
        }
        [POST("SavedQuery2/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, SavedQueryModel m)
        {
            m.Pager.Set("/SavedQuery2/Results", page, size, sort, dir);
            return View(m);
        }
        [POST("SavedQuery2/Edit/{id:guid}")]
        public ActionResult Edit(Guid id)
        {
            var m = new SavedQueryInfo(id);
            return View(m);
        }
        [POST("SavedQuery2/Update")]
        public ActionResult Update(SavedQueryInfo m)
        {
            m.UpdateModel();
            return View("Row", m);
        }
        [POST("SavedQuery2/Delete/{id:guid}")]
        public ActionResult Delete(Guid id)
        {
            var q = DbUtil.Db.LoadQueryById2(id);
            DbUtil.Db.Queries.DeleteOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
//        [POST("SavedQuery2/PostPublic")]
//        public ActionResult PostPublic(int pk, string value)
//        {
//            var c = DbUtil.Db.QueryBuilderClauses.SingleOrDefault(cc => cc.QueryId == pk);
//            c.IsPublic = value == "1";
//            DbUtil.Db.SubmitChanges();
//            return Content(value);
//        }
    }
}
