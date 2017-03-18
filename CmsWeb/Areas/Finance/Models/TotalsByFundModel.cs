using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text;

namespace CmsWeb.Models
{
    public class TotalsByFundModel
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }
        public int CampusId { get; set; }
        public string Sort { get; set; }
        public string Dir { get; set; }
        public string TaxDedNonTax { get; set; }
        public int Online { get; set; }
        public bool Pledges { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public bool IncludeBundleType { get; set; }
        public bool NonTaxDeductible { get; set; }
        public bool FilterByActiveTag { get; set; }

        public TotalsByFundModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
                first = first.AddMonths(-1);
            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
            Online = 2;
        }

        public FundTotalInfo FundTotal;

        public IEnumerable<FundTotalInfo> TotalsByFund()
        {
            List<FundTotalInfo> q = null;

            var api = new APIContributionSearchModel(DbUtil.Db)
            {
                model =
                {
                    StartDate = Dt1,
                    EndDate = Dt2,
                    IncludeUnclosedBundles = IncUnclosedBundles,
                    TaxNonTax = TaxDedNonTax,
                    CampusId = CampusId,
                    Status = ContributionStatusCode.Recorded,
                    Online = Online,
                    FilterByActiveTag = FilterByActiveTag,
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
                         GeneralLedgerId = g.First().ContributionFund.FundIncomeAccount,
                         Total = g.Sum(t => t.ContributionAmount).Value,
                         Count = g.Count(),
                         model = this
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
                         GeneralLedgerId = g.First().ContributionFund.FundIncomeAccount,
                         Total = g.Sum(t => t.ContributionAmount).Value,
                         Count = g.Count(),
                         model = this
                     }).ToList();

            FundTotal = new FundTotalInfo
            {
                Count = q.Sum(t => t.Count),
                Total = q.Sum(t => t.Total),
                model = this
            };
            return q;
        }

        public GetTotalContributionsRange RangeTotal;

        public IEnumerable<GetTotalContributionsRange> TotalsByRange()
        {
            var list = (from r in DbUtil.Db.GetTotalContributionsRange(Dt1, Dt2, CampusId, NonTaxDeductible ? (bool?)null : false, IncUnclosedBundles)
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
            var qc = DbUtil.Db.Campus.AsQueryable();
            qc = DbUtil.Db.Setting("SortCampusByCode")
                ? qc.OrderBy(cc => cc.Code)
                : qc.OrderBy(cc => cc.Description);
            var list = (from c in qc
                        select new SelectListItem()
                        {
                            Value = c.Id.ToString(),
                            Text = c.Description,
                        }).ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public SelectList TaxTypes()
        {
            return new SelectList(
                new List<CodeValueItem>
                {
                    new CodeValueItem { Code = "TaxDed", Value = "Tax Deductible" },
                    new CodeValueItem { Code = "NonTaxDed", Value = "Non-Tax Deductible" },
                    new CodeValueItem { Code = "Both", Value = "Both" },
                },
                "Code", "Value", TaxDedNonTax
            );
        }
        public SelectList OnlineOptions()
        {
            return new SelectList(
                new List<CodeValueItem>
                {
                    new CodeValueItem { Id = 2, Value = "Both" },
                    new CodeValueItem { Id = 1, Value = "Online" },
                    new CodeValueItem { Id = 0, Value = "Not Online" },
                },
                "Id", "Value", Online
            );
        }

        public string BundleTotalsUrl(int? fundid = null, int? bundletypeid = null)
        {
            return BuildUrl("/BundleTotals", fundid, bundletypeid);
        }

        public string ContributionsUrl(int? fundid = null, int? bundletypeid = null)
        {
            return BuildUrl("/Contributions", fundid, bundletypeid);
        }
        private string connector;
        private string BuildUrl(string baseurl, int? fundid, int? bundletypeid)
        {
            connector = "?";
            var sb = new StringBuilder(baseurl);

            if (fundid.HasValue)
                Append(sb, "fundid=" + fundid);
            if (bundletypeid.HasValue)
                Append(sb, "bundletype=" + bundletypeid);

            if (Dt1.HasValue)
                Append(sb, "dt1=" + Dt1.ToSortableDate());
            if (Dt2.HasValue)
                Append(sb, "dt2=" + Dt2.ToSortableDate());
            if (!IncUnclosedBundles)
                Append(sb, "includeunclosedbundles=false");
            if (TaxDedNonTax != "TaxDed")
                Append(sb, "taxnontax=" + TaxDedNonTax);
            if (CampusId > 0)
                Append(sb, "campus=" + CampusId);
            if (Online < 2)
                Append(sb, "online=" + Online);

            return sb.ToString();
        }
        private void Append(StringBuilder sb, string val)
        {
            sb.Append(connector);
            sb.Append(val);
            connector = "&";
        }
    }
}