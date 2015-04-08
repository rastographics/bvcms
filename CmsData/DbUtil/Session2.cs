using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using UtilityExtensions;
using System.Web.Caching;

namespace CmsData
{
    public partial class CMSDataContext
    {
        private CurrentOrg currentOrg;
        public CurrentOrg CurrentOrg
        {
            get { return currentOrg ?? (currentOrg = Util2.CurrentOrganization); }
            set
            {
                Util2.CurrentOrganization = value;
                currentOrg = value;
            }
        }

        public void SetCurrentOrgId(int? id)
        {
            if(CurrentOrg == null)
                CurrentOrg = new CurrentOrg() { GroupSelect = Codes.GroupSelectCode.Member };
            CurrentOrg.Id = id;
        }

        public int? CurrentOrgId { get; set; }

        public int CurrentOrgId0
        {
            get
            {
                return (CurrentOrg ?? new CurrentOrg()).Id ?? Util2.CurrentOrgId ?? 0;
            }
        }

        public int[] CurrentGroups { get; set; }
        public string CurrentGroupsPrefix { get; set; }
        public int CurrentGroupsMode { get; set; }
        public int CurrentPeopleId { get; set; }
        public string CurrentTagName { get; set; }
        public int? CurrentTagOwnerId { get; set; }
        public int VisitLookbackDays { get; set; }
        public bool OrgMembersOnly { get; set; }
        public bool OrgLeadersOnly { get; set; }
        public DateTime? QbStartDateOverride { get; set; }
        public DateTime? QbEndDateOverride { get; set; }
        public int? QbDivisionOverride { get; set; }
        public string Host { get; set; }

        public string CmsHost
        {
            get
            {
                // choose DefaultHost setting first
                var defaultHost = Setting("DefaultHost", "");

                // if no DefaultHost setting exists, use current URL
                if (!defaultHost.HasValue() && HttpContext.Current != null)
                {
                    var request = HttpContext.Current.Request;
                    defaultHost = Util.URLCombine(Util.Scheme() + "://" + request.Url.Authority, "");
                }

                // finally, try the "cmshost" setting
                if (!defaultHost.HasValue())
                    defaultHost = Util.URLCombine(ConfigurationManager.AppSettings["cmshost"], "");

                if (defaultHost.HasValue())
                    return defaultHost.Replace("{church}", Host, ignoreCase: true);

                throw (new Exception("No URL for Server in CmsHost"));
            }
        }

        public string ServerLink(string path = "")
        {
            return Util.URLCombine(CmsHost, path);
        }

        public void CopySession()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (Util2.UseNewOrg)
                {
                    if (CurrentOrg == null)
                        CurrentOrg = new CurrentOrg() {Id = Util2.CurrentOrgId};
                    CurrentOrgId = CurrentOrg.Id;
                }
                else
                {
                    CurrentOrgId = Util2.CurrentOrgId;
                    CurrentGroups = Util2.CurrentGroups;
                    CurrentGroupsPrefix = Util2.CurrentGroupsPrefix;
                    CurrentGroupsMode = Util2.CurrentGroupsMode;
                }
                CurrentPeopleId = Util2.CurrentPeopleId;
                CurrentTagOwnerId = Util2.CurrentTagOwnerId;
                CurrentTagName = Util2.CurrentTagName;
                OrgMembersOnly = Util2.OrgMembersOnly;
                OrgLeadersOnly = Util2.OrgLeadersOnly;
                VisitLookbackDays = Util2.VisitLookbackDays;
                Host = Util.Host;
            }
        }
        public string GetSetting(string name, string defaultvalue)
        {
            var setting = Settings.SingleOrDefault(ss => ss.Id == name);
            if (setting == null)
                return defaultvalue;
            return setting.SettingX ?? defaultvalue ?? string.Empty;
        }
        public string Setting(string name, string defaultvalue)
        {
			var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                try
                {
                    list = Settings.ToDictionary(c => c.Id.Trim(), c => c.SettingX,
                        StringComparer.OrdinalIgnoreCase);
					HttpRuntime.Cache.Insert(Host + "Setting", list, null,
						DateTime.Now.AddSeconds(15), Cache.NoSlidingExpiration);
                }
                catch (SqlException)
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
            name = name.Trim();
			var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                list = Settings.ToDictionary(c => c.Id.Trim(), c => c.SettingX);
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
                list = Settings.ToDictionary(c => c.Id.Trim(), c => c.SettingX);
				HttpRuntime.Cache.Insert(Host + "Setting", list, null,
						DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
            }
            list.Remove(name);

            var setting = Settings.SingleOrDefault(c => c.Id == name);
            if (setting != null)
                Settings.DeleteOnSubmit(setting);
        }
        public void Log(string s)
        {
            var output = ConfigurationManager.AppSettings["SharedFolder"].Replace("%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"));
            output = output + "\\log-{0}-{1}.txt".Fmt(Host, DateTime.Today.ToSortableDate());
            var text = "{0} {1}\r\n".Fmt(DateTime.Now.ToSortableTime(), s);
            File.AppendAllText(output, text);
        }
    }
}
