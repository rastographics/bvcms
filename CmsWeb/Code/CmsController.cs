using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Code;
using CmsWeb.Models;
using OfficeOpenXml;
using UtilityExtensions;

namespace CmsWeb
{
    [MyRequireHttps]
    public class CmsController : CmsControllerNoHttps
    {
    }

    public class CmsControllerNoHttps : Controller
    {
        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            throw new HttpException(404, "404");
        }

        public static string HeaderHtml(string altcontent, string headertext, string logoimg)
        {
            var c = DbUtil.Content("Site2Header" + altcontent) ?? DbUtil.Content("Site2Header");
            if (c != null)
                return c.Body;
            return $@"
        <div id=""header"">
           <div id=""title"">
              <h1><img alt=""logo"" src='{logoimg}' align=""middle"" />&nbsp;{headertext}</h1>
           </div>
        </div>";
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Util.Helpfile = $"_{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}_{filterContext.ActionDescriptor.ActionName}";
            DbUtil.Db.UpdateLastActivity(Util.UserId);
            if (AccountController.TryImpersonate())
            {
                var returnUrl = Request.QueryString["returnUrl"];
                filterContext.Result = Redirect(returnUrl.HasValue()
                    ? returnUrl
                    : Request.RawUrl);
            }
        }

        public string AuthenticateDeveloper(bool log = false, string addrole = "")
        {
            return AuthHelper.AuthenticateDeveloper(System.Web.HttpContext.Current, log, addrole).Message;
        }

        public ViewResult Message(string text)
        {
            return View("Message", model: text);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult {Data = data, ContentType = contentType, ContentEncoding = contentEncoding, JsonRequestBehavior = behavior };
        }

    }

    [MyRequireHttps]
    public class CmsStaffController : Controller
    {
        public bool NoCheckRole { get; set; }

        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            throw new HttpException(404, "404");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var s = "/Logon?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl);
                if (Request.QueryString.Count > 0)
                    s += "&" + Request.QueryString.ToString();
                filterContext.Result = Redirect(s);
            }
            else if (!NoCheckRole)
            {
                var r = AccountModel.CheckAccessRole(Util.UserName);
                if (r.HasValue())
                    filterContext.Result = Redirect(r);
            }
            base.OnActionExecuting(filterContext);
            Util.Helpfile = $"_{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}_{filterContext.ActionDescriptor.ActionName}";
            DbUtil.Db.UpdateLastActivity(Util.UserId);
        }

        public static string ErrorUrl(string message)
        {
            return $"/Home/ShowError/?error={System.Web.HttpContext.Current.Server.UrlEncode(message)}&url={System.Web.HttpContext.Current.Request.Url.OriginalString}";
        }

        public ActionResult RedirectShowError(string message)
        {
            return new RedirectResult(ErrorUrl(message));
        }

        public ViewResult Message(string text)
        {
            return View("Message", model: text);
        }

        public ViewResult Message2(string text)
        {
            return View("Message2", model: text);
        }
    }

    [MyRequireHttps]
    public class CmsStaffAsyncController : AsyncController
    {
        public bool NoCheckRole { get; set; }

        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            throw new HttpException(404, "404");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var s = "/Logon?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl);
                if (Request.QueryString.Count > 0)
                    s += "&" + Request.QueryString.ToString();
                filterContext.Result = Redirect(s);
            }
            else if (!NoCheckRole)
            {
                var r = AccountModel.CheckAccessRole(Util.UserName);
                if (r.HasValue())
                    filterContext.Result = Redirect(r);
            }
            base.OnActionExecuting(filterContext);
            Util.Helpfile = $"_{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}_{filterContext.ActionDescriptor.ActionName}";
            DbUtil.Db.UpdateLastActivity(Util.UserId);
        }
    }

    public class RequireBasicAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            if (!req.Headers["Authorization"].HasValue() && !req.Headers["username"].HasValue())
            {
                var res = filterContext.HttpContext.Response;
                res.StatusCode = 401;
                res.AddHeader("WWW-Authenticate", $"Basic realm=\"{DbUtil.Db.Host}\"");
                res.End();
            }
        }
    }

    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
#if DEBUG
#else
            var context = filterContext.HttpContext;
            if (context.Session != null)
                if (context.Session.IsNewSession)
                {
                    string sessionCookie = context.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        filterContext.Result = new RedirectResult("/Errors/SessionTimeout.htm");
                }
#endif
            base.OnActionExecuting(filterContext);
        }
    }

    public class EpplusResult : ActionResult
    {
        private ExcelPackage pkg;
        private string fn;

        public EpplusResult(ExcelPackage pkg, string fn)
        {
            this.pkg = pkg;
            this.fn = fn;
        }

        public EpplusResult(string fn)
        {
            pkg = new ExcelPackage();
            pkg.Workbook.Worksheets.Add("Sheet1");
            this.fn = fn;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + fn);
            pkg.SaveAs(context.HttpContext.Response.OutputStream);
        }
    }

    public class MyRequireHttpsAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            if (filterContext.HttpContext != null)
            {
                if (filterContext.HttpContext.Request.IsLocal)
                    return;
                if (ConfigurationManager.AppSettings["INSERT_X-FORWARDED-PROTO"] == "true")
                {
                    if (filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"] == "http")
                    {
                        var s = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
                        filterContext.Result = new RedirectResult(s);
                    }
                    return;
                }
                else if (!ConfigurationManager.AppSettings["cmshost"].StartsWith("https:"))
                    return;
            }
            base.OnAuthorization(filterContext);
        }

        public static bool NeedRedirect(HttpRequestBase Request)
        {
            if (ConfigurationManager.AppSettings["INSERT_X-FORWARDED-PROTO"] == "true")
                return Request.Headers["X-Forwarded-Proto"] == "http";
            return ConfigurationManager.AppSettings["cmshost"].StartsWith("https:");
        }
    }
/*
    public class CMSLogAttribute : ActionFilterAttribute
    {
        protected DateTime start_time;
        protected Guid id;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            id = Guid.NewGuid();
            start_time = DateTime.Now;
            var routeData = filterContext.RouteData;
            var ts = (DateTime.Now - start_time);
            var duration = ts.TotalSeconds;
            string querystring = "";
            if(filterContext.HttpContext.Request.Url != null)
                querystring = filterContext.HttpContext.Request.Url.Query;
            var method = filterContext.HttpContext.Request.HttpMethod;
            var controller = (string)routeData.Values["controller"];
            var action = (string)routeData.Values["action"];
            var userid = Util.UserName;
            var dbname = Util.Host;
            if (action == "FetchPrintJobs")
                return;
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
                if (cs != null)
                {
                    using (var cn = new SqlConnection(cs.ConnectionString))
                    {
                        cn.Open();
                        var cmd = new SqlCommand("LogRequest2", cn) {CommandType = CommandType.StoredProcedure};
                        cmd.Parameters.AddWithValue("dbname", dbname);
                        cmd.Parameters.AddWithValue("method", method);
                        cmd.Parameters.AddWithValue("controller", controller);
                        cmd.Parameters.AddWithValue("action", action);
                        cmd.Parameters.AddWithValue("userid", userid);
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Parameters.AddWithValue("qs", querystring.Truncate(100));
                        cmd.Parameters.AddWithValue("newui", userid.HasValue());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var routeData = filterContext.RouteData;
            var action = (string)routeData.Values["action"];
            if (action == "FetchPrintJobs")
                return;

            var ts = (DateTime.Now - start_time);
            var duration = ts.TotalSeconds;
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
                if (cs != null)
                {
                    using (var cn = new SqlConnection(cs.ConnectionString))
                    {
                        cn.Open();
                        var userid = Util.UserName;
                        if (!userid.HasValue())
                            userid = AccountModel.UserName2;
                        if (userid.HasValue())
                            cn.Execute(
                                "update dbo.RequestLog set duration = @duration, userid = @userid where id = @id",
                                new {duration, id, userid});
                        else
                            cn.Execute("update dbo.RequestLog set duration = @duration where id = @id",
                                new {duration, id});
                    }
                }
            }
            catch
            {
            }
        }
    }
*/
}
