using Newtonsoft.Json;

namespace CmsWeb.Models
{
    [JsonObject]
    public class CheckinProfiles
    {
        public int CheckinProfileId { get; set; }
        public string Name { get; set; }
        public CheckinProfileSettingsModel Settings { get; set; }
    }
}
