using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Code;
using CmsWeb.Models;
using OfficeOpenXml;
using RegistrationSettingsParser;
using UtilityExtensions;

namespace CmsWeb
{
    [MyRequireHttps]
    public class CmsController : CmsControllerNoHttps
    {
        public static void ConvertRegistration()
        {
            if (Util.IsHosted) // if hosted by Touchpoint, this is already done
                return;
            if (DbUtil.Db.RegistrationsConverted())
                return;
            var q = from o in DbUtil.Db.Organizations
                where (o.RegSetting ?? "").Length > 0
                where o.RegSettingXml == null
                orderby o.OrganizationId
                select o;
            foreach (var o in q)
            {
                var rs = Parser.ParseSettings(o.RegSetting);
                var s = Util.Serialize(rs);
                o.RegSettingXml = s;
                DbUtil.Db.SubmitChanges();
            }
            DbUtil.Db.SetRegistrationsConverted();
        }
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
        public ViewResult Message(string text)
        {
            return View("Message", model: text);
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
}
