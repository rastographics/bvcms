using CmsWeb.Areas.Manage.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Activity"), Route("{action}")]
    public class ActivityController : CmsStaffController
    {
        public ActivityController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/LastActivity")]
        public ActionResult Index(int? userid, int? peopleid, int? orgid)
        {
            var m = new ActivityModel
            {
                UserId = userid,
                PeopleId = peopleid,
                OrgId = orgid,
            };
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(ActivityModel m)
        {
            return View(m);
        }
    }
}
