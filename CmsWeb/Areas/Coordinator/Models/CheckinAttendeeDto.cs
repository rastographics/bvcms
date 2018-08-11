using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Coordinator.Models
{
    public class CheckinAttendeeDto
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public string AttendeeType { get; set; }
    }
}
