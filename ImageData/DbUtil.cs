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
