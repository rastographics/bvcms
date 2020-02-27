using System.Collections.Generic;

namespace CmsWeb.Areas.Finance.Models
{
    public class RemoteDepositCaptureError
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
