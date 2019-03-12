using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Security.Principal;
using CmsData;
using Dapper;
using Moq;
using UtilityExtensions;

namespace UnitTests
{
    public class DatabaseFixture : IDisposable
    {
        public static bool BuildDb = true;
        public static bool DropDb = false;
        public static IDictionary Items;
        private const string Url = "https://test.tpsdb.com";

        public DatabaseFixture()
        {
            Items = new Dictionary<string, object>();
            var c = FakeHttpContext();
            HttpContextFactory.SetCurrentContext(c);
            if (BuildDb)
            {
                var csMaster = Util.GetConnectionString2("master");
                var csElmah = Util.GetConnectionString2("elmah");
                var scriptsDir = ScriptsDirectory();
                if (DropDb)
                {
                    var cn = new SqlConnection(csMaster);
                    cn.Execute(@"drop database if exists CMS_test");
                }
                DbUtil.CreateDatabase(
                    Util.Host,
                    scriptsDir,
                    csMaster,
                    Util.ConnectionStringImage,
                    csElmah,
                    Util.ConnectionString);
            }
        }

        private static string ScriptsDirectory()
        {
            var cb = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            var dir = Path.GetDirectoryName(cb);
            dir = new Uri(dir ?? "").LocalPath;
            dir = Path.GetDirectoryName(dir);
            dir = Path.GetDirectoryName(dir);
            dir = Path.GetDirectoryName(dir) + "\\SqlScripts\\";
            return dir;
        }

        public void Dispose()
        {
            DbUtil.Db.Connection.Close();
            DbUtil.DbDispose();
            Items = null;
        }
        internal static HttpContextBase FakeHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            user.Setup(usr => usr.Identity).Returns(identity.Object);
            identity.SetupGet(ident => ident.IsAuthenticated).Returns(true);
            request.SetupGet(req => req.Url).Returns(new Uri(Url));

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            context.Setup(ctx => ctx.Items).Returns(Items);

            return context.Object;
        }
    }
}
