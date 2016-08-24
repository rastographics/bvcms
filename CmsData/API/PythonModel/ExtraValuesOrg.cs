using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void AddExtraValueBoolOrg(int oid, string name, bool b)
        {
            var db2 = NewDataContext();
            var o = db2.LoadOrganizationById(oid);
            o.AddEditExtraBool(name, b);
            db2.SubmitChanges();
            db2.Dispose();
        }

        public void AddExtraValueCodeOrg(int oid, string name, string text)
        {
            var db2 = NewDataContext();
            var o = db2.LoadOrganizationById(oid);
            o.AddEditExtraCode(name, text);
            db2.SubmitChanges();
            db2.Dispose();
        }

        public void AddExtraValueDateOrg(int oid, string name, object dt)
        {
            var dt2 = dt.ToDate();
            if (!dt2.HasValue)
                return;
            var db2 = NewDataContext();
            var o = db2.LoadOrganizationById(oid);
                o.AddEditExtraDate(name, dt2.Value);
            db2.SubmitChanges();
            db2.Dispose();
        }

        public void AddExtraValueIntOrg(int oid, string name, int n)
        {
            var db2 = NewDataContext();
            var o = db2.LoadOrganizationById(oid);
            o.AddEditExtraInt(name, n);
            db2.SubmitChanges();
            db2.Dispose();
        }

        public void AddExtraValueTextOrg(int oid, string name, string text)
        {
            var db2 = NewDataContext();
            var o = db2.LoadOrganizationById(oid);
            o.AddEditExtraText(name, text);
            db2.SubmitChanges();
            db2.Dispose();
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