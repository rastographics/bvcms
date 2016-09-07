using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Search.Models;
using System.Text.RegularExpressions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix="PictureDirectory"), Route("{action=index}")]
    public class PictureDirectoryController : CmsController
    {
        [Route("~/PictureDirectory")]
        [Route("~/PictureDirectory/Index")]
        public ActionResult Index()
        {
            var flag = DbUtil.Db.Setting("PictureDirectoryStatusFlag", "");
            var match = Regex.IsMatch(flag, @"\AF\d\d\z");
            if (!match)
                return Message("No PictureDirectory Configured");
            ViewBag.Controller = this;
            return View(new PictureDirectoryModel());
        }
        [HttpPost]
        public ActionResult Results(PictureDirectoryModel m)
        {
            if (m.CanView || User.IsInRole("Admin"))
                return Content(m.Results(this));
            return Content("unauthorized");
        }
        public ActionResult NoAccess()
        {
            return View("NoAccess");
        }
    }
}
