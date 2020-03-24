using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class MobileAppMenuController : Controller
    {
        private const string MOBILE_APP_RETURN_URL = "bvcmsapp://";

        public static string Source
        {
            get
            {
                if (HttpContextFactory.Current != null)
                {
                    return Util.GetFromSession("source", HttpContextFactory.Current.Request.QueryString["source"]);                    
                }
                return string.Empty;
            }
            set { Util.SetValueInSession("source", value); }
        }

        public static bool InMobileAppMode
        {
            get { return !string.IsNullOrWhiteSpace(Source); }
        }

        public static string MobileAppReturnUrl
        {
            get { return MOBILE_APP_RETURN_URL; }
        }

        public ActionResult Index()
        {
            ViewBag.InMobileAppMode = InMobileAppMode;
            ViewBag.MobileAppReturnUrl = MobileAppReturnUrl;
            return PartialView();
        }
    }
}
