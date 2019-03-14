/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.Reports.Controllers;
using CmsWeb.Models;
using Dapper;
using MoreLinq;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class OrgSearchModel
    {
        private int? _count;
        internal string noticelist;
        private List<OrgSearch> _organizations;

        public OrgSearchModel()
        {
            Pager = new PagerModel2();
            Pager.GetCount = Count;
        }

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
        public bool FromWeekAtAGlance { get; set; }
        public bool PublicView { get; set; }

        public string ExtraValues
        {
            set
            {
                ExtraValuesDict = new Dictionary<string, string>();
                if (value == null)
                    return;
                var list = value.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));
                foreach (var item in list)
                {
                    var parts = item.Split('=');
                    if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1])) continue;
                    ExtraValuesDict.Add(parts[0], parts[1]);
                }
            }
            get { return ExtraValuesDict != null ? string.Join(",", ExtraValuesDict?.Select(x => $"{x.Key}={x.Value}")) : ""; }
        }
        public Dictionary<string, string> ExtraValuesDict { get; set; }

        [JsonIgnore]
        public PagerModel2 Pager { get; set; }

        public Division Division()
        {
            var d = DbUtil.Db.Divisions.SingleOrDefault(dd => dd.Id == DivisionId);
            return d;
        }

        public IEnumerable<OrganizationInfo> OrganizationList()
        {
            var organizations = FetchOrgsList();
            if (!_count.HasValue)
                _count = organizations.Count;

            organizations = ApplySort(organizations).Skip(Pager.StartRow).Take(Pager.PageSize).ToList();
            return OrganizationList(organizations, TagProgramId, TagDiv);
        }

        public static IEnumerable<OrganizationInfo> OrganizationList(List<OrgSearch> query, int? TagProgramId, int? TagDiv)
        {
            var q = from os in query
                    select new OrganizationInfo
                    {
                        Id = os.OrganizationId,
                        OrganizationName = os.OrganizationName,
                        LeaderName = os.LeaderName,
                        OrganizationStatus = os.OrganizationStatusId,
                        MemberCount = os.MemberCount,
                        RegClosed = os.RegistrationClosed ?? false,
                        RegTypeId = os.RegistrationTypeId,
                        ClassFilled = os.ClassFilled ?? false,
                        AppCategory = os.AppCategory ?? "Other",
                        PublicSortOrder = os.PublicSortOrder,
                        ProgramName = os.Program,
                        ProgramId = os.ProgramId,
                        DivisionName = os.Division,
                        Divisions = os.Divisions,
                        FirstMeetingDate = os.FirstMeetingDate.FormatDate(),
                        LastMeetingDate = os.LastMeetingDate.FormatDate(),
                        RegStart = os.RegStart.FormatDate(),
                        RegEnd = os.RegEnd.FormatDate(),
                        Schedule = os.ScheduleDescription,
                        Location = os.Location,
                        AllowSelfCheckIn = os.CanSelfCheckin ?? false,
                        Tag = os.Tag,
                        ChangeMain = os.ChangeMain == 1,
                        LeaderId = os.LeaderId,
                        PrevMemberCount = os.PrevMemberCount ?? 0,
                        ProspectCount = os.ProspectCount ?? 0,
                        Description = os.Description,
                        UseRegisterLink2 = os.UseRegisterLink2,
                        DivisionId = os.DivisionId,
                        BDayStart = os.BirthDayStart.FormatDate(),
                        BDayEnd = os.BirthDayEnd.FormatDate()
                    };
            return q;
        }

        public EpplusResult OrganizationExcelList()
        {
            var q = FetchOrgs();
            var q2 = from os in q
                     join o in DbUtil.Db.Organizations on os.OrganizationId equals o.OrganizationId
                     select new
                     {
                         OrgId = os.OrganizationId,
                         Name = os.OrganizationName,
                         os.Description,
                         Leader = os.LeaderName,
                         Members = os.MemberCount ?? 0,
                         os.Division,
                         FirstMeeting = os.FirstMeetingDate.FormatDate(),
                         LastMeeting = os.LastMeetingDate.FormatDate(),
                         Schedule = os.ScheduleDescription,
                         os.Campus,
                         os.Location,
                         RegStart = os.RegStart.FormatDate(),
                         RegEnd = os.RegEnd.FormatDate(),
                         RollSheetVisitorWks = o.RollSheetVisitorWks ?? 0,
                         Limit = o.Limit.ToString(),
                         o.LimitToRole,
                         CanSelfCheckin = os.CanSelfCheckin ?? false,
                         BirthDayStart = os.BirthDayStart.FormatDate(),
                         BirthDayEnd = os.BirthDayEnd.FormatDate(),
                         Gender = o.Gender.Description,
                         Grade = o.GradeAgeStart ?? 0,
                         LastDayBeforeExtra = o.LastDayBeforeExtra.FormatDate(),
                         NoSecurityLabel = o.NoSecurityLabel ?? false,
                         NumCheckInLabels = o.NumCheckInLabels ?? 0,
                         NumWorkerCheckInLabels = o.NumWorkerCheckInLabels ?? 0,
                         o.PhoneNumber,
                         MainFellowshipOrg = o.IsBibleFellowshipOrg ?? false,
                         EntryPoint = o.EntryPoint.Description,
                         o.OnLineCatalogSort,
                         os.LeaderType,
                         os.OrganizationStatusId,
                         os.AppCategory,
                         os.PublicSortOrder,
                         os.UseRegisterLink2
                     };
            return q2.ToDataTable().ToExcel("Organizations.xlsx");
        }

        public EpplusResult OrgsMemberList()
        {
            var q = FetchOrgs();
            if (Util2.OrgLeadersOnly)
            {
                var oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
                q = q.Where(oo => oids.Contains(oo.OrganizationId));
            }
            return DbUtil.Db.CurrOrgMembers(string.Join(",", q.OrderBy(mm => mm.OrganizationName).Select(mm => mm.OrganizationId)))
                         .ToDataTable().ToExcel("OrgsMembers.xlsx");
        }

        public EpplusResult RegOptionsList()
        {
            var q = FetchOrgs();
            var q2 = from os in q
                     join op in DbUtil.Db.ViewRegsettingOptions on os.OrganizationId equals op.OrganizationId
                     select op;
            return q2.ToDataTable().ToExcel("RegOptions.xlsx");
        }

        public EpplusResult RegQuestionsUsage()
        {
            var q = FetchOrgs();
            var q2 = from os in q
                     join op in DbUtil.Db.ViewRegsettingCounts on os.OrganizationId equals op.OrganizationId
                     select op;
            return q2.ToDataTable().ToExcel("RegQuestionsUsage.xlsx");
        }

        public EpplusResult RegSettingUsages()
        {
            var q = FetchOrgs();
            var q2 = from os in q
                     join op in DbUtil.Db.ViewRegsettingUsages on os.OrganizationId equals op.OrganizationId
                     select op;
            return q2.ToDataTable().ToExcel("RegQuestionsUsage.xlsx");
        }

        public void RegSettingsXml(Stream stream)
        {
            var q = FetchOrgs();
            var w = new CmsData.API.APIWriter(stream);
            w.Start("OrgSearch");
            foreach (var o in q)
            {
                var os = DbUtil.Db.CreateRegistrationSettings(o.OrganizationId);
                Util.Serialize(os, w.writer);
            }
            w.End();
            w.writer.Flush();
        }

        public void RegMessagesXml(Stream stream, Settings.Messages messages)
        {
            var q = FetchOrgs();
            var w = new CmsData.API.APIWriter(stream);
            w.Start("OrgSearch");
            foreach (var o in q)
            {
                var os = DbUtil.Db.CreateRegistrationSettings(o.OrganizationId);
                os.WriteXmlMessages(w.writer, messages);
            }
            w.End();
            w.writer.Flush();
        }

        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchOrgsList().Count;
            return _count.Value;
        }

        public string FilteredCount()
        {
            return $"{Count():N0} {(IsFiltered() ? " (filtered)" : "")}";
        }

        public bool IsFiltered()
        {
            return Name.HasValue()
                   || ProgramId > 0
                   || (TypeId ?? 0) != 0
                   || CampusId > 0
                   || ScheduleId > 0
                   || StatusId != 30
                   || OnlineReg > 0;
        }

        private List<OrgSearch> FetchOrgsList()
        {
            if (_organizations == null)
            {
                _organizations = FetchOrgs().ToList();
            }

            return _organizations;
        }

        public IQueryable<OrgSearch> FetchOrgs()
        {
            var queryable = DbUtil.Db.OrgSearch(Name, ProgramId, DivisionId, TypeId, CampusId, ScheduleId, StatusId, OnlineReg,
                 DbUtil.Db.CurrentUser.UserId, TagDiv);

            if (ExtraValuesDict != null && ExtraValuesDict.Any())
            {
                var orgIds = queryable.Select(x => x.OrganizationId).ToList();

                foreach (var ev in ExtraValuesDict)
                {
                    orgIds = DbUtil.Db.OrganizationExtras
                        .Where(x => orgIds.Contains(x.OrganizationId) && x.Field == ev.Key &&
                            (
                             x.Type.Equals("Code") ? x.StrValue.ToLower().Equals(ev.Value) : x.StrValue.ToLower().Contains(ev.Value) ||
                             x.Type.Equals("Code") ? x.Data.ToLower().Equals(ev.Value) : x.Data.ToLower().Contains(ev.Value)) ||
                             x.DateValue != null && x.DateValue.ToString().Contains(ev.Value) ||
                             x.IntValue != null && x.IntValue.ToString().Contains(ev.Value) ||
                             x.BitValue != null && x.BitValue.ToString().Contains(ev.Value)
                             )
                        .Select(x => x.OrganizationId).ToList();
                }

                queryable = queryable.Where(x => orgIds.Contains(x.OrganizationId));
            }

            return queryable;
        }

        public static IQueryable<OrgSearch> FetchOrgs(int orgId)
        {
            return DbUtil.Db.OrgSearch(orgId.ToString(), null, null, null, null, null, null, null, DbUtil.Db.CurrentUser.UserId, null);
        }

        // ReSharper disable once FunctionComplexityOverflow
        public List<OrgSearch> ApplySort(List<OrgSearch> query)
        {
            var regdt = DateTime.Today.AddYears(5);
            IEnumerable<OrgSearch> list = query;
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "ID":
                        list = from o in query
                               orderby o.OrganizationId
                               select o;
                        break;
                    case "Division":
                    case "Program/Division":
                        list = from o in query
                               orderby o.Program, o.Division, o.OrganizationName
                               select o;
                        break;
                    case "Name":
                        list = from o in query
                               orderby o.OrganizationName
                               select o;
                        break;
                    case "Location":
                        list = from o in query
                               orderby o.Location
                               select o;
                        break;
                    case "Schedule":
                        list = from o in query
                               orderby o.ScheduleId
                               select o;
                        break;
                    case "Self CheckIn":
                        list = from o in query
                               orderby (o.CanSelfCheckin ?? false)
                               select o;
                        break;
                    case "Leader":
                        list = from o in query
                               orderby o.LeaderName,
                                   o.OrganizationName
                               select o;
                        break;
                    case "Filled":
                        list = from o in query
                               orderby o.ClassFilled, o.OrganizationName
                               select o;
                        break;
                    case "Closed":
                        list = from o in query
                               orderby o.RegistrationClosed, o.OrganizationName
                               select o;
                        break;
                    case "RegType":
                        list = from o in query
                               orderby o.RegistrationTypeId, o.OrganizationName
                               select o;
                        break;
                    case "Category":
                        list = from o in query
                               orderby o.AppCategory ?? "zzz", o.PublicSortOrder ?? "zzz", o.OrganizationName
                               select o;
                        break;
                    case "Members":
                    case "Current":
                        list = from o in query
                               orderby o.MemberCount, o.OrganizationName
                               select o;
                        break;
                    case "FirstDate":
                        list = from o in query
                               orderby o.FirstMeetingDate, o.LastMeetingDate
                               select o;
                        break;
                    case "RegStart":
                        list = from o in query
                               orderby o.RegStart ?? regdt
                               select o;
                        break;
                    case "RegEnd":
                        list = from o in query
                               orderby o.RegEnd ?? regdt
                               select o;
                        break;
                    case "LastMeetingDate":
                        list = from o in query
                               orderby o.LastMeetingDate, o.FirstMeetingDate
                               select o;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "ID":
                        list = from o in query
                               orderby o.OrganizationId descending
                               select o;
                        break;
                    case "Program/Division":
                    case "Division":
                        list = from o in query
                               orderby o.Program descending, o.Division descending,
                                   o.OrganizationName descending
                               select o;
                        break;
                    case "Name":
                        list = from o in query
                               orderby o.OrganizationName descending
                               select o;
                        break;
                    case "Location":
                        list = from o in query
                               orderby o.Location descending
                               select o;
                        break;
                    case "Schedule":
                        list = from o in query
                               orderby o.ScheduleId descending
                               select o;
                        break;
                    case "Self CheckIn":
                        list = from o in query
                               orderby (o.CanSelfCheckin ?? false) descending
                               select o;
                        break;
                    case "Leader":
                        list = from o in query
                               orderby o.LeaderName descending,
                                   o.OrganizationName descending
                               select o;
                        break;
                    case "Filled":
                        list = from o in query
                               orderby o.ClassFilled descending,
                                   o.OrganizationName descending
                               select o;
                        break;
                    case "Closed":
                        list = from o in query
                               orderby o.RegistrationClosed descending,
                                   o.OrganizationName descending
                               select o;
                        break;
                    case "RegType":
                        list = from o in query
                               orderby o.RegistrationTypeId descending,
                                   o.OrganizationName descending
                               select o;
                        break;
                    case "Category":
                        list = from o in query
                               orderby o.AppCategory ?? "zzz", o.PublicSortOrder ?? "zzz", o.OrganizationName
                               select o;
                        break;
                    case "Members":
                    case "Current":
                        list = from o in query
                               orderby o.MemberCount descending,
                                   o.OrganizationName descending
                               select o;
                        break;
                    case "FirstDate":
                        list = from o in query
                               orderby o.FirstMeetingDate descending,
                                   o.LastMeetingDate descending
                               select o;
                        break;
                    case "RegStart":
                        list = from o in query
                               orderby o.RegStart descending
                               select o;
                        break;
                    case "RegEnd":
                        list = from o in query
                               orderby o.RegEnd descending
                               select o;
                        break;
                    case "LastMeetingDate":
                        list = from o in query
                               orderby o.LastMeetingDate descending,
                                   o.FirstMeetingDate descending
                               select o;
                        break;
                }
            return list.ToList();
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
            list.Insert(0, new SelectListItem {Value = "0", Text = "(not specified)"});
            return list;
        }

        public IEnumerable<SelectListItem> CampusIds()
        {
            var qc = DbUtil.Db.Campus.AsQueryable();
            qc = DbUtil.Db.Setting("SortCampusByCode")
                ? qc.OrderBy(cc => cc.Code)
                : qc.OrderBy(cc => cc.Description);
            var list = (from c in qc
                        select new SelectListItem()
                        {
                            Value = c.Id.ToString(),
                            Text = c.Description,
                        }).ToList();
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
                Text = "(not specified)"
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
                Text = ProgId == 0 ? "(select a program)" : "(not specified)"
            });
            return list;
        }

        public IEnumerable<SelectListItem> ScheduleIds()
        {
            var q = from sc in DbUtil.Db.OrgSchedules
                    group sc by new {sc.ScheduleId, sc.MeetingTime}
                    into g
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
                Text = "(None)"
            });
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "(not specified)"
            });
            return list;
        }

        public static IEnumerable<SelectListItem> OrgTypeFilters()
        {
            var list = OrgTypes().ToList();
            list.Insert(0, new SelectListItem {Text = "Suspended Checkin", Value = OrgType.SuspendedCheckin.ToString()});
            list.Insert(0, new SelectListItem {Text = "Main Fellowship", Value = OrgType.MainFellowship.ToString()});
            list.Insert(0, new SelectListItem {Text = "Not Main Fellowship", Value = OrgType.NotMainFellowship.ToString()});
            list.Insert(0, new SelectListItem {Text = "Parent Org", Value = OrgType.ParentOrg.ToString()});
            list.Insert(0, new SelectListItem {Text = "Child Org", Value = OrgType.ChildOrg.ToString()});
            list.Insert(0, new SelectListItem {Text = "Orgs Without Type", Value = OrgType.NoOrgType.ToString()});
            list.Insert(0, new SelectListItem {Text = "Orgs With Fees", Value = OrgType.Fees.ToString()});
            list.Insert(0, new SelectListItem {Text = "Orgs Without Fees", Value = OrgType.NoFees.ToString()});
            list.Insert(0, new SelectListItem {Text = "(not specified)", Value = "0"});
            return list;
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
            return q;
        }

        public static IEnumerable<SelectListItem> RegistrationTypeIds()
        {
            var list = new List<SelectListItem>();
            var spec = new Dictionary<int, string>
            {
                {RegClass.NotSpecified, "(Not Specified)"},
                {RegClass.Active, "(Active Registration)"},
                {RegClass.OnApp, "(Any Reg on App)"},
                {RegClass.NotOnApp, "(Any Reg not on App)"},
                {RegClass.MasterOrStandalone, "(Master or StandAlone Reg)"},
                {RegClass.AnyRegistration, "(Any Registration)"},
                {RegClass.ChildOfMaster, "(Child of Master Reg)"},
                {RegClass.Standalone, "(StandAlone reg)"}
            };
            list.AddRange(spec.Select(dd => new SelectListItem {Value = dd.Key.ToString(), Text = dd.Value}));

            var codes = RegistrationTypeCode.GetCodePairs();
            list.AddRange(codes.Select(dd => new SelectListItem {Value = dd.Key.ToString(), Text = dd.Value}));

            list.Add(new SelectListItem {Value = RegClass.MissionTrip.ToString(), Text = "Mission Trip"});

            return list;
        }

        public static DateTime DefaultMeetingDate(int scheduleid)
        {
            var sdt = Organization.GetDateFromScheduleId(scheduleid);
            if (sdt == null)
                return DateTime.Now.Date.AddHours(8);
            var dt = Util.Now.Date;
            dt = dt.AddDays(-(int) dt.DayOfWeek); // prev sunday
            dt = dt.AddDays(sdt.Value.Day);
            if (dt < Util.Now.Date)
                dt = dt.AddDays(7);
            return dt.Add(sdt.Value.TimeOfDay);
        }

        public Dictionary<Person, string> NoticesToSend()
        {
            var leaderNotices = new Dictionary<Person, string>();

            var olist = FetchOrgs().Select(oo => oo.OrganizationId).ToList();

            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var orgs = string.Join(",", olist);
            var alist = cn.Query<RecentAbsentsInfo>("RecentAbsentsSP2", new {orgs},
                commandType: CommandType.StoredProcedure, commandTimeout: 600).ToList();

            var mlist = (from r in DbUtil.Db.LastMeetings(orgs)
                         where olist.Contains(r.OrganizationId)
                         select r).ToList();

            var plist = (from om in DbUtil.Db.ViewOrganizationLeaders
                         where olist.Contains(om.OrganizationId)
                         group om.OrganizationId by om.PeopleId
                         into leaderlist
                         select leaderlist).ToList();

            PythonModel.RegisterHelpers(DbUtil.Db);
            var template = HandlebarsDotNet.Handlebars.Compile(Resource1.RecentVisitsAbsents);
            var sb = new StringBuilder("Notices sent to:</br>\n<table>\n");
            foreach (var p in plist)
            {
                var leader = DbUtil.Db.LoadPersonById(p.Key);
                var orgids = p.ToList();
                var meetings =
                    (from m in mlist
                     where orgids.Contains(m.OrganizationId)
                     let visitors = DbUtil.Db.OrgVisitorsAsOfDate(m.OrganizationId, m.Lastmeeting, true).ToList()
                     let absents = (from a in alist where a.OrganizationId == m.OrganizationId select a).ToList()
                     let org = DbUtil.Db.LoadOrganizationById(m.OrganizationId)
                     select new
                     {
                         m.MeetingId,
                         m.OrganizationId,
                         m.Lastmeeting,
                         OrgName = Organization.FormatOrgName(m.OrganizationName, m.LeaderName, m.Location),
                         m.LeaderName,
                         ConsecutiveAbsentsThreshold = org.ConsecutiveAbsentsThreshold ?? 2,
                         HasAbsents = absents.Any(),
                         Absents = (from a in absents
                                    orderby a.LastAttend descending
                                    select new
                                    {
                                        a.PeopleId,
                                        a.Consecutive,
                                        a.Name2,
                                        HomePhone = a.HomePhone.FmtFone(),
                                        CellPhone = a.CellPhone.FmtFone(),
                                        a.EmailAddress,
                                        a.LeaderName,
                                        a.OrganizationName,
                                        a.OtherAttends,
                                        a.LastAttend
                                    }).ToList(),
                         HasVisits = visitors.Any(),
                         Visits = (from a in visitors
                                   select new
                                   {
                                       a.PeopleId,
                                       a.LastName,
                                       a.PreferredName,
                                       a.LastAttended,
                                       HomePhone = a.HomePhone.FmtFone(),
                                       CellPhone = a.CellPhone.FmtFone(),
                                       a.Email,
                                   }).ToList(),
                     }).ToList();
                foreach (var m in meetings)
                    sb.Append($"<tr><td>{leader.Name}</td><td>{m.OrgName}</td><td>{m.Lastmeeting:g}</td></tr>\n");

                leaderNotices.Add(leader, template(meetings));
            }
            sb.Append("</table>\n");
            noticelist = sb.ToString();
            return leaderNotices;
        }

        public void SendNotices()
        {
            var leaders = NoticesToSend();
            foreach (var leader in leaders)
            {
                DbUtil.Db.Email(DbUtil.Db.CurrentUser.Person.FromEmail, leader.Key, null,
                    DbUtil.Db.Setting("SubjectAttendanceNotices", "Attendance reports are ready for viewing"), leader.Value, false);
            }
            DbUtil.Db.Email(DbUtil.Db.CurrentUser.Person.FromEmail, DbUtil.Db.CurrentUser.Person, null,
                "Attendance emails sent", noticelist, false);
        }

        public string EncodedJson()
        {
            var j = JsonConvert.SerializeObject(this);
            return Util.EncryptForUrl(j);
        }

        public string SqlTable(string report, string oids, DateTime? meetingDate1, DateTime? meetingDate2)
        {
            using (var rd = ExecuteReader(report, oids, meetingDate1, meetingDate2))
                return GridResult.Table(rd);
        }
        public EpplusResult SqlTableExcel(string report, string oids, DateTime? meetingDate1, DateTime? meetingDate2)
        {
            using (var rd = ExecuteReader(report, oids, meetingDate1, meetingDate2))
            return rd.ToExcel(report + ".xlsx", fromSql: true);
        }

        private IDataReader ExecuteReader(string report, string oids, DateTime? meetingDate1, DateTime? meetingDate2)
        {
            var content = DbUtil.Db.ContentOfTypeSql(report);
            if (!content.HasValue())
                throw new Exception("no content");
            if (!SpecialReportViewModel.CanRunScript(content))
                throw new Exception("Not Authorized to run this script");

            if (!content.Contains("@OrgIds", ignoreCase: true))
                throw new Exception("missing @OrgIds");

            var p = GetSqlParameters(oids, meetingDate1, meetingDate2, content);
            var cs = HttpContextFactory.Current.User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            return cn.ExecuteReader(content, p, commandTimeout: 1200);
        }

        public DynamicParameters GetSqlParameters(string oids, DateTime? meetingDate1, DateTime? meetingDate2,
                                                          string content)
        {
            var p = new DynamicParameters();
            p.Add("@OrgIds", oids);
            if (content.Contains("@MeetingDate1", ignoreCase: true))
            {
                p.Add("@MeetingDate1", meetingDate1);
                p.Add("@MeetingDate2", meetingDate2);
            }
            if (content.Contains("@userid", ignoreCase: true))
                p.Add("@userid", Util.UserId);
            return p;
        }

        public static OrgSearchModel DecodedJson(string parameter)
        {
            var j = Util.DecryptFromUrl(parameter);
            return JsonConvert.DeserializeObject<OrgSearchModel>(j);
        }

        public string ConvertToSearch()
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();
            var c = cc.AddNewClause(QueryType.OrgSearchMember, CompareType.Equal, "1,True");
            if (Name.HasValue())
                c.OrgName = Name;
            if (ProgramId != 0)
                c.Program = ProgramId.ToString();
            if (DivisionId != 0)
                c.Division = DivisionId.ToString();
            if (StatusId != 0)
                c.OrgStatus = StatusId.ToString();
            if (TypeId != 0)
                c.OrgType2 = TypeId ?? 0;
            if (CampusId != 0)
                c.Campus = CampusId.ToString();
            if (ScheduleId != 0)
                c.Schedule = ScheduleId.ToString();
            if (OnlineReg != 0)
                c.OnlineReg = OnlineReg.ToString();

            cc.Save(DbUtil.Db);
            return "/Query/" + cc.Id;
        }

        public class RecentAbsentsInfo
        {
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; }
            public string LeaderName { get; set; }
            public int ConsecutiveAbsentsThreshold { get; set; }
            public int? Consecutive { get; set; }
            public int PeopleId { get; set; }
            public string Name2 { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string EmailAddress { get; set; }
            public int? OtherAttends { get; set; }
            public DateTime? LastAttend { get; set; }
            public DateTime? LastMeeting { get; set; }
        }

        public class OrgMemberInfoClass : ExportInvolvements.MemberInfoClass
        {
            public int OrganizationId { get; set; }
            public string Organization { get; set; }
            public string Schedule { get; set; }
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

        public class RegClass
        {
            public const int NotSpecified = -1;
            public const int AnyRegistration = 99;
            public const int MissionTrip = 98;
            public const int MasterOrStandalone = 97;
            public const int Active = 96;
            public const int NotOnApp = 95;
            public const int OnApp = 94;
            public const int Standalone = 93;
            public const int ChildOfMaster = 92;
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

            public string RegType => RegistrationTypeCode.Lookup(RegTypeId ?? 0);

            public string RegStart { get; set; }
            public string RegEnd { get; set; }
            public string Description { get; set; }
            public string AppCategory { get; set; }
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
            public string ToolTip => $@"{OrganizationName} ({Id})|
Program: {ProgramName} ({ProgramId})|
Division: {DivisionName} ({DivisionId})|
Leader: {LeaderName}|
First Meeting: {FirstMeetingDate}|
Last Meeting: {LastMeetingDate}|
Schedule: {Schedule}|
Location: {Location}|
Divisions: {Divisions}";
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
            public string Schedule => $"{MeetingTime:ddd h:mm tt}";
            public string Location { get; set; }
        }

        public HtmlString NameHelp => new HtmlString(@"
<p>Search by:</p>
<ul>
<li>OrganizationId</li>
<li>Location</li>
<li>Part of Name (of: organization, leader, division, or location)</li>
</ul>
<p>Advanced:</p>
<ul>
<li><code>ev:<em>name</em></code> has an ExtraValue like <em>name</em></li>
<li><code>-ev:<em>name</em></code> does not have an ExtraValue like <em>name</em></li>
<li><code>childof:<em>orgid</em></code> is a child org of the parent <em>orgid</em></li>
<li><code>master:<em>orgid</em></code> is in the picklist of the master <em>orgid</em></li>
<li><code>regsetting:<em>comma,separated,keywords</em></code> searches regsetting usage</li>
</ul>
<p>For more see <em><strong><a href='https://docs.touchpointsoftware.com/Organizations/OrgSearchNameAdvanced.html' target='_blank'>the documentation</a></p>
");
    }
}
