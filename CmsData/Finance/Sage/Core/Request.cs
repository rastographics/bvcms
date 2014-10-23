using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace CmsData.Finance.Sage.Core
{
    internal abstract class Request
    {

        public string BaseAddress { get; private set; }

        public string Operation { get; private set; }

        public NameValueCollection Data { get; protected set; }

        protected Request(string baseAddress, string operation, string id, string key)
        {
            BaseAddress = baseAddress;
            Operation = operation;
            Data = new NameValueCollection
            {
                {"M_ID", id},
                {"M_KEY", key}
            };
        }

        public string Execute()
        {
            using (var client = new WebClient())
            {
                client.BaseAddress = BaseAddress;
                var result = client.UploadValues(Operation, "POST", Data);
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}
