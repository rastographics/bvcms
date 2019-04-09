using Newtonsoft.Json;

namespace CmsData.Finance.Acceptiva.Core
{
    public class AcceptivaResponse<T>
    {
        [JsonProperty(PropertyName = "response")]
        public T Response { get; set; }
    }
}
