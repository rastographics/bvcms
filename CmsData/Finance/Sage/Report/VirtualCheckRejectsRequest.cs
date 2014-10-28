using System;

namespace CmsData.Finance.Sage.Report
{
    internal class VirtualCheckRejectsRequest : ReportRequest
    {
        public VirtualCheckRejectsRequest(string id, string key, DateTime rejectDate)
            : base(id, key, "VIEW_VIRTUAL_CHECK_REJECTS")
        {
            Data["REJECT_DATE"] = rejectDate.ToShortDateString();
        }

        public VirtualCheckRejectsRequest(string id, string key, DateTime startDate, DateTime endDate)
            : base(id, key, "VIEW_VIRTUAL_CHECK_REJECTS")
        {
            Data["START_DATE"] = startDate.ToShortDateString();
            Data["END_DATE"] = endDate.ToShortDateString();
        }

        public new VirtualCheckRejectsResponse Execute()
        {
            var response = base.Execute();
            return new VirtualCheckRejectsResponse(response);
        }
    }
}
