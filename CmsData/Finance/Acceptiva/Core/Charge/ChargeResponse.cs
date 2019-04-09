using Newtonsoft.Json;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class ChargeResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "request_id_str")]
        public string RequestIdStr { get; set; }
        [JsonProperty(PropertyName = "trans_id_str")]
        public string TransIdStr { get; set; }
        [JsonProperty(PropertyName = "trans_recur_id_str")]
        public string TransRecurIdStr { get; set; }
        [JsonProperty(PropertyName = "trans_status")]
        public string TransStatus { get; set; }
        [JsonProperty(PropertyName = "trans_status_msg")]
        public string TransStatusMsg { get; set; }
        [JsonProperty(PropertyName = "processor_response_code")]
        public string ProcessorResponseCode { get; set; }
        [JsonProperty(PropertyName = "processor_response_msg")]
        public string ProcessorResponseMsg { get; set; }
        [JsonProperty(PropertyName = "processor_response_msg_public")]
        public string ProcessorResponseMsgPublic { get; set; }
        [JsonProperty(PropertyName = "acct_last_four")]
        public string AcctLastFour { get; set; }
        [JsonProperty(PropertyName = "amt_processed")]
        public double AmtProcessed { get; set; }
        [JsonProperty(PropertyName = "payer_id_str")]
        public string PayerIdStr { get; set; }
    }
}
