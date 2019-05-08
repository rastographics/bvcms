using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Store
{
    internal class PayerDataResponse : Response
    {
        [JsonProperty(PropertyName = "payer_id_str")]
        public string PayerIdStr { get; set; }
        [JsonProperty(PropertyName = "client_payer_id")]
        public string ClientPayerId { get; set; }
        [JsonProperty(PropertyName = "payer_prefix")]
        public string PayerPrefix { get; set; }
        [JsonProperty(PropertyName = "payer_fname")]
        public string PayerFName { get; set; }
        [JsonProperty(PropertyName = "payer_lname")]
        public string PayerLName { get; set; }
        [JsonProperty(PropertyName = "payer_suffix")]
        public string PayeSuffix { get; set; }
        [JsonProperty(PropertyName = "payer_address")]
        public string PayerAddress { get; set; }
        [JsonProperty(PropertyName = "payer_address2")]
        public string PayerAddress2 { get; set; }
        [JsonProperty(PropertyName = "payer_city")]
        public string PayerCity { get; set; }
        [JsonProperty(PropertyName = "payer_state")]
        public string PayerState { get; set; }
        [JsonProperty(PropertyName = "payer_zip")]
        public string PayerZip { get; set; }
        [JsonProperty(PropertyName = "payer_country")]
        public string PayerCountry { get; set; }
        [JsonProperty(PropertyName = "payer_phone")]
        public string PayerPhone { get; set; }
        [JsonProperty(PropertyName = "payer_email")]
        public string PayerEmail { get; set; }
        [JsonProperty(PropertyName = "accts")]
        public List<Account> Accts { get; set; }
    }
}
