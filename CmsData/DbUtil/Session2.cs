using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using UtilityExtensions;
using System.Web.Caching;

namespace CmsData
{
    public partial class CMSDataContext
    {
        public int? CurrentOrgId { get; set; }
        public int CurrentOrgId0 { get { return CurrentOrgId ?? 0; } }
        public int[] CurrentGroups { get; set; }
        public string CurrentGroupsPrefix { get; set; }
        public int CurrentGroupsMode { get; set; }
        public int CurrentPeopleId { get; set; }
        public int? CurrentTagOwnerId { get; set; }
        public string CurrentTagName { get; set; }
        public int VisitLookbackDays { get; set; }
        public bool OrgMembersOnly { get; set; }
        public bool OrgLeadersOnly { get; set; }
        public string Host { get; set; }
        private string cmshost;
        public string CmsHost
        {
            get
            {
                if (cmshost.HasValue())
                    return cmshost;
                return cmshost = ServerLink();
            }
            set { cmshost = value; }
        }
        public string ServerLink(string path = "")
        {
            if (HttpContext.Current != null)
            {
                var Request = HttpContext.Current.Request;
                return Util.URLCombine(Util.Scheme() + "://" + Request.Url.Authority, path);
            }
            var h = ConfigurationManager.AppSettings["cmshost"];
            if(h.HasValue())
                return h.Replace("{church}", Host, ignoreCase: true);
            throw (new Exception("No URL for Server in ServerLink"));
        }

        public void CopySession()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                CurrentOrgId = Util2.CurrentOrgId;
                CurrentGroups = Util2.CurrentGroups;
                CurrentGroupsPrefix = Util2.CurrentGroupsPrefix;
                CurrentGroupsMode = Util2.CurrentGroupsMode;
                CurrentPeopleId = Util2.CurrentPeopleId;
                CurrentTagOwnerId = Util2.CurrentTagOwnerId;
                CurrentTagName = Util2.CurrentTagName;
                OrgMembersOnly = Util2.OrgMembersOnly;
                OrgLeadersOnly = Util2.OrgLeadersOnly;
                VisitLookbackDays = Util2.VisitLookbackDays;
                Host = Util.Host;
            }
        }
        public string Setting(string name, string defaultvalue)
        {
			var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                try
                {
                    list = Settings.ToDictionary(c => c.Id, c => c.SettingX,
                        StringComparer.OrdinalIgnoreCase);
					HttpRuntime.Cache.Insert(Host + "Setting", list, null,
						DateTime.Now.AddSeconds(15), Cache.NoSlidingExpiration);
                }
                catch (SqlException ex)
                {
                    throw;
                }
                catch (Exception)
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
        public void SetSetting(string name, string value)
        {
			var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                list = Settings.ToDictionary(c => c.Id, c => c.SettingX);
				HttpRuntime.Cache.Insert(Host + "Setting", list, null,
						DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
            }
            list[name] = value;

            var setting = Settings.SingleOrDefault(c => c.Id == name);
            if (setting == null)
            {
                setting = new Setting { Id = name, SettingX = value };
                Settings.InsertOnSubmit(setting);
            }
            else
                setting.SettingX = value;
        }
        public void DeleteSetting(string name)
        {
			var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                list = Settings.ToDictionary(c => c.Id, c => c.SettingX);
				HttpRuntime.Cache.Insert(Host + "Setting", list, null,
						DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
            }
            list.Remove(name);

            var setting = Settings.SingleOrDefault(c => c.Id == name);
            if (setting != null)
                Settings.DeleteOnSubmit(setting);
        }
    }
}
