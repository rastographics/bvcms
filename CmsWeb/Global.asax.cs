using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using UtilityExtensions;
using CmsWeb;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using CmsWeb.Areas.Manage.Controllers;
using System.Web.Caching;
using Elmah;
using System.Web.Security;
using Thinktecture.IdentityModel.Http.Cors.Mvc;

namespace CmsWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        //        private void RegisterCors(MvcCorsConfiguration corsConfig)
        //        {
        //            corsConfig
        //                .ForResources("APIMeta")
        //                .AllowAllOrigins()
        //                .AllowAllMethods()
        //                .AllowCookies()
        //                .AllowRequestHeaders("Content-Type", "Authorization");
        //        }
        protected void Application_Start()
        {
            ModelBinders.Binders.DefaultBinder = new SmartBinder();
            ModelMetadataProviders.Current = new ModelViewMetadataProvider();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteTable.Routes.RouteExistingFiles = true;
            HttpRuntime.Cache.Remove("BuildDate");
            //            RegisterCors(MvcCorsConfiguration.Configuration);
#if DEBUG
            //HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlProfiler.Initialize();
#endif
        }
//        public static void RegisterRoutes(RouteCollection routes)
//        {
//            routes.IgnoreRoute("Demo/{*pathInfo}");
//            routes.IgnoreRoute("index.html");
//            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
//            routes.IgnoreRoute("{myWebForms}.aspx/{*pathInfo}");
//            routes.IgnoreRoute("{myWebForms}.ashx/{*pathInfo}");
//            routes.IgnoreRoute("{myWebForms}.asmx/{*pathInfo}");
//            routes.IgnoreRoute("{myWebForms}.js/{*pathInfo}");
//            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
//            routes.IgnoreRoute("*/pagerror.gif");
//            routes.IgnoreRoute("*/refresh.gif");
//            routes.IgnoreRoute("{name}.png");
//            routes.IgnoreRoute("Admin/{*pathInfo}");
//            routes.IgnoreRoute("AppReview/{*pathInfo}");
//            routes.IgnoreRoute("CustomErrors/{*pathInfo}");
//            routes.IgnoreRoute("Contributions/{*pathInfo}");
//            routes.IgnoreRoute("Report/{*pathInfo}");
//            routes.IgnoreRoute("Dialog/{*pathInfo}");
//            routes.IgnoreRoute("ckeditor/{*pathInfo}");
//            routes.IgnoreRoute("StaffOnly/{*pathInfo}");
//            routes.IgnoreRoute("images/{*pathInfo}");
//            routes.IgnoreRoute("App_Themes/{*pathInfo}");
//            routes.IgnoreRoute("Content/{*pathInfo}");
//            routes.IgnoreRoute("Errors/{*pathInfo}");
//            routes.IgnoreRoute("demos/{*pathInfo}");
//            routes.IgnoreRoute("Scripts/{*pathInfo}");
//            routes.IgnoreRoute("Upload/{*pathInfo}");
//            routes.IgnoreRoute("{myWebPage}.htm");
//            routes.IgnoreRoute("{myReport}.rdlc");
//            routes.IgnoreRoute("{dir1}/{dir2}/{file}.js");
//            routes.IgnoreRoute("{dir1}/{dir2}/{file}.css");
//            routes.IgnoreRoute("elmah.axd");
//            routes.MapMvcAttributeRoutes();
//            routes.MapRoute(
//                "Default", // Route name
//                "{controller}/{action}/{id}", // URL with parameters
//                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
//            );
//        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (Util.Host.StartsWith("direct"))
                return;
            if (User.Identity.IsAuthenticated)
            {
                if (!DbUtil.DatabaseExists())
                {
#if DEBUG
                    DbUtil.CreateDatabase();
#else
                    Response.Redirect("/Errors/DatabaseNotFound.aspx?dbname=" + Util.Host);
                    return;
#endif
                }
                if (1 == 1) // should be 1 == 1 (or just true) to run normally
                    Models.AccountModel.SetUserInfo(Util.UserName, Session);
                else
                    Models.AccountModel.SetUserInfo("trecord", Session);
            }
            Util.SysFromEmail = ConfigurationManager.AppSettings["sysfromemail"];
            Util.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Util.SessionStarting = true;
            LogBrowser();
        }

        private void LogBrowser()
        {
            var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
            if (cs != null)
            {
                var cn = new SqlConnection(cs.ConnectionString);
                cn.Open();
                var cmd = new SqlCommand("LogBrowser", cn) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("browser", Request.Browser.Type);
                cmd.Parameters.AddWithValue("who", Util.UserName);
                cmd.Parameters.AddWithValue("host", Util.Host);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var url = Request.Url.OriginalString;
            if (url.Contains("/Errors/") || url.Contains("healthcheck.txt"))
                return;

            if (Util.AppOffline)
            {
                Response.Redirect("/Errors/AppOffline.htm");
                return;
            }
            if (!DbUtil.DatabaseExists())
            {
#if DEBUG
                DbUtil.CreateDatabase();
#else
                Response.Redirect("/Errors/DatabaseNotFound.aspx?dbname=" + Util.Host);
                return;
#endif
            }

            Util.AdminMail = DbUtil.Db.Setting("AdminMail", "");

            var cul = DbUtil.Db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current != null)
                DbUtil.DbDispose();
            if (Response.Status.StartsWith("401")
                    && Request.Url.AbsolutePath.EndsWith(".aspx"))
            {
                var r = Models.AccountModel.CheckAccessRole(User.Identity.Name);
                if (r.HasValue())
                    Response.Redirect(r);
            }
        }

        public void ErrorLog_Logged(object sender, ErrorLoggedEventArgs args)
        {
            HttpContext.Current.Items["error"] = args.Entry.Error.Exception.Message;
        }

        public void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            Filter(e);
        }

        public void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            Filter(e);
        }

        private void Filter(ExceptionFilterEventArgs e)
        {
            var ex = e.Exception.GetBaseException();
            var httpex = ex as HttpException;

            if (httpex != null)
            {
                if (httpex.GetHttpCode() == 404)
                    e.Dismiss();
                else if (httpex.Message.Contains("The remote host closed the connection"))
                    e.Dismiss();
                else if (httpex.Message.Contains("A potentially dangerous Request.Path value was detected from the client"))
                    e.Dismiss();
            }
            if (ex is FileNotFoundException || ex is HttpRequestValidationException)
                e.Dismiss();
        }
    }
}