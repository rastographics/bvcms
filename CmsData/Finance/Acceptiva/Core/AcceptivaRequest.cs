using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class AcceptivaRequest
    {
        public string Url { get; private set; }

        public NameValueCollection Data { get; protected set; }

        protected AcceptivaRequest(string url, string apiKey, string action)
        {
            Url = url;
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
                var result = client.UploadValues(Url, Data);
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}
