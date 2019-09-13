using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using CmsData;
using UtilityExtensions;
using Moq;

namespace PythonExec
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Do(args[0], args[1]);
        }

        private static void Do(string dbname, string text)
        {
            var context = HttpContext();
            HttpContextFactory.SetCurrentContext(context);
            var m = new PythonModel(dbname);

            if (File.Exists(text))
            {
                Console.WriteLine(m.RunScriptFile(text));
            }
            else
            {
                var s = text;
                Console.WriteLine(m.RunScript(s));
            }
            Console.Write("Done ");
            Console.ReadKey();
        }

        private static HttpContextBase HttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            user.Setup(usr => usr.Identity).Returns(identity.Object);
            user.Setup(usr => usr.IsInRole(It.IsAny<string>())).Returns(true);
            identity.SetupGet(ident => ident.IsAuthenticated).Returns(true);
            context.Setup(ctx => ctx.User).Returns(user.Object);

            return context.Object;
        }
    }
}
