using System;

namespace CmsWeb.Areas.Search.Models
{
    public class TaskInfo
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Status { get; set; }
        public string About { get; set; }
        public string Delegate { get; set; }
        public string Owner { get; set; }
        public int? ContactId { get; set; }
    }
}