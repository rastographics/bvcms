using System;

namespace CmsWeb.Areas.Org.Models
{
    public class MeetingInfo
    {
        public int MeetingId { get; set; }
        public DateTime? MeetingDate { get; set; }
        public DateTime? MeetingTime
        {
            get
            {
                if (MeetingDate.HasValue)
                    if (MeetingDate.Value.TimeOfDay.TotalSeconds > 0)
                        return MeetingDate;
                return null;
            }
        }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string MeetingName { get; set; }
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public string MemberType { get; set; }
        public string AttendType { get; set; }
        public bool AttendFlag { get; set; }
        public bool RegisteredFlag { get; set; }
        public int RollSheetSectionId { get; set; }
        public int OtherAttends { get; set; }
        public int? HeadCount { get; set; }
        public int NumPresent { get; set; }
        public int NumVisitors { get; set; }
        public string Location { get; set; }
        public bool GroupMeeting { get; set; }
        public string Description { get; set; }
    }
}
