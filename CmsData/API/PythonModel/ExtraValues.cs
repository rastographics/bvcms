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
                Person.AddEditExtraBool(db, pid, name, b);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueCode(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraValue(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueDate(object savedQuery, string name, object dt)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            var dt2 = dt.ToDate();
            foreach (var pid in list)
            {
                Person.AddEditExtraDate(db, pid, name, dt2);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueInt(object savedQuery, string name, int n)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraInt(db, pid, name, n);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueText(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraData(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
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