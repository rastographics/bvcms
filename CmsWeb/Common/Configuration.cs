using System.Configuration;

namespace CmsWeb.Common
{
    /// <summary>
    ///     Abstraction of our configuration class
    /// </summary>
    public class Configuration
    {
        public static Configuration Current => new Configuration();

        public bool IsDeveloperMode => GetBool("IsDeveloperMode");

        public string OAuth2TokenEndpoint => GetString("OAuth2TokenEndpoint");

        public string OAuth2AuthorizeEndpoint => GetString("OAuth2AuthorizeEndpoint");

        public string OrgBaseDomain => GetString("OrgBaseDomain");

        public string PushpayAPIBaseUrl => GetString("PushpayAPIBaseUrl");

        public string PushpayClientID => GetString("PushpayClientID");

        public string PushpayClientSecret => GetString("PushpayClientSecret");

        public string PushpayGivingLinkBase => GetString("PushpayGivingLinkBase");

        public string PushpayScope => GetString("PushpayScope");

        public string StatusCheckUrl => GetString("StatusCheckUrl");

        public string TenantHostDev => GetString("TenantHostDev");

        public string TouchpointAuthServer => GetString("TouchpointAuthServer");

        public string CmsHost
        {
            get { return GetString("cmshost"); }
        }

        /// <summary>
        ///     Returns a string representation of this application setting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetString(string key) => ConfigurationManager.AppSettings[key] ?? "";

        /// <summary>
        ///     Returns a boolean representation of this application setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private bool GetBool(string key, bool defaultValue = false)
        {
            string result = GetString(key);
            if (string.IsNullOrEmpty(result))
            {
                return defaultValue;
            }
            return bool.Parse(result);
        }

        /// <summary>
        ///     Returns an integer representation of this application setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private int GetInt(string key, int defaultValue = 0)
        {
            string val = GetString(key);
            if (string.IsNullOrEmpty(val))
            {
                return defaultValue;
            }
            int result = 0;
            if (!int.TryParse(val, out result)) return defaultValue;
            return result;
        }
    }
}
