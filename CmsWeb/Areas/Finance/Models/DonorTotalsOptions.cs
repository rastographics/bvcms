using System;
using System.ComponentModel;
using CmsWeb.Code;

namespace CmsWeb.Models
{
    public class DonorTotalSummaryOptionsModel
    {
        public int NumberOfYears { get; set; }

        public DateTime StartDate { get; set; }

        public int MinimumMedianTotal { get; set; }

        [SettingDisplayName("Campus")]
        public CodeInfo Campus { get; set; }

        [DisplayName("Fund")]
        public CodeInfo Fund { get; set; }
    }
}