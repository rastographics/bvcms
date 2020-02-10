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

        [HttpGet]
        public ActionResult Index()
        {
            if (CurrentDatabase.Setting("EnableWebCheckin"))
            {
                return View();
            }
            else
            {
                return Content("Web Checkin isn't enabled.");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            return View();
        }
    }
}
