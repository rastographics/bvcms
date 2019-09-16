using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
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
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("PythonExec scriptfilename");
                Console.WriteLine("PythonExec scripttext");
                Console.WriteLine("PythonExec -f [filename having list of scriptfiles]");
            }
            AppConfig.Change(GetWebConfig());
            if (args[0] == "-f")
            {
                var scripts = File.ReadAllLines(args[1]);
                for (; ; )
                {
                    for (var n = 1; n <= scripts.Length; n++)
                    {
                        Console.WriteLine($" {n}: {Path.GetFileName(scripts[n - 1])}");
                    }
                    Console.Write($"\nEnter Script Number (1-{scripts.Length}): ");
                    var ret = Console.ReadLine();
                    if (!ret.HasValue() || ret == "q")
                        break;
                    Do(scripts[ret.ToInt() - 1]);
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
            var dbname = ConfigurationManager.AppSettings["Host"];
            var db = CMSDataContext.Create(dbname);
            db.FromBatch = true;
            var m = new PythonModel(db);

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
    }
}
