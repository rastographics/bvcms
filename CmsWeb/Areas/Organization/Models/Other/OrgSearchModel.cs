/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CmsData.View;
using CmsWeb.Models;
using MoreLinq;
using Newtonsoft.Json;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Org.Controllers;

namespace CmsWeb.Areas.Org.Models
{

    public class OrgSearchModel
    {
        public string Name { get; set; }
        public int? ProgramId { get; set; }
        public int? DivisionId { get; set; }
        public int? TagProgramId { get; set; }
        public int? TagDiv { get; set; }
        public int? ScheduleId { get; set; }
        public int? CampusId { get; set; }
        public int? StatusId { get; set; }
        public int? TypeId { get; set; }
        public string tagstr { get; set; }
        public int? OnlineReg { get; set; }
        public bool? MainFellowship { get; set; }
        public bool FromWeekAtAGlance { get; set; }
        public bool PublicView { get; set; }

        [JsonIgnore]
        public PagerModel2 Pager { get; set; }

        public OrgSearchModel()
        {
            StatusId = OrgStatusCode.Active;
            Pager = new PagerModel2();
            Pager.GetCount = Count;
        }
        public Division Division()
        {
            var d = DbUtil.Db.Divisions.SingleOrDefault(dd => dd.Id == DivisionId);
            return d;
        }

        private IQueryable<Organization> organizations;
        public IEnumerable<OrganizationInfo> OrganizationList()
        {
            organizations = FetchOrgs();
            if (!_count.HasValue)
                _count = organizations.Count();
            organizations = ApplySort(organizations).Skip(Pager.StartRow).Take(Pager.PageSize);
            return OrganizationList(organizations, TagProgramId, TagDiv);
        }
        public static IEnumerable<OrganizationInfo> OrganizationList(IQueryable<Organization> query, int? TagProgramId, int? TagDiv)
        {
            var q = from o in query
                    join v in DbUtil.Db.ViewPreviousMemberCounts on o.OrganizationId equals v.OrganizationId into j
                    from v in j.DefaultIfEmpty()
                    join r in DbUtil.Db.ViewProspectCounts on o.OrganizationId equals r.OrganizationId into g
                    from r in g.DefaultIfEmpty()
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    select new OrganizationInfo
                    {
                        Id = o.OrganizationId,
                        OrganizationStatus = o.OrganizationStatusId,
                        OrganizationName = o.OrganizationName,
                        LeaderName = o.LeaderName,
                        LeaderId = o.LeaderId,
                        MemberCount = o.MemberCount,
                        PrevMemberCount = v.Prevcount ?? 0,
                        ProspectCount = r.Prospectcount ?? 0,
                        ClassFilled = o.ClassFilled ?? false,
                        RegClosed = o.RegistrationClosed ?? false,
                        RegTypeId = o.RegistrationTypeId,
                        Description = o.Description,
                        PublicSortOrder = o.PublicSortOrder,
                        UseRegisterLink2 = o.UseRegisterLink2,
                        ProgramName = o.Division.Program.Name,
                        ProgramId = o.Division.ProgId,
                        DivisionId = o.DivisionId,
                        DivisionName = o.Division.Name,
                        Divisions = string.Join(",", o.DivOrgs.Select(d => "{0} ({1})".Fmt(d.Division.Name, d.DivId)).ToArray()),
                        FirstMeetingDate = o.FirstMeetingDate.FormatDate(),
                        LastMeetingDate = o.LastMeetingDate.FormatDate(),
                        RegStart = o.RegStart.FormatDate(),
                        RegEnd = o.RegEnd.FormatDate(),
                        Schedule = DbUtil.Db.GetScheduleDesc(sc.MeetingTime),
                        Location = o.Location,
                        AllowSelfCheckIn = o.CanSelfCheckin ?? false,
                        BDayStart = o.BirthDayStart.FormatDate("na"),
                        BDayEnd = o.BirthDayEnd.FormatDate("na"),
                        Tag = (TagDiv ?? 0) == 0 ? "" : o.DivOrgs.Any(ot => ot.DivId == TagDiv) ? "Remove" : "Add",
                        ChangeMain = (o.DivisionId == null || o.DivisionId != TagDiv) && o.DivOrgs.Any(d => d.DivId == TagDiv),
                    };
            return q;
        }
        public EpplusResult OrganizationExcelList()
        {
            var q = FetchOrgs();
            var q2 = from o in q
                     let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                     let lt = DbUtil.Db.MemberTypes.SingleOrDefault(ttt => ttt.Id == o.LeaderMemberTypeId)
                     select new
                     {
                         OrgId = o.OrganizationId,
                         Name = o.OrganizationName,
                         o.Description,
                         Leader = o.LeaderName,
                         Members = o.MemberCount ?? 0,
                         Division = o.Division.Name,
                         FirstMeeting = o.FirstMeetingDate.FormatDate(),
                         LastMeeting = o.LastMeetingDate.FormatDate(),
                         Schedule = DbUtil.Db.GetScheduleDesc(sc.MeetingTime),
                         Campus = o.Campu.Description ?? "",
                         SecurityType = o.SecurityTypeId == 0 ? "None" : o.SecurityTypeId == 2 ? "LeadersOnly" : "UnShared",
                         o.Location,
                         RegStart = o.RegStart.FormatDate(),
                         RegEnd = o.RegEnd.FormatDate(),
                         RollSheetVisitorWks = o.RollSheetVisitorWks ?? 0,
                         Limit = o.Limit.ToString(),
                         CanSelfCheckin = o.CanSelfCheckin ?? false,
                         BirthDayStart = o.BirthDayStart.FormatDate(),
                         BirthDayEnd = o.BirthDayEnd.FormatDate(),
                         Gender = o.Gender.Description,
                         GradeAgeStart = o.GradeAgeStart ?? 0,
                         LastDayBeforeExtra = o.LastDayBeforeExtra.FormatDate(),
                         NoSecurityLabel = o.NoSecurityLabel ?? false,
                         NumCheckInLabels = o.NumCheckInLabels ?? 0,
                         NumWorkerCheckInLabels = o.NumWorkerCheckInLabels ?? 0,
                         o.PhoneNumber,
                         MainFellowshipOrg = o.IsBibleFellowshipOrg ?? false,
                         EntryPoint = o.EntryPoint.Description,
                         LeaderType = lt == null ? "" : lt.Description,
                         o.OrganizationStatusId,
                     };
            return q2.ToDataTable().ToExcel("Organizations.xlsx");
        }
        public class OrgMemberInfoClass : ExportInvolvements.MemberInfoClass
        {
            public int OrganizationId { get; set; }
            public string Organization { get; set; }
            public string Schedule { get; set; }
        }
        public EpplusResult OrgsMemberList()
        {
            var q = FetchOrgs();
            return DbUtil.Db.CurrOrgMembers(string.Join(",", q.OrderBy(mm => mm.OrganizationName).Select(mm => mm.OrganizationId)))
                .ToDataTable().ToExcel("OrgsMembers.xlsx");
        }

        private int TagSubDiv(string s)
        {
            if (!s.HasValue())
                return 0;
            var a = s.Split(':');
            if (a.Length > 1)
                return a[1].ToInt();
            return 0;
        }

        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchOrgs().Count();
            return _count.Value;
        }

        public IQueryable<Organization> FetchOrgs()
        {
            var me = Util.UserPeopleId;

            if (organizations != null)
                return organizations;

            var u = DbUtil.Db.CurrentUser;

            var roles = u.UserRoles.Select(uu => uu.Role.RoleName).ToArray();
            organizations = from o in DbUtil.Db.Organizations
                            where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                            select o;

            if (Util2.OrgLeadersOnly)
            {
                var oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
                organizations = DbUtil.Db.Organizations.Where(o => oids.Contains(o.OrganizationId));
            }

            if (Name.HasValue())
            {
                if (Name.AllDigits())
                    organizations = from o in organizations
                                    where o.OrganizationId == Name.ToInt()
                                    select o;
                else if (Name.ToInt() < 0)
                {
                    var loc = Name.GetDigits();
                    organizations = from o in organizations
                                    where o.Location == loc
                                    select o;
                }
                else if (Name.StartsWith("ev:"))
                {
                    organizations = from o in organizations
                                    where o.OrganizationExtras.Any(ee => ee.Field.StartsWith(Name.Substring(3)))
                                    select o;
                }
                else if (Name.StartsWith("-ev:"))
                {
                    organizations = from o in organizations
                                    where !o.OrganizationExtras.Any(ee => ee.Field.StartsWith(Name.Substring(4)))
                                    select o;
                }
                else
                    organizations = from o in organizations
                                    where o.OrganizationName.Contains(Name)
                                          || o.LeaderName.Contains(Name)
                                          || o.Location == Name
                                          || o.PendingLoc == Name
                                          || o.DivOrgs.Any(t => t.Division.Name.Contains(Name))
                                    select o;
            }
            if (DivisionId > 0)
                organizations = from o in organizations
                                where o.DivOrgs.Any(t => t.DivId == DivisionId)
                                select o;
            else if (ProgramId > 0)
                if (FromWeekAtAGlance)
                    organizations = from o in organizations
                                    where o.DivOrgs.Any(d => d.Division.ProgDivs.Any(p => p.ProgId == ProgramId
                                        && p.Division.ReportLine > 0))
                                    || (o.Division.ProgId == ProgramId && o.Division.ReportLine > 0)
                                    select o;
                else
                    organizations = from o in organizations
                                    where o.DivOrgs.Any(d => d.Division.ProgDivs.Any(p => p.ProgId == ProgramId))
                                    || o.Division.ProgId == ProgramId
                                    select o;

            if (ScheduleId > 0)
                organizations = from o in organizations
                                where o.OrgSchedules.Any(os => os.ScheduleId == ScheduleId)
                                select o;
            if (ScheduleId == -1)
                organizations = from o in organizations
                                where !o.OrgSchedules.Any()
                                select o;

            if (StatusId > 0)
                organizations = from o in organizations
                                where o.OrganizationStatusId == StatusId
                                select o;

            if (TypeId > 0)
                organizations = from o in organizations
                                where o.OrganizationTypeId == TypeId
                                select o;
            else if (TypeId == OrgType.NoOrgType)
                organizations = from o in organizations
                                where o.OrganizationTypeId == null
                                select o;
            else if (TypeId == OrgType.MainFellowship)
                organizations = from o in organizations
                                where o.IsBibleFellowshipOrg == true
                                select o;
            else if (TypeId == OrgType.NotMainFellowship)
                organizations = from o in organizations
                                where (o.IsBibleFellowshipOrg ?? false) == false
                                select o;
            else if (TypeId == OrgType.SuspendedCheckin)
                organizations = from o in organizations
                                where (o.SuspendCheckin ?? false)
                                select o;
            else if (TypeId == OrgType.ParentOrg)
                organizations = from o in organizations
                                where o.ChildOrgs.Any()
                                select o;
            else if (TypeId == OrgType.ChildOrg)
                organizations = from o in organizations
                                where o.ParentOrgId != null
                                select o;
            else if (TypeId == OrgType.Fees)
                organizations = from o in organizations
                                join f in DbUtil.Db.ViewOrgsWithFees on o.OrganizationId equals f.OrganizationId
                                select o;
            else if (TypeId == OrgType.NoFees)
                organizations = from o in organizations
                                join f in DbUtil.Db.ViewOrgsWithoutFees on o.OrganizationId equals f.OrganizationId
                                select o;


            if (CampusId > 0)
                organizations = from o in organizations
                                where o.CampusId == CampusId
                                select o;
            else if (CampusId == -1)
                organizations = from o in organizations
                                where o.CampusId == null
                                select o;

            if (OnlineReg == RegistrationClassification.AnyOnlineReg99)
                organizations = from o in organizations
                                where o.RegistrationTypeId > 0
                                select o;
            else if (OnlineReg == RegistrationClassification.AnyOnlineRegMissionTrip98)
                organizations = from o in organizations
                                where o.RegistrationTypeId > 0 && o.IsMissionTrip == true
                                select o;
            else if (OnlineReg == RegistrationClassification.AnyOnlineRegNonPicklist97)
                organizations = from o in organizations
                                join p in DbUtil.Db.ViewMasterOrgs on o.OrganizationId equals p.PickListOrgId into j
                                from p in j.DefaultIfEmpty()
                                where p.PickListOrgId == null
                                where o.RegistrationTypeId > 0
                                select o;
            else if (OnlineReg == RegistrationClassification.AnyOnlineRegActive96)
                organizations = from o in organizations
                                join p in DbUtil.Db.ViewMasterOrgs on o.OrganizationId equals p.PickListOrgId into j
                                from p in j.DefaultIfEmpty()
                                where p.PickListOrgId == null
                                where o.RegistrationTypeId > 0
                                where (o.RegistrationClosed ?? false) == false
                                where (o.ClassFilled ?? false) == false
                                where o.RegStart == null || o.RegStart < DateTime.Now
                                where o.RegEnd == null || o.RegEnd > DateTime.Now
                                select o;
            else if (OnlineReg > 0)
                organizations = from o in organizations
                                where o.RegistrationTypeId == OnlineReg
                                select o;
            else if (OnlineReg == 0)
                organizations = from o in organizations
                                where (o.RegistrationTypeId ?? 0) == 0
                                select o;

            return organizations;
        }
        public IQueryable<Organization> ApplySort(IQueryable<Organization> query)
        {
            var regdt = DateTime.Today.AddYears(5);
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "ID":
                        query = from o in query
                                orderby o.OrganizationId
                                select o;
                        break;
                    case "Division":
                    case "Program/Division":
                        query = from o in query
                                orderby o.Division.Program.Name, o.Division.Name,
                                o.OrganizationName
                                select o;
                        break;
                    case "Name":
                        query = from o in query
                                orderby o.OrganizationName
                                select o;
                        break;
                    case "Location":
                        query = from o in query
                                orderby o.Location
                                select o;
                        break;
                    case "Schedule":
                        query = from o in query
                                let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                                orderby sc.ScheduleId
                                select o;
                        break;
                    case "Self CheckIn":
                        query = from o in query
                                orderby (o.CanSelfCheckin ?? false)
                                select o;
                        break;
                    case "Leader":
                        query = from o in query
                                orderby o.LeaderName,
                                o.OrganizationName
                                select o;
                        break;
                    case "Filled":
                        query = from o in query
                                orderby o.ClassFilled, o.OrganizationName
                                select o;
                        break;
                    case "Closed":
                        query = from o in query
                                orderby o.RegistrationClosed, o.OrganizationName
                                select o;
                        break;
                    case "RegType":
                        query = from o in query
                                orderby o.RegistrationTypeId, o.OrganizationName
                                select o;
                        break;
                    case "App Order":
                        query = from o in query
                                orderby o.PublicSortOrder ?? "zzz", o.OrganizationName
                                select o;
                        break;
                    case "Members":
                    case "Curr":
                        query = from o in query
                                orderby o.MemberCount, o.OrganizationName
                                select o;
                        break;
                    case "FirstDate":
                        query = from o in query
                                orderby o.FirstMeetingDate, o.LastMeetingDate
                                select o;
                        break;
                    case "RegStart":
                        query = from o in query
                                orderby o.RegStart ?? regdt
                                select o;
                        break;
                    case "RegEnd":
                        query = from o in query
                                orderby o.RegEnd ?? regdt
                                select o;
                        break;
                    case "LastMeetingDate":
                        query = from o in query
                                orderby o.LastMeetingDate, o.FirstMeetingDate
                                select o;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "ID":
                        query = from o in query
                                orderby o.OrganizationId descending
                                select o;
                        break;
                    case "Program/Division":
                    case "Division":
                        query = from o in query
                                orderby o.Division.Program.Name descending, o.Division.Name descending,
                                o.OrganizationName descending
                                select o;
                        break;
                    case "Name":
                        query = from o in query
                                orderby o.OrganizationName descending
                                select o;
                        break;
                    case "Location":
                        query = from o in query
                                orderby o.Location descending
                                select o;
                        break;
                    case "Schedule":
                        query = from o in query
                                let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                                orderby sc.ScheduleId descending
                                select o;
                        break;
                    case "Self CheckIn":
                        query = from o in query
                                orderby (o.CanSelfCheckin ?? false) descending
                                select o;
                        break;
                    case "Leader":
                        query = from o in query
                                orderby o.LeaderName descending,
                                o.OrganizationName descending
                                select o;
                        break;
                    case "Filled":
                        query = from o in query
                                orderby o.ClassFilled descending,
                                o.OrganizationName descending
                                select o;
                        break;
                    case "Closed":
                        query = from o in query
                                orderby o.RegistrationClosed descending,
                                o.OrganizationName descending
                                select o;
                        break;
                    case "RegType":
                        query = from o in query
                                orderby o.RegistrationTypeId descending,
                                o.OrganizationName descending
                                select o;
                        break;
                    case "App Order":
                        query = from o in query
                                orderby o.PublicSortOrder ?? "zzz", o.OrganizationName 
                                select o;
                        break;
                    case "Members":
                    case "Curr":
                        query = from o in query
                                orderby o.MemberCount descending,
                                o.OrganizationName descending
                                select o;
                        break;
                    case "FirstDate":
                        query = from o in query
                                orderby o.FirstMeetingDate descending,
                                o.LastMeetingDate descending
                                select o;
                        break;
                    case "RegStart":
                        query = from o in query
                                orderby o.RegStart descending 
                                select o;
                        break;
                    case "RegEnd":
                        query = from o in query
                                orderby o.RegEnd descending 
                                select o;
                        break;
                    case "LastMeetingDate":
                        query = from o in query
                                orderby o.LastMeetingDate descending,
                                o.FirstMeetingDate descending
                                select o;
                        break;
                }
            return query;
        }

        public static IEnumerable<SelectListItem> StatusIds()
        {
            var q = from s in DbUtil.Db.OrganizationStatuses
                    select new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return list;
        }
        public IEnumerable<SelectListItem> CampusIds()
        {
            var q = from c in DbUtil.Db.Campus
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = "(not assigned)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "(not specified)"
            });
            return list;
        }
        public IEnumerable<SelectListItem> ProgramIds()
        {
            var q = from c in DbUtil.Db.Programs
                    orderby c.Name
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "(not specified)",
            });
            return list;
        }
        public IEnumerable<SelectListItem> DivisionIds()
        {
            return DivisionIds(ProgramId ?? 0);
        }
        public static IEnumerable<SelectListItem> DivisionIds(int ProgId)
        {
            var q = from d in DbUtil.Db.Divisions
                    where d.ProgId == ProgId || d.ProgDivs.Any(p => p.ProgId == ProgId)
                    orderby d.Name
                    select new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = ProgId == 0 ? "(select a program)" : "(not specified)",
            });
            return list;
        }
        public IEnumerable<SelectListItem> ScheduleIds()
        {
            var q = from sc in DbUtil.Db.OrgSchedules
                    group sc by new { sc.ScheduleId, sc.MeetingTime } into g
                    orderby g.Key.ScheduleId
                    where g.Key.ScheduleId != null
                    select new SelectListItem
                    {
                        Value = g.Key.ScheduleId.Value.ToString(),
                        Text = DbUtil.Db.GetScheduleDesc(g.Key.MeetingTime)
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = "(None)",
            });
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "(not specified)",
            });
            return list;
        }

        public class OrgType
        {
            public const int NoFees = -8;
            public const int Fees = -7;
            public const int ChildOrg = -6;
            public const int ParentOrg = -5;
            public const int SuspendedCheckin = -4;
            public const int MainFellowship = -3;
            public const int NotMainFellowship = -2;
            public const int NoOrgType = -1;
        }
        public static IEnumerable<SelectListItem> OrgTypes()
        {
            var q = from t in DbUtil.Db.OrganizationTypes
                    orderby t.Code
                    select new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Description
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "Suspended Checkin", Value = OrgType.SuspendedCheckin.ToString() });
            list.Insert(0, new SelectListItem { Text = "Main Fellowship", Value = OrgType.MainFellowship.ToString() });
            list.Insert(0, new SelectListItem { Text = "Not Main Fellowship", Value = OrgType.NotMainFellowship.ToString() });
            list.Insert(0, new SelectListItem { Text = "Parent Org", Value = OrgType.ParentOrg.ToString() });
            list.Insert(0, new SelectListItem { Text = "Child Org", Value = OrgType.ChildOrg.ToString() });
            list.Insert(0, new SelectListItem { Text = "Orgs Without Type", Value = OrgType.NoOrgType.ToString() });
            list.Insert(0, new SelectListItem { Text = "Orgs With Fees", Value = OrgType.Fees.ToString() });
            list.Insert(0, new SelectListItem { Text = "Orgs Without Fees", Value = OrgType.NoFees.ToString() });
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public class RegistrationClassification
        {
            public const int NotSpecified = -1;
            public const int AnyOnlineReg99 = 99;
            public const int AnyOnlineRegMissionTrip98 = 98;
            public const int AnyOnlineRegNonPicklist97 = 97;
            public const int AnyOnlineRegActive96 = 96;
        }
        public static IEnumerable<SelectListItem> RegistrationTypeIds()
        {
            var q = from o in CmsData.Codes.RegistrationTypeCode.GetCodePairs()
                    select new SelectListItem
                    {
                        Value = o.Key.ToString(),
                        Text = o.Value
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegActive96.ToString(),
                Text = "(any active registration)",
            });
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegNonPicklist97.ToString(),
                Text = "(any registration, no picklist)",
            });
            list.Insert(0, new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineReg99.ToString(),
                Text = "(any registration)",
            });
            list.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = "(not specified)",
            });
            list.Add(new SelectListItem
            {
                Value = RegistrationClassification.AnyOnlineRegMissionTrip98.ToString(),
                Text = "Mission Trip",
            });
            return list;
        }
        public static DateTime DefaultMeetingDate(int scheduleid)
        {
            var sdt = Organization.GetDateFromScheduleId(scheduleid);
            if (sdt == null)
                return DateTime.Now.Date.AddHours(8);
            var dt = Util.Now.Date;
            dt = dt.AddDays(-(int)dt.DayOfWeek); // prev sunday
            dt = dt.AddDays((int)sdt.Value.Day);
            if (dt < Util.Now.Date)
                dt = dt.AddDays(7);
            return dt.Add(sdt.Value.TimeOfDay);
        }
        private static string RecentAbsentsEmail(OrgSearchController c, IEnumerable<RecentAbsent> list)
        {
            var q = from p in list
                    orderby p.Consecutive, p.Name2
                    select p;
            return ViewExtensions2.RenderPartialViewToString(c, "RecentAbsentsEmail", q);
        }
        private static string RecentVisitsEmail(OrgSearchController c, IEnumerable<OrgVisitorsAsOfDate> list)
        {
            var q = from p in list
                    orderby p.LastAttended, p.LastName, p.PreferredName
                    select p;
            return ViewExtensions2.RenderPartialViewToString(c, "RecentVisitsEmail", q);
        }
        public void SendNotices(OrgSearchController c)
        {
            const int days = 36;

            var olist = FetchOrgs().Select(oo => oo.OrganizationId).ToList();

            var alist = (from p in DbUtil.Db.RecentAbsents(null, null, days)
                         where olist.Contains(p.OrganizationId)
                         select p).ToList();

            var mlist = (from r in DbUtil.Db.LastMeetings(null, null, days)
                         where olist.Contains(r.OrganizationId)
                         select r).ToList();

            var plist = (from om in DbUtil.Db.ViewOrganizationLeaders
                         where olist.Contains(om.OrganizationId)
                         group om.OrganizationId by om.PeopleId into leaderlist
                         select leaderlist).ToList();

            var sb2 = new StringBuilder("Notices sent to:</br>\n<table>\n");
            foreach (var p in plist)
            {
                var sb = new StringBuilder("The following meetings are ready to be viewed:<br/>\n");
                var orgids = p.Select(vv => vv).ToList();
                var meetings = mlist.Where(m => orgids.Contains(m.OrganizationId)).ToList();
                var leader = DbUtil.Db.LoadPersonById(p.Key);
                foreach (var m in meetings)
                {
                    string orgname = Organization.FormatOrgName(m.OrganizationName, m.LeaderName, m.Location);
                    sb.AppendFormat("<a href='{0}/Meeting/{1}'>{2} - {3}</a><br/>\n",
                                    DbUtil.Db.CmsHost, m.MeetingId, orgname, m.Lastmeeting.FormatDateTm());
                    sb2.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2:g}</td></tr>\n",
                                     leader.Name, orgname, m.Lastmeeting.FormatDateTm());
                }
                foreach (var m in meetings)
                {
                    var absents = alist.Where(a => a.OrganizationId == m.OrganizationId);
                    var vlist = DbUtil.Db.OrgVisitorsAsOfDate(m.OrganizationId, m.Lastmeeting, NoCurrentMembers: true).ToList();
                    sb.Append(RecentAbsentsEmail(c, absents));
                    sb.Append(RecentVisitsEmail(c, vlist));
                }
                DbUtil.Db.Email(DbUtil.Db.CurrentUser.Person.FromEmail, leader, null,
                                "Attendance reports are ready for viewing", sb.ToString(), false);
            }
            sb2.Append("</table>\n");
            DbUtil.Db.Email(DbUtil.Db.CurrentUser.Person.FromEmail, DbUtil.Db.CurrentUser.Person, null,
                            "Attendance emails sent", sb2.ToString(), false);
        }

        public class OrganizationInfo
        {
            public int Id { get; set; }
            public int? OrganizationStatus { get; set; }
            public string OrganizationName { get; set; }
            public string LeaderName { get; set; }
            public int? LeaderId { get; set; }
            public int? MemberCount { get; set; }
            public int ProspectCount { get; set; }
            public int PrevMemberCount { get; set; }
            public bool ClassFilled { get; set; }
            public bool RegClosed { get; set; }
            public int? RegTypeId { get; set; }
            public string RegType
            {
                get { return RegistrationTypeCode.Lookup(RegTypeId ?? 0); }
            }

            public string RegStart { get; set; }
            public string RegEnd { get; set; }
            public string Description { get; set; }
            public string PublicSortOrder { get; set; }
            public bool? UseRegisterLink2 { get; set; }
            public string ProgramName { get; set; }
            public int? ProgramId { get; set; }
            public int? DivisionId { get; set; }
            public string DivisionName { get; set; }
            public string Divisions { get; set; }
            public string FirstMeetingDate { get; set; }
            public string LastMeetingDate { get; set; }
            public int SchedDay { get; set; }
            public string Schedule { get; set; }
            public string Location { get; set; }
            public string Tag { get; set; }
            public bool? ChangeMain { get; set; }
            public int? VisitorCount { get; set; }
            public bool AllowSelfCheckIn { get; set; }
            public string BDayStart { get; set; }
            public string BDayEnd { get; set; }
            public string ToolTip
            {
                get
                {
                    return
@"{0} ({1})|
Program: {2} ({3})|
Division: {4} ({5})|
Leader: {6}|
First Meeting: {7}|
Last Meeting: {8}|
Schedule: {9}|
Location: {10}|
Divisions: {11}".Fmt(
                    OrganizationName,
                    Id,
                    ProgramName,
                    ProgramId,
                    DivisionName,
                    DivisionId,
                    LeaderName,
                    FirstMeetingDate,
                    LastMeetingDate,
                    Schedule,
                    Location,
                    Divisions
                    );
                }
            }
        }
        public class OrganizationInfoExcel
        {
            public int OrgId { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string Leader { get; set; }
            public int Members { get; set; }
            public string Division { get; set; }
            public string FirstMeeting { get; set; }
            public DateTime? MeetingTime { get; set; }
            public string Schedule { get { return "{0:ddd h:mm tt}".Fmt(MeetingTime); } }
            public string Location { get; set; }
        }

        public string EncodedJson()
        {
            var s = HttpUtility.UrlEncode(Util.Encrypt(JsonConvert.SerializeObject(this)));
            return s;
        }
        public static OrgSearchModel DecodedJson(string parameter)
        {
            return JsonConvert.DeserializeObject<OrgSearchModel>(HttpUtility.UrlDecode(Util.Decrypt(parameter)));
        }

        public string ConvertToSearch()
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset(DbUtil.Db);
            var c = cc.AddNewClause(QueryType.OrgSearchMember, CompareType.Equal, "1,T");
            if(Name.HasValue())
                c.OrgName = Name;
            if (ProgramId != 0) 
                c.Program = ProgramId ?? 0;
            if (DivisionId != 0)
                c.Division = DivisionId ?? 0;
            if (StatusId != 0)
                c.OrgStatus = StatusId ?? 0;
            if (TypeId != 0)
                c.OrgType2 = TypeId ?? 0;
            if (CampusId != 0)
                c.Campus = CampusId ?? 0;
            if (ScheduleId != 0)
                c.Schedule = ScheduleId ?? 0;
            if (OnlineReg != 0)
                c.OnlineReg = OnlineReg ?? 0;

            cc.Save(DbUtil.Db);
            return "/Query/" + cc.Id;
        }
    }
}
