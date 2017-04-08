using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class FailedMailInfo
    {
        public DateTime? time { get; set; }
        public string eventx { get; set; }
        public string type { get; set; }
        public string reason { get; set; }
        public string name { get; set; }
        public int? peopleid { get; set; }
        public int? emailid { get; set; }
        public string subject { get; set; }
        public string email { get; set; }
        public bool admin { get; set; }
        public bool devel { get; set; }
        public bool canunblock
        {
            get
            {
                if (!admin || !email.HasValue())
                    return false;
                if ((eventx != "bounce" || type != "blocked") && eventx != "dropped")
                    return false;
                if (eventx == "dropped" && reason.Contains("spam", ignoreCase: true))
                    return false;
                var apikey = ConfigurationManager.AppSettings["SendGridApiKey"];
                return apikey.HasValue();
            }
        }
        public bool canunspam
        {
            get
            {
                if (!devel || !email.HasValue())
                    return false;
                if ((eventx != "bounce" || type != "blocked") && eventx != "dropped")
                    return false;
                if (eventx == "dropped" && !reason.Contains("spam", ignoreCase: true))
                    return false;
                var apikey = ConfigurationManager.AppSettings["SendGridApiKey"];
                return apikey.HasValue();
            }
        }
    }
}
