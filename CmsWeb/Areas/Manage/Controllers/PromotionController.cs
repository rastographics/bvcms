using System.Web.Mvc;
using CmsWeb.Models;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix= "Promotion"), Route("{action=index}/{id?}")]
    public class PromotionController : CmsStaffController
    {
        public ActionResult Index()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignPending()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            m.AssignPending();
            return RedirectToAction("Index");
        }
        public ActionResult List()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            return PartialView("List", m);
        }
        public ActionResult Export()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            return new ExcelResult(m.Export(), "promotion.xls");
        }
    }
}
