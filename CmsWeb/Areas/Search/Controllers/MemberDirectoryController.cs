using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Search.Models;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Org", AreaPrefix="MemberDirectory"), Route("{action=index}")]
    public class MemberDirectoryController : CmsController
    {
        [Route("~/PictureDirectory")]
        [Route("~/PictureDirectory/Index")]
        public ActionResult Index()
        {
            return View(new MemberDirectoryModel(this));
            //return RedirectToAction("NoAccess");
        }
        [HttpPost]
        public ActionResult Results(MemberDirectoryModel m)
        {
            if (User.IsInRole("Admin") ||
                DbUtil.Db.OrganizationMembers.Any(
                    mm => mm.OrganizationId == m.OrgId && mm.PeopleId == UtilityExtensions.Util.UserPeopleId))
                return View(m);
            return Content("unauthorized");
        }
        public ActionResult NoAccess()
        {
            return View("NoAccess");
        }
    }
}
