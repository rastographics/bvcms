using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Report
{
    internal class ReportRequest : Request
    {
        private const string BASE_ADDRESS = "https://gateway.sagepayments.net/web_services/vterm_extensions/reporting.asmx/";

        public ReportRequest(string id, string key, string operation)
            : base(BASE_ADDRESS, operation, id, key) {}
    }
}
