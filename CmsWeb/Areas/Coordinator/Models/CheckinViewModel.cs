using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Coordinator.Models
{
    public class CheckinViewModel
    {
        public IEnumerable<CheckinScheduleDto> DailySchedules { get; set; }
        public IEnumerable<int> UniqueScheduleIds { get; set; }
        public IEnumerable<int> UniqueOrganizationIds { get; set; }


        public IEnumerable<int> GetUniqueTimeslotsFromSchedules()
        {
            return DailySchedules.AsQueryable()
                .OrderBy(s => s.NextMeetingDate)
                .Select(s => s.OrgScheduleId)
                .Distinct()
                .ToList();
        }

        public object GetUniqueOrganizationsFromTimeslot(int timeslotId)
        {
            return null;
        }
    }

    public class CheckinScheduleDto
    {
        public int OrgScheduleId { get; set; }
        public DateTime? NextMeetingDate { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int SubgroupId { get; set; }
        public string SubgroupName { get; set; }
        public int CheckInCapacity { get; set; }
        public bool CheckInOpen { get; set; }
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
    }
}
