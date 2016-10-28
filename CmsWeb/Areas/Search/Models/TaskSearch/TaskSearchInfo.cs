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
        public bool Archived { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }

    }
}