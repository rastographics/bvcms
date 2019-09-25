using CmsData;
using CmsData.API;
using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ContributionsExcelResult
    {
        public DateTime Dt1 { get; set; }
        public DateTime Dt2 { get; set; }
        public int fundid { get; set; }
        public int campusid { get; set; }
        public int Online { get; set; }
        public string TaxDedNonTax { get; set; }
        public bool IncUnclosedBundles { get; set; }
        public bool IncludeBundleType { get; set; }
        public bool FilterByActiveTag { get; set; }
        public string FundSet { get; set; }

        private CMSDataContext _db;
        public CMSDataContext db
        {
            get => _db ?? (_db = DbUtil.Db);
            set => _db = value;
        }        

        public EpplusResult ToExcel(string type)
        {
            var _exportPeople = new ExportPeople();
            bool? nontaxdeductible = null; // both
            switch (TaxDedNonTax)
            {
                case "TaxDed":
                    nontaxdeductible = false;
                    break;
                case "NonTaxDed":
                    nontaxdeductible = true;
                    break;
            }

            var tagid = FilterByActiveTag ? db.TagCurrent()?.Id : (int?)null;
            var customFundIds = APIContributionSearchModel.GetCustomFundSetList(db, FundSet);
            var authorizedFundIds = db.ContributionFunds.ScopedByRoleMembership(db).Select(f => f.FundId).ToList();
            string fundIds = string.Empty;

            if (customFundIds?.Count > 0)
            {
                fundIds = authorizedFundIds.Where(f => customFundIds.Contains(f)).JoinInts(",");
            }
            else
            {
                fundIds = authorizedFundIds.JoinInts(",");
            }

            switch (type)
            {
                case "ledgerincome":
                    var cd = new CommandDefinition("dbo.LedgerIncomeExport", new
                    {
                        fd = Dt1,
                        td = Dt2,
                        campusid,
                        nontaxded = nontaxdeductible.HasValue ? nontaxdeductible.ToInt() : (int?)null,
                        includeunclosed = IncUnclosedBundles,
                        includeBundleType = IncludeBundleType,
                    }, commandType: CommandType.StoredProcedure);
                    return db.Connection.ExecuteReader(cd).ToExcel("LedgerIncome.xlsx");
                case "donorfundtotals":
                    return _exportPeople.ExcelDonorFundTotals(Dt1, Dt2, fundid, campusid, false, nontaxdeductible, IncUnclosedBundles, tagid, fundIds)                          
                        .ToExcel("DonorFundTotals.xlsx");
                case "donortotals":
                    return _exportPeople.ExcelDonorTotals(Dt1, Dt2, campusid, false, nontaxdeductible, IncUnclosedBundles, tagid, fundIds)
                        .ToExcel("DonorTotals.xlsx");
                case "donordetails":
                    return _exportPeople.DonorDetails(Dt1, Dt2, fundid, campusid, false, nontaxdeductible, IncUnclosedBundles, tagid, fundIds)
                        .ToExcel("DonorDetails.xlsx");
            }
            return null;
        }
    }
}
