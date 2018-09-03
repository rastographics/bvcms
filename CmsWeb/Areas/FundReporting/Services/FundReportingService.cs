using CmsWeb.Areas.FundReporting.Models;

namespace CmsWeb.Areas.FundReporting.Services
{
    public class FundReportingService
    {
        public void TotalByFundReport()
        {

        }
    }

    public abstract class Report
    {

    }

    public class TotalByFundReport : Report
    {
        public void Execute() { };
    }
}



namespace Test
{
    public class ReportParameters { }
    public class ReportResult { }
    public class ReportDefinition { }

    public class ReportRunner
    {
        public ReportResult Execute(ReportParameters reportParameters)
        {
            return null;
        }
    }

    public class Report
    {

    }

    public class TotalByFundReport : Report
    {

    }
}
