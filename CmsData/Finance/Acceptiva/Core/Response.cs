using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Core
{
    internal  class Response
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "errors")]
        public List<Error> Errors { get; set; }
        [JsonProperty(PropertyName = "request_id_str")]
        public string RequestIdStr { get; set; }
    }
}
