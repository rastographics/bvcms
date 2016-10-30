using System;
using CmsWeb.Code;

namespace CmsWeb.Areas.Search.Models
{
    [Serializable]
    public class TaskSearchInfo
    {
        public string Delegate { get; set; }
        public string Originator { get; set; }
        public string Owner { get; set; }
        public string About { get; set; }
        public bool? Archived { get; set; }
        public CodeInfo Status { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }
        public int? Lookback { get; set; }
        public bool? IsPrivate { get; set; }

        public TaskSearchInfo()
        {
            Status = new CodeInfo("TaskStatus");
        }
    }
}