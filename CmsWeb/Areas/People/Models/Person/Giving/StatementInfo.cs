using System;

namespace CmsWeb.Areas.People.Models
{
    public class StatementInfo
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Amount { get; set; }
        public int Count { get; set; }
    }
}