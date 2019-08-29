using System;
using System.Data.SqlClient;
using System.IO;
using CmsData;
using System.Xml;
using System.Configuration;
using UtilityExtensions;

namespace SharedTestFixtures
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            EnsureDatabaseExists();
        }
        
        private static string _host;
        public static string Host => _host ?? (_host = GetHostFromWebConfig());

        private static string GetHostFromWebConfig()
        {
            var config = LoadWebConfig();
            var hostKey = config.SelectSingleNode("configuration/appSettings/add[@key='host']");
            return Util.Host = Util.PickFirst(hostKey?.Attributes["value"].Value, "localhost");
        }

        public static void EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["CMS"].ConnectionString);
            var sqlScriptsPath = FindSqlScriptsPath();
            builder.InitialCatalog = "master";
            var masterConnectionString = builder.ConnectionString;
            builder.InitialCatalog = $"CMSi_{Host}";
            var imageConnectionString = builder.ConnectionString;
            builder.InitialCatalog = "ELMAH";
            var elmahConnectionString = builder.ConnectionString;
            builder.InitialCatalog = $"CMS_{Host}";
            var standardConnectionString = builder.ConnectionString;

            if (!DbUtil.DatabaseExists(masterConnectionString, $"CMS_{Host}"))
            {
                var result = DbUtil.CreateDatabase(Host, sqlScriptsPath, masterConnectionString, imageConnectionString, elmahConnectionString, standardConnectionString);
                if (!string.IsNullOrEmpty(result))
                {
                    throw new Exception(result);
                }
            }
        }

        public static XmlDocument LoadWebConfig()
        {
            var config = new XmlDocument();
            config.Load(FindWebConfigPath());
            return config;
        }

        private static string FindWebConfigPath()
        {
            string file = null;
            foreach(var path in new[] { @"..\..\..\..\CmsWeb\web.config", @"..\..\..\CmsWeb\web.config" })
            {
                file = Path.GetFullPath(path);
                if (File.Exists(file))
                {
                    break;
                }
            }
            return file;
        }

        private static string FindSqlScriptsPath()
        {
            string dir = null;
            foreach(var path in new[] { @"..\..\..\..\SqlScripts", @"..\..\..\SqlScripts" })
            {
                dir = Path.GetFullPath(path);
                if (Directory.Exists(dir))
                {
                    break;
                }
            }
            return dir;
        }

        public void Dispose()
        {
            //DbUtil.Db = null;
        }
    }
}
