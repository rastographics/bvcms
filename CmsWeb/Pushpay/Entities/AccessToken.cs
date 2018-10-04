using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Pushpay.Entities
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
