using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Setup.Models
{
    public class SettingListModel
    {
        public SettingListModel(IEnumerable<SettingMetadatum> settings)
        {
            List = settings ?? new List<SettingMetadatum>();
        }

        public SettingListModel(IEnumerable<Setting> settings)
        {
            List = settings?.Select(s => new SettingMetadatum { Setting = s, SettingId = s.Id }) ?? new List<SettingMetadatum>();
        }

        public IEnumerable<SettingMetadatum> List { get; set; }

        public bool CanDelete { get; set; }
    }
}
