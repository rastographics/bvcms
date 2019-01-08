using System;

namespace CmsWeb.Areas.Coordinator.Models
{
    public class CheckinAttendeeDto
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public bool IsWorker { get; set; }
        public DateTime MeetingDate { get; set; }
        public int OrganizationId { get; set; }
        public int SubGroupId { get; set; }
        public string SubGroupName { get; set; }
    }
}
