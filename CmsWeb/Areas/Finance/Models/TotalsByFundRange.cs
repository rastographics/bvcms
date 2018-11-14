using CmsData;
using CmsData.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class TotalsByFundRangeModel
    {
        public bool Pledged { get; set; }
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }
        public int CampusId { get; set; }
        public int FundId { get; set; }
        public string FundSet { get; set; }

        public RangeInfo RangeTotal { get; set; }

        public TotalsByFundRangeModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
            {
                first = first.AddMonths(-1);
            }

            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public IEnumerable<RangeInfo> GetTotalsByFundRange()
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

            var list = (from r in DbUtil.Db.GetContributionsRange(Dt1, Dt2, CampusId, false, true, Pledged, FundId, fundIds)
                        orderby r.Range
                        select r).ToList();
            RangeTotal = new RangeInfo
            {
                Total = list.Sum(vv => vv.Total ?? 0),
                Count = list.Sum(vv => vv.Count ?? 0),
                DonorCount = list.Sum(vv => vv.DonorCount ?? 0),
            };
            var list2 = from r in list
                        select new RangeInfo
                        {
                            RangeId = r.Range,
                            Total = r.Total ?? 0,
                            Count = r.Count ?? 0,
                            DonorCount = r.DonorCount ?? 0,
                            PctCount = (r.Count ?? 0) / Convert.ToDecimal(RangeTotal.Count) * 100,
                            PctTotal = (r.Total ?? 0) / RangeTotal.Total * 100,
                        };
            return list2;
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

        public IEnumerable<SelectListItem> Funds()
        {
            var list = (from c in DbUtil.Db.ContributionFunds.ScopedByRoleMembership()
                        where c.FundStatusId == 1
                        orderby c.FundName
                        select new SelectListItem()
                        {
                            Value = c.FundId.ToString(),
                            Text = c.FundName,
                        }).ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
    }
}
