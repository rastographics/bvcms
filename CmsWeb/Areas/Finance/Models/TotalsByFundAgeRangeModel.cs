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
    public class TotalsByFundAgeRangeModel
    {
        public DateTime? Dt1 { get; set; }

        public DateTime? Dt2 { get; set; }

        public int CampusId { get; set; }

        public RangeInfo RangeTotal { get; set; }

        public TotalsByFundAgeRangeModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
                first = first.AddMonths(-1);
            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public IEnumerable<AgeRangeInfo> GetTotalsByFundAgeRange()
        {
            var model = new BundleModel();
            var ageRangeInfos = model.TotalsByFundAgeRange(0, Dt1.GetValueOrDefault(), Dt2.GetValueOrDefault(), string.Empty, CampusId);
            RangeTotal = model.RangeTotal;
            return ageRangeInfos;
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
        
    }
}