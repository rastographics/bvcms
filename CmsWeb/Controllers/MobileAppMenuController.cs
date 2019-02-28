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
                if (HttpContextFactory.Current != null && HttpContextFactory.Current.Session != null)
                {
                    if (HttpContextFactory.Current.Session["source"] == null ||
                        string.IsNullOrWhiteSpace(HttpContextFactory.Current.Session["source"].ToString()))
                    {
                        // check within querystring.
                        if (HttpContextFactory.Current.Request.QueryString["source"] != null &&
                            !string.IsNullOrWhiteSpace(HttpContextFactory.Current.Request.QueryString["source"]))
                        {
                            // set session variable
                            HttpContextFactory.Current.Session["source"] = HttpContextFactory.Current.Request.QueryString["source"];
                            return HttpContextFactory.Current.Session["source"].ToString();  
                        }
                    }
                    else
                        return HttpContextFactory.Current.Session["source"].ToString();                    
                }
                return string.Empty;
            }
            set { HttpContextFactory.Current.Session["source"] = value; }
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
