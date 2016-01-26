namespace CmsWeb.Models
{
    public class FundTotalInfo
    {
        public int? FundId { get; set; }
        public int QBSynced { get; set; }
        public int OnLine { get; set; }
        public string BundleType { get; set; }
        public int? BundleTypeId { get; set; }
        public string FundName { get; set; }
        public string GeneralLedgerId { get; set; }
        public decimal? Total { get; set; }
        public int? Count { get; set; }
        internal TotalsByFundModel model;

        public string BundleTotalsUrl()
        {
            return model.BundleTotalsUrl(FundId, BundleTypeId);
        }

        public string ContributionsUrl()
        {
            return model.ContributionsUrl(FundId, BundleTypeId);
        }
    }
}