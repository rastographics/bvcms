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
        private static CMSImageDataContext _InternalDb;
        private static CMSImageDataContext InternalDb
        {
            get
            {
                return _InternalDb ?? (CMSImageDataContext)HttpContextFactory.Current.Items[CMSDbKEY];
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    HttpContextFactory.Current.Items[CMSDbKEY] = value;
                }
                else
                {
                    _InternalDb = value;
                }
            }
        }
        public static CMSImageDataContext Db
        {
            get
            {
                if (InternalDb != null)
                {
                    return InternalDb;
                }
                else if (HttpContextFactory.Current == null)
                {
                    return new CMSImageDataContext(Util.ConnectionStringImage);
                }

                return new CMSImageDataContext(Util.ConnectionStringImage);
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
