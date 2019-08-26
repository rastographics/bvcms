using System.Collections.Generic;
using CmsData;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingListModel
    {
        public SettingListModel(IEnumerable<SettingMetadatum> settings)
        {
            List = settings ?? new List<SettingMetadatum>();
        }

        public IEnumerable<SettingMetadatum> List { get; set; }

        public bool CanDelete { get; set; }
    }
}
