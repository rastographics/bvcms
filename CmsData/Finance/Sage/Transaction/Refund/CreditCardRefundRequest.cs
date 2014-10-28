namespace CmsData.Finance.Sage.Transaction.Refund
{
    internal class CreditCardRefundRequest : TransactionProcessingRequest
    {
        public CreditCardRefundRequest(string id, string key, string transactionId, decimal amount)
            : base(id, key, "BANKCARD_CREDIT")
        {
            Data["T_REFERENCE"] = transactionId;
            Data["T_AMT"] = amount.ToString("n2");
        }
    }
}
