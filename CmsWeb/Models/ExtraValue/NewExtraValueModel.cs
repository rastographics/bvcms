using CmsData;
using CmsData.ExtraValue;
using CmsWeb.Code;
using Dapper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class NewExtraValueModel
    {
        public int Id { get; set; }
        public int Id2 { get; set; }
        public Guid QueryId { get; set; }
        public string ExtraValueTable { get; set; }
        public string ExtraValueLocation { get; set; }
        public bool ClearOldValuesFirst { get; set; }

        [DisplayName("Name"), Required]
        public string ExtraValueName { get; set; }

        [DisplayName("Type")]
        public CodeInfo ExtraValueType { get; set; }

        [DisplayName("Checkboxes Prefix"), StringLength(12)]
        public string ExtraValueBitPrefix { get; set; }

        [DisplayName("Type")]
        public CodeInfo AdhocExtraValueType { get; set; }

        [DisplayName("HyperLink")]
        public string ExtraValueLink { get; set; }

        [DisplayName("Text Value")]
        public string ExtraValueTextBox { get; set; }

        [DisplayName("Text Value"), UIHint("Textarea")]
        public string ExtraValueTextArea { get; set; }

        [DisplayName("Date Value")]
        public DateTime? ExtraValueDate { get; set; }

        [DisplayName("Checkbox Value")]
        public bool ExtraValueCheckbox { get; set; }

        public bool AddToFamilyRecordInsteadOfPerson { get; set; }
        public bool DeleteFromFamilyRecordInsteadOfPerson { get; set; }

        [DisplayName("Integer Value")]
        public int ExtraValueInteger { get; set; }

        public object Hidden(string type)
        {
            return type == AdhocExtraValueType.Value
                ? new { hide = "" }
                : new { hide = "hide" };
        }

        public bool RemoveAnyValue { get; set; }

        private string BitPrefix
        {
            get
            {
                if (ExtraValueBitPrefix.HasValue())
                {
                    return ExtraValueBitPrefix + ":";
                }

                return "";
            }
        }

        [DisplayName("Codes")]
        public string ExtraValueCodes { get; set; }

        [DisplayName("Checkboxes"), UIHint("Textarea")]
        public string ExtraValueCheckboxes { get; set; }

        [DisplayName("Limit to Roles")]
        public string VisibilityRoles
        {
            get
            {
                var s = string.Join(", ", VisibilityRolesList ?? new string[0]);
                return !s.HasValue() ? null : s;
            }
            set
            {
                VisibilityRolesList = (value ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            }
        }

        [DisplayName("Limit to Roles")]
        public string[] VisibilityRolesList { get; set; }

        [DisplayName("Editable by Roles")]
        public string EditableRoles
        {
            get
            {
                var s = string.Join(", ", EditableRolesList ?? new string[0]);
                return !s.HasValue() ? null : s;
            }
            set
            {
                EditableRolesList = (value ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            }
        }

        [DisplayName("Editable by Roles")]
        public string[] EditableRolesList { get; set; }

        public IEnumerable<SelectListItem> VisibilitySelectedRoles()
        {
            var list = AllRoles.ToList();
            foreach (var item in list.Where(item => (VisibilityRoles ?? "").Contains(item.Text)))
            {
                item.Selected = true;
            }
            return list;
        }

        public IEnumerable<SelectListItem> EditableSelectedRoles()
        {
            var list = AllRoles.ToList();
            foreach (var item in AllRoles.Where(item => (VisibilityRoles ?? "").Contains(item.Text)))
            {
                item.Selected = true;
            }
            return list;
        }

        private IEnumerable<SelectListItem> AllRoles
        {
            get
            {
                var q = from r in DbUtil.Db.Roles
                        orderby r.RoleName
                        select new SelectListItem
                        {
                            Value = r.RoleName,
                            Text = r.RoleName
                        };
                return q.ToList();
            }
        }

        public NewExtraValueModel(Guid id)
        {
            ExtraValueType = new CodeInfo("Text", "ExtraValueType");
            AdhocExtraValueType = new CodeInfo("Text", "AdhocExtraValueType");
            QueryId = id;
            ExtraValueTable = "People";
            ExtraValueLocation = "Adhoc";
        }

        public NewExtraValueModel(int id, string table, string location)
        {
            ExtraValueType = new CodeInfo("Text", "ExtraValueType");
            AdhocExtraValueType = new CodeInfo("Text", "AdhocExtraValueType");
            Id = id;
            ExtraValueTable = table;
            ExtraValueLocation = location;
        }
        public NewExtraValueModel(int id, string table, string name, string location)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, table, false, location).Single(ee => ee.Name == name);
            ExtraValueType = new CodeInfo(f.Type, "ExtraValueType");
            Id = id;
            ExtraValueName = name;
            ExtraValueTable = table;
            ExtraValueLocation = location;
            VisibilityRoles = f.VisibilityRoles;
            EditableRoles = f.EditableRoles;
            ExtraValueLink = HttpUtility.HtmlDecode(f.Link);
            var codes = string.Join("\n", f.Codes.Select(x => x.Text));
            switch (ExtraValueType.Value)
            {
                case "Bits":
                    ExtraValueCheckboxes = codes;
                    break;
                case "Code":
                    ExtraValueCodes = codes;
                    break;
            }
        }
        public NewExtraValueModel(int id, int id2, string table, string location)
        {
            ExtraValueType = new CodeInfo("Text", "ExtraValueType");
            AdhocExtraValueType = new CodeInfo("Text", "AdhocExtraValueType");
            Id = id;
            Id2 = id2;
            ExtraValueTable = table;
            ExtraValueLocation = location;
        }

        public NewExtraValueModel()
        {
        }

        public NewExtraValueModel(string table, string name)
        {
            var f = Views.GetStandardExtraValues(DbUtil.Db, table).Single(ee => ee.Name == name);
            ExtraValueType = new CodeInfo(f.Type, "ExtraValueType");
            ExtraValueName = name;
            ExtraValueTable = table;
            VisibilityRoles = f.VisibilityRoles;
            EditableRoles = f.EditableRoles;
            ExtraValueLink = HttpUtility.HtmlDecode(f.Link);
            var codes = string.Join("\n", f.Codes.Select(x => x.Text));
            switch (ExtraValueType.Value)
            {
                case "Bits":
                    ExtraValueCheckboxes = codes;
                    break;
                case "Code":
                    ExtraValueCodes = codes;
                    break;
            }
        }

        private void TryCheckIntegrity()
        {
            if (ClearOldValuesFirst)
            {
                return;
            }

            const string nameAlreadyExistsAsADifferentType = "{0} already exists as a different type";
            string type = ExtraValueLocation == "Adhoc" ? AdhocExtraValueType.Value : ExtraValueType.Value;
            if (type == "Text2")
            {
                type = "Text";
            }

            switch (ExtraValueTable)
            {
                case "People":
                    if (type == "Bits")
                    {
                        foreach (var b in ConvertToCodes().Where(b => DbUtil.Db.PeopleExtras.Any(ee => ee.Field == b.Text && ee.Type != "Bit")))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, b));
                        }
                    }
                    else
                    {
                        if (DbUtil.Db.PeopleExtras.Any(ee => ee.Field == ExtraValueName && ee.Type != type))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, ExtraValueName));
                        }

                        CheckDifferentCase();
                    }
                    break;
                case "Family":
                    if (type == "Bits")
                    {
                        foreach (var b in ConvertToCodes().Where(b => DbUtil.Db.FamilyExtras.Any(ee => ee.Field == b.Text && ee.Type != "Bit")))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, b));
                        }
                    }
                    else
                    {
                        if (DbUtil.Db.FamilyExtras.Any(ee => ee.Field == ExtraValueName && ee.Type != type))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, ExtraValueName));
                        }

                        CheckDifferentCase();
                    }
                    break;
                case "Organization":
                    if (type == "Bits")
                    {
                        foreach (var b in ConvertToCodes().Where(b => DbUtil.Db.OrganizationExtras.Any(ee => ee.Field == b.Text && ee.Type != "Bit")))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, b));
                        }
                    }
                    else
                    {
                        if (DbUtil.Db.OrganizationExtras.Any(ee => ee.Field == ExtraValueName && ee.Type != type))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, ExtraValueName));
                        }

                        CheckDifferentCase();
                    }
                    break;
                case "Meeting":
                    if (type == "Bits")
                    {
                        foreach (var b in ConvertToCodes().Where(b => DbUtil.Db.MeetingExtras.Any(ee => ee.Field == b.Text && ee.Type != "Bit")))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, b));
                        }
                    }
                    else
                    {
                        if (DbUtil.Db.MeetingExtras.Any(ee => ee.Field == ExtraValueName && ee.Type != type))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, ExtraValueName));
                        }

                        CheckDifferentCase();
                    }
                    break;
                case "OrgMember":
                    if (type == "Bits")
                    {
                        foreach (var b in ConvertToCodes().Where(b => DbUtil.Db.OrgMemberExtras.Any(ee => ee.Field == b.Text && ee.Type != "Bit")))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, b));
                        }
                    }
                    else
                    {
                        if (DbUtil.Db.OrgMemberExtras.Any(ee => ee.Field == ExtraValueName && ee.Type != type))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, ExtraValueName));
                        }

                        CheckDifferentCase();
                    }
                    break;
                case "Contact":
                    if (type == "Bits")
                    {
                        foreach (var b in ConvertToCodes().Where(b => DbUtil.Db.ContactExtras.Any(ee => ee.Field == b.Text && ee.Type != "Bit")))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, b));
                        }
                    }
                    else
                    {
                        if (DbUtil.Db.ContactExtras.Any(ee => ee.Field == ExtraValueName && ee.Type != type))
                        {
                            throw new Exception(string.Format(nameAlreadyExistsAsADifferentType, ExtraValueName));
                        }

                        CheckDifferentCase();
                    }
                    break;
            }
        }

        private void CheckDifferentCase()
        {
            var fields = Views.GetStandardExtraValues(DbUtil.Db, ExtraValueTable);
            var existing = fields.SingleOrDefault(ff => ff.Name.Equal(ExtraValueName));
            if (existing != null && existing.Name != ExtraValueName)
            {
                throw new Exception($"{existing.Name} <> {ExtraValueName}");
            }
        }

        public List<CmsData.Classes.ExtraValues.Code> ConvertToCodes()
        {
            const string defaultCodes = @"
Option 1
Option 2
";
            var codes = ExtraValueType.Value == "Bits"
                ? ExtraValueCheckboxes ?? defaultCodes
                : ExtraValueType.Value == "Code"
                    ? ExtraValueCodes ?? defaultCodes
                    : null;
            return codes.SplitLines(noblanks: true).Select(ss => new CmsData.Classes.ExtraValues.Code { Text = BitPrefix + ss }).ToList();
        }

        public string AddAsNewStandard()
        {
            if (ExtraValueType.Value != "HTML")
            {
                ExtraValueName = ExtraValueName.Replace('/', '-');
            }

            var fields = Views.GetStandardExtraValues(DbUtil.Db, ExtraValueTable, false, ExtraValueLocation);
            var existing = fields.SingleOrDefault(ff => ff.Name == ExtraValueName);
            if (existing != null)
            {
                throw new Exception($"{ExtraValueName} already exists");
            }

            TryCheckIntegrity();

            var v = new CmsData.ExtraValue.Value
            {
                Type = ExtraValueType.Value,
                Name = ExtraValueName,
                VisibilityRoles = VisibilityRoles,
                EditableRoles = EditableRoles,
                Codes = ConvertToCodes(),
                Link = HttpUtility.HtmlEncode(ExtraValueLink)
            };
            var i = Views.GetViewsView(DbUtil.Db, ExtraValueTable, ExtraValueLocation);
            i.view.Values.Add(v);
            i.views.Save(DbUtil.Db);
            return null;
        }

        public string AddAsNewAdhoc()
        {
            TryCheckIntegrity();
            if (Id > 0)
            {
                return AddNewExtraValueToRecord();
            }

            return AddNewExtraValueToSelectionFromQuery();
        }

        private string AddNewExtraValueToRecord()
        {
            var o = ExtraValueModel.TableObject(Id, ExtraValueTable, Id2);
            switch (AdhocExtraValueType.Value)
            {
                case "Code":
                    o.AddEditExtraCode(ExtraValueName, ExtraValueTextBox);
                    break;
                case "Text":
                    o.AddEditExtraText(ExtraValueName, ExtraValueTextArea);
                    break;
                case "Text2":
                    o.AddEditExtraText(ExtraValueName, ExtraValueTextArea);
                    break;
                case "Date":
                    o.AddEditExtraDate(ExtraValueName, ExtraValueDate);
                    break;
                case "Int":
                    o.AddEditExtraInt(ExtraValueName, ExtraValueInteger);
                    break;
                case "Bit":
                    o.AddEditExtraBool(ExtraValueName, ExtraValueCheckbox);
                    break;
            }
            DbUtil.Db.SubmitChanges();
            o.LogExtraValue("add", ExtraValueName);
            return null;
        }

        private string AddNewExtraValueToSelectionFromQuery()
        {
            var list = DbUtil.Db.PeopleQuery(QueryId).Select(pp => pp.PeopleId).ToList();
            if (ClearOldValuesFirst)
            {
                using (var db = DbUtil.Create(DbUtil.Db.Host))
                {
                    var q = DbUtil.Db.PeopleQuery(QueryId).Select(pp => pp.PeopleId);
                    var tag = DbUtil.Db.PopulateTemporaryTag(q);
                    var cmd = AddToFamilyRecordInsteadOfPerson
                        ? "dbo.ClearFamilyExtraValuesForTag"
                        : "dbo.ClearExtraValuesForTag";
                    DbUtil.Db.ExecuteCommand(cmd + " {0}, {1}", ExtraValueName, tag.Id);
                }
            }

            if (AddToFamilyRecordInsteadOfPerson)
            {
                ExtraValueTable = "Family";
            }

            switch (AdhocExtraValueType.Value)
            {
                case "Code":
                    return AddNewExtraValueCodes(list);
                case "Text":
                case "Text2":
                    return AddNewExtraValueDatums(list);
                case "Date":
                    return AddNewExtraValueDates(list);
                case "Int":
                    return AddNewExtraValueInts(list);
                case "Bit":
                    return AddNewExtraValueBools(list);
            }
            DbUtil.LogActivity($"EV AddNewFromQuery {ExtraValueName} {AdhocExtraValueType.Value}");
            return null;
        }

        private string AddNewExtraValueCodes(IEnumerable<int> list)
        {
            foreach (var pid in list)
            {
                if (AddToFamilyRecordInsteadOfPerson)
                {
                    Family.AddEditExtraValue(DbUtil.Db, pid, ExtraValueName, ExtraValueTextBox);
                }
                else
                {
                    Person.AddEditExtraValue(DbUtil.Db, pid, ExtraValueName, ExtraValueTextBox);
                }

                DbUtil.Db.SubmitChanges();
                //DbDispose();
            }
            return null;
        }

        private string AddNewExtraValueDatums(IEnumerable<int> list)
        {
            foreach (var pid in list)
            {
                if (AddToFamilyRecordInsteadOfPerson)
                {
                    Family.AddEditExtraData(DbUtil.Db, pid, ExtraValueName, ExtraValueTextArea);
                }
                else
                {
                    Person.AddEditExtraData(DbUtil.Db, pid, ExtraValueName, ExtraValueTextArea);
                }

                DbUtil.Db.SubmitChanges();
                //DbDispose();
            }
            return null;
        }

        private string AddNewExtraValueDates(IEnumerable<int> list)
        {
            foreach (var pid in list)
            {
                if (AddToFamilyRecordInsteadOfPerson)
                {
                    Family.AddEditExtraDate(DbUtil.Db, pid, ExtraValueName, ExtraValueDate);
                }
                else
                {
                    Person.AddEditExtraDate(DbUtil.Db, pid, ExtraValueName, ExtraValueDate);
                }

                DbUtil.Db.SubmitChanges();
                //DbDispose();
            }
            return null;
        }

        private string AddNewExtraValueInts(IEnumerable<int> list)
        {
            foreach (var pid in list)
            {
                if (AddToFamilyRecordInsteadOfPerson)
                {
                    Family.AddEditExtraInt(DbUtil.Db, pid, ExtraValueName, ExtraValueInteger);
                }
                else
                {
                    Person.AddEditExtraInt(DbUtil.Db, pid, ExtraValueName, ExtraValueInteger);
                }

                DbUtil.Db.SubmitChanges();
                //DbDispose();
            }
            return null;
        }

        private string AddNewExtraValueBools(IEnumerable<int> list)
        {
            foreach (var pid in list)
            {
                if (AddToFamilyRecordInsteadOfPerson)
                {
                    Family.AddEditExtraBool(DbUtil.Db, pid, ExtraValueName, ExtraValueCheckbox);
                }
                else
                {
                    Person.AddEditExtraBool(DbUtil.Db, pid, ExtraValueName, ExtraValueCheckbox);
                }

                DbUtil.Db.SubmitChanges();
                //DbDispose();
            }
            return null;
        }

        public void DeleteFromQuery()
        {
            var tag = DbUtil.Db.PopulateSpecialTag(QueryId, DbUtil.TagTypeId_ExtraValues);

            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();

            const string fsql = @"
DELETE dbo.FamilyExtra
FROM FamilyExtra fe
WHERE fe.Field = @name
AND EXISTS(SELECT NULL FROM dbo.People p
			WHERE p.FamilyId = fe.FamilyId
			AND p.PeopleId IN (
				SELECT PeopleId FROM TagPerson WHERE Id = @id))
";
            const string psql = @"
delete from dbo.PeopleExtra
where Field = @name
and PeopleId in (select PeopleId from TagPerson where Id = @id)
";
            var sql = DeleteFromFamilyRecordInsteadOfPerson ? fsql : psql;

            if (RemoveAnyValue)
            {
                cn.Execute(sql, new { name = ExtraValueName, id = tag.Id });
                DbUtil.LogActivity($"EV DeleteFromQuery {ExtraValueName}");
                return;
            }
            switch (AdhocExtraValueType.Value)
            {
                case "Bit":
                    cn.Execute(sql + "and BitValue = @value",
                        new { name = ExtraValueName, value = ExtraValueCheckbox, id = tag.Id });
                    break;
                case "Code":
                    cn.Execute(sql + "and StrValue = @value",
                        new { name = ExtraValueName, value = ExtraValueCheckbox, id = tag.Id });
                    break;
                case "Text2":
                case "Text":
                    cn.Execute(sql + "and Data = @value",
                        new { name = ExtraValueName, value = ExtraValueTextArea, id = tag.Id });
                    break;
                case "Date":
                    cn.Execute(sql + "and Date = @value",
                        new { name = ExtraValueName, value = ExtraValueDate, id = tag.Id });
                    break;
                case "Int":
                    cn.Execute(sql + "and IntValue = @value",
                        new { name = ExtraValueName, value = ExtraValueInteger, id = tag.Id });
                    break;
            }
            DbUtil.LogActivity($"EV DeleteFromQuery {ExtraValueName} {AdhocExtraValueType.Value}");
        }

        public void ConvertToStandard(string name)
        {
            //            var oldfields = StandardExtraValues.GetExtraValues().ToList();
            var oldfields = Views.GetStandardExtraValues(DbUtil.Db, "People");
            ExtraValue ev = null;
            List<string> codes = null;
            var v = new CmsData.ExtraValue.Value { Name = name };
            switch (ExtraValueTable)
            {
                case "People":
                    ev = (from vv in DbUtil.Db.PeopleExtras
                          where vv.Field == name
                          select new ExtraValue(vv, null)).First();

                    if (CreateExtraValueBits(name, ev, v))
                    {
                        return;
                    }

                    //StandardExtraValues.Field bits = null;
                    var bits = oldfields.SingleOrDefault(ff => ff.Codes.Select(x => x.Text).Contains(name));
                    if (bits != null)
                    {
                        codes = bits.Codes.Select(x => x.Text).ToList();
                        ev.Type = "Bits";
                        v.Name = bits.Name;
                        v.VisibilityRoles = bits.VisibilityRoles;
                        v.EditableRoles = bits.EditableRoles;
                    }
                    else
                    {
                        var f = oldfields.SingleOrDefault(ff => ff.Name == name);
                        if (f != null)
                        {
                            v.VisibilityRoles = f.VisibilityRoles;
                            v.EditableRoles = f.EditableRoles;
                        }
                        if (ev.Type == "Code")
                        {
                            codes = (from vv in DbUtil.Db.PeopleExtras
                                     where vv.Field == name
                                     select vv.StrValue).Distinct().ToList();
                        }
                    }
                    break;
                case "Organization":
                    ev = (from vv in DbUtil.Db.OrganizationExtras
                          where vv.Field == name
                          select new ExtraValue(vv, null)).First();
                    if (ev.Type == "Code")
                    {
                        codes = (from vv in DbUtil.Db.OrganizationExtras
                                 where vv.Field == name
                                 select vv.StrValue).Distinct().ToList();
                    }

                    break;
                case "Family":
                    ev = (from vv in DbUtil.Db.FamilyExtras
                          where vv.Field == name
                          select new ExtraValue(vv, null)).First();
                    if (ev.Type == "Code")
                    {
                        codes = (from vv in DbUtil.Db.FamilyExtras
                                 where vv.Field == name
                                 select vv.StrValue).Distinct().ToList();
                    }

                    break;
                case "Meeting":
                    ev = (from vv in DbUtil.Db.MeetingExtras
                          where vv.Field == name
                          select new ExtraValue(vv, null)).First();
                    if (ev.Type == "Code")
                    {
                        codes = (from vv in DbUtil.Db.MeetingExtras
                                 where vv.Field == name
                                 select vv.StrValue).Distinct().ToList();
                    }

                    break;
                case "OrgMember":
                    ev = (from vv in DbUtil.Db.OrgMemberExtras
                          where vv.Field == name
                          select new ExtraValue(vv, null)).First();
                    if (ev.Type == "Code")
                    {
                        codes = (from vv in DbUtil.Db.OrgMemberExtras
                                 where vv.Field == name
                                 select vv.StrValue).Distinct().ToList();
                    }

                    break;
                case "Contact":
                    ev = (from vv in DbUtil.Db.ContactExtras
                          where vv.Field == name
                          select new ExtraValue(vv, null)).First();
                    if (ev.Type == "Code")
                    {
                        codes = (from vv in DbUtil.Db.ContactExtras
                                 where vv.Field == name
                                 select vv.StrValue).Distinct().ToList();
                    }

                    break;
                default:
                    return;
            }
            v.Type = ev.Type;
            v.Codes = codes?.Select(x => new CmsData.Classes.ExtraValues.Code { Text = x }).ToList();
            var i = Views.GetViewsView(DbUtil.Db, ExtraValueTable, ExtraValueLocation);
            i.view.Values.Add(v);
            i.views.Save(DbUtil.Db);
            DbUtil.LogActivity($"EV{ExtraValueTable} ConvertToStandard {name}");
        }

        private bool CreateExtraValueBits(string name, ExtraValue ev, CmsData.ExtraValue.Value v)
        {
            if (!name.Contains(":"))
            {
                return false;
            }

            var prefix = name.GetCsvToken(1, 2, ":");
            var allbits = (from vv in DbUtil.Db.PeopleExtras
                           where vv.Field.StartsWith($"{prefix}:")
                           orderby vv.Field
                           select vv.Field).Distinct().ToList();
            if (allbits.Count <= 1)
            {
                return false;
            }

            v.Name = prefix;
            v.Type = "Bits";
            v.Codes = allbits.Select(x => new CmsData.Classes.ExtraValues.Code { Text = x }).ToList();
            var view = Views.GetViewsView(DbUtil.Db, ExtraValueTable, ExtraValueLocation);
            view.view.Values.Add(v);
            view.views.Save(DbUtil.Db);
            DbUtil.LogActivity($"EV{ExtraValueTable} ConvertToStandard {name}");
            return true;
        }
    }
}
