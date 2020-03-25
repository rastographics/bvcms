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
    public class ContributionsExcelResult : IDbBinder
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
        public bool IncludePledges { get; set; }

        private CMSDataContext _currentDatabase;
        public CMSDataContext CurrentDatabase
        {
            get => _currentDatabase;
            set
            {
                _currentDatabase = value;
            }
        }
        public ContributionsExcelResult() { }
        public ContributionsExcelResult(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        public EpplusResult ToExcel(string type)
        {
            var _exportPeople = new ExportPeople(CurrentDatabase);
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

            var tagid = FilterByActiveTag ? CurrentDatabase.TagCurrent()?.Id : (int?)null;
            var customFundIds = APIContributionSearchModel.GetCustomFundSetList(CurrentDatabase, FundSet);
            var authorizedFundIds = CurrentDatabase.ContributionFunds.ScopedByRoleMembership(CurrentDatabase).Select(f => f.FundId).ToList();
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
                    return CurrentDatabase.Connection.ExecuteReader(cd).ToExcel("LedgerIncome.xlsx");
                case "donorfundtotals":
                    return _exportPeople.ExcelDonorFundTotals(Dt1, Dt2, fundid, campusid, IncludePledges, nontaxdeductible, IncUnclosedBundles, tagid, fundIds, Online)                          
                        .ToExcel("DonorFundTotals.xlsx");
                case "donortotals":
                    return _exportPeople.ExcelDonorTotals(Dt1, Dt2, campusid, IncludePledges, nontaxdeductible, Online, IncUnclosedBundles, tagid, fundIds)
                        .ToExcel("DonorTotals.xlsx");
                case "donordetails":
                    return _exportPeople.DonorDetails(Dt1, Dt2, fundid, campusid, IncludePledges, nontaxdeductible, IncUnclosedBundles, tagid, fundIds, Online)
                        .ToExcel("DonorDetails.xlsx");
            }
            return null;
        }
    }
}
