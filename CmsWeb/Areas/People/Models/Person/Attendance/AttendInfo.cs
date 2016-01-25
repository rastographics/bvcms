using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.People.Models
{
    public class AttendInfo
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
        public string Commitment { get; set; }
        public bool conflict { get; set; }
        public string AttendCredit { get; set; }
    }
}
