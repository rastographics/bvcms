using System;

namespace CmsWeb.Models
{
    public class RecurringManagment
    {
        public DateTime NextPayment { get; set; }
        public decimal Amount { get; set; }
        public string Fund { get; set; }
        public string Frequency { get; set; }
        public string LinkToEdit { get; set; }
    }
}
