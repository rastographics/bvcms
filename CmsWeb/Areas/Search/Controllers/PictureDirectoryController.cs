using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix="PictureDirectory"), Route("{action=index}")]
    public class PictureDirectoryController : CmsController
    {
        [Route("~/PictureDirectory/{id?}")]
        public ActionResult Index(string id = null)
        {
#if DEBUG
            DbUtil.Db.ExecuteCommand(@"
DELETE dbo.Content 
WHERE (name = 'PictureDirectorySql' AND TypeID = 4)
OR (name = 'PictureDirectory' AND TypeID = 4)
OR (name = 'PictureDirectoryTemplate' AND TypeID = 1)
");
#endif
            ViewBag.Controller = this;
            return View(new PictureDirectoryModel(id));
        }
        [HttpPost]
        public ActionResult Results(PictureDirectoryModel m)
        {
            m.Initialize();
            if (m.TemplateName.HasValue() && (m.CanView == true || User.IsInRole("Admin")))
                return Content(m.Results(this));
            return Content("unauthorized");
        }
        public ActionResult NoAccess()
        {
            return View("NoAccess");
        }
    }
}
