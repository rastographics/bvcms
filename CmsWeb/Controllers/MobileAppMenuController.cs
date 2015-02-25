using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Controllers
{
    public class MobileAppMenuController : Controller
    {
        private const string MOBILE_APP_RETURN_URL = "bvcmsapp://";

        public static string Source
        {
            get
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                {
                    if (System.Web.HttpContext.Current.Session["source"] == null ||
                        string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session["source"].ToString()))
                    {
                        // check within querystring.
                        if (System.Web.HttpContext.Current.Request.QueryString["source"] != null &&
                            !string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Request.QueryString["source"]))
                        {
                            // set session variable
                            System.Web.HttpContext.Current.Session["source"] = System.Web.HttpContext.Current.Request.QueryString["source"];
                            return System.Web.HttpContext.Current.Session["source"].ToString();  
                        }
                    }
                    else
                        return System.Web.HttpContext.Current.Session["source"].ToString();                    
                }
                return string.Empty;
            }
            set { System.Web.HttpContext.Current.Session["source"] = value; }
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