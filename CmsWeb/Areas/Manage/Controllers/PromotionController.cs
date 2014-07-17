using System.Web.Mvc;
using CmsWeb.Models;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix= "Promotion"), Route("{action=index}/{id?}")]
    public class PromotionController : CmsStaffController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var m = new PromotionModel();
            return View(m);
        }
        [HttpGet]
        [Route("~/Promotion/Reload")]
        public ActionResult Reload(PromotionModel m)
        {
            return Redirect("/Promotion");
        }

        [HttpPost]
        public ActionResult AssignPending(PromotionModel m)
        {
            m.AssignPending();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult List(PromotionModel m)
        {
            return View("List", m);
        }
        [HttpPost]
        public ActionResult Export(PromotionModel m)
        {
            return new ExcelResult(m.Export(), "promotion.xls");
        }
    }
}
