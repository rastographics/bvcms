using System;

namespace CmsData.Finance.Sage.Report
{
    internal class SettledBatchSummaryRequest : ReportRequest
    {
        public SettledBatchSummaryRequest(string id, string key, DateTime startDate, DateTime endDate, bool includeCreditCards, bool includeEChecks)
            : base(id, key, "VIEW_SETTLED_BATCH_SUMMARY")
        {
            Data["START_DATE"] = startDate.ToShortDateString();
            Data["END_DATE"] = endDate.ToShortDateString();
            Data["INCLUDE_BANKCARD"] = includeCreditCards.ToString();
            Data["INCLUDE_VIRTUAL_CHECK"] = includeEChecks.ToString();
        }

        public new SettledBatchSummaryResponse Execute()
        {
            var response = base.Execute();
            return new SettledBatchSummaryResponse(response);
        }
    }
}
