
namespace CmsData.Finance.Sage.Transaction.Void
{
    internal class AchVoidRequest : TransactionProcessingRequest
    {
        public AchVoidRequest(string id, string key, string transactionId)
            : base(id, key, "VIRTUAL_CHECK_VOID")
        {
            Data["T_REFERENCE"] = transactionId;
        }
    }
}
