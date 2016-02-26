
namespace CmsData.Finance.TransNational.Transaction.Void
{
    internal class AchVoidRequest : TransactRequest
    {
        public AchVoidRequest(string userName, string password, string transactionId) 
            : base(userName, password)
        {
            Data["type"] = "void";
            Data["payment"] = "check";
            Data["transactionid"] = transactionId;
        }
    }
}
