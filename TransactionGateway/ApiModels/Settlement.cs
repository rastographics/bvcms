using System;

namespace TransactionGateway.ApiModels
{
    public class Settlement : BaseResponse
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int TotalPayments { get; set; }  // how many payments
        public Money TotalAmount { get; set; } // the value of the payments
        public DateTime EstimatedDepositDate { get; set; }
        public bool IsReconciled { get; set; }
    }
}
