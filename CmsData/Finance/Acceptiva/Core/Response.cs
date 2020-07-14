using CmsData.Finance.Acceptiva.Transaction;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Core
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal  class Response
    {
        [JsonProperty(PropertyName = "trans_id_str")]
        public string TransIdStr { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "processor_response_code")]
        public string ProcessorResponseCode { get; set; }
        [JsonProperty(PropertyName = "errors")]
        public List<Error> Errors { get; set; }
        [JsonProperty(PropertyName = "request_id_str")]
        public string RequestIdStr { get; set; }
        [JsonProperty(PropertyName = "trans_status_msg")]
        public string TransStatusMsg { get; set; }
        [JsonProperty(PropertyName = "items")]
        public List<TransItem> Items { get; set; }        
    }
}
