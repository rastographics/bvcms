using Newtonsoft.Json;

namespace CmsData.Finance.Acceptiva.Transaction
{
    internal class TransItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "desc")]
        public string  Desc{ get; set; }
        [JsonProperty(PropertyName = "amt")]
        public double Amt { get; set; }
    }
}
