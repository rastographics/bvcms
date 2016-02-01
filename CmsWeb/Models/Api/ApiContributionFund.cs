using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    public class ApiContributionFund
    {
        [Key]
        public int FundId { get; set; }
        public string FundName { get; set; }
        public string FundDescription { get; set; }
        public int FundStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool FundPledgeFlag { get; set; }
    }
}
