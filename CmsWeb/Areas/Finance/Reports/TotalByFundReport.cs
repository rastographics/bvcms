using System;
using System.Data;
using TouchpointSoftware.Cms.Reporting;

namespace CmsWeb.Areas.Finance.Reports
{
    public class TotalByFundReportQuery : ReportQuery<TotalByFundReportParameters>
    {
        public TotalByFundReportQuery(TotalByFundReportParameters parameters) : base(parameters)
        {
            CommandText = "reports.TotalByFund";
            CommandType = System.Data.CommandType.StoredProcedure;
            CommandTimeout = 60;
        }
    }


    public class TotalByFundReportParameters : ReportParameterModel
    {
        [SqlParameter(Name = "StartDate", Direction = ParameterDirection.Input, Type = DbType.AnsiString)]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
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
    }

    public class TotalByFundReportLineItem
    {

    }
}
