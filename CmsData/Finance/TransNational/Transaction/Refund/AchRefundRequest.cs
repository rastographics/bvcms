
namespace CmsData.Finance.TransNational.Transaction.Refund
{
    internal class AchRefundRequest : TransactRequest
    {
        public AchRefundRequest(string userName, string password, string transactionId) 
            : base(userName, password)
        {
            Data["type"] = "refund";
            Data["payment"] = "check";
            Data["transactionid"] = transactionId;
        }

        public AchRefundRequest(string userName, string password, string transactionId, decimal amount)
            :this(userName, password, transactionId)
        {
            Data["amount"] = amount.ToString("0.00");
        }
    }
}
