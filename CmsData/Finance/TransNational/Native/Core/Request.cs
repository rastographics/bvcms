using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace CmsData.Finance.TransNational.Native.Core
{
    internal abstract class Request
    {

        public string Url { get; private set; }

        public NameValueCollection Data { get; protected set; }

        protected Request(string url, string userName, string password)
        {
            Url = url;
            Data = new NameValueCollection
            {
                {"username", userName},
                {"password", password}
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
