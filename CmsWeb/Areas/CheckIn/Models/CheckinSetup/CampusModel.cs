using Newtonsoft.Json;

namespace CmsWeb.Models
{
    [JsonObject]
    public class CampusModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
