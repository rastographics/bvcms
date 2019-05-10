using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace TransactionGateway.Entities
{
    /// <summary>
    ///     Object containing information about an API request. Also used for message broadcasting
    /// </summary>
    public class RequestDetails
    {
        public string BaseUrl = "";
        public string Method = "GET";
        public string RelativeUrl = "";
        public string SerializedContent = "";
        private Dictionary<string, object> Params { get; set; }

        /// <summary>
        ///     Concatenates our base and relative URLs
        /// </summary>
        public string FullUrl
        {
            get
            {
                string s = BaseUrl + RelativeUrl;
                if (!string.IsNullOrWhiteSpace(QueryString)) s += "?" + QueryString;
                return s;
            }
        }

        /// <summary>
        ///     Values required for our query string
        /// </summary>
        public Dictionary<string, object> QueryStringValues
        {
            get
            {
                if (Method == "GET" || Method == "DELETE") return Params;
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        ///     Formats our parameters into a query string appropriate for a URL
        /// </summary>
        public string QueryString
        {
            get
            {
                
                NameValueCollection queryParameters = HttpUtility.ParseQueryString("");
                QueryStringValues.Where(x => x.Value != null && !string.IsNullOrWhiteSpace(x.Value.ToString())).ToList().ForEach(kvp => queryParameters[kvp.Key] = kvp.Value.ToString());
                return queryParameters.ToString();
            }
        }

        /// <summary>
        ///     Values required to be encoded into our form request body
        /// </summary>
        public Dictionary<string, object> FormValues
        {
            get
            {
                if (Method == "POST") return Params;
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        ///     Clears our parameter list
        /// </summary>
        public void ClearParameters()
        {
            Params = new Dictionary<string, object>();
        }

        /// <summary>
        ///     Allows our users to set the private parameter source
        /// </summary>
        /// <param name="value"></param>
        /// <param name="obj"></param>
        public void SetParameter(string value, object obj)
        {
            Params[value] = obj;
        }
    }
}
