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

        public string Setting(string name, string defaultvalue, int ProcessId)
        {
            int? GatewayAccountId = db.PaymentProcess.Where(x => x.ProcessId == ProcessId).Select(x => x.GatewayAccountId).FirstOrDefault();
            if (name == null)
                return defaultvalue;
            var list = db.GatewayDetails.Where(x => x.GatewayAccountId == GatewayAccountId).ToDictionary(x => x.GatewayDetailName.Trim(), x => x.GatewayDetailValue);
            // var listD = db.GatewayDetails.Where(x => x.GatewayAccountId == GatewayAccountId).Select(x => new { x.GatewayDetailName, x.GatewayDetailValue });

            /*var list = (from e in db.GatewayDetails
                         select new
                         {
                             e.GatewayDetailName,
                             e.GatewayDetailValue
                         }).ToList().ToDictionary(x => x.GatewayDetailName.Trim(), x => x.GatewayDetailValue);*/

            if (list == null)
            {
                try
                {
                    list = db.GatewayDetails.ToList().ToDictionary(x => x.GatewayDetailName.Trim(), x => x.GatewayDetailValue,
                        StringComparer.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    DbUtil.LogActivity($"Gateway: Could not get gateway-{ProcessId} details\nErr:\n{ex.Message}");
                    return string.Empty;
                }
            }
            if (list.ContainsKey(name) && list[name].HasValue())
                return list[name];
            if (defaultvalue.HasValue())
                return defaultvalue;
            return string.Empty;
        }

        public bool Setting(string name, int ProcessId, bool defaultValue = false)
        {
            var setting = Setting(name, null, ProcessId);
            if (!setting.HasValue())
                return defaultValue;

            return setting.ToLower() == "true";
        }
    }
}
