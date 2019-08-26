using CmsData;
using System.Collections.Generic;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingModel
    {
        public SettingModel()
        {
            Settings = new List<SettingMetadatum>();
            SettingTypes = new List<SettingTypeModel>();
        }

        public IEnumerable<SettingMetadatum> Settings { get; set; }

        public List<SettingTypeModel> SettingTypes { get; set; }
    }

    public enum SettingDataType
    {
        Boolean = 1,
        Date = 2,
    }
}
