using System;
using System.Collections.Generic;
using CmsData;
using UtilityExtensions;
using System.Data.SqlClient;
using Dapper;
using System.Collections;
using System.Web;
using Moq;
using System.Security.Principal;
using System.IO;
using System.Web.Routing;
using System.Text;
using Xunit;
using System.Collections.Specialized;

namespace CMSWebTests
{
    public static class ContextTestUtils
    {
        public static Mock<HttpContextBase> HttpContext { get; set; }
        public static Mock<HttpRequestBase> Request { get; set; }
        public static Mock<HttpResponseBase> Response { get; set; }
        public static RouteData RouteData { get; set; }
        public static StringBuilder responseBody = new StringBuilder();
        public static NameValueCollection ServerVariables { get; set; }

        public static IDictionary Items { get; set; }
        private const string Url = "https://localhost.tpsdb.com";
        
        internal static HttpContextBase FakeHttpContext()
        {
            HttpContext = new Mock<HttpContextBase>();
            Items = new Dictionary<string, object>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            var responseBody = new StringBuilder();
            ServerVariables = new NameValueCollection {
                { "HTTP_X_FORWARDED_FOR", null },
                { "REMOTE_ADDR", "::1" }
            };

            user.Setup(usr => usr.Identity).Returns(identity.Object);
            identity.SetupGet(ident => ident.IsAuthenticated).Returns(true);
            Request.SetupGet(r => r.Url).Returns(new Uri(Url));
            Request.SetupGet(r => r.QueryString).Returns(new NameValueCollection { });
            Request.SetupGet(r => r.ServerVariables).Returns(ServerVariables);

            Response.SetupGet(ctx => ctx.Output).Returns(new StringWriter(responseBody));

            HttpContext.SetupGet(ctx => ctx.Request).Returns(Request.Object);
            HttpContext.SetupGet(ctx => ctx.Response).Returns(Response.Object);
            HttpContext.SetupGet(ctx => ctx.Session).Returns(session.Object);
            HttpContext.SetupGet(ctx => ctx.Server).Returns(server.Object);
            HttpContext.SetupGet(ctx => ctx.User).Returns(user.Object);
            HttpContext.SetupGet(ctx => ctx.Items).Returns(Items);

            HttpContextFactory.SetCurrentContext(HttpContext.Object);

            return HttpContext.Object;
        }
    }
}
