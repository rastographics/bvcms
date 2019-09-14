using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using CmsData;
using CmsData.Classes.Twilio;
using UtilityExtensions;
using Moq;
using UtilityExtensions.Config;

namespace PythonExec
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                for(;;)
                {
                    for (var n = 1; n <= args.Length; n++)
                    {
                        Console.WriteLine($" {n}: {Path.GetFileName(args[n-1])}");
                    }
                    Console.Write($"\nEnter Script Number (1-{args.Length}): ");
                    var ret = Console.ReadLine();
                    if(!ret.HasValue() || ret == "q")
                        break;
                    Do(args[ret.ToInt()-1]);
                }
            }
            else
            {
                Do(args[0]);
                Console.Write("Done ");
                Console.ReadKey();
            }
        }

        private static void Do(string script)
        {
            using (AppConfig.Change(GetWebConfig()))
            {
                var dbname = ConfigurationManager.AppSettings["Host"];
                var mock = new MockHttpContext();
                HttpContextFactory.SetCurrentContext(mock.Object);
                var m = new PythonModel(dbname);

                if (File.Exists(script))
                {
                    Console.WriteLine(m.RunScriptFile(script));
                }
                else
                {
                    var s = script;
                    Console.WriteLine(m.RunScript(s));
                }
            }
        }

        private static string GetWebConfig()
        {
            string curDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string webconfig = Path.GetFullPath(Path.Combine(curDir, @"..\web.config"));
            if (!File.Exists(webconfig))
            {
                webconfig = Path.GetFullPath(Path.Combine(curDir, @"..\..\..\CmsWeb\web.config"));
            }
            return webconfig;
        }
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
                user.Setup(usr => usr.IsInRole(It.IsAny<string>())).Returns(true);
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
}
