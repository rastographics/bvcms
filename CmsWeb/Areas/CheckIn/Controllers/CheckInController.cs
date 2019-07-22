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
            // todo: pass list of profiles to the view and use it to populate sign in dropdown
            return View("~/Areas/CheckIn/Views/CheckIn/CheckIn.cshtml");
        }
    }
}
