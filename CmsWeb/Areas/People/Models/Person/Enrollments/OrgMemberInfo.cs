using System;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
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
        public Decimal? AttendPct { get; set; }
        public string DivisionName { get; set; }
        public string ProgramName { get; set; }
        public string OrgType { get; set; }
        public string HasDirectory { get; set; }

        public string Schedule { get { return "{0:ddd h:mm tt}".Fmt(MeetingTime); } }
        public string SchComma { get { return MeetingTime.HasValue ? ", " : ""; } }
        public string LocComma { get { return Location.HasValue() ? ", " : ""; } }
    }
}
