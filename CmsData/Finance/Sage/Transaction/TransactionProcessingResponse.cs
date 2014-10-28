
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction
{
    internal class TransactionProcessingResponse : Response
    {
        public ApprovalIndicator ApprovalIndicator { get; private set; }

        public string Code { get; private set; }

        public string Reference { get; private set; }

        public string AvsIndicator { get; private set; }

        public string CvvIndicator { get; private set; }

        public string OrderNumber { get; private set; }

        public TransactionProcessingResponse(string xml)
            : base(xml)
        {
            ApprovalIndicator = GetApprovalIndicator(Data.Element("APPROVAL_INDICATOR").Value.Trim());
            Code = Data.Element("CODE").Value.Trim();
            Reference = Data.Element("REFERENCE").Value.Trim();
            OrderNumber = Data.Element("ORDER_NUMBER").Value.Trim();

            // check for the existance of the avs and cvv indicators and set them.
            var avsIndicator = Data.Element("AVS_INDICATOR");
            if (avsIndicator != null) 
                AvsIndicator = avsIndicator.Value.Trim();

            var cvvIndicator = Data.Element("CVV_INDICATOR");
            if (cvvIndicator != null) 
                CvvIndicator = cvvIndicator.Value.Trim();
        }

        private ApprovalIndicator GetApprovalIndicator(string approvalIndicatorCode)
        {
            switch (approvalIndicatorCode)
            {
                case "A":
                    return ApprovalIndicator.Approved;
                case "E":
                    return ApprovalIndicator.FrontEndError;
                case "X":
                    return ApprovalIndicator.GatewayError;
                default:
                    return ApprovalIndicator.NotSet;
            }
        }
    }
}
