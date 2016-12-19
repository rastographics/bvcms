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
            var pw = ConfigurationManager.AppSettings["readonlypassword"];
            if (!pw.HasValue())
                return db.Connection;
            var cb = new SqlConnectionStringBuilder(db.ConnectionString) {IntegratedSecurity = false};
            var finance = db.CurrentUser?.InRole("Finance") ?? true; 
            cb.UserID = (finance ? $"ro-{cb.InitialCatalog}-finance" : $"ro-{cb.InitialCatalog}");
            cb.Password = pw;
            return new SqlConnection(cb.ConnectionString);
        }
    }
}