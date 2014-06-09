using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using System.IO;
using System.Threading;
using System.Globalization;
using Elmah;

namespace CmsWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            ModelBinders.Binders.DefaultBinder = new SmartBinder();
            ModelMetadataProviders.Current = new ModelViewMetadataProvider();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteTable.Routes.RouteExistingFiles = true;
            HttpRuntime.Cache.Remove("BuildDate");
#if DEBUG
            //HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlProfiler.Initialize();
#endif
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (Util.Host.StartsWith("direct"))
                return;
            if (User.Identity.IsAuthenticated)
            {
                if (!DbUtil.CmsDatabaseExists())
                {
                    Response.Redirect("/Errors/DatabaseNotFound.aspx?dbname=" + Util.Host);
                    return;
                }
                Models.AccountModel.SetUserInfo(Util.UserName, Session);
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
                var cmd = new SqlCommand("LogBrowser", cn) { CommandType = CommandType.StoredProcedure };
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
            if (!DbUtil.CmsDatabaseExists())
            {
#if DEBUG
                var r = DbUtil.CheckDatabaseExists(Util.Host);
                if (r == DbUtil.CheckDatabaseResult.ServerNotFound)
                {
                    Response.Redirect("/Errors/DatabaseServerNotFound.aspx?server=" + Util.DbServer);
                    return;
                }
                var ret = DbUtil.CreateDatabase();
                if (ret.HasValue())
                {
                    Response.Redirect("/Errors/DatabaseCreationError.aspx?error=" + HttpUtility.UrlEncode(ret));
                    return;
                }
#else
                Response.Redirect("/Errors/DatabaseNotFound.aspx?dbname=" + Util.Host);
                return;
#endif
            }
            try
            {
                Util.AdminMail = DbUtil.Db.Setting("AdminMail", "");
            }
            catch (SqlException)
            {
                Response.Redirect("/Errors/DatabaseNotInitialized.aspx?dbname=" + Util.Host);
            }

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

        public void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var ex = e.Exception.GetBaseException();
            var httpex = ex as HttpException;

            if (httpex != null)
            {
                var status = httpex.GetHttpCode();
                if (status == 400 || status == 404)
                    e.Dismiss();
                else if (httpex.Message.Contains("The remote host closed the connection"))
                    e.Dismiss();
                else if (httpex.Message.Contains("A potentially dangerous Request.Path value was detected from the client"))
                    e.Dismiss();
            }
            if (ex is FileNotFoundException || ex is HttpRequestValidationException)
                e.Dismiss();
        }
        public void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var ex = e.Exception.GetBaseException();
            var httpex = ex as HttpException;
            if (httpex != null)
            {
                var status = httpex.GetHttpCode();
                if (status == 400 || status == 404)
                    e.Dismiss();
                else if (httpex.Message.Contains("The remote host closed the connection"))
                    e.Dismiss();
                else if (
                    httpex.Message.Contains(
                        "A potentially dangerous Request.Path value was detected from the client"))
                    e.Dismiss();
            }
            if (ex is FileNotFoundException || ex is HttpRequestValidationException)
                e.Dismiss();
        }
    }
}