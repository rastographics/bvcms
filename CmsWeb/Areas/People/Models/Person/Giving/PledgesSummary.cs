namespace CmsWeb.Areas.People.Models
{
    public class PledgesSummary
    {
        public int FundId { get; set; }
        public string Fund { get; set; }
        public decimal AmountPledged { get; set; }
        public decimal AmountContributed { get; set; }
        public decimal Balance { get; set; }
    }
}
