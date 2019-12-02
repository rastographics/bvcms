using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingTypeModel
    {
        public SettingType SettingType { get; set; }
        public IEnumerable<SettingMetadatum> Settings { get; set; }
        public SettingTypeModel() { }

        public SettingTypeModel(IEnumerable<SettingMetadatum> resources)
        {
            SettingType = resources.First().SettingCategory.SettingType;
            Settings = resources;
        }
    }
}
