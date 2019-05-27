using System;

namespace TransactionGateway.ApiModels
{
    public class RecurringPayment : BaseResponse
    {
        public Schedule Schedule { get; set; }
        public string Status { get; set; }
        public string PaymentToken { get; set; }
        public Money Amount { get; set; }
        public Payer Payer { get; set; }
        public Merchant Recipient { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string PaymentMethodType { get; set; }
        public string Source { get; set; }
        public Card Card { get; set; }
        public Fund Fund { get; set; }
        public Campus Campus { get; set; }
    }

    public class Schedule
    {
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public string EndType { get; set; }
        public DateTime NextPaymentDate { get; set; }
    }
}
