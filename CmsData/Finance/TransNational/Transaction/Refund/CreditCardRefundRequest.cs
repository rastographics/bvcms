
namespace CmsData.Finance.TransNational.Transaction.Refund
{
    internal class CreditCardRefundRequest : TransactRequest
    {
        public CreditCardRefundRequest(string userName, string password, string transactionId) 
            : base(userName, password)
        {
            Data["type"] = "refund";
            Data["payment"] = "creditcard";
            Data["transactionid"] = transactionId;
        }

        public CreditCardRefundRequest(string userName, string password, string transactionId, decimal amount)
            :this(userName, password, transactionId)
        {
            Data["amount"] = amount.ToString("0.00");
        }
    }
}
