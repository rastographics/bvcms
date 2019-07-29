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

namespace CMSWebTests
{
    [Collection("Database collection")]
    public static class ContexTestUtils
    {
        public static Mock<HttpContextBase> HttpContext { get; set; }
        public static Mock<HttpRequestBase> Request { get; set; }
        public static Mock<HttpResponseBase> Response { get; set; }
        public static RouteData RouteData { get; set; }
        public static StringBuilder responseBody = new StringBuilder();

        public static bool BuildDb = true;
        public static bool DropDb = false;
        public static IDictionary Items;
        private const string Url = "https://localhost.tpsdb.com";

        internal static CMSDataContext CurrentDatabase()
        {
            Items = new Dictionary<string, object>();
            var c = FakeHttpContext();
            HttpContextFactory.SetCurrentContext(c);
            var dbname = $"CMS_" + Util.Host;
            var dbExists = DbUtil.CheckDatabaseExists(dbname).Equals(DbUtil.CheckDatabaseResult.DatabaseExists);
            if (!dbExists && BuildDb)
            {
                var csMaster = Util.GetConnectionString2("master");
                var csElmah = Util.GetConnectionString2("elmah");
                var scriptsDir = ScriptsDirectory();
                if (DropDb)
                {
                    var cn = new SqlConnection(csMaster);
                    cn.Execute($"DROP DATABASE IF EXISTS {dbname}");
                }
                var result = DbUtil.CreateDatabase(
                    Util.Host,
                    scriptsDir,
                    csMaster,
                    Util.ConnectionStringImage,
                    csElmah,
                    Util.ConnectionString);
                if (result.HasValue())
                {
                    throw new Exception(result);
                }
            }
            var db = DbUtil.Db;
            return db;
        }

        private static string ScriptsDirectory()
        {
            var dir = Environment.CurrentDirectory;
            return Path.GetFullPath(Path.Combine(dir, @"..\..\..\..\SqlScripts"));
        }

        internal static HttpContextBase FakeHttpContext()
        {
            HttpContext = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            StringBuilder responseBody = new StringBuilder();

            user.Setup(usr => usr.Identity).Returns(identity.Object);
            identity.SetupGet(ident => ident.IsAuthenticated).Returns(true);
            Request.SetupGet(req => req.Url).Returns(new Uri(Url));

            HttpContext.Setup(ctx => ctx.Request).Returns(Request.Object);
            HttpContext.Setup(ctx => ctx.Request.QueryString).Returns(new System.Collections.Specialized.NameValueCollection { });
            HttpContext.Setup(ctx => ctx.Request.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection{
                { "HTTP_X_FORWARDED_FOR", null },
                { "REMOTE_ADDR", "::1" }
            });
            HttpContext.Setup(ctx => ctx.Response).Returns(Response.Object);
            HttpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            HttpContext.Setup(ctx => ctx.Server).Returns(server.Object);
            HttpContext.Setup(ctx => ctx.User).Returns(user.Object);
            HttpContext.Setup(ctx => ctx.Items).Returns(Items);
            Response.Setup(ctx => ctx.Output).Returns(new StringWriter(responseBody));

            return HttpContext.Object;
        }
    }
}
