
using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Transaction
{
    internal class TransactResponse : Response
    {
        public string TransactionId { get; private set; }

        public string AuthCode { get; private set; }

        public string AvsResponse { get; private set; }

        public string CvvResponse { get; private set; }

        public string OrderId { get; private set; }

        public TransactResponse(string response)
            : base(response)
        {
            TransactionId = Data["transactionid"];
            AuthCode = Data["authcode"];
            AvsResponse = Data["avsresponse"];
            CvvResponse = Data["cvvresponse"];
            OrderId = Data["orderid"];
        }
    }
}
