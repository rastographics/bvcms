using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CmsWeb.Pushpay;
using CmsWeb.Common;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Pushpay"), Route("{action=index}/{id?}")]
    public class PushpayController : CmsStaffController
    {
        /// <summary>
        ///     Opens the developer console in a separate VIEW
        /// </summary>
        /// <returns></returns>
        public ActionResult DeveloperConsole()
        {
            return View();
        }
		
        /// <summary>
        ///     Entry point / home page into the application
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string redirectUrl = Configuration.Current.OAuth2AuthorizeEndpoint
                + "?client_id="+Configuration.Current.PushpayClientID
                + "&response_type=code"
                +"&redirect_uri="+Configuration.Current.OrgBaseRedirect
                + "&scope=read";
            
            return Redirect(redirectUrl);
        }

        public ActionResult Complete()
        {
            return View();
        }
            /// <summary>
            ///     A landing page where the user can get started with the API
            /// </summary>
            /// <returns></returns>
            public ActionResult Start()
        {
            return View();
        }
    }
}
