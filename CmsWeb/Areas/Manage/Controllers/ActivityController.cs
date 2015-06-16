using System.Web.Mvc;
using CmsWeb.Areas.Manage.Models;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Activity"), Route("{action}")]
    public class ActivityController : CmsStaffController
    {
        [HttpGet, Route("~/LastActivity")]
        public ActionResult Index(int? userid, string machine, int? peopleid, int? orgid)
        {
            var m = new ActivityModel
            {
                UserId = userid, 
                Machine = machine, 
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
