using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void AddExtraValueBool(object savedQuery, string name, bool b)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraBool(db2, pid, name, b);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueCode(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraValue(db2, pid, name, text);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueDate(object savedQuery, string name, object dt)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            var dt2 = dt.ToDate();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraDate(db2, pid, name, dt2);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueInt(object savedQuery, string name, int n)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraInt(db2, pid, name, n);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public void AddExtraValueText(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                var db2 = NewDataContext();
                Person.AddEditExtraData(db2, pid, name, text);
                db2.SubmitChanges();
                db2.Dispose();
            }
        }

        public string ExtraValue(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.StrValue ?? "";
            return "";
        }

        public bool ExtraValueBit(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.BitValue ?? false;
            return false;
        }

        public string ExtraValueCode(object pid, string name)
        {
            return ExtraValue(pid, name);
        }

        public DateTime ExtraValueDate(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.DateValue ?? DateTime.MinValue;
            return DateTime.MinValue;
        }

        public int ExtraValueInt(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.IntValue ?? 0;
            return 0;
        }

        public string ExtraValueText(object pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid.ToInt(), name);
            if (ev != null)
                return ev.Data ?? "";
            return "";
        }
    }
}