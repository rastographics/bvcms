using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Manage.Models
{
    public class ActivityInfo
    {
        public string Machine { get; set; }
        public DateTime? Date { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Activity { get; set; }
        public string OrgName { get; set; }
        public int? OrgId { get; set; }
        public int? PeopleId { get; set; }
        public string PersonName { get; set; }
    }
}