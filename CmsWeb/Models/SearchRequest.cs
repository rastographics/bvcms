using Newtonsoft.Json;

namespace CmsWeb.Models
{
    public class SearchRequest
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("context")]
        public string Context { get; set; }
    }
}
