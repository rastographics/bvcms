using System;
using System.ComponentModel;
using CmsWeb.Code;

namespace CmsWeb.Areas.Dialog.Models
{
    public class NewMeetingInfo
    {
        [DisplayName("Choose A Schedule")]
        public CodeInfo Schedule { get; set; }
        public CodeInfo AttendCredit { get; set; }
        public string Description { get; set; }
        [DisplayName("Description")]
        public CodeInfo DescriptionList { get; set; }
        [DateAndTimeValid]
        public DateTime MeetingDate { get; set; }
        public bool ByGroup { get; set; }
        public string GroupFilterPrefix { get; set; }
        public string HighlightGroup { get; set; }
        public bool UseAltNames { get; set; }
        public int? OrganizationId { get; set; }
        public bool UseMeetingDescriptionPickList { get; set; }
        public bool UseWord { get; set; }
    }
}
