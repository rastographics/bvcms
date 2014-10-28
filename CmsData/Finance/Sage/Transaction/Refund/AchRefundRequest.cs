namespace CmsData.Finance.Sage.Transaction.Refund
{
    internal class AchRefundRequest : TransactionProcessingRequest
    {
        public AchRefundRequest(string id, string key, string transactionId, decimal amount)
            : base(id, key, "VIRTUAL_CHECK_CREDIT_BY_REFERENCE")
        {
            Data["T_REFERENCE"] = transactionId;
            Data["T_AMT"] = amount.ToString("n2");
        }
    }
}
