using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using Dapper;
using Elmah;
using System;
using System.Configuration;
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
using UtilityExtensions;
using CMSImageDataContext = ImageData.CMSImageDataContext;

namespace CmsWeb
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DependencyConfig.RegisterSimpleInjector();

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

            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());
            if (Util.IsDebug())
            {
                DbUtil.Migrate();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (ShouldBypassProcessing())
            {
                return;
            }

            if (Request.Url.Authority.StartsWith("direct"))
            {
                return;
            }

            if (User.Identity.IsAuthenticated)
            {
                var host = CMSDataContext.GetHost(new HttpContextWrapper(Context));
                var r = DbUtil.CheckDatabaseExists($"CMS_{host}");
                var redirect = ViewExtensions2.DatabaseErrorUrl(r);
                if (redirect != null)
                {
                    Response.Redirect(redirect);
                    return;
                }

                var db = CMSDataContext.Create(HttpContextFactory.Current);
                var idb = CMSImageDataContext.Create(HttpContextFactory.Current);
                AccountModel.SetUserInfo(db, idb, Util.UserName, HttpContextFactory.Current.Session);
            }
            Util.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Util.SessionStarting = true;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (IsRackspaceMonitoring())
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                Response.ContentType = "text/plain";
                Response.Write("OK");
                CompleteRequest();
                return;
            }

            if (HandleBvcmsDomain())
            {
                return;
            }

            if (ShouldBypassProcessing())
            {
                return;
            }

            Response.Headers?.Remove("Server");

            if (Util.AppOffline)
            {
                Response.Redirect("/Errors/AppOffline.htm");
                return;
            }

            var host = CMSDataContext.GetHost(new HttpContextWrapper(Context));
            var r = DbUtil.CheckDatabaseExists($"CMS_{host}");
            var redirect = ViewExtensions2.DatabaseErrorUrl(r);
            if (Util.IsDebug())
            {
                if (r == DbUtil.CheckDatabaseResult.ServerNotFound)
                {
                    Response.Redirect(redirect);
                    return;
                }
                if (r == DbUtil.CheckDatabaseResult.DatabaseDoesNotExist && "localhost".Equal(Request.Url.Host))
                {
                    var ret = DbUtil.CreateDatabase(host);
                    if (ret.HasValue())
                    {
                        Response.Redirect($"/Errors/DatabaseCreationError.aspx?error={HttpUtility.UrlEncode(ret)}");
                        return;
                    }
                }
            }
            else
            {
                if (redirect != null)
                {
                    Response.Redirect(redirect);
                    return;
                }
            }

            var db = CMSDataContext.Create(new HttpContextWrapper(Context));

            Util.AdminMail = db.Setting("AdminMail", "");
            Util.DateSimulation = db.Setting("UseDateSimulation");

            var cul = db.Setting("Culture", "en-US");
            Util.Culture = cul;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            var checkip = ConfigurationManager.AppSettings["CheckIp"];
            if (Util.IsHosted && checkip.HasValue())
            {
                if (1 == db.Connection.ExecuteScalar<int>(checkip, new { ip = Request.UserHostAddress }))
                {
                    Response.Redirect("/Errors/AccessDenied.htm");
                }
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (IsRackspaceMonitoring() || ShouldBypassProcessing())
            {
                return;
            }

            if (Response.Status.StartsWith("401")
                    && Request.Url.AbsolutePath.EndsWith(".aspx"))
            {
                var r = AccountModel.CheckAccessRole(User.Identity.Name);
                if (r.HasValue())
                {
                    Response.Redirect(r);
                }
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (ShouldBypassProcessing())
            {
                return;
            }
        }

        public void ErrorLog_Logged(object sender, ErrorLoggedEventArgs args)
        {
            // This is the text shown in Error.aspx
            Context.Items["error"] = args?.Entry?.Error?.Exception?.Message ?? "Error Unknown";
        }

        public void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var ex = e.Exception.GetBaseException();
            var httpex = ex as HttpException;

            if (httpex != null)
            {
                var status = httpex.GetHttpCode();
                if (status == 400 || status == 404)
                {
                    e.Dismiss();
                }
                else if (httpex.Message.Contains("The remote host closed the connection"))
                {
                    e.Dismiss();
                }
                else if (httpex.Message.Contains("A potentially dangerous Request.Path value was detected from the client"))
                {
                    e.Dismiss();
                }
            }

            if (ex is FileNotFoundException || ex is HttpRequestValidationException)
            {
                e.Dismiss();
            }
        }

        public void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var ex = e.Exception.GetBaseException();
            var httpex = ex as HttpException;

            if (httpex != null)
            {
                var status = httpex.GetHttpCode();
                if (status == 400 || status == 404)
                {
                    e.Dismiss();
                }
                else if (httpex.Message.Contains("The remote host closed the connection"))
                {
                    e.Dismiss();
                }
                else if (
                    httpex.Message.Contains(
                        "A potentially dangerous Request.Path value was detected from the client"))
                {
                    e.Dismiss();
                }
            }

            if (ex is FileNotFoundException || ex is HttpRequestValidationException)
            {
                e.Dismiss();
            }
            else if (IsEmailBodyError(ex))
            {
                e.Dismiss();
            }

            FilterOutSensitiveFormData(e);
        }

        private static void FilterOutSensitiveFormData(ExceptionFilterEventArgs e)
        {
            var ctx = e.Context as HttpContext;
            if (ctx == null)
            {
                return;
            }

            var sensitiveFormKeys = new[] { "creditcard", "ccv", "cvv", "cardnumber", "cardcode", "password", "account", "routing" };

            var sensitiveFormData = ctx.Request.Form.AllKeys.Where(
                k => sensitiveFormKeys.Contains(k.ToLower(), StringComparer.OrdinalIgnoreCase)).ToList();

            if (!sensitiveFormData.Any() || Util.IsDebug())
            {
                return;
            }

            var error = new Error(e.Exception, ctx);
            sensitiveFormData.ForEach(k => error.Form.Set(k, "*****"));
            ErrorLog.GetDefault(ctx).Log(error);
            e.Dismiss();
        }

        private void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (IsEmailBodyError(ex))
            {
                Context.ClearError();
            }
        }

        private bool IsEmailBodyError(Exception ex)
        {
            if (ex is ArgumentException && ex.Message.Contains("EmailBody(Int32)"))
            {
                var a = Request.Path.Split('/');
                if (a.Length > 0 && !a[a.Length - 1].AllDigits())
                {
                    return true;
                }
            }
            return false;
        }

        private bool HandleBvcmsDomain()
        {
            var bvcms = "bvcms.com";
            if (Request.Url.Host.Contains(bvcms, true))
            {
                var url = Request.Url.OriginalString;
                var host = CMSDataContext.GetHost(new HttpContextWrapper(Context));
                var dbExists = DbUtil.CheckDatabaseExists($"CMS_{host}") == DbUtil.CheckDatabaseResult.DatabaseExists;
                var newBaseUrl = "tpsdb.com";
                if (!dbExists)
                {
                    newBaseUrl = "touchpointsoftware.com";
                    bvcms = Request.Url.Host;
                }
                Response.RedirectPermanent(url.Replace(bvcms, newBaseUrl));
                CompleteRequest();
                return true;
            }
            return false;
        }

        private bool ShouldBypassProcessing()
        {
            var url = Request.Url.OriginalString;

            return url.Contains("/Errors/", ignoreCase: true) ||
                url.Contains("/Content/touchpoint/", ignoreCase: true) ||
                url.Contains("healthcheck.txt", ignoreCase: true) ||
                url.Contains("analytics.txt", ignoreCase: true) ||
                url.Contains("favicon.ico", ignoreCase: true);
        }

        private bool IsRackspaceMonitoring()
        {
            return (Request.UserAgent ?? "").Contains("Rackspace Monitoring");
        }
    }
}
