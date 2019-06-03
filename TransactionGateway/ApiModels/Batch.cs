using System;

namespace TransactionGateway.ApiModels
{
    public class Batch : BaseResponse
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Merchant Recipient { get; set; }
        public int TotalPayments { get; set; }  // how many payments
        public Money TotalAmount { get; set; } // the value of the payments
        public Money TotalAchAmount { get; set; }
        public Money TotalCardAmount { get; set; }
        public Money TotalCashAmount { get; set; }
        public Money TotalCheckAmount { get; set; }
    }
}
