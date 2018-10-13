using System.Configuration;

namespace CmsWeb.Common
{
    /// <summary>
    ///     Abstraction of our configuration class
    /// </summary>
    public class Configuration
    {
        public static Configuration Current
        {
            get { return new Configuration(); }
        }

        public bool IsDeveloperMode
        {
            get { return GetBool("IsDeveloperMode"); }
        }

        public string PushpayAPIBaseUrl
        {
            get { return GetString("PushpayAPIBaseUrl"); }
        }

        public string PushpayClientID
        {
            get { return GetString("PushpayClientID"); }
        }

        public string PushpayClientSecret
        {
            get { return GetString("PushpayClientSecret"); }
        }

        public string OAuth2TokenEndpoint
        {
            get { return GetString("OAuth2TokenEndpoint"); }
        }

        public string OAuth2AuthorizeEndpoint
        {
            get { return GetString("OAuth2AuthorizeEndpoint"); }
        }

        public string TouchpointAuthServer
        {
            get { return GetString("TouchpointAuthServer"); }
        }

        public string OrgBaseDomain
        {
            get { return GetString("OrgBaseDomain"); }
        }

        public string TenantHostDev
        {
            get { return GetString("TenantHostDev"); }
        }

        public string PushpayScope
        {
            get { return GetString("PushpayScope"); }
        }

        /// <summary>
        ///     Returns a string representation of this application setting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetString(string key)
        {
            string result = ConfigurationManager.AppSettings[key] ?? "";
            return result;
        }

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
