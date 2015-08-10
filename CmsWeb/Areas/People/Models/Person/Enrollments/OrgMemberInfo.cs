using System;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class OrgMemberInfo
    {
        public int OrgId { get; set; }
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string LeaderName { get; set; }
        public DateTime? MeetingTime { get; set; }
        public string MemberType { get; set; }
        public int? LeaderId { get; set; }
        public DateTime? EnrollDate { get; set; }
        public DateTime? DropDate { get; set; }
        public decimal? AttendPct { get; set; }
        public string DivisionName { get; set; }
        public string ProgramName { get; set; }
        public string OrgType { get; set; }
        public bool HasDirectory { get; set; }

        public string Schedule => $"{MeetingTime:ddd h:mm tt}";
        public string SchComma => MeetingTime.HasValue ? ", " : "";
        public string LocComma => Location.HasValue() ? ", " : "";
    }
}
