using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData
{
    public partial class QueryFunctions
    {
        private readonly CMSDataContext db;
        private readonly Dictionary<string, object> dictionary;
        private dynamic data;
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
            this.data = new DynamicData(dictionary);
        }

        private DbConnection GetReadonlyConnection()
        {
            return db.ReadonlyConnection();
        }
    }
}