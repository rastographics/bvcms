/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using UtilityExtensions;
using System.Web.Caching;
using System.Data.SqlClient;

namespace CmsData
{
    public static partial class DbUtil
    {
        public static bool DatabaseExists(string name)
        {
            using (var cn = new SqlConnection(Util.GetConnectionString2("master", 3)))
            {
                cn.Open();
                return DatabaseExists(cn, name);
            }
        }
        private static bool DatabaseExists(SqlConnection cn, string name)
        {
            var cmd = new SqlCommand(
                    "SELECT CAST(CASE WHEN EXISTS(SELECT NULL FROM sys.databases WHERE name = '"
                    + name + "') THEN 1 ELSE 0 END AS BIT)", cn);
            return (bool)cmd.ExecuteScalar();
        }
        public enum CheckDatabaseResult
        {
            DatabaseExists,
            DatabaseDoesNotExist,
            ServerNotFound
        }
        public static bool CmsDatabaseExists()
        {
            var exists = (bool?)HttpRuntime.Cache[Util.Host + "-DatabaseExists"];
            if (exists.HasValue)
                return exists.Value;

            var r = CheckDatabaseExists(Util.CmsHost);
            var b = CheckDatabaseResult.DatabaseExists == r;
            HttpRuntime.Cache.Insert(Util.Host + "-DatabaseExists", b, null,
                DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
            return b;
        }

        public static CheckDatabaseResult CheckDatabaseExists(string name, bool nocache = false)
        {
            if (nocache == false)
            {
                var r1 = HttpRuntime.Cache[Util.Host + "-CheckDatabaseResult"];
                if (r1 != null)
                    return (CheckDatabaseResult)r1;
            }

            using (var cn = new SqlConnection(Util.GetConnectionString2("master", 3)))
            {
                CheckDatabaseResult ret;
                try
                {
                    cn.Open();
                    var b = DatabaseExists(cn, name);
                    ret = b ? CheckDatabaseResult.DatabaseExists : CheckDatabaseResult.DatabaseDoesNotExist;
                }
                catch (Exception)
                {
                    ret = CheckDatabaseResult.ServerNotFound;
                }
                if (nocache == false)
                {
                    HttpRuntime.Cache.Insert(Util.Host + "-CheckDatabaseResult", ret, null,
                        DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
                }
                return ret;
            }
        }
        public static string CreateDatabase()
        {
            try
            {
                var Server = HttpContext.Current.Server;
                var path = Server.MapPath("/");
                string cs = Util.GetConnectionString2("master");
                RunScripts(cs, "create database CMS_" + Util.Host);
                if (!DatabaseExists("CMSi_" + Util.Host))
                {
                    RunScripts(cs, "create database CMSi_" + Util.Host);
                    RunScripts(Util.ConnectionStringImage,
                        File.ReadAllText(path + @"..\SqlScripts\BuildImageDatabase.sql"));
                }
                if (!DatabaseExists("Elmah"))
                {
                    RunScripts(cs, "create database Elmah");
                    RunScripts(Util.GetConnectionString2("Elmah"),
                        File.ReadAllText(path + @"..\SqlScripts\BuildElmahDb.sql"));
                }

                using (var cn = new SqlConnection(Util.ConnectionString))
                {
                    cn.Open();
                    var scripts = File.ReadAllLines(path + @"..\SqlScripts\allscripts.txt");
                    foreach (var fn in scripts)
                    {
                        var script = File.ReadAllText(path + @"..\SqlScripts\BuildDb\" + fn);
                        RunScripts(cn, script);
                    }
                    string datascript = null;
                    datascript = Util.Host == "testdb"
                        ? File.ReadAllText(path + @"..\SqlScripts\BuildDb\datascriptTest.sql")
                        : File.ReadAllText(path + @"..\SqlScripts\BuildDb\datascriptStarter.sql");
                    RunScripts(cn, datascript);
                }
                HttpRuntime.Cache.Remove(Util.Host + "-DatabaseExists");
                HttpRuntime.Cache.Remove(Util.Host + "-CheckDatabaseResult");

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        private static void RunScripts(string cs, string script)
        {
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                RunScripts(cn, script);
            }
        }
        private static void RunScripts(SqlConnection cn, string script)
        {
            var cmd = new SqlCommand { Connection = cn };
            var scripts = Regex.Split(script, "\r\nGO\r\n", RegexOptions.Multiline);
            foreach (var s in scripts)
                if (s.HasValue())
                {
                    cmd.CommandText = s;
                    cmd.ExecuteNonQuery();
                }
        }
    }
}
