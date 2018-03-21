using CmsData.Classes.Twilio;
using System;
using System.IO;
using System.Reflection;

namespace CmsTwilio
{
    class CmsTwilio
    {
        static int listID;
        static string host;

        static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
                Environment.ExitCode = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Environment.ExitCode = 1;
            }
        }

        private static void Run(string[] args)
        {
            using (AppConfig.Change(GetWebConfig()))
            {
                ParseArguments(args);

                if (!string.IsNullOrEmpty(host) && listID != 0)
                {
                    TwilioHelper.ProcessQueue(listID, host);
                }
                else
                {
                    throw new ArgumentException("Invalid command line arguments");
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

        private static void ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg == "--host")
                {
                    i++;
                    if (args.Length > i)
                    {
                        host = args[i];
                        Console.WriteLine($"Using host {host}");
                    }
                    else
                    {
                        throw new ArgumentException("Host was not specified");
                    }
                }
                else
                {
                    if (!int.TryParse(arg, out listID))
                    {
                        throw new ArgumentException("Invalid list ID");
                    }
                }
            }
        }
    }
}
