using System;
using System.Collections.Generic;
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
    }
}