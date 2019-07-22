using CmsData;
using System.Collections.Generic;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingModel
    {
        public SettingModel()
        {
            Settings = new List<Setting>();
            SettingTypes = new List<SettingType>();
        }

        public IEnumerable<Setting> Settings { get; set; }

        public List<SettingType> SettingTypes { get; set; }

    }
}
