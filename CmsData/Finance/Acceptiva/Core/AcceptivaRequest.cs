using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class AcceptivaRequest
    {
        private const string URL = "https://sandbox.acceptivapro.com/api/api_request.php";

        public NameValueCollection Data { get; protected set; }

        protected AcceptivaRequest(string apiKey, string action)
        {
            Data = new NameValueCollection
            {
                {"api_key", apiKey},
                {"action", action}
            };
        }

        public string Execute()
        {
            using (var client = new WebClient())
            {
                var result = client.UploadValues(URL, Data);
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}
