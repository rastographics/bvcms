using CmsData;
using System;

namespace RepairOrg
{
    class RepairOrg
    {
        static string ConnectionString;
        static string Host;
        static int OrgId;

        static void Main(string[] args)
        {
            ParseCommandLine(args);
            Run();
        }

        private static void Run()
        {
            using (var db = CMSDataContext.Create(ConnectionString, Host))
            {
                Environment.ExitCode = db.RepairTransactions(OrgId);
            }
        }

        private static void ParseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "--connection":
                        i++;
                        ConnectionString = args[i];
                        break;
                    case "--host":
                        i++;
                        Host = args[i];
                        break;
                    default:
                        OrgId = Convert.ToInt32(arg);
                        break;
                }
            }
        }
    }
}
