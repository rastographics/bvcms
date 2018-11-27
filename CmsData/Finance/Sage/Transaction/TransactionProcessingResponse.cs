
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
            if (Data.HasElements)
            {
                ApprovalIndicator = GetApprovalIndicator(Data.Element("APPROVAL_INDICATOR")?.Value.Trim() ?? null);
                Code = Data.Element("CODE")?.Value.Trim() ?? null;
                Reference = Data.Element("REFERENCE").Value.Trim() ?? null;
                OrderNumber = Data.Element("ORDER_NUMBER")?.Value ?? null;
                // check for the existance of the avs and cvv indicators and set them.
                AvsIndicator = Data.Element("AVS_INDICATOR")?.Value.Trim() ?? AvsIndicator;
                CvvIndicator = Data.Element("CVV_INDICATOR")?.Value.Trim() ?? CvvIndicator;                
            }
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
