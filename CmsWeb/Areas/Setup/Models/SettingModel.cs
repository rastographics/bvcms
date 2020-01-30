using CmsData;
using System.Collections.Generic;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingModel
    {
        public SettingModel()
        {
            GeneralSettings = new List<Setting>();
            SettingTypes = new List<SettingTypeModel>();
        }

        public IEnumerable<Setting> GeneralSettings { get; set; }

        public List<SettingTypeModel> SettingTypes { get; set; }
    }

    public enum SettingDataType
    {
        Boolean = 1,
        Date = 2,
        Text = 3,
        Obscured = 4,
        Int = 5
    }
}
