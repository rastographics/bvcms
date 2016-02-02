using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class QueryFunctions
    {
        private readonly CMSDataContext db;
        private readonly Dictionary<string, object> dictionary;
        private DateTime? lastSunday;

        public QueryFunctions(CMSDataContext db)
        {
            this.db = db;
        }

        public QueryFunctions(string dbname)
        {
            db = DbUtil.Create(dbname);
        }

        public QueryFunctions(CMSDataContext db, Dictionary<string, object> dictionary) : this(db)
        {
            this.dictionary = dictionary;
        }

        private SqlConnection GetReadonlyConnection()
        {
            var cs = db.CurrentUser == null || db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            return cn;
        }
    }
}