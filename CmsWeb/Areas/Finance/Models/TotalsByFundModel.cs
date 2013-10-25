using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.View;
using DocumentFormat.OpenXml.Wordprocessing;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class TotalsByFundModel
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }
        public int CampusId { get; set; }
        public string Sort { get; set; }
        public string Dir { get; set; }
        public bool NonTaxDeductible { get; set; }
        public int Online { get; set; }
        public bool Pledges { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public bool IncludeBundleType { get; set; }
        public int? FundId { get; set; }
        public string closedbundlesonly { get { return IncUnclosedBundles ? "false" : "true"; } }

        public TotalsByFundModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
                first = first.AddMonths(-1);
            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public FundTotalInfo FundTotal;

        public IEnumerable<FundTotalInfo> TotalsByFund()
        {
            List<FundTotalInfo> q = null;

            var api = new APIContributionSearchModel(DbUtil.Db)
            {
                model =
                {
                    FundId = FundId,
                    StartDate = Dt1,
                    EndDate = Dt2,
                    ClosedBundlesOnly = !IncUnclosedBundles,
                    TaxNonTax = NonTaxDeductible ? "All" : "TaxDed",
                    CampusId = CampusId,
                    Status = ContributionStatusCode.Recorded,
                    Online = Online
                }
            };

            if (IncludeBundleType)
                q = (from c in api.FetchContributions()
                     let BundleType = c.BundleDetails.First().BundleHeader.BundleHeaderType.Description
                     let BundleTypeId = c.BundleDetails.First().BundleHeader.BundleHeaderTypeId
                     group c by new { c.FundId, c.QBSyncID, BundleTypeId, BundleType } into g
                     orderby g.Key.FundId, g.Key.QBSyncID, g.Key.BundleTypeId
                     select new FundTotalInfo
                                {
                                    BundleType = g.Key.BundleType,
                                    BundleTypeId = g.Key.BundleTypeId,
                                    FundId = g.Key.FundId,
                                    QBSynced = g.Key.QBSyncID ?? 0,
                                    FundName = g.First().ContributionFund.FundName,
                                    Total = g.Sum(t => t.ContributionAmount).Value,
                                    Count = g.Count(),
                                }).ToList();
            else
                q = (from c in api.FetchContributions()
                     group c by new { c.FundId, c.QBSyncID } into g
                     orderby g.Key.FundId, g.Key.QBSyncID
                     select new FundTotalInfo
                                {
                                    FundId = g.Key.FundId,
                                    QBSynced = g.Key.QBSyncID ?? 0,
                                    FundName = g.First().ContributionFund.FundName,
                                    Total = g.Sum(t => t.ContributionAmount).Value,
                                    Count = g.Count(),
                                }).ToList();

            FundTotal = new FundTotalInfo
                            {
                                Count = q.Sum(t => t.Count),
                                Total = q.Sum(t => t.Total),
                            };
            return q;
        }

        public GetTotalContributionsRange RangeTotal;

        public IEnumerable<GetTotalContributionsRange> TotalsByRange()
        {
            var list = (from r in DbUtil.Db.GetTotalContributionsRange(Dt1, Dt2, CampusId, NonTaxDeductible, IncUnclosedBundles)
                        orderby r.Range
                        select r).ToList();
            RangeTotal = new GetTotalContributionsRange
                             {
                                 Count = list.Sum(t => t.Count),
                                 Total = list.Sum(t => t.Total),
                             };
            return list;
        }

        public IEnumerable<SelectListItem> Campuses()
        {
            var list = (from c in DbUtil.Db.Campus
                        orderby c.Description
                        select new SelectListItem()
                                   {
                                       Value = c.Id.ToString(),
                                       Text = c.Description,
                                   }).ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }

        public class FundTotalInfo
        {
            public int FundId { get; set; }
            public int QBSynced { get; set; }
            public int OnLine { get; set; }
            public string BundleType { get; set; }
            public int? BundleTypeId { get; set; }
            public string FundName { get; set; }
            public decimal? Total { get; set; }
            public int? Count { get; set; }
        }
    }
}