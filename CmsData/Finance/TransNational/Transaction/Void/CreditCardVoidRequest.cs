
namespace CmsData.Finance.TransNational.Transaction.Void
{
    internal class CreditCardVoidRequest : TransactRequest
    {
        public CreditCardVoidRequest(string userName, string password, string transactionId) 
            : base(userName, password)
        {
            Data["type"] = "void";
            Data["payment"] = "creditcard";
            Data["transactionid"] = transactionId;
        }
    }
}
