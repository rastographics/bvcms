using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;
using CmsData;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public enum OriginTable
    {
        Family,
        Person,
        Organization,
        Meeting
    }
    public class ExtraValueModel
    {
        public string Location { get; set; }
        public int Id { get; set; }
        public OriginTable Table { get; set; }
        public string OtherLocations;

        public ExtraValueModel(int id, string table)
            : this(id, null, table)
        {
        }

        public ExtraValueModel(int id, string location, string table)
        {
            Id = id;
            Location = location;
            OriginTable e;
            Enum.TryParse(table, true, out e);
            Table = e;
        }
        public ExtraValueModel(int id, string location, OriginTable table)
        {
            Id = id;
            Location = location;
            Table = table;
        }
        public IEnumerable<Field> GetStandardExtraValues(string location)
        {
            var emptylist = new List<Field>() as IEnumerable<Field>;
            if (DbUtil.Db.Setting("UseStandardExtraValues", "false") != "true")
                return emptylist;
            var xml = DbUtil.StandardExtraValues();
            var sr = new StringReader(xml);
            var fs = new XmlSerializer(typeof(Fields)).Deserialize(sr) as Fields;

            if (fs == null)
                return emptylist;
            var fields = (from ff in (fs.fields ?? emptylist)
                          where ff.table == Table.ToString()
                          where ff.location == location || location == null
                          select ff).ToList();
            var n = 0;
            foreach (var ff in fields)
                ff.order = n++;
            return fields;
        }

        public string HelpLink(string page)
        {
            return Util.HelpLink("_AddExtraValue_{0}".Fmt(page));
        }
        public IEnumerable<ExtraValue> ListExtraValues()
        {
            IEnumerable<ExtraValue> q = null;
            switch (Table)
            {
                case OriginTable.Person:
                    q = from ee in DbUtil.Db.PeopleExtras
                        where ee.PeopleId == Id
                        select new ExtraValue(ee, this);
                    break;
                case OriginTable.Family:
                    q = from ee in DbUtil.Db.FamilyExtras
                        where ee.FamilyId == Id
                        select new ExtraValue(ee, this);
                    break;
                case OriginTable.Organization:
                    q = from ee in DbUtil.Db.OrganizationExtras
                        where ee.OrganizationId == Id
                        select new ExtraValue(ee, this);
                    break;
                case OriginTable.Meeting:
                    q = from ee in DbUtil.Db.MeetingExtras
                        where ee.MeetingId == Id
                        select new ExtraValue(ee, this);
                    break;
                default:
                    q = new List<ExtraValue>();
                    break;
            }
            return q.ToList();
        }
        private ITableWithExtraValues TableObject()
        {
            switch (Table)
            {
                case OriginTable.Person:
                    return DbUtil.Db.LoadPersonById(Id);
                case OriginTable.Organization:
                    return DbUtil.Db.LoadOrganizationById(Id);
                case OriginTable.Family:
                    return DbUtil.Db.Families.Single(ff => ff.FamilyId == Id);
                //  case OriginTable.Meeting:
                //  break;
                default:
                    return null;
            }
        }

        public IEnumerable<Field> GetExtraValues(string loc = null)
        {
            var standardfields = GetStandardExtraValues(loc ?? Location).ToList();
            var extraValues = ListExtraValues();

            if ((loc ?? Location ?? "").ToLower() == "adhoc")
                return from v in extraValues
                       join f in standardfields on v.Field equals f.name into j
                       from f in j.DefaultIfEmpty()
                       where f == null
                       where !standardfields.Any(ff => ff.Codes.Any(cc => cc == v.Field))
                       orderby v.Field
                       select Field.AddField(f, v, this);
            return from f in standardfields
                   join v in extraValues on f.name equals v.Field into j
                   from v in j.DefaultIfEmpty()
                   orderby f.order
                   select Field.AddField(f, v, this);
        }
        public List<SelectListItem> ExtraValueCodes()
        {
            var q = from e in DbUtil.Db.PeopleExtras
                    where e.StrValue != null || e.BitValue != null
                    group e by new { e.Field, val = e.StrValue ?? (e.BitValue == true ? "1" : "0") }
                        into g
                        select g.Key;
            var list = q.ToList();

            var ev = GetStandardExtraValues(null);
            var q2 = from e in list
                     let f = ev.SingleOrDefault(ff => ff.name == e.Field)
                     where f == null || f.UserCanView()
                     orderby e.Field, e.val
                     select new SelectListItem()
                            {
                                Text = e.Field + ":" + e.val,
                                Value = e.Field + ":" + e.val,
                            };
            return q2.ToList();
        }
        public Dictionary<string, string> Codes(string name)
        {
            var f = GetStandardExtraValues(null).Single(ee => ee.name == name);
            return f.Codes.ToDictionary(ee => ee, ee => ee);
        }

        public string CodesJson(string name)
        {
            var f = GetStandardExtraValues(null).Single(ee => ee.name == name);
            var q = from c in f.Codes
                    select new { value = c, text = c };
            return JsonConvert.SerializeObject(q.ToArray());
        }
        public Dictionary<string, bool> ExtraValueBits(string name)
        {
            var f = GetStandardExtraValues(Location).Single(ee => ee.name == name);
            var list = ListExtraValues().Where(pp => f.Codes.Contains(pp.Field)).ToList();
            var q = from c in f.Codes
                    join e in list on c equals e.Field into j
                    from e in j.DefaultIfEmpty()
                    select new { value = c, selected = (e != null && (e.BitValue ?? false)) };
            return q.ToDictionary(ee => ee.value, ee => ee.selected);
        }
        public string DropdownBitsJson(string name)
        {
            var f = GetStandardExtraValues(Location).Single(ee => ee.name == name);
            var list = ListExtraValues().Where(pp => f.Codes.Contains(pp.Field)).ToList();
            var q = from c in f.Codes
                    join e in list on c equals e.Field into j
                    from e in j.DefaultIfEmpty()
                    select new { value = c, text = c };
            return JsonConvert.SerializeObject(q.ToArray());
        }
        public string ListBitsJson(string name)
        {
            var f = GetStandardExtraValues(null).Single(ee => ee.name == name);
            var list = ListExtraValues().Where(pp => f.Codes.Contains(pp.Field)).ToList();
            var q = from c in f.Codes
                    join e in list on c equals e.Field into j
                    from e in j.DefaultIfEmpty()
                    where e != null && e.BitValue == true
                    select c;
            return JsonConvert.SerializeObject(q.ToArray());
        }

        public void EditExtra(string type, string name, string value)
        {
            var o = TableObject();
            if (o == null)
                return;
            if (value == null)
                value = HttpContext.Current.Request.Form["value[]"];
            switch (type)
            {
                case "Code":
                    o.AddEditExtraValue(name, value);
                    break;
                case "Data":
                    o.AddEditExtraData(name, value);
                    break;
                case "Date":
                    {
                        DateTime dt;
                        if (DateTime.TryParse(value, out dt))
                            o.AddEditExtraDate(name, dt);
                        else
                            o.RemoveExtraValue(DbUtil.Db, name);
                    }
                    break;
                case "Int":
                    o.AddEditExtraInt(name, value.ToInt());
                    break;
                case "Bit":
                    if (value == "True")
                        o.AddEditExtraBool(name, true);
                    else
                        o.RemoveExtraValue(DbUtil.Db, name);
                    break;
                case "Bits":
                    {
                        var cc = ExtraValueBits(name);
                        var aa = value.Split(',');
                        foreach (var c in cc)
                        {
                            if (aa.Contains(c.Key)) // checked now
                                if (!c.Value) // was not checked before
                                    o.AddEditExtraBool(c.Key, true);
                            if (!aa.Contains(c.Key)) // not checked now
                                if (c.Value) // was checked before
                                    o.RemoveExtraValue(DbUtil.Db, c.Key);
                        }
                        break;
                    }
            }
            DbUtil.Db.SubmitChanges();
        }

        public static void NewExtra(int id, string field, string type, string value)
        {
            field = field.Replace('/', '-');
            var v = new PeopleExtra { PeopleId = id, Field = field };
            DbUtil.Db.PeopleExtras.InsertOnSubmit(v);
            switch (type)
            {
                case "string":
                    v.StrValue = value;
                    break;
                case "text":
                    v.Data = value;
                    break;
                case "date":
                    var dt = DateTime.MinValue;
                    DateTime.TryParse(value, out dt);
                    v.DateValue = dt;
                    break;
                case "int":
                    v.IntValue = value.ToInt();
                    break;
            }
            DbUtil.Db.SubmitChanges();
        }
    }
}