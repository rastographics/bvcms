using System;

namespace CmsWeb.Areas.Search.Models
{
    public class TaskInfo
    {
        public int ContactId { get; set; }
        public string Comments { get; set; }
        public DateTime ContactDate { get; set; }
        public string TypeOfContact { get; set; }
        public string ContactReason { get; set; }
        public string ContactorList { get; set; }
        public string ContacteeList { get; set; }
        public string Ministry { get; set; }
        public string Results { get; set; }
    }
}