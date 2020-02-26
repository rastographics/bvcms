using System;
using System.Collections.Generic;

namespace eSpace.Models
{
    public class eSpaceOccurrence
    {
        public long OccurrenceId { get; set; }
        public DateTime? SetUpStart { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public DateTime? TearDownEnd { get; set; }
        public bool IsAllDay { get; set; }
        public string OccurrenceStatus { get; set; }
        public List<eSpaceItem> Items { get; set; }
        public string EventName { get; set; }
        public long EventId { get; set; }
        public long ScheduleId { get; set; }
        public string EventStatus { get; set; }
        public string PublicCalendarImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public string PublicNotes { get; set; }
        public string PublicHtmlNotes { get; set; }
        public List<eSpaceContact> Contacts { get; set; }
        public bool IsFinalApproved { get; set; }
    }
}
