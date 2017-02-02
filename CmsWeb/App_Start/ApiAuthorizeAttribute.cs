using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using CmsWeb.Code;

namespace CmsWeb
{
    internal class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return AuthHelper.AuthenticateDeveloper(HttpContext.Current, additionalRole: "APIOnly").IsAuthenticated;
        }
    }
    internal class ApiWriteAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return AuthHelper.AuthenticateDeveloper(HttpContext.Current, additionalRole: "APIWrite").IsAuthenticated;
        }
    }
}
