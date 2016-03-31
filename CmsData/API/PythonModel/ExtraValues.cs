using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void AddExtraValueBool(object query, string name, bool b)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraBool(db2, pid, name, b);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueCode(object query, string name, string text)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraValue(db2, pid, name, text);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueDate(object query, string name, object dt)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            var dt2 = dt.ToDate();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraDate(db2, pid, name, dt2);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueInt(object query, string name, int n)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraInt(db2, pid, name, n);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueText(object query, string name, string text)
        {
            var list = db.PeopleQuery2(query).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraData(db2, pid, name, text);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public string ExtraValue(int pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid, name);
            if (ev != null)
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
                    case "Data":
                        return $"{ev.StrValue};{ev.DateValue.FormatDate()};{ev.BitValue};{ev.IntValue};{ev.Data}";
                    default:
                        return $"unknown type: {ev.Type}";
                }
            return "";
        }

        public bool ExtraValueBit(int pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid, name);
            if (ev != null)
                return ev.BitValue ?? false;
            return false;
        }

        public string ExtraValueCode(int pid, string name)
        {
            return ExtraValue(pid, name);
        }

        public DateTime ExtraValueDate(int pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid, name);
            if (ev != null)
                return ev.DateValue ?? DateTime.MinValue;
            return DateTime.MinValue;
        }

        public int ExtraValueInt(int pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid, name);
            if (ev != null)
                return ev.IntValue ?? 0;
            return 0;
        }

        public string ExtraValueText(int pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid, name);
            if (ev != null)
                return ev.Data ?? "";
            return "";
        }
    }
}