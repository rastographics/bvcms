using System;
using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class Payment : BaseResponse
    {
        internal static IEqualityComparer<Payment> Comparer => new PaymentComparer();

        public string TransactionId { get; set; }
        public string PaymentToken { get; set; }
        public string SplitPaymentKey { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? GivenOn { get; set; }
        public Money Amount { get; set; }
        public Fund Fund { get; set; }
        public Payer Payer { get; set; }
        public Batch Batch { get; set; }
        public Settlement Settlement { get; set; }
        public Merchant Recipient { get; set; }
        public Campus Campus { get; set; }
        public string Source { get; set; }
        public string PaymentMethodType { get; set; }
        public object RecordedCheck { get; set; }
        public Card Card { get; set; }
    }

    internal class PaymentComparer : IEqualityComparer<Payment>
    {
        public bool Equals(Payment x, Payment y) => x?.TransactionId == y?.TransactionId;
        public int GetHashCode(Payment obj) => obj.TransactionId.GetHashCode();
    }
}
