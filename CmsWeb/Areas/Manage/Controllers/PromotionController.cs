using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Promotion"), Route("{action=index}/{id?}")]
    public class PromotionController : CmsStaffController
    {
        public PromotionController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
        [HttpGet]
        public ActionResult Export(int id)
        {
            return new PromotionModel(id).Export().ToExcel("promotion.xlsx");
        }

    }
}
