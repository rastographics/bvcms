using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Areas.CheckIn.Controllers
{
    public class CheckInController : CMSBaseController
    {
        public CheckInController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/CheckIn")]
        public ActionResult CheckIn()
        {
            if (CurrentDatabase.Setting("EnableWebCheckin"))
            {
                return View("~/Areas/CheckIn/Views/CheckIn/CheckIn.cshtml");
            }
            else
            {
                return Content("Web Checkin isn't enabled.");
            }
        }

        [HttpGet, Route("~/CheckIn/Logout")]
        public ActionResult Logout()
        {
            return View("~/Areas/CheckIn/Views/CheckIn/LogOut.cshtml");
        }
    }
}
