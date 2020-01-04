using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
    public class Profile
    {
        public int id = 0;
        public string name = "Default";

        public int CampusId = 0;
        public int EarlyCheckin = 60;
        public int LateCheckin = 60;
        public bool Testing = false;
        public int? TestDay = null;
        public bool DisableJoin = false;
        public bool DisableTimer = false;
        public string BackgroundImageURL = null;
        public int CutoffAge = 18;
        public string AdminPIN = "54321";
        public string Logout = "12345";
        public bool GuestLabels = false;
        public bool LocationLabels = false;
        public int SecurityType = 0;
        public int ShowCheckinConfirmation = 5;

        public Profile(CMSDataContext CurrentDatabase)
        {
            EarlyCheckin = int.TryParse(CurrentDatabase.GetSetting("EarlyCheckin", "60"), out EarlyCheckin) ? EarlyCheckin : 60;
            LateCheckin = int.TryParse(CurrentDatabase.GetSetting("LateCheckin", "60"), out LateCheckin) ? LateCheckin : 60;
        }

        public void populate(CmsData.CheckinProfileSetting s)
        {
            id = s.CheckinProfile.CheckinProfileId;
            name = s.CheckinProfile.Name;

            CampusId = (s.CampusId == null) ? 0 : s.CampusId.Value;
            Testing = s.Testing;
            TestDay = s.TestDay;
            DisableJoin = s.DisableJoin;
            DisableTimer = s.DisableTimer;
            BackgroundImageURL = s.BackgroundImageURL;
            CutoffAge = s.CutoffAge;
            AdminPIN = s.AdminPIN;
            Logout = s.Logout;
            GuestLabels = s.Guest;
            LocationLabels = s.Location;
            SecurityType = s.SecurityType;
            ShowCheckinConfirmation = s.ShowCheckinConfirmation;
        }
    }
}
