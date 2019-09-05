using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace SharedTestFixtures
{
    public class MockHttpContext : Mock<HttpContextBase>
    {
        public Mock<HttpRequestBase> MockRequest { get; set; }
        public Mock<HttpResponseBase> MockResponse { get; set; }
        public NameValueCollection Headers { get; set; }
        public NameValueCollection ServerVariables { get; set; }

        public static IDictionary Items { get; set; }
        private const string Url = "https://localhost.tpsdb.com";

        public MockHttpContext()
        {
            Items = new Dictionary<string, object>();
            MockRequest = new Mock<HttpRequestBase>();
            MockResponse = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            var responseBody = new StringBuilder();
            Headers = new NameValueCollection();
            ServerVariables = new NameValueCollection {
                { "HTTP_X_FORWARDED_FOR", null },
                { "REMOTE_ADDR", "::1" }
            };

            user.Setup(usr => usr.Identity).Returns(identity.Object);
            identity.SetupGet(ident => ident.IsAuthenticated).Returns(true);
            MockRequest.SetupGet(r => r.Url).Returns(new Uri(Url));
            MockRequest.SetupGet(r => r.QueryString).Returns(new NameValueCollection { });
            MockRequest.SetupGet(r => r.ServerVariables).Returns(ServerVariables);
            MockRequest.SetupGet(r => r.Headers).Returns(Headers);

            MockResponse.SetupGet(ctx => ctx.Output).Returns(new StringWriter(responseBody));

            SetupGet(ctx => ctx.Request).Returns(MockRequest.Object);
            SetupGet(ctx => ctx.Response).Returns(MockResponse.Object);
            SetupGet(ctx => ctx.Session).Returns(session.Object);
            SetupGet(ctx => ctx.Server).Returns(server.Object);
            SetupGet(ctx => ctx.User).Returns(user.Object);
            SetupGet(ctx => ctx.Items).Returns(Items);
        }
    }
}
