
namespace CmsData.Finance.TransNational.Native.Transaction.Void
{
    internal class VoidRequest : TransactRequest
    {
        public VoidRequest(string userName, string password, string transactionId) 
            : base(userName, password)
        {
            Data["type"] = "void";
            Data["transactionid"] = transactionId;
        }
    }
}
