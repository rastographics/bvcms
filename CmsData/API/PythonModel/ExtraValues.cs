using System;
using Dapper;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void AddExtraValueBool(object query, string name, bool b)
        {
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    Person.AddEditExtraBool(db2, pid, name, b);
                    db2.SubmitChanges();
                }
        }

        public void AddExtraValueCode(object query, string name, object text)
        {
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    Person.AddEditExtraValue(db2, pid, name, text.ToString());
                    db2.SubmitChanges();
                }
        }

        public void AddExtraValueDate(object query, string name, object dt)
        {
            var dt2 = dt.ToDate();
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    Person.AddEditExtraDate(db2, pid, name, dt2);
                    db2.SubmitChanges();
                }
        }

        public void AddExtraValueInt(object query, string name, int n)
        {
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    Person.AddEditExtraInt(db2, pid, name, n);
                    db2.SubmitChanges();
                }
        }

        public void AddExtraValueText(object query, string name, object text)
        {
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    Person.AddEditExtraData(db2, pid, name, text.ToString());
                    db2.SubmitChanges();
                }
        }
        public void AddExtraValueAttributes(object query, string name, string text)
        {
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    Person.AddEditExtraAttributes(db2, pid, name, text);
                    db2.SubmitChanges();
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
        public string ExtraValueAttributes(int pid, string name)
        {
            var ev = Person.GetExtraValue(db, pid, name);
            if (ev?.IsAttributes != true)
                return "";
            return ev.Data ?? "";
        }

        public void DeleteExtraValue(object query, string name)
        {
            using (var db2 = NewDataContext())
                foreach (var pid in db2.PeopleQueryIds(query))
                {
                    var ev = Person.GetExtraValue(db2, pid, name);
                    db2.PeopleExtras.DeleteOnSubmit(ev);
                    db2.SubmitChanges();
                }
        }
        public void DeleteAllExtraValueLike(string value)
        {
            DbUtil.LogActivity(db.Host, $"Delete PeopleExtra where Field like '{value}'");
            db.Connection.Execute("delete dbo.PeopleExtra where Field like @value", new {value});
        }
    }
}
