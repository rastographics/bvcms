using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.CheckInAPI
{
    public class CheckInInformation
    {
        public List<CheckInSettingsEntry> settings;
        public List<CheckInCampus> campuses;
        public List<CheckInLabelFormat> labels;

        public CheckInInformation(List<CheckInSettingsEntry> settings, List<CheckInCampus> campuses, List<CheckInLabelFormat> labels)
        {
            this.settings = settings;
            this.campuses = campuses;
            this.labels = labels;
        }
    }
}