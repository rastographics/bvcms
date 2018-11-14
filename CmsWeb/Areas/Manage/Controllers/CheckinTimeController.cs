using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage")]
    public class CheckinTimeController : CmsController
    {
        public CheckinTimeController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/CheckinTimes")]
        public ActionResult Index(CheckinTimeModel m)
        {
            if (m.Locations().Count == 0)
            {
                return Content("Building Checkin mode not setup, no checkin times available");
            }

            UpdateModel(m.Pager);
            return View(m);
        }
    }
}
