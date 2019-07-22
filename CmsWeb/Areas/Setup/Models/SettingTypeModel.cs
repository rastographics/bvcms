using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingTypeModel
    {
        public SettingType SettingType { get; set; }
        public IEnumerable<Setting> Settings { get; set; }
        public SettingTypeModel() { }
        public SettingTypeModel(SettingType resourceType)
        {
            SettingType = resourceType;
            Settings = resourceType.Settings
                .OrderBy(r => r.SettingCategory.DisplayOrder)
                .ThenBy(r => r.SettingType.DisplayOrder);
        }

        public SettingTypeModel(SettingType resourceType, IEnumerable<Setting> resources)
        {
            SettingType = resourceType;
            Settings = resources;
        }
    }
}
