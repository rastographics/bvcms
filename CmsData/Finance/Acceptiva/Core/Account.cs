using Newtonsoft.Json;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class Account
    {
        [JsonProperty(PropertyName = "acct_type")]
        public string AcctType { get; set; }
        [JsonProperty(PropertyName = "acct_name")]
        public string AcctName { get; set; }
        [JsonProperty(PropertyName = "acct_data")]
        public string AcctData { get; set; }
        [JsonProperty(PropertyName = "acct_desc")]
        public string AcctDesc { get; set; }
        [JsonProperty(PropertyName = "payer_acct_active")]
        public string PayerAcctActive { get; set; }
    }
}
