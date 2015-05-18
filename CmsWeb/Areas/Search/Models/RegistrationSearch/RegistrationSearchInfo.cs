using System;
using CmsWeb.Code;

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
        public bool FromMobileAppOnly { get; set; }
        public string Count { get; set; }

        public RegistrationSearchInfo()
        {
            var yna = CodeValueModel.YesNoAll().ToSelect("Value");
            Active = new CodeInfo("All", yna);
            Complete = new CodeInfo("All", yna);
            Abandoned = new CodeInfo("All", yna);
            Expired = new CodeInfo("All", yna);
        }
    }
}