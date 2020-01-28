using Newtonsoft.Json;

namespace CmsData.Finance.Acceptiva.Core
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AcceptivaResponse<T>
    {
        [JsonProperty(PropertyName = "response")]
        public T Response { get; set; }
    }
}
