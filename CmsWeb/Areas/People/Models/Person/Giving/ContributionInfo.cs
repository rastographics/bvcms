using System;

namespace CmsWeb.Areas.People.Models
{
    public class ContributionInfo
    {
        public DateTime Date { get; set; }
        public string CheckNo { get; set; }
        public int ImageId { get; set; }
        public int ContributionId { get; set; }
        public decimal Amount { get; set; }
        public string Fund { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
