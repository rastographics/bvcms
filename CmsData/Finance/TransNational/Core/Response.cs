using System.Collections.Specialized;
using System.Web;

namespace CmsData.Finance.TransNational.Core
{
    internal class Response
    {
        protected NameValueCollection Data { get; private set; }

        public ResponseStatus ResponseStatus { get; private set; }

        public string ResponseText { get; private set; }

        public string ResponseCode { get; private set; }

        protected Response(string response)
        {
            Data = HttpUtility.ParseQueryString(response);
            ResponseStatus = (ResponseStatus)int.Parse(Data["response"]);
            ResponseText = Data["responsetext"];
            ResponseCode = Data["response_code"];
        }
    }
}
