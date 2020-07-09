using System;
using System.Collections.Generic;
using CmsData;

namespace CmsWeb.Models.ExtraValues
{
    public abstract class ExtraInfo
    {
        public CMSDataContext CurrentDatabase { get; set; }

        protected ExtraInfo(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string TypeDisplay { get; set; }
        public int Count { get; set; }
        public bool Standard { get; set; }
        public bool CanView { get; set; }

        public abstract string QueryUrl { get; }
        public abstract string DeleteAllUrl { get; }
        public abstract string ConvertToStandardUrl { get; }
        public abstract string RenameAllUrl { get; }

        public abstract void RenameAll(string field, string newname);
        public abstract string DeleteAll(string type, string field, string value);
        public abstract IEnumerable<ExtraInfo> CodeSummary();

        public static ExtraInfo GetExtraInfo(CMSDataContext db, string table)
        {
            switch (table)
            {
                case "People":
                    return new ExtraInfoPeople(db);
                case "Family":
                    return new ExtraInfoFamily(db);
                case "Organization":
                    return new ExtraInfoOrganization(db);
            }
            throw new Exception("unknown table " + table);
        }
    }
}
