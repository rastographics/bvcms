using CmsData;
using CmsData.Codes;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Models
{
    public class GLExportResult : ActionResult
    {
        public DateTime Dt1 { get; set; }
        public DateTime Dt2 { get; set; }
        public int fundid { get; set; }
        public int campusid { get; set; }
        public int Online { get; set; }
        public bool pledges { get; set; }
        public bool nontaxdeductible { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public string type { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment;filename=GLTRN2000.txt");

            var qIncomeFundNo67 =
                from c in DbUtil.Db.Contributions
                from d in c.BundleDetails
                where Dt1 <= c.PostingDate.Value.Date
                where c.PostingDate.Value.Date <= Dt2
                where d.BundleHeader.BundleStatusId == 0
                where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                where c.ContributionTypeId != ContributionTypeCode.Pledge
                group c by new { d.BundleHeaderId, c.ContributionFund, c.ContributionDate }
                into g
                select new ExtractInfo
                {
                    Fund = g.Key.ContributionFund.FundIncomeFund,
                    Month = ((g.Max(c => c.PostingDate.Value.Month) + 8) % 12) + 1,
                    HeaderId = g.Key.BundleHeaderId + "",
                    ContributionDate = g.Key.ContributionDate.Value,
                    FundName = g.Key.ContributionFund.FundName,
                    FundDept = g.Key.ContributionFund.FundIncomeDept, // Income
                    FundAcct = g.Key.ContributionFund.FundIncomeAccount, // Income
                    Amount = -g.Sum(c => c.ContributionAmount.Value),
                    PostingDate = g.Max(c => c.PostingDate.Value)
                };
            var qCashFundNo67 =
                from c in DbUtil.Db.Contributions
                from d in c.BundleDetails
                where Dt1 <= c.PostingDate.Value.Date
                where c.PostingDate.Value.Date <= Dt2
                where d.BundleHeader.BundleStatusId == 0
                where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId) // no 6,7(reversals, returns)
                where c.ContributionTypeId != ContributionTypeCode.Pledge
                group c by new { d.BundleHeaderId, c.ContributionFund, c.ContributionDate }
                into g
                select new ExtractInfo
                {
                    Fund = g.Key.ContributionFund.FundCashFund,
                    Month = ((g.Max(c => c.PostingDate.Value.Month) + 8) % 12) + 1,
                    HeaderId = g.Key.BundleHeaderId + "",
                    ContributionDate = g.Key.ContributionDate.Value,
                    FundName = g.Key.ContributionFund.FundName,
                    FundDept = g.Key.ContributionFund.FundCashDept, // Cash
                    FundAcct = g.Key.ContributionFund.FundCashAccount, // Cash
                    Amount = g.Sum(c => c.ContributionAmount.Value),
                    PostingDate = g.Max(c => c.PostingDate.Value)
                };
            var qIncomeFundYes67 =
                from c in DbUtil.Db.Contributions
                where Dt1 <= c.PostingDate.Value.Date
                where c.PostingDate.Value.Date <= Dt2
                where ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId) // Yes 6,7 (reversals, returns)
                where c.ContributionTypeId != ContributionTypeCode.Pledge
                select new ExtractInfo
                {
                    Fund = c.ContributionFund.FundIncomeFund,
                    Month = ((c.PostingDate.Value.Month + 8) % 12) + 1,
                    HeaderId = "0",
                    ContributionDate = c.ContributionDate.Value,
                    FundName = c.ContributionFund.FundName,
                    FundDept = c.ContributionFund.FundIncomeDept, // Income
                    FundAcct = c.ContributionFund.FundIncomeAccount, // Income
                    Amount = c.ContributionAmount.Value,
                    PostingDate = c.PostingDate.Value
                };
            var qCashFundYes67 =
                from c in DbUtil.Db.Contributions
                where Dt1 <= c.PostingDate.Value.Date
                where c.PostingDate.Value.Date <= Dt2
                where ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId) // Yes 6,7 (reversals, returns)
                where c.ContributionTypeId != ContributionTypeCode.Pledge
                select new ExtractInfo
                {
                    Fund = c.ContributionFund.FundCashFund,
                    Month = (c.PostingDate.Value.Month + 8) % 12 + 1,
                    HeaderId = "0",
                    ContributionDate = c.ContributionDate.Value,
                    FundName = c.ContributionFund.FundName,
                    FundDept = c.ContributionFund.FundCashDept, // Cash
                    FundAcct = c.ContributionFund.FundCashAccount, // Cash
                    Amount = -c.ContributionAmount.Value,
                    PostingDate = c.PostingDate.Value
                };
            var q = qIncomeFundNo67
                .Union(qCashFundNo67)
                .Union(qIncomeFundYes67)
                .Union(qCashFundYes67);
            q = from i in q
                orderby i.Fund, i.Month, i.HeaderId
                select i;

            var GLBundlePrefix = DbUtil.Db.Setting("GLBundlePrefix", "CM");
            foreach (var i in q)
            {
                Response.Write(
                    $"\"00000\",\"001{i.Fund}{i.Month:00}{GLBundlePrefix}{i.HeaderId.PadLeft(5, '0')}\",\"000\",\"{i.ContributionDate:MMddyy}\",\"{i.FundName}\",\"\",\"{i.FundDept}0000{i.FundAcct}\",\"{i.Amount * 100:00000000000}\",\"\"\r\n");
            }
            Response.Flush();
            Response.Close();
        }
    }
}
