using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace CmsData.Finance.TransNational.Core
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
//#if DEBUG
//                var values = Data.Cast<string>().Select(e => $"{e}: {Data[e]}"); 
//                var str = string.Join("\n", values); 
//                Debug.WriteLine(str);
//#endif
                var result = client.UploadValues(Url, Data);
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}
