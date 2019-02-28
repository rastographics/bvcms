using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using UtilityExtensions;

namespace ImageData
{
    public static class DbUtil
    {
        private const string CMSDbKEY = "CMSImageDbKey";
        private static CMSImageDataContext InternalDb
        {
            get
            {
                return (CMSImageDataContext)HttpContextFactory.Current.Items[CMSDbKEY];
            }
            set
            {
                HttpContextFactory.Current.Items[CMSDbKEY] = value;
            }
        }
        public static CMSImageDataContext Db
        {
            get
            {
                if (HttpContextFactory.Current == null)
                    return new CMSImageDataContext(Util.ConnectionStringImage);
                if (InternalDb == null)
                {
                    InternalDb = new CMSImageDataContext(Util.ConnectionStringImage);
                    //InternalDb.CommandTimeout = 1200;
                }
                return InternalDb;
            }
            set
            {
                InternalDb = value;
            }
        }
        public static bool CheckDatabaseExists()
        {
            using (var cn = new SqlConnection(Util.GetConnectionString2("master")))
            {
                try
                {
                    cn.Open();
                    var cmd = new SqlCommand(
                            "SELECT CAST(CASE WHEN EXISTS(SELECT NULL FROM sys.databases WHERE name = 'CMSi_"
                            + Util.Host + "') THEN 1 ELSE 0 END AS BIT)", cn);
                    return (bool)cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public static void DbDispose()
        {
            if (InternalDb != null)
            {
                InternalDb.Dispose();
                InternalDb = null;
            }
        }
    }
}
