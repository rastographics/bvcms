using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
    public class Profile
    {
        public int id = 0;
        public string name = "Default";

        public int CampusId = 0;
        public int? EarlyCheckIn = 60;
        public int? LateCheckIn = 60;
        public bool Testing = false;
        public int? TestDay = null;
        public bool DisableJoin = false;
        public bool DisableTimer = false;
        public string BackgroundImageURL = null;
        public int CutoffAge = 18;
        public string Logout = "12345";
        public bool GuestLabels = false;
        public bool LocationLabels = false;
        public int SecurityType = 0;
        public int ShowCheckinConfirmation = 5;


        public void populate(CmsData.CheckinProfileSettings s)
        {
            id = s.CheckinProfiles.CheckinProfileId;
            name = s.CheckinProfiles.Name;

            CampusId = (s.CampusId == null) ? 0 : s.CampusId.Value;
            EarlyCheckIn = (s.EarlyCheckin == -1) ? 0 : s.EarlyCheckin;
            LateCheckIn = (s.LateCheckin == -1) ? 0 : s.LateCheckin;
            Testing = s.Testing;
            TestDay = s.TestDay;
            DisableJoin = s.DisableJoin;
            DisableTimer = s.DisableTimer;
            BackgroundImageURL = s.BackgroundImageURL;
            CutoffAge = s.CutoffAge;
            Logout = s.Logout;
            GuestLabels = s.Guest;
            LocationLabels = s.Location;
            SecurityType = s.SecurityType;
            ShowCheckinConfirmation = s.ShowCheckinConfirmation;
        }
    }
}
