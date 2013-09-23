using System;

namespace CmsWeb.Areas.People.Models
{
    public class TaskInfo
    {
        public int TaskId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Desc { get; set; }
        public string About { get; set; }
        public int? AboutId { get; set; }
        public string AssignedTo { get; set; }
        public int AssignedToId { get; set; }
        public string link { get; set; }
        public DateTime? completed { get; set; }
    }
}