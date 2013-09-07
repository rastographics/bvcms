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
            m.Pager.Set("/SavedQuery2/Results", 1, null, "Description", "asc");
            return View(m);
        }
        [POST("SavedQuery2/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, SavedQueryModel m)
        {
            m.Pager.Set("/SavedQuery2/Results", page, size, sort, dir);
            return View(m);
        }
        [POST("SavedQuery2/Edit/{id:int}")]
        public ActionResult Edit(int id)
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
