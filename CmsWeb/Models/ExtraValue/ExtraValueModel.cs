using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.ExtraValue;
using CmsWeb.Code;
using Dapper;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class ExtraValueModel
    {
        public string Location { get; set; }
        public int Id { get; set; }
        public int? Id2 { get; set; }
        public string Table { get; set; }
        public string OtherLocations;

        public CodeInfo ExtraValueType { get; set; }
        public string Name { get; set; }
        public string CodeStrings { get; set; }
        public string ValueText { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBit { get; set; }
        public int? ValueInt { get; set; }

        public Guid CurrentPersonQueryId()
        {
            var qb = DbUtil.Db.QueryIsCurrentPerson();
            return qb.QueryId;
        }

        public int? CurrentPersonMainFellowshipId()
        {
            var qb = (from p in DbUtil.Db.People
                      where p.PeopleId == Id
                      select p.BibleFellowshipClassId).SingleOrDefault();
            return qb;
        }
        public ExtraValueModel() { }
        public ExtraValueModel(int id, string table)
            : this(id, table, null)
        {
        }

        public ExtraValueModel(string table)
            : this(0, table, null)
        {
        }

        public ExtraValueModel(string table, string location)
            : this(0, table, location)
        {
        }

        public ExtraValueModel(int id, string table, string location)
        {
            Id = id;
            Location = location;
            Table = table;
        }

        public ExtraValueModel(int id, int id2, string table, string location)
        {
            Id = id;
            Id2 = id2;
            Location = location;
            Table = table;
        }

        public static string HelpLink(string page)
        {
            return Util.HelpLink($"_AddExtraValue_{page}");
        }

        public IEnumerable<ExtraValue> ListExtraValues()
        {
            IEnumerable<ExtraValue> q = null;
            switch (Table)
            {
                case "People":
                    q = from ee in DbUtil.Db.PeopleExtras
                        where ee.PeopleId == Id
                        select new ExtraValue(ee, this);
                    break;
                case "Family":
                    q = from ee in DbUtil.Db.FamilyExtras
                        where ee.FamilyId == Id
                        select new ExtraValue(ee, this);
                    break;
                case "Organization":
                    q = from ee in DbUtil.Db.OrganizationExtras
                        where ee.OrganizationId == Id
                        select new ExtraValue(ee, this);
                    break;
                case "Meeting":
                    q = from ee in DbUtil.Db.MeetingExtras
                        where ee.MeetingId == Id
                        select new ExtraValue(ee, this);
                    break;
                case "OrgMember":
                    q = from ee in DbUtil.Db.OrgMemberExtras
                        where ee.OrganizationId == Id
                        where ee.PeopleId == Id2
                        select new ExtraValue(ee, this);
                    break;
                case "Contact":
                    q = from ee in DbUtil.Db.ContactExtras
                        where ee.ContactId == Id
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
            return TableObject(Id, Table, Id2);
        }

        public static ITableWithExtraValues TableObject(int id, string table, int? id2 = null)
        {
            switch (table)
            {
                case "People":
                    return DbUtil.Db.LoadPersonById(id);
                case "Organization":
                    return DbUtil.Db.LoadOrganizationById(id);
                case "Family":
                    return DbUtil.Db.Families.SingleOrDefault(f => f.FamilyId == id);
                case "OrgMember":
                    return DbUtil.Db.OrganizationMembers.SingleOrDefault(f => f.OrganizationId == id && f.PeopleId == id2);
                case "Contact":
                    return DbUtil.Db.LoadContactById(id);
                default:
                    return null;
            }
        }

        public List<Value> GetStandardExtraValues(string table, string location = null)
        {
            var q = from v in Views.GetStandardExtraValuesOrdered(DbUtil.Db, table, location)
                    where v.Type != "Data"
                    select Value.FromValue(v);
            return q.ToList();
        }

        public List<Value> GetStandardExtraValuesWithDataType(string table, string location = null)
        {
            var q = from v in Views.GetStandardExtraValuesOrdered(DbUtil.Db, table, location)
                    select Value.FromValue(v);
            return q.ToList();
        }

        public IEnumerable<Value> GetExtraValues()
        {
            var realExtraValues = ListExtraValues();

            if (Location.ToLower() == "adhoc")
            {
                var standardExtraValues = GetStandardExtraValues(Table);
                return from v in realExtraValues
                       join f in standardExtraValues on v.Field equals f.Name into j
                       from f in j.DefaultIfEmpty()
                       where f == null
                       // only adhoc values
                       where !standardExtraValues.Any(ff => ff.Codes.Any(cc => cc.Text == v.Field))
                       where v.Type != "Data"
                       orderby v.Field
                       select Value.AddField(f, v, this);
            }

            return from f in GetStandardExtraValues(Table, Location)
                   join v in realExtraValues on f.Name equals v.Field into j
                   from v in j.DefaultIfEmpty()
                   where v?.Type != "Data"
                   orderby f.Order
                   select Value.AddField(f, v, this);
        }

        public Dictionary<string, string> Codes(string name)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, Table, false, Location).Single(ee => ee.Name == name);
            return f.Codes.ToDictionary(ee => ee.Text, ee => ee.Text);
        }

        public string CodesJson(string name)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, Table, false, Location).Single(ee => ee.Name == name);
            var q = from c in f.Codes
                    select new {value = c.Text, text = c.Text};
            return JsonConvert.SerializeObject(q.ToArray());
        }

        private class AllBitsCheckedOrNot
        {
            public string Name { get; set; }
            public bool Checked { get; set; }
        }

        private IEnumerable<AllBitsCheckedOrNot> ExtraValueBits(string name)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, Table, false, Location).Single(ee => ee.Name == name);
            var list = ListExtraValues().Where(pp => f.Codes.Select(x => x.Text).Contains(pp.Field)).ToList();
            var q = from c in f.Codes
                    join e in list on c.Text equals e.Field into j
                    from e in j.DefaultIfEmpty()
                    select new AllBitsCheckedOrNot()
                    {
                        Name = c.Text,
                        Checked = (e != null && (e.BitValue ?? false))
                    };
            return q;
        }

        public string DropdownBitsJson(string name)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, Table, false, Location).Single(ee => ee.Name == name);
            var list = ListExtraValues().Where(pp => f.Codes.Select(x => x.Text).Contains(pp.Field)).ToList();
            var q = from c in f.Codes
                    join e in list on c.Text equals e.Field into j
                    from e in j.DefaultIfEmpty()
                    select new
                    {
                        value = c.Text,
                        text = Value.NoPrefix(c.Text)
                    };
            return JsonConvert.SerializeObject(q.ToArray());
        }

        public string ListBitsJson(string name)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, Table, false, Location).First(ee => ee.Name == name);
            var list = ListExtraValues().Where(pp => f.Codes.Select(x => x.Text).Contains(pp.Field)).ToList();
            var q = from c in f.Codes
                    join e in list on c.Text equals e.Field into j
                    from e in j.DefaultIfEmpty()
                    where e != null && e.BitValue == true
                    select c.Text;
            return JsonConvert.SerializeObject(q.ToArray());
        }

        private bool logged;

        private void Log(ITableWithExtraValues record, string op, string name)
        {
            if (logged)
                return;
            record.LogExtraValue(op, name);
            logged = true;
        }

        private void RemoveExtraValue(ITableWithExtraValues record, string name)
        {
            record.RemoveExtraValue(DbUtil.Db, name);
            Log(record, "remove", name);
        }

        public void EditExtra(string type, string name, string[] values)
        {
            var value = values?.First();

            var record = TableObject();
            if (record == null)
                return;
            switch (type)
            {
                case "Code":
                    record.AddEditExtraCode(name, value, Location);
                    break;
                case "Data":
                case "Text":
                case "Text2":
                    if (!value.HasValue())
                        RemoveExtraValue(record, name);
                    else
                        record.AddEditExtraText(name, value);
                    break;
                case "Date":
                {
                    DateTime dt;
                    if (DateTime.TryParse(value, out dt))
                        record.AddEditExtraDate(name, dt);
                    else
                        RemoveExtraValue(record, name);
                }
                    break;
                case "Int":
                    record.AddEditExtraInt(name, value.ToInt());
                    break;
                case "Bit":
                    if (value == "True")
                        record.AddEditExtraBool(name, true);
                    else
                        record.RemoveExtraValue(DbUtil.Db, name);
                    break;
                case "Bits":
                {
                    var existingBits = ExtraValueBits(name);
                    var newCheckedBits = values ?? new string[] {};
                    foreach (var currentBit in existingBits)
                    {
                        if (newCheckedBits.Contains(currentBit.Name))
                            if (!currentBit.Checked)
                                record.AddEditExtraBool(currentBit.Name, true, name, Location);
                        if (!newCheckedBits.Contains(currentBit.Name))
                            if (currentBit.Checked)
                                RemoveExtraValue(record, currentBit.Name);
                    }
                    break;
                }
            }
            DbUtil.Db.SubmitChanges();
            Log(record, "set", name);
        }

        public static void NewExtra(int id, string field, string type, string value)
        {
            //field = field.Replace('/', '-');
            var v = new PeopleExtra {PeopleId = id, Field = field};
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
                    DateTime dt;
                    DateTime.TryParse(value, out dt);
                    v.DateValue = dt;
                    break;
                case "int":
                    v.IntValue = value.ToInt();
                    break;
            }
            DbUtil.Db.SubmitChanges();
        }

        public void DeleteStandard(string name, bool removedata)
        {
            var i = Views.GetViewsViewValue(DbUtil.Db, Table, name, Location);
            i.view.Values.Remove(i.value);
            i.views.Save(DbUtil.Db);

            if (!removedata)
                return;
            var cn = DbUtil.Db.Connection;
            cn.Open();
            if (i.value.Codes.Count == 0)
                cn.Execute($"delete from dbo.{Table}Extra where Field = @name", new {name});
            else
                cn.Execute($"delete from dbo.{Table}Extra where Field in @codes", new {codes = i.value.Codes.Select(x => x.Text)});
        }

        public void Delete(string name)
        {
            var o = TableObject();
            o.RemoveExtraValue(DbUtil.Db, name);
            DbUtil.Db.SubmitChanges();
        }

        public void ApplyOrder(Dictionary<string, int> orders)
        {
            var i = Views.GetViewsView(DbUtil.Db, Table, Location);
            var q = from v in i.view.Values
                    join o in orders on v.Name equals o.Key
                    orderby o.Value
                    select v;
            i.view.Values = q.ToList();
            int n = 1;
            foreach (var v in i.view.Values)
                v.Order = n++;
            i.views.Save(DbUtil.Db);
        }

        public void SwitchMultiline(string name)
        {
            var i = Views.GetViewsViewValue(DbUtil.Db, Table, name, Location);
            i.value.Type = i.value.Type == "Text" ? "Text2" : "Text";
            i.views.Save(DbUtil.Db);
        }
    }
}
