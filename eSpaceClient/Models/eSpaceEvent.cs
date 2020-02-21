using System;
using System.Collections.Generic;

namespace eSpace.Models
{
    public class eSpaceEvent
    {
        public List<DateTime> AdditionalDates { get; set; }
        public bool AutoApprove { get; set; }
        public List<eSpaceCategory> Categories { get; set; }
        public List<eSpaceContact> Contacts { get; set; }
        public string Description { get; set; }
        public List<eSpacePerson> Editors { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventName { get; set; }
        public long EventId { get; set; }
        public bool IsAllDayEvent { get; set; }
        public bool IsFinalApproved { get; set; }
        public bool IsMultiDayEvent { get; set; }
        public bool IsOffSite { get; set; }
        public bool IsPublic { get; set; }
        public List<eSpaceLocation> Locations { get; set; }
        public int? NumOfPeople { get; set; }
        public string OffsiteLocation { get; set; }
        public long OwnerId { get; set; }
        public string PublicCalendarImageUrl { get; set; }
        public string PublicHtmlNotes { get; set; }
        public string PublicLink { get; set; }
        public List<eSpaceLocation> PublicLocations { get; set; }
        public string PublicNotes { get; set; }
        public long ScheduleId { get; set; }
        public DateTime SetupStartDate { get; set; }
        public DateTimeOffset SetupStartTime { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string Status { get; set; }
        public DateTime TeardownEndDate { get; set; }
        public DateTimeOffset TearDownEndTime { get; set; }
    }
}
