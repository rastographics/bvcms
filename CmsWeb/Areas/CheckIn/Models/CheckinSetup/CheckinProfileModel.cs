using CmsData;
using Newtonsoft.Json;

namespace CmsWeb.Models
{
    [JsonObject]
    public class CheckinProfilesModel
    {
        public int CheckinProfileId { get; set; }
        public string Name { get; set; }
        public CheckinProfileSettingsModel CheckinProfileSettings { get; set; }

        public static CheckinProfile CreateDefault(CMSDataContext CurrentDatabase)
        {
            CheckinProfile profile = new CheckinProfile
            {
                Name = "Default"
            };
            CurrentDatabase.CheckinProfiles.InsertOnSubmit(profile);
            CurrentDatabase.SubmitChanges();

            CheckinProfileSetting settings = new CheckinProfileSetting
            {
                CheckinProfileId = profile.CheckinProfileId,
                AdminPIN = "54321",
                CampusId = null,
                Testing = false,
                TestDay = null,
                DisableJoin = false,
                DisableTimer = false,
                BackgroundImageURL = null,
                CutoffAge = 13,
                Logout = "12345",
                Guest = true,
                Location = true,
                SecurityType = 3,
                ShowCheckinConfirmation = 5
            };

            CurrentDatabase.CheckinProfileSettings.InsertOnSubmit(settings);
            CurrentDatabase.SubmitChanges();
            return profile;
        }
    }
}
