using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.Areas.Org.Models
{
    public class GroupListItem
    {
        public int value { get; internal set; }
        public string text { get { return name + schedule?.ToString(" - {0}"); } }
        public OrgSchedule schedule { get; internal set; }
        public string name { get; internal set; }
    }
}
