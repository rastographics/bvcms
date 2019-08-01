using CmsData;
using System.Collections.Generic;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingModel
    {
        public SettingModel()
        {
            Settings = new List<Setting>();
            SettingTypes = new List<SettingTypeModel>();
        }

        public IEnumerable<Setting> Settings { get; set; }

        public List<SettingTypeModel> SettingTypes { get; set; }
    }

    public enum SettingDataType
    {
        Boolean = 1,
        Date = 2,
    }
}
