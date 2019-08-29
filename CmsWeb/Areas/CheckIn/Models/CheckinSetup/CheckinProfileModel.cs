using Newtonsoft.Json;

namespace CmsWeb.Models
{
    [JsonObject]
    public class CheckinProfilesModel
    {
        public int CheckinProfileId { get; set; }
        public string Name { get; set; }
        public CheckinProfileSettingsModel CheckinProfileSettings { get; set; }
    }
}
