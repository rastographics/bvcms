using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using UtilityExtensions;

namespace CmsWeb.Common.Status
{
    public class StatusHelper
    {
        public static async Task<AppStatus> GetStatusAsync(HttpContextBase context)
        {
            var status = context.Cache.Get("AdminAlerts") as AppStatus;
            if (status == null)
            {
                var statusUrl = Configuration.Current.StatusCheckUrl;
                if (statusUrl.HasValue())
                {
                    using (var client = new WebClient())
                    {
                        var json = await client.DownloadStringTaskAsync(string.Format(statusUrl, DateTime.Now.Ticks));
                        var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        var announcement = values["Announce"] as string;
                        if (announcement.HasValue() && !announcement.Contains("No reported issues"))
                        {
                            status = new AppStatus { title = "System status", message = announcement };
                        }
                        context.Cache.Add("AdminAlerts", status ?? new AppStatus(), null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                }
            }

            return status;
        }
    }
}
