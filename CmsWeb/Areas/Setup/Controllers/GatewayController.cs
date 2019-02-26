using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Lifecycle;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Gateway"), Route("{action}/{id?}")]
    public class GatewayController : CmsStaffController
    {
        public GatewayController(IRequestManager requestManager): base(requestManager)
        {

        }

        [Route("~/Gateway")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
