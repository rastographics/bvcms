using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
            switch (fieldMap.Type)
            {
                case FieldType.Bit:
                case FieldType.NullBit:
                    return ConvertToSelect(BitCodes, fieldMap.DataValueField);
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    if (fieldMap.DataSource == "ExtraValues")
                        return StandardExtraValues.ExtraValueCodes();
                    if (fieldMap.DataSource == "Campuses")
                        return Campuses();
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
                case FieldType.DateField:
                    return ConvertToSelect(Util.CallMethod(cvctl, fieldMap.DataSource), fieldMap.DataValueField);
            }
            return null;
        }
        private List<SelectListItem> ConvertToSelect(object items, string valuefield, List<string> values = null )
        {
            var list = items as IEnumerable<CodeValueItem>;
            List<SelectListItem> list2;
            if (values == null)
            {
                if (CodeValues != null)
                    values = CodeValues.ToList();
                else
                    values = new List<string>();
            }
            switch (valuefield)
            {
                case "IdCode":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.IdCode, Selected = values.Contains(c.IdCode) }).ToList();
                    break;
                case "Id":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Id.ToString(), Selected = values.Contains(c.Id.ToString()) }).ToList();
                    break;
                case "Code":
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Code, Selected = values.Contains(c.Code) }).ToList();
                    break;
                default:
                    list2 = list.Select(c => new SelectListItem { Text = c.Value, Value = c.Value, Selected = values.Contains(c.Value) }).ToList();
                    break;
            }
            return list2;
        }
        public IEnumerable<SelectListItem> GroupComparisons()
        {
            return from c in CompareClass2.Comparisons
                   where c.FieldType == FieldType.Group
                   select new SelectListItem
                   {
                       Text = c.CompType == CompareType.AllTrue ? "All"
                           : c.CompType == CompareType.AnyTrue ? "Any"
                               : c.CompType == CompareType.AllFalse ? "None"
                                   : "unknown",
                       Value = c.CompType.ToString()
                   };
        }
        public IEnumerable<SelectListItem> Comparisons()
        {
            return from c in CompareClass2.Comparisons
                   where c.FieldType == fieldMap.Type
                   select new SelectListItem { Text = c.CompType.ToString(), Value = c.CompType.ToString() };
        }
        public IEnumerable<SelectListItem> Schedules()
        {
            if (!ScheduleVisible)
                return null;
            var q = from o in Db.Organizations
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where sc != null
                    group o by new { ScheduleId = sc.ScheduleId ?? 10800, sc.MeetingTime } into g
                    orderby g.Key.ScheduleId
                    select new SelectListItem
                    {
                        Value = g.Key.ScheduleId.ToString(),
                        Text = Db.GetScheduleDesc(g.Key.MeetingTime)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Campuses()
        {
            if (!CampusVisible)
                return null;
            var q = from o in Db.Organizations
                    where o.CampusId != null
                    group o by o.CampusId into g
                    orderby g.Key
                    select new SelectListItem
                    {
                        Value = g.Key.ToString(),
                        Text = g.First().Campu.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(None)", Value = "-1" });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> OrgTypes()
        {
            if (!OrgTypeVisible)
                return null;
            var q = from t in Db.OrganizationTypes
                    orderby t.Code
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public IEnumerable<SelectListItem> Programs()
        {
            if (!ProgramVisible)
                return null;
            var q = from t in Db.Programs
                    orderby t.Name
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public static IEnumerable<SelectListItem> Divisions(int? id)
        {
            if(!id.HasValue)
                return null;
            var q = from div in DbUtil.Db.Divisions
                    where div.ProgDivs.Any(d => d.ProgId == id)
                    orderby div.Name
                    select new SelectListItem
                    {
                        Value = div.Id.ToString(),
                        Text = div.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public static IEnumerable<SelectListItem> Organizations(int? id)
        {
            if (!id.HasValue)
                return null;
            var roles = DbUtil.Db.CurrentRoles();
            var q = from ot in DbUtil.Db.DivOrgs
                    where ot.Organization.LimitToRole == null || roles.Contains(ot.Organization.LimitToRole)
                    where ot.DivId == id
                          && (SqlMethods.DateDiffMonth(ot.Organization.OrganizationClosedDate, Util.Now) < 14
                              || ot.Organization.OrganizationStatusId == 30)
                    where (Util2.OrgMembersOnly == false && Util2.OrgLeadersOnly == false) || (ot.Organization.SecurityTypeId != 3)
                    orderby ot.Organization.OrganizationStatusId, ot.Organization.OrganizationName
                    select new SelectListItem
                    {
                        Value = ot.OrgId.ToString(),
                        Text = CmsData.Organization.FormatOrgName(ot.Organization.OrganizationName,
                            ot.Organization.LeaderName, ot.Organization.Location)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
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
                        Value = t.MinistryId.ToString(),
                        Text = t.MinistryName
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
    }
}