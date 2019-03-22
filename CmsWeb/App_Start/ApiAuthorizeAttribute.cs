using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb
{
    internal class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return AuthHelper.AuthenticateDeveloper(HttpContextFactory.Current, additionalRole: "APIOnly").IsAuthenticated;
        }
    }
    internal class ApiWriteAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return AuthHelper.AuthenticateDeveloper(HttpContextFactory.Current, additionalRole: "APIWrite").IsAuthenticated;
        }
    }
}
