using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Charge
{
    internal class ChargeResponse : Response
    {
        [JsonProperty(PropertyName = "trans_recur_id_str")]
        public string TransRecurIdStr { get; set; }
        [JsonProperty(PropertyName = "trans_status")]
        public string TransStatus { get; set; }
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
