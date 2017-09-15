using System;
using System.Data.SqlTypes;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void AddExtraValueBoolOrg(int oid, string name, bool b)
        {
            using (var db2 = NewDataContext())
            {
                var o = db2.LoadOrganizationById(oid);
                o.AddEditExtraBool(name, b);
                db2.SubmitChanges();
            }
        }

        public void AddExtraValueCodeOrg(int oid, string name, object text)
        {
            using (var db2 = NewDataContext())
            {
                var o = db2.LoadOrganizationById(oid);
                o.AddEditExtraCode(name, text.ToString());
                db2.SubmitChanges();
            }
        }

        public void AddExtraValueDateOrg(int oid, string name, object dt)
        {
            var dt2 = dt.ToDate();
            if (!dt2.HasValue)
                return;
            using (var db2 = NewDataContext())
            {
                var o = db2.LoadOrganizationById(oid);
                o.AddEditExtraDate(name, dt2.Value);
                db2.SubmitChanges();
            }
        }

        public void AddExtraValueIntOrg(int oid, string name, int n)
        {
            using (var db2 = NewDataContext())
            {
                var o = db2.LoadOrganizationById(oid);
                o.AddEditExtraInt(name, n);
                db2.SubmitChanges();
            }
        }

        public void AddExtraValueTextOrg(int oid, string name, object text)
        {
            using (var db2 = NewDataContext())
            {
                var o = db2.LoadOrganizationById(oid);
                o.AddEditExtraText(name, text.ToString());
                db2.SubmitChanges();
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
        public DateTime? ExtraValueDateOrgNull(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            return ev?.DateValue;
        }

        public int ExtraValueIntOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev != null)
                return ev.IntValue ?? 0;
            return 0;
        }

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

        public string ExtraValueTextOrg(int oid, string name)
        {
            var ev = Organization.GetExtraValue(db, oid, name);
            if (ev != null)
                return ev.Data ?? "";
            return "";
        }
    }
}
