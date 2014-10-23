
namespace CmsData.Finance.Sage.Report
{
    internal class AchSettledBatchListingRequest : ReportRequest
    {
        public AchSettledBatchListingRequest(string id, string key, string batchReference)
            : base(id, key, "VIEW_VIRTUAL_CHECK_SETTLED_BATCH_LISTING")
        {
            Data["BATCH_REFERENCE"] = batchReference;
        }
    }
}
