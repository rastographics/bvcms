using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "PictureDirectory"), Route("{action=index}")]
    public class PictureDirectoryController : CmsController
    {
        public PictureDirectoryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/PictureDirectory/{id?}")]
        public ActionResult Index(string id = null)
        {
            ViewBag.Controller = this;
            return View("Index", new PictureDirectoryModel(id));
        }
        [HttpPost]
        public ActionResult Results(PictureDirectoryModel m)
        {
            m.Initialize();
            if (m.TemplateName.HasValue() && (m.CanView == true || User.IsInRole("Admin")))
            {
                return Content(m.Results(this));
            }

            return Content("unauthorized");
        }
    }
}
