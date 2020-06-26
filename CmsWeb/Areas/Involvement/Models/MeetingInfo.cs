using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Involvement.Models
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
        public int? HeadCount { get; set; }
        public int NumPresent { get; set; }
        public int NumVisitors { get; set; }
        public string Venue { get; set; }
        public bool GroupMeeting { get; set; }
        public string Description { get; set; }
        public bool Conflict { get; set; }
    }
}
