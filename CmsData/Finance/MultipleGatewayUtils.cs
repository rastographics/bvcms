using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public class MultipleGatewayUtils
    {
        CMSDataContext db = new CMSDataContext();
        public MultipleGatewayUtils(CMSDataContext db)
        {
            this.db = db;
        }

        public string Setting(string name, string defaultvalue, int GatewayId)
        {
            if (name == null)
                return defaultvalue;
            var list = db.GatewayDetails.Where(x => x.GatewayId == GatewayId).Select(x => new { x.GatewayDetailName, x.GatewayDetailValue }) as Dictionary<string, string>;
            if (list == null)
            {
                try
                {
                    list = db.GatewayDetails.ToList().ToDictionary(x => x.GatewayDetailName.Trim(), x => x.GatewayDetailValue,
                        StringComparer.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
            if (list.ContainsKey(name) && list[name].HasValue())
                return list[name];
            if (defaultvalue.HasValue())
                return defaultvalue;
            return string.Empty;
        }

        public bool Setting(string name, int GatewayId, bool defaultValue = false)
        {
            var setting = Setting(name, null, GatewayId);
            if (!setting.HasValue())
                return defaultValue;

            return setting.ToLower() == "true";
        }
    }
}
