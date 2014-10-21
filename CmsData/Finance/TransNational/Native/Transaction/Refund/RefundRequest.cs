
namespace CmsData.Finance.TransNational.Native.Transaction.Refund
{
    internal class RefundRequest : TransactRequest
    {
        public RefundRequest(string userName, string password, string transactionId) 
            : base(userName, password)
        {
            Data["type"] = "refund";
            Data["transactionid"] = transactionId;
        }

        public RefundRequest(string userName, string password, string transactionId, decimal amount)
            :this(userName, password, transactionId)
        {
            Data["amount"] = amount.ToString("0.00");
        }
    }
}
