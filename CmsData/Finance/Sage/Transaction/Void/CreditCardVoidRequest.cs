
namespace CmsData.Finance.Sage.Transaction.Void
{
    internal class CreditCardVoidRequest : TransactionProcessingRequest
    {
        public CreditCardVoidRequest(string id, string key, string transactionId)
            : base(id, key, "BANKCARD_VOID")
        {
            Data["T_REFERENCE"] = transactionId;
        }
    }
}
