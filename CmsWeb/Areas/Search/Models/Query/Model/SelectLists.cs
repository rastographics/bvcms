using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel
    {
        public IEnumerable<SelectListItem> GetCodeData()
        {
            if (!CodeVisible)
                return null;
            var cvctl = new CodeValueModel();
            var valtype = "IdValue";
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                    return ConvertToSelect(BitCodes, valtype);
                case FieldType.EqualBit:
                    return ConvertToSelect(EqualBitCodes, valtype);
                case FieldType.Code:
                case FieldType.NullCode:
                    if (fieldMap.DataSource == "Campuses")
                        return SelectedList(Campuses());
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource),
                        Util.PickFirst(fieldMap.DataValueField, valtype));
                case FieldType.CodeStr:
                    switch (fieldMap.DataSource)
                    {
                        case "ExtraValues":
                            return SelectedList(ExtraValueCodes(Db));
                        case "FamilyExtraValues":
                            return SelectedList(FamilyExtraValueCodes(Db));
                        case "Attributes":
                            return SelectedList(ExtraValueAttributes(Db));
                        default:
                            return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                    }
                case FieldType.DateField:
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
            }
            return null;
        }
        public static List<SelectListItem> ExtraValueCodes(CMSDataContext db)
        {
            var q = from e in db.PeopleExtras
                    where e.StrValue != null || e.BitValue != null
                    group e by new { e.Field, val = e.StrValue ?? (e.BitValue == true ? "1" : "0") }
                        into g
                        select g.Key;
            var list = q.ToList();

            var ev = CmsData.ExtraValue.Views.GetStandardExtraValues(db, "People");
            var q2 = from e in list
                     let f = ev.SingleOrDefault(ff => ff.Name == e.Field)
                     where f == null || f.UserCanView(db)
                     orderby e.Field, e.val
                     select new SelectListItem()
                            {
                                Text = e.Field + ":" + e.val,
                                Value = e.Field + ":" + e.val,
                            };
            return q2.ToList();
        }
        public static List<SelectListItem> FamilyExtraValueCodes(CMSDataContext db)
        {
            var q = from e in db.FamilyExtras
                    where e.StrValue != null || e.BitValue != null
                    group e by new { e.Field, val = e.StrValue ?? (e.BitValue == true ? "1" : "0") }
                        into g
                        select g.Key;
            var list = q.ToList();

            var ev = CmsData.ExtraValue.Views.GetStandardExtraValues(db, "Family");
            var q2 = from e in list
                     let f = ev.SingleOrDefault(ff => ff.Name == e.Field)
                     where f == null || f.UserCanView(db)
                     orderby e.Field, e.val
                     select new SelectListItem()
                            {
                                Text = e.Field + ":" + e.val,
                                Value = e.Field + ":" + e.val,
                            };
            return q2.ToList();
        }
        public static List<SelectListItem> ExtraValueAttributes(CMSDataContext db)
        {
            var q = from e in db.ViewAttributes
                    group e by new { e.Field, e.Name, e.ValueX } 
                        into g
                        select g.Key;
            var list = q.ToList();

            var q2 = from e in list
                     orderby e.Field, e.Name, e.ValueX
                     select new SelectListItem()
                            {
                                Text = $"{e.Field}:{e.Name}:{e.ValueX}",
                                Value = $"{e.Field}:{e.Name}:{e.ValueX}",
                            };
            return q2.ToList();
        }

        private IEnumerable<SelectListItem> SelectedList(IEnumerable<SelectListItem> items)
        {
            if (CodeValues != null)
                return items.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Text,
                        Value = c.Value,
                        Selected = CodeValues.Contains(c.Value)
                    });
            return items;
        }

        private List<SelectListItem> ConvertToSelect(object items, string valuefield, List<string> values = null)
        {
            var codeValueItems = items as IEnumerable<CodeValueItem>;
            if (codeValueItems == null)
                return new List<SelectListItem>();

            var codeValueList = codeValueItems.ToList();

            List<SelectListItem> list2;
            if (values == null)
                values = CodeValues?.ToList() ?? new List<string>();

            switch (valuefield)
            {
                case "IdCode":
                    list2 = codeValueList.Select(c => new SelectListItem { Text = c.Value, Value = c.IdCode, Selected = values.Contains(c.IdCode) }).ToList();
                    break;
                case "IdValue":
                    list2 = codeValueList.Select(c => new SelectListItem { Text = c.Value, Value = c.IdValue, Selected = values.Any(vv => vv.StartsWith($"{c.Id},") || vv == $"{c.Id}") }).ToList();
                    break;
                case "Id":
                    list2 = codeValueList.Select(c => new SelectListItem { Text = c.Value, Value = c.Id.ToString(), Selected = values.Contains(c.Id.ToString()) }).ToList();
                    break;
                case "CodeValue":
                    list2 = codeValueList.Select(c => new SelectListItem { Text = c.Value, Value = c.CodeValue, Selected = values.Any(vv => vv.StartsWith($"{c.Code}:") || vv == c.Code)}).ToList();
                    break;
                case "Code":
                    list2 = codeValueList.Select(c => new SelectListItem { Text = c.Value, Value = c.Code, Selected = values.Contains(c.Code) }).ToList();
                    break;
                default:
                    list2 = codeValueList.Select(c => new SelectListItem { Text = c.Value, Value = c.Value, Selected = values.Contains(c.Value) }).ToList();
                    break;
            }
            return list2;
        }
        public IEnumerable<SelectListItem> GroupComparisons()
        {
            return from c in CompareClass.Comparisons
                   where c.FieldType == FieldType.Group
                   let comp = c.CompType == CompareType.AllTrue ? "All"
                       : c.CompType == CompareType.AnyTrue ? "Any"
                           : c.CompType == CompareType.AllFalse ? "None"
                               : c.CompType == CompareType.AnyFalse ? "Not All"
                                   : "unknown"
                   select new SelectListItem
                   {
                       Text = comp,
                       Value = c.CompType.ToString(),
                       Selected = Comparison == comp
                   };
        }
        public IEnumerable<SelectListItem> Comparisons()
        {
            return from c in CompareClass.Comparisons
                   where c.FieldType == fieldMap.Type
                   let ct = c.CompType.ToString()
                   select new SelectListItem
                   {
                       Text = ct,
                       Value = c.CompType.ToString(),
                       Selected = Comparison == ct
                   };
        }
        public IEnumerable<SelectListItem> Schedules()
        {
            if (!ScheduleVisible)
                return null;
            var q = from o in Db.Organizations
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where sc != null && sc.MeetingTime != null
                    group o by new { ScheduleId = sc.ScheduleId ?? 10800, sc.MeetingTime } into g
                    orderby g.Key.ScheduleId
                    let text = Db.GetScheduleDesc(g.Key.MeetingTime)
                    select new SelectListItem
                    {
                        Value = $"{g.Key.ScheduleId},{text}",
                        Text = text,
                        Selected = ScheduleInt == g.Key.ScheduleId
                    };
            var slist = q.ToList();
            slist.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            slist.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return slist;
        }
        public IEnumerable<SelectListItem> Campuses()
        {
            if (!CampusVisible && fieldMap.DataSource != "Campuses") 
                return null;

            var q = from o in Db.Organizations
                where o.CampusId != null
                group o by o.CampusId into g
                orderby g.Key
                let descr = g.First().Campu.Description
                select new SelectListItem
                {
                    Value = $"{g.Key},{descr}",
                    Text = descr,
                    Selected = CampusInt == g.Key
                };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            listItems.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return listItems;
        }
        public IEnumerable<SelectListItem> OrgTypes()
        {
            if (!OrgTypeVisible)
                return null;
            var q = from t in Db.OrganizationTypes
                    orderby t.Code
                    let orgtypeid = t.Id.ToString()
                    select new SelectListItem
                    {
                        Value = $"{t.Id},{t.Description}",
                        Text = t.Description,
                        Selected = OrgTypeInt == t.Id
                    };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return listItems;
        }
        public IEnumerable<SelectListItem> Programs()
        {
            if (!ProgramVisible)
                return null;
            var q = from t in Db.Programs
                    orderby t.Name
                    select new SelectListItem
                    {
                        Value = $"{t.Id},{t.Name}",
                        Text = t.Name,
                        Selected = ProgramInt == t.Id
                    };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return listItems;
        }
        public IEnumerable<SelectListItem> Divisions(string id)
        {
            var progid = id.GetCsvToken().ToInt();
            var q = from div in Db.Divisions
                    where div.ProgDivs.Any(d => d.ProgId == progid)
                    orderby div.Name
                    select new SelectListItem
                    {
                        Value = $"{div.Id},{div.Name}",
                        Text = div.Name,
                        Selected = DivisionInt == div.Id
                    };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return listItems;
        }
        public IEnumerable<SelectListItem> Organizations(string id)
        {
            var divid = id.GetCsvToken().ToInt();
            var roles = Db.CurrentRoles();
            var q = from ot in Db.DivOrgs
                    where ot.Organization.LimitToRole == null || roles.Contains(ot.Organization.LimitToRole)
                    let name = ot.Organization.OrganizationName
                    where ot.DivId == divid
                          && (SqlMethods.DateDiffMonth(ot.Organization.OrganizationClosedDate, Util.Now) < 14
                              || ot.Organization.OrganizationStatusId == 30)
                    where Util2.OrgLeadersOnly == false || ot.Organization.SecurityTypeId != 3
                    orderby ot.Organization.OrganizationStatusId, ot.Organization.OrganizationName
                    select new SelectListItem
                    {
                        Value = $"{ot.OrgId},{name}",
                        Text = CmsData.Organization.FormatOrgName(ot.Organization.OrganizationName,
                            ot.Organization.LeaderName, ot.Organization.Location),
                        Selected = OrganizationInt == ot.OrgId
                    };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return listItems;
        }
        public IEnumerable<SelectListItem> SavedQueries()
        {
            if (!SavedQueryVisible)
                return null;
            string uname = Util.UserName;
            var g = new Guid();
            if (SavedQuery.HasValue() && SavedQuery.Contains(":"))
            {
                var lines = SavedQuery.Split(":".ToCharArray(), 2);
                Guid.TryParse(lines[0], out g);
            }
            var q1 = from qb in Db.Queries
                     where qb.Owner == uname
                     orderby qb.Name
                     select new SelectListItem
                     {
                         Value = qb.QueryId + ":" + qb.Name,
                         Selected = qb.QueryId == g,
                         Text = qb.Name
                     };
            var q2 = from qb in Db.Queries
                     where qb.Owner != uname && qb.Ispublic
                     orderby qb.Owner, qb.Name
                     select new SelectListItem
                     {
                         Value = qb.QueryId + ":" + qb.Name,
                         Selected = qb.QueryId == g,
                         Text = qb.Owner + ":" + qb.Name,
                     };

            return q1.Union(q2);
        }
        public List<SelectListItem> Ministries()
        {
            if (!MinistryVisible)
                return null;
            var q = from t in Db.Ministries
                    orderby t.MinistryDescription
                    select new SelectListItem
                    {
                        Value = $"{t.MinistryId},{t.MinistryName}",
                        Text = t.MinistryName,
                        Selected = MinistryInt == t.MinistryId
                    };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem
            {
                Value = "0,(not specified)",
                Text = "(not specified)",
                Selected = MinistryInt == 0
            });
            return listItems;
        }
        public IEnumerable<SelectListItem> StatusIds()
        {
            var q = from s in Db.OrganizationStatuses
                    select new SelectListItem
                    {
                        Value = $"{s.Id},{s.Description}",
                        Text = s.Description,
                        Selected = OrgStatusInt == s.Id
                    };
            var listItems = q.ToList();
            listItems.Insert(0, new SelectListItem
            {
                Value = "0,(not specified)",
                Text = "(not specified)",
                Selected = OrgStatusInt == 0
            });
            return listItems;
        }
        public IEnumerable<SelectListItem> RegistrationTypeIds()
        {
            var items = OrgSearchModel.RegistrationTypeIds().ToList();
            var q = from s in items
                    select new SelectListItem
                    {
                        Value = $"{s.Value},{s.Text}",
                        Text = s.Text,
                        Selected = s.Value == OnlineRegInt.ToString()
                    };
            return q;
        }
        public IEnumerable<SelectListItem> TagData(int? userpeopleid = null)
        {
            var q = new CodeValueModel().UserTags(userpeopleid ?? Util.UserPeopleId).ToList();
            return ToIdValueSelectList(q);
        }

        public IEnumerable<SelectListItem> PmmLabelData()
        {
            return ToIdValueSelectList(CodeValueModel.PmmLabels());
        }
        public IEnumerable<SelectListItem> StatusFlags()
        {
            var q = from s in CodeValueModel.StatusFlags() 
                    select new SelectListItem
                    {
                        Value = $"{s.Code}:{s.Value}",
                        Text = s.Value,
                        Selected = CodeValues.Any(vv => vv == s.Code)
                    };
            return q;
        }
        public IEnumerable<SelectListItem> ToIdValueSelectList(List<CodeValueItem> items)
        {
            var q = from s in items
                    select new SelectListItem
                    {
                        Value = $"{s.Id},{s.Value}",
                        Text = s.Value,
                        Selected = TagValues.Any(vv => vv.GetCsvToken().ToInt() == s.Id)
                    };
            return q;
        }

    }
}
