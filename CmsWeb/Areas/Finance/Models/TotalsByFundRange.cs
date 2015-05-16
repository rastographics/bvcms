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
    public class TotalsByFundRangeModel
    {
        public string Pledged { get; set; }

        public DateTime? Dt1 { get; set; }

        public DateTime? Dt2 { get; set; }

        public int CampusId { get; set; }

        public int FundId { get; set; }

        public RangeInfo RangeTotal { get; set; }

        public TotalsByFundRangeModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
                first = first.AddMonths(-1);
            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public IEnumerable<RangeInfo> GetTotalsByFundRange()
        {
            var model = new BundleModel();
            var rangeInfos = model.TotalsByFundRange(FundId, Dt1.GetValueOrDefault(), Dt2.GetValueOrDefault(), string.IsNullOrWhiteSpace(Pledged) ? "false" : Pledged, CampusId);
            RangeTotal = model.RangeTotal;
            return rangeInfos;
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

        public IEnumerable<SelectListItem> Funds()
        {
            var list = (from c in DbUtil.Db.ContributionFunds
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