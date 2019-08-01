using Newtonsoft.Json;

namespace CmsWeb.Models
{
    [JsonObject]
    public class CheckinProfileSettingsModel
    {
        public int CheckinProfileId { get; set; }
        public int? CampusId { get; set; }
        public int? EarlyCheckin { get; set; }
        public int? LateCheckin { get; set; }
        public bool Testing { get; set; }
        public int TestDay { get; set; }
        public string AdminPIN { get; set; }
        public int? PINTimeout { get; set; }
        public bool DisableJoin { get; set; }
        public bool DisableTimer { get; set; }
        public int? BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }
        public string BackgroundImageURL { get; set; }
        public int CutoffAge { get; set; }
        public string Logout { get; set; }
        public bool Guest { get; set; }
        public bool Location { get; set; }
        public int SecurityType { get; set; }
        public int ShowCheckinConfirmation { get; set; }
    }
}
