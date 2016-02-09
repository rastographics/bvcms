using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public string ExtraValueOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev == null)
                return "";
            switch (ev.Type)
            {
                case "Code":
                    return ev.StrValue;
                case "Text":
                    return ev.Data;
                case "Date":
                    return ev.DateValue.FormatDate();
                case "Bit":
                    return ev.BitValue.ToString();
                case "Int":
                    return ev.IntValue.ToString();
                default:
                    return $"unknown type: {ev.Type}";
            }
        }

        public bool ExtraValueBitOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev != null)
                return ev.BitValue ?? false;
            return false;
        }

        public string ExtraValueCodeOrg(int oid, string name)
        {
            return ExtraValueOrg(oid, name);
        }

        public DateTime ExtraValueDateOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev != null)
                return ev.DateValue ?? DateTime.MinValue;
            return DateTime.MinValue;
        }

        public int ExtraValueIntOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev != null)
                return ev.IntValue ?? 0;
            return 0;
        }

        public string ExtraValueTextOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev != null)
                return ev.Data ?? "";
            return "";
        }
    }
}