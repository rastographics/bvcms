using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using Dapper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
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
        public string TaxDedNonTax { get; set; }
        public string FundSet { get; set; }
        public int Online { get; set; }
        public bool Pledges { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public bool IncludeBundleType { get; set; }
        public bool NonTaxDeductible { get; set; }
        public bool FilterByActiveTag { get; set; }
        public EpplusResult epr;

        public TotalsByFundModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
            {
                first = first.AddMonths(-1);
            }

            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
            Online = 2;
        }

        public FundTotalInfo FundTotal;

        private class ContributionIdItem
        {
            public int ContributionId { get; set; }
        }

        public void SaveAsExcel()
        {
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

            var x = api.FetchContributions();
            var list = x.Select(xx => new ContributionIdItem { ContributionId = xx.ContributionId }).ToList();
            var dt = ExcelExportModel.ToDataTable(list);
            dt.SaveAs("D:\\cids.xlsx");
        }

        public IEnumerable<string> CustomReports()
        {
            // if a user is a member of the fundmanager role, we do not want to enable custom reports as this could bypass the fund restrictions at present
            var fundmanagerRoleName = "FundManager";
            var currentUserIsFundManager = DbUtil.Db.CurrentUser.Roles.Contains(fundmanagerRoleName, StringComparer.OrdinalIgnoreCase);

            if (currentUserIsFundManager)
            {
                return new string[] { };
            }

            var q = from c in DbUtil.Db.Contents
                    where c.TypeID == ContentTypeCode.TypeSqlScript
                    where c.Body.Contains("--class=TotalsByFund")
                    select c.Name;

            return q;
        }

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
                    FundSet = FundSet,
                }
            };
#if DEBUG2
            // for reconciliation by developer
            var v =  from c in api.FetchContributions()
                     orderby c.ContributionId
                     select c.ContributionId;
            using(var tw = new StreamWriter("D:\\totalsbyfund.txt"))
               foreach (var s in v)
                  tw.WriteLine(s);
#endif


            if (IncludeBundleType)
            {
                q = (from c in api.FetchContributions()
                     let BundleType = c.BundleDetails.First().BundleHeader.BundleHeaderType.Description
                     let BundleTypeId = c.BundleDetails.First().BundleHeader.BundleHeaderTypeId
                     group c by new { c.FundId, BundleTypeId, BundleType }
                     into g
                     orderby g.Key.FundId, g.Key.BundleTypeId
                     select new FundTotalInfo
                     {
                         BundleType = g.Key.BundleType,
                         BundleTypeId = g.Key.BundleTypeId,
                         FundId = g.Key.FundId,
                         FundName = g.First().ContributionFund.FundName,
                         GeneralLedgerId = g.First().ContributionFund.FundIncomeAccount,
                         Total = g.Sum(t => t.ContributionAmount).Value,
                         Count = g.Count(),
                         model = this
                     }).ToList();
            }
            else
            {
                q = (from c in api.FetchContributions()
                     group c by c.FundId into g
                     orderby g.Key
                     select new FundTotalInfo
                     {
                         FundId = g.Key,
                         FundName = g.First().ContributionFund.FundName,
                         GeneralLedgerId = g.First().ContributionFund.FundIncomeAccount,
                         Total = g.Sum(t => t.ContributionAmount).Value,
                         Count = g.Count(),
                         model = this
                     }).ToList();
            }

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
            var customFundIds = APIContributionSearchModel.GetCustomFundSetList(DbUtil.Db, FundSet);
            var authorizedFundIds = DbUtil.Db.ContributionFunds.ScopedByRoleMembership().Select(f => f.FundId).ToList();

            string fundIds = string.Empty;

            if (customFundIds?.Count > 0)
            {
                fundIds = authorizedFundIds.Where(f => customFundIds.Contains(f)).JoinInts(",");
            }
            else
            {
                fundIds = authorizedFundIds.JoinInts(",");
            }

            var list = (from r in DbUtil.Db.GetTotalContributionsRange(Dt1, Dt2, CampusId, NonTaxDeductible ? (bool?)true : false, IncUnclosedBundles, fundIds)
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
            {
                Append(sb, "fundid=" + fundid);
            }

            if (bundletypeid.HasValue)
            {
                Append(sb, "bundletype=" + bundletypeid);
            }

            if (Dt1.HasValue)
            {
                Append(sb, "dt1=" + Dt1.ToSortableDate());
            }

            if (Dt2.HasValue)
            {
                Append(sb, "dt2=" + Dt2.ToSortableDate());
            }

            if (!IncUnclosedBundles)
            {
                Append(sb, "includeunclosedbundles=false");
            }

            if (TaxDedNonTax != "TaxDed")
            {
                Append(sb, "taxnontax=" + TaxDedNonTax);
            }

            if (CampusId > 0)
            {
                Append(sb, "campus=" + CampusId);
            }

            if (Online < 2)
            {
                Append(sb, "online=" + Online);
            }

            return sb.ToString();
        }

        private void Append(StringBuilder sb, string val)
        {
            sb.Append(connector);
            sb.Append(val);
            connector = "&";
        }

        public DynamicParameters GetDynamicParameters()
        {
            var p = new DynamicParameters();
            p.Add("@StartDate", Dt1);
            p.Add("@EndDate", Dt2);
            p.Add("@CampusId", CampusId);
            p.Add("@Online", Online);
            p.Add("@TaxNonTax", TaxDedNonTax);
            p.Add("@IncludeUnclosedBundles", IncUnclosedBundles);
            var fundset = APIContributionSearchModel.GetCustomFundSetList(DbUtil.Db, FundSet).JoinInts(",");
            p.Add("@FundSet", fundset);

            if (FilterByActiveTag)
            {
                var tagid = DbUtil.Db.TagCurrent().Id;
                p.Add("@ActiveTagFilter", tagid);
            }
            else
            {
                p.Add("@ActiveTagFilter");
            }

            return p;
        }
    }
}
