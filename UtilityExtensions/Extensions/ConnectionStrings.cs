/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System.Web;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static string Host
        {
            get
            {
                var h = ConfigurationManager.AppSettings["host"];
                if (h.HasValue())
                    return h;
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.Url.Authority.SplitStr(".:")[0];
                return null;
            }
        }
        public static string DbServer
        {
            get
            {
                var s = ConfigurationManager.AppSettings["dbserver"];
                if (s.HasValue())
                    return s;
                return null;
            }
        }

        public static string CmsHost2
        {
            get
            {
                var h = ConfigurationManager.AppSettings["cmshost"];
                return h.Replace("{church}", Host, ignoreCase: true);
            }
        }
        public static bool IsHosted
        {
            get
            {
                return ConfigurationManager.AppSettings["CMSHosted"] == "true";
            }
        }
        public static string GetConnectionString(string host)
        {
            var cs = ConnectionStringSettings(host) ?? ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            if (string.IsNullOrEmpty(cb.DataSource))
                cb.DataSource = DbServer;
            var a = host.SplitStr(".:");
            cb.InitialCatalog = "CMS_{0}".Fmt(a[0]);
            return cb.ConnectionString;
        }
        public static string GetConnectionString2(string db, int? timeout = null)
        {
            var cs = ConnectionStringSettings(db) ?? ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            if (timeout.HasValue)
                cb.ConnectTimeout = timeout.Value;
            if (string.IsNullOrEmpty(cb.DataSource))
                cb.DataSource = DbServer;
            cb.InitialCatalog = db;
            return cb.ConnectionString;
        }

        private static ConnectionStringSettings ConnectionStringSettings(string host)
        {
            var h2 = ConfigurationManager.AppSettings["CmsHosted2"];
            if (h2.HasValue())
            {
                var a = h2.Split(',');
                if (a.Contains(host))
                    return ConfigurationManager.ConnectionStrings["CMS2"];
            }
            return ConfigurationManager.ConnectionStrings["CMS"];
        }

        private const string STR_ConnectionString = "ConnectionString";
        public static string ConnectionString
        {
            get
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_ConnectionString] != null)
                            return HttpContext.Current.Session[STR_ConnectionString].ToString();

                var cs = ConnectionStringSettings(Host);
                var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
                if (string.IsNullOrEmpty(cb.DataSource))
                    cb.DataSource = DbServer;
                cb.InitialCatalog = "CMS_{0}".Fmt(Host);
                return cb.ConnectionString;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ConnectionString] = value;
            }
        }

        public static string ConnectionStringImage
        {
            get
            {
                var cs = ConnectionStringSettings(Host);
                var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
                var a = Host.SplitStr(".:");
                if (string.IsNullOrEmpty(cb.DataSource))
                    cb.DataSource = DbServer;
                cb.InitialCatalog = "CMSi_{0}".Fmt(a[0]);
                return cb.ConnectionString;
            }
        }

    }
}

