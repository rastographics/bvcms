using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using Elmah;
using StackExchange.Profiling;
using UtilityExtensions;

namespace CmsWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            ModelBinders.Binders.DefaultBinder = new SmartBinder();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(int?), new NullableIntModelBinder());
            ModelMetadataProviders.Current = new ModelViewMetadataProvider();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteTable.Routes.RouteExistingFiles = true;
            HttpRuntime.Cache.Remove("BuildDate");

            //Remove and JsonValueProviderFactory and add JsonNetValueProviderFactory
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());

            MiniProfiler.Settings.Results_List_Authorize = IsAuthorizedToViewProfiler;
            MiniProfiler.Settings.Results_Authorize = IsAuthorizedToViewProfiler;

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (Util.Host.StartsWith("direct"))
                return;
            if (User.Identity.IsAuthenticated)
            {
                var r = DbUtil.CheckDatabaseExists(Util.CmsHost);
                var redirect = ViewExtensions2.DatabaseErrorUrl(r);
                if (redirect != null)
                {
                    Response.Redirect(redirect);
                    return;
                }
                AccountModel.SetUserInfo(Util.UserName, Session);
            }
            Util.SysFromEmail = ConfigurationManager.AppSettings["sysfromemail"];
            Util.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Util.SessionStarting = true;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var url = Request.Url.OriginalString;
            if (url.Contains("/Errors/") || url.Contains("/Content/touchpoint/") || url.Contains("healthcheck.txt"))
                return;

            if (Util.AppOffline)
            {
                Response.Redirect("/Errors/AppOffline.htm");
                return;
            }

            MiniProfiler.Start();

            var r = DbUtil.CheckDatabaseExists(Util.CmsHost);
            var redirect = ViewExtensions2.DatabaseErrorUrl(r);
#if DEBUG
            if (r == DbUtil.CheckDatabaseResult.ServerNotFound)
            {
                Response.Redirect(redirect);
                return;
            }
            if (r == DbUtil.CheckDatabaseResult.DatabaseDoesNotExist && HttpContext.Current.Request.Url.LocalPath.EndsWith("/"))
            {
                var ret = DbUtil.CreateDatabase();
                if (ret.HasValue())
                {
                    Response.Redirect($"/Errors/DatabaseCreationError.aspx?error={HttpUtility.UrlEncode(ret)}");
                    return;
                }
            }
#else
            if (redirect != null)
            {
                Response.Redirect(redirect);
                return;
            }
#endif
            try
            {
                Util.AdminMail = DbUtil.Db.Setting("AdminMail", "");
            }
            catch (SqlException)
            {
                Response.Redirect($"/Errors/DatabaseNotInitialized.aspx?dbname={Util.Host}");
            }

            var cul = DbUtil.Db.Setting("Culture", "en-US");
            Util.Culture = cul;
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

            MiniProfiler.Stop();
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (!IsAuthorizedToViewProfiler(Request))
                MiniProfiler.Stop(discardResults: true);
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

            FilterOutSensitiveFormData(e);
        }

        private static void FilterOutSensitiveFormData(ExceptionFilterEventArgs e)
        {
            var ctx = e.Context as HttpContext;
            if (ctx == null)
                return;

            var sensitiveFormKeys = new[] {"creditcard", "ccv", "cvv", "cardnumber", "cardcode", "password", "account", "routing"};

            var sensitiveFormData = ctx.Request.Form.AllKeys.Where(
                k => sensitiveFormKeys.Contains(k.ToLower(), StringComparer.OrdinalIgnoreCase)).ToList();

            if (!sensitiveFormData.Any() || Util.IsDebug())
                return;

            var error = new Error(e.Exception, ctx);
            sensitiveFormData.ForEach(k => error.Form.Set(k, "*****"));
            ErrorLog.GetDefault(ctx).Log(error);
            e.Dismiss();
        }

        private static bool IsAuthorizedToViewProfiler(HttpRequest request)
        {
            if (request.IsLocal)
                return false;

            var ctx = request.RequestContext.HttpContext;
            if (ctx?.User == null)
                return false;

            return ctx.User.IsInRole("Developer") && DbUtil.Db.Setting("MiniProfileEnabled", "false") == "true";
        }
    }
}
