using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models.Report
{
    public class MeetingsModel : OrgSearchModel
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }

        public bool NoZero { get; set; }
        public bool FromWeekAtAGlance { get; set; }

        public MeetingsModel()
        {
            Direction = "asc";
            Sort = "Time";
        }

        public int MeetingsCount { get; set; }
        public int TotalAttends { get; set; }
        public int OtherAttends { get; set; }
        public int TotalPeople { get; set; }

        public IEnumerable<MeetingInfo> MeetingsForDate()
        {
            if (FromWeekAtAGlance)
                StatusId = null;
            var q = FetchOrgs();
            var q2 = from o in q
                     join m in DbUtil.Db.Meetings on o.OrganizationId equals m.OrganizationId into mr
                     from m in mr.Where(m => m.MeetingDate >= Dt1 && m.MeetingDate <= Dt2).DefaultIfEmpty()
                     where !(o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Inactive && (m == null || m.NumPresent == 0))
                     where (m != null && m.NumPresent > 0) || NoZero == false
                     let div = o.Division
                     select new MeetingInfo
                     {
                         Program = div.Program.Name,
                         Division = div.Name,
                         OrganizationId = o.OrganizationId,
                         Organization = o.OrganizationName,
                         MeetingId = m.MeetingId,
                         Tracking = m.GroupMeetingFlag ? "Group" : "Individual",
                         Time = m.MeetingDate,
                         Attended = m.NumPresent,
                         Leader = o.LeaderName,
                         Description = m.Description,
                         OtherAttends = m.NumOtherAttends,
                         Inactive = o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Inactive
                     };
            switch (Direction)
            {
                case "asc":
                    switch (Sort)
                    {
                        case "Attended":
                            q2 = q2.OrderBy(m => m.Attended);
                            break;
                        case "Leader":
                            q2 = q2.OrderBy(m => m.Leader);
                            break;
                        case "Time":
                            q2 = q2.OrderBy(m => m.Time).ThenBy(m => m.Division).ThenBy(m => m.Organization);
                            break;
                        case "Division":
                            q2 = q2.OrderBy(m => m.Division).ThenBy(m => m.Organization);
                            break;
                        case "Organization":
                            q2 = q2.OrderBy(m => m.Organization);
                            break;
                    }
                    break;
                case "desc":
                    switch (Sort)
                    {
                        case "Attended":
                            q2 = q2.OrderByDescending(m => m.Attended);
                            break;
                        case "Leader":
                            q2 = q2.OrderByDescending(m => m.Leader);
                            break;
                        case "Time":
                            q2 = q2.OrderByDescending(m => m.Time).ThenByDescending(m => m.Division).ThenByDescending(m => m.Organization);
                            break;
                        case "Division":
                            q2 = q2.OrderByDescending(m => m.Division).ThenByDescending(m => m.Organization);
                            break;
                        case "Organization":
                            q2 = q2.OrderByDescending(m => m.Organization);
                            break;
                    }
                    break;
            }
            var list = q2.ToList();

            MeetingsCount = list.Count(a => a.Attended > 0);
            TotalAttends = list.Sum(m => m.Attended ?? 0);
            OtherAttends = list.Sum(m => m.OtherAttends ?? 0);
            var q3 = from o in q
                     join m in DbUtil.Db.Meetings on o.OrganizationId equals m.OrganizationId into mr
                     from m in mr.Where(m => m.MeetingDate >= Dt1 && m.MeetingDate <= Dt2)
                     let div = o.Division
                     from a in m.Attends
                     where a.AttendanceFlag == true
                     select a.PeopleId;
            TotalPeople = q3.Distinct().Count();
            return q2;
        }
    }
    public class MeetingInfo
    {
        public string Program { get; set; }
        public string Division { get; set; }
        public int OrganizationId { get; set; }
        public int? MeetingId { get; set; }
        public string Organization { get; set; }
        public string Tracking { get; set; }
        public DateTime? Time { get; set; }
        public int? Attended { get; set; }
        public int? OtherAttends { get; set; }
        public string Leader { get; set; }
        public string Description { get; set; }
        public bool Inactive { get; set; }
    }
}