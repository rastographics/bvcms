using Newtonsoft.Json;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class Error
    {
        [JsonProperty(PropertyName = "err_no")]
        public int ErrorNo { get; set; }
        [JsonProperty(PropertyName = "err_msg")]
        public string ErrorMsg { get; set; }
    }
}
