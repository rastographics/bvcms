using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    public class ApiMeeting
    {
        [Key]
        public int MeetingId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrganizationId { get; set; }
        public int NumPresent { get; set; }
        public int NumMembers { get; set; }
        public int NumVstMembers { get; set; }
        public int NumRepeatVst { get; set; }
        public int NumNewVisit { get; set; }
        public string Location { get; set; }
        public DateTime? MeetingDate { get; set; }
        public bool GroupMeetingFlag { get; set; }
        public string Description { get; set; }
        public int? NumOutTown { get; set; }
        public int? NumOtherAttends { get; set; }
        public int? AttendCreditId { get; set; }
        public int? ScheduleId { get; set; }
        public bool? NoAutoAbsents { get; set; }
        public int? HeadCount { get; set; }
        public int? MaxCount { get; set; }
    }
}
