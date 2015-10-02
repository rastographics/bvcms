using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    [ApiMapName("Contributions")]
    public class ApiContribution
    {
        public int? PeopleId { get; set; }
        public int? FamilyId { get; set; }
        public decimal? ContributionAmount { get; set; }
        public DateTime? ContributionDate { get; set; }
        public int FundId { get; set; }
        public string CheckNo { get; set; }
        [Key]
        public int ContributionId { get; set; }
    }
}
