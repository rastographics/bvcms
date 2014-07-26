using System;
using CmsWeb.Code;
using NPOI.SS.Formula.Functions;

namespace CmsWeb.Areas.Search.Models
{
    [Serializable]
    public class RegistrationSearchInfo
    {
        public string Registrant { get; set; }
        public string User { get; set; }
        public string Organization { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CodeInfo Active { get; set; }
        public CodeInfo Expired { get; set; }
        public CodeInfo Complete { get; set; }
        public CodeInfo Abandoned { get; set; }
        public string count { get; set; }

        public RegistrationSearchInfo()
        {
            Active = new CodeInfo("YesNoAll");
            Complete = new CodeInfo("YesNoAll");
            Abandoned = new CodeInfo("YesNoAll");
            Expired = new CodeInfo("YesNoAll");
        }
    }
}