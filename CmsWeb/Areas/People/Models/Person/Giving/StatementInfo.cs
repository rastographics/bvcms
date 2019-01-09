using System;

namespace CmsWeb.Areas.People.Models
{
    public class StatementInfo
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }

    public class StatementInfoWithFund : StatementInfo
    {
        public int FundId { get; set; }
        public string FundName { get; set; }
    }
}
