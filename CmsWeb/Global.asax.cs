using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using Dapper;
using Elmah;
using UtilityExtensions;

namespace CmsWeb
{
    public class MvcApplication : HttpApplication
    {

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            ModelBinders.Binders.DefaultBinder = new SmartBinder();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(int?), new NullableIntModelBinder());
            ModelMetadataProviders.Current = new ModelViewMetadataProvider();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteTable.Routes.RouteExistingFiles = true;
            HttpRuntime.Cache.Remove("BuildDate");

            //Remove and JsonValueProviderFactory and add JsonNetValueProviderFactory
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());

//            MiniProfiler.Settings.Results_List_Authorize = IsAuthorizedToViewProfiler;
//            MiniProfiler.Settings.Results_Authorize = IsAuthorizedToViewProfiler;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (ShouldBypassProcessing()) return;

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
            Util.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Util.SessionStarting = true;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ShouldBypassProcessing()) return;

            if (Util.AppOffline)
            {
                Response.Redirect("/Errors/AppOffline.htm");
                return;
            }

//            MiniProfiler.Start();

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
                Util.AdminMail = CurrentDatabase.Setting("AdminMail", "");
                Util.DateSimulation = CurrentDatabase.Setting("UseDateSimulation");
            }
            catch (SqlException)
            {
                throw;
                //Response.Redirect($"/Errors/DatabaseNotInitialized.aspx?dbname={Util.Host}");
            }

            var cul = CurrentDatabase.Setting("Culture", "en-US");
            Util.Culture = cul;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            var checkip = ConfigurationManager.AppSettings["CheckIp"];
            if(Util.IsHosted && checkip.HasValue())
                if (1 == CurrentDatabase.Connection.ExecuteScalar<int>(checkip, new {ip = Request.UserHostAddress}))
                    Response.Redirect("/Errors/AccessDenied.htm");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }


        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (ShouldBypassProcessing()) return;

            if (HttpContext.Current != null)
                DbDispose();
            if (Response.Status.StartsWith("401")
                    && Request.Url.AbsolutePath.EndsWith(".aspx"))
            {
                var r = AccountModel.CheckAccessRole(User.Identity.Name);
                if (r.HasValue())
                    Response.Redirect(r);
            }

//            MiniProfiler.Stop();
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (ShouldBypassProcessing()) return;

//            if (!IsAuthorizedToViewProfiler(Request))
//                MiniProfiler.Stop(discardResults: true);
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
            else if (IsEmailBodyError(ex))
                e.Dismiss();
            FilterOutSensitiveFormData(e);
        }

        private static void FilterOutSensitiveFormData(ExceptionFilterEventArgs e)
        {
            var ctx = e.Context as HttpContext;
            if (ctx == null)
                return;

            var sensitiveFormKeys = new[] { "creditcard", "ccv", "cvv", "cardnumber", "cardcode", "password", "account", "routing" };

            var sensitiveFormData = ctx.Request.Form.AllKeys.Where(
                k => sensitiveFormKeys.Contains(k.ToLower(), StringComparer.OrdinalIgnoreCase)).ToList();

            if (!sensitiveFormData.Any() || Util.IsDebug())
                return;

            var error = new Error(e.Exception, ctx);
            sensitiveFormData.ForEach(k => error.Form.Set(k, "*****"));
            ErrorLog.GetDefault(ctx).Log(error);
            e.Dismiss();
        }

        void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (!IsEmailBodyError(ex))
                return;
            var httpContext = ((MvcApplication)sender).Context;
            httpContext.ClearError();
        }

        private bool IsEmailBodyError(Exception ex)
        {
            if (ex is ArgumentException && ex.Message.Contains("EmailBody(Int32)"))
            {
                var a = Request.Path.Split('/');
                if (a.Length > 0 && !a[a.Length - 1].AllDigits())
                    return true;
            }
            return false;
        }

//        private static bool IsAuthorizedToViewProfiler(HttpRequest request)
//        {
//            if (request.IsLocal)
//                return false;
//
//            var ctx = request.RequestContext.HttpContext;
//            if (ctx?.User == null)
//                return false;
//
//            return ctx.User.IsInRole("Developer") && CurrentDatabase.Setting("MiniProfileEnabled");
//        }

        private bool ShouldBypassProcessing()
        {
            var url = Request.Url.OriginalString;

            return url.Contains("/Errors/", ignoreCase: true) ||
                url.Contains("/Content/touchpoint/", ignoreCase: true) ||
                url.Contains("healthcheck.txt", ignoreCase: true) ||
                url.Contains("favicon.ico", ignoreCase: true);
        }
    }
}
