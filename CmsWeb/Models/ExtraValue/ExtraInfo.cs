using System;
using System.Collections.Generic;

namespace CmsWeb.Models.ExtraValues
{
    public abstract class ExtraInfo
    {
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

        public static IEnumerable<ExtraInfo> CodeSummary(string table)
        {
            return GetExtraInfo(table).CodeSummary();
        }
        public static void RenameAll(string table, string field, string newname)
        {
            GetExtraInfo(table).RenameAll(field, newname.Trim());
        }
        public static string DeleteAll(string table, string type, string field, string value)
        {
            return GetExtraInfo(table).DeleteAll(type, field, value);
        }
        public static ExtraInfo GetExtraInfo(string table)
        {
            switch (table)
            {
                case "People":
                    return new ExtraInfoPeople();
                case "Family":
                    return new ExtraInfoFamily();
                case "Organization":
                    return new ExtraInfoOrganization();
            }
            throw new Exception("unknown table " + table);
        }
    }
}
