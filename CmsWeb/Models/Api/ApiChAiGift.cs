using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    public class ApiChAiGift
    {
        [Key]
        public int ContributionId { get; set; }
        [Key]
        public int? PeopleId { get; set; }
        [Key]
        public int? FamilyId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ContributionDate { get; set; }
        public decimal? ContributionAmount { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string BundleType { get; set; }
        public string FundName { get; set; }
        public string PaymentType { get; set; }
        public string CheckNo { get; set; }
    }
}
