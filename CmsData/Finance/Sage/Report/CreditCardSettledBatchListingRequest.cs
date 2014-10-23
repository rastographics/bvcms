
namespace CmsData.Finance.Sage.Report
{
    internal class CreditCardSettledBatchListingRequest : ReportRequest
    {
        public CreditCardSettledBatchListingRequest(string id, string key, string batchReference)
            : base(id, key, "VIEW_BANKCARD_SETTLED_BATCH_LISTING")
        {
            Data["BATCH_REFERENCE"] = batchReference;
        }
    }
}
