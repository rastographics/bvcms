using System;
using CmsWeb.Code;

namespace CmsWeb.Areas.Search.Models
{
    [Serializable]
    public class TaskSearchInfo
    {
        public string ContacteeName { get; set; }
        public string ContactorName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CodeInfo ContactType { get; set; }
        public CodeInfo ContactReason { get; set; }
        public CodeInfo Ministry { get; set; }
        public CodeInfo ContactResult { get; set; }
        public string CreatedBy { get; set; }
        public bool Incomplete { get; set; }
        public bool Private { get; set; }

        public TaskSearchInfo()
        {
            ContactType = new CodeInfo("ContactType");
            ContactReason = new CodeInfo("ContactReason");
            Ministry = new CodeInfo("Ministry");
            ContactResult = new CodeInfo("ContactResult");
        }
    }
}