using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using UtilityExtensions;

namespace CmsData
{
    public partial class CMSDataContext
    {
        public void SetCurrentOrgId(int? id)
        {
            Util2.CurrentOrgId = id;
        }
        public int CurrentSessionOrgId => Util2.CurrentOrgId ?? 0;

        public int CurrentPeopleId { get; set; }
        public string CurrentTagName { get; set; }
        public int? CurrentTagOwnerId { get; set; }
        public int VisitLookbackDays { get; set; }
        public bool OrgLeadersOnly { get; set; }
        public DateTime? QbStartDateOverride { get; set; }
        public DateTime? QbEndDateOverride { get; set; }
        public int? QbDivisionOverride { get; set; }
        public string Host { get; set; }

        private const string STR_UserId = "UserId";
        public int UserId
        {
            //TODO: we're only going to fall back to Util.UserId while we transition to the new session provider so users don't get forced to log out
            get => GetSessionValue(STR_UserId).ToInt2() ?? SetSessionValue(STR_UserId, Util.UserId);
            set => SetSessionValue(STR_UserId, value);
        }

        public int UserId1 => UserId == 0 ? 1 : UserId;

        private const string STR_SessionDictionary = "STR_SessionDictionary";
        private Dictionary<string, string> DictionaryFromCurrentContext => HttpContextFactory.Current?.Items?[STR_SessionDictionary] as Dictionary<string, string>;

        private string GetSessionValue(string key)
        {
            return GetSessionValueFromContext(key) ??
                SetSessionValueInContext(key, SessionValues
                    .SingleOrDefault(v => v.SessionId == CurrentSessionId && v.Name == key)?.Value);
        }

        private string GetSessionValueFromContext(string key)
        {
            if (HttpContextFactory.Current?.Items != null)
            {
                var dict = DictionaryFromCurrentContext;
                if (dict != null)
                {
                    return dict[key];
                }
            }
            return null;
        }

        private string SetSessionValueInContext(string key, string value)
        {
            if (HttpContextFactory.Current?.Items != null && CurrentSessionId != null)
            {
                var dict = DictionaryFromCurrentContext;
                if (dict == null)
                {
                    dict = SessionValues.Where(v => v.SessionId == CurrentSessionId).ToDictionary(v => v.Name, v => v.Value);
                    HttpContextFactory.Current.Items[STR_SessionDictionary] = dict;
                }
                dict[key] = value;
            }
            return value;
        }

        private T SetSessionValue<T>(string key, T value)
        {
            string stringValue = null;
            if (value is string)
            {
                stringValue = value as string;
            }
            else if (value != null)
            {
                stringValue = JsonConvert.SerializeObject(value);
            }
            FetchOrCreateSessionValue(key, stringValue);
            SetSessionValueInContext(key, stringValue);
            return value;
        }

        private SessionValue FetchOrCreateSessionValue(string key, string value)
        {
            var sv = SessionValues.FirstOrDefault(v => v.SessionId == CurrentSessionId && v.Name == key);
            if (sv == null && value != null && CurrentSessionId != null)
            {
                sv = new SessionValue
                {
                    Name = key,
                    SessionId = CurrentSessionId,
                    CreatedDate = DateTime.UtcNow
                };
                SessionValues.InsertOnSubmit(sv);
            }
            if (sv != null)
            {
                if (value == null)
                {
                    SessionValues.DeleteOnSubmit(sv);
                }
                else
                {
                    sv.Value = value;
                }
                SubmitChanges();
            }
            return sv;
        }

        public string CurrentSessionId => HttpContextFactory.Current?.Session?.SessionID;

        public string CmsHost
        {
            get
            {
                string defaultHost = null;
                if (Util.IsDebug())
                {
                    defaultHost = ConfigurationManager.AppSettings["cmshost"];
                }

                // choose DefaultHost setting first
                if (!defaultHost.HasValue())
                {
                    defaultHost = Setting("DefaultHost", "");
                }

                // if no DefaultHost setting exists, use current URL
                if (!defaultHost.HasValue() && HttpContextFactory.Current != null)
                {
                    var request = HttpContextFactory.Current.Request;
                    defaultHost = Util.URLCombine(Scheme() + "://" + request.Url.Authority, "");
                }

                // finally, try the "cmshost" setting
                if (!defaultHost.HasValue())
                {
                    defaultHost = Util.URLCombine(ConfigurationManager.AppSettings["cmshost"], "");
                }

                if (Host.HasValue())
                {
                    return defaultHost.Replace("{church}", Host, ignoreCase: true);
                }

                throw (new Exception("No URL for Server in CmsHost"));
            }
        }

        private string Scheme()
        {
            if (HttpContextFactory.Current != null)
            {
                var Request = HttpContextFactory.Current.Request;
                var scheme = Request.Url.Scheme;
                if (Request.Headers["X-Forwarded-Proto"] == "https")
                {
                    scheme = "https";
                }

                return scheme;
            }
            return "http";
        }

        public string ServerLink(string path = "")
        {
            return Util.URLCombine(CmsHost, path);
        }

        public void CopySession()
        {
            if (HttpContextFactory.Current != null && HttpContextFactory.Current.Session != null)
            {
                CurrentPeopleId = Util2.CurrentPeopleId;
                CurrentTagOwnerId = Util2.CurrentTagOwnerId;
                CurrentTagName = Util2.CurrentTagName;
                OrgLeadersOnly = Util2.OrgLeadersOnly;
                VisitLookbackDays = Util2.VisitLookbackDays;
                Host = Util.Host;
            }
        }

        public string GetSetting(string name, string defaultvalue)
        {
            var setting = Settings.SingleOrDefault(ss => ss.Id == name);
            if (setting == null)
            {
                return defaultvalue;
            }

            return setting.SettingX ?? defaultvalue ?? string.Empty;
        }

        public string Setting(string name, string defaultvalue)
        {
            if (name == null)
            {
                return defaultvalue;
            }

            var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                try
                {
                    list = Settings.ToList().ToDictionary(c => c.Id.Trim(), c => c.SettingX,
                        StringComparer.OrdinalIgnoreCase);
                    if (UseRuntimeSettingsCache)
                    {
                        HttpRuntime.Cache.Insert(Host + "Setting", list, null,
                        DateTime.Now.AddSeconds(15), Cache.NoSlidingExpiration);
                    }
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
            {
                return list[name];
            }
            if (defaultvalue.HasValue())
            {
                return defaultvalue;
            }
            return string.Empty;
        }

        public bool Setting(string name, bool defaultValue = false)
        {
            var setting = Setting(name, null);
            return !setting.HasValue() ? defaultValue : setting.ToLower() == "true";
        }

        public void SetSetting(string name, string value)
        {
            name = name.Trim();
            var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                list = Settings.ToDictionary(c => c.Id.Trim(), c => c.SettingX);
                if (UseRuntimeSettingsCache)
                {
                    HttpRuntime.Cache.Insert(Host + "Setting", list, null,
                            DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
                }
            }
            list[name] = value;

            var setting = Settings.SingleOrDefault(c => c.Id == name);
            if (setting == null)
            {
                setting = new Setting { Id = name, SettingX = value };
                Settings.InsertOnSubmit(setting);
            }
            else
            {
                setting.SettingX = value;
            }
        }

        public Task SetTaskDetails(int id, string name, string value)
        {
            var task = Tasks.Single(c => c.Id == id);
            var field = task.GetType().GetProperty(name);

            switch (name)
            {
                case "Due":
                    if (string.IsNullOrEmpty(value))
                    {
                        field.SetValue(task, null);
                    }
                    else
                    {
                        field.SetValue(task, DateTime.Parse(value));
                    }
                    break;
                case "StatusId":
                    field.SetValue(task, int.Parse(value));
                    break;
                case "ForceCompleteWContact":
                    field.SetValue(task, bool.Parse(value));
                    break;
                default:
                    field.SetValue(task, value);
                    break;
            }
            return task;
        }

        public void DeleteSetting(string name)
        {
            var list = HttpRuntime.Cache[Host + "Setting"] as Dictionary<string, string>;
            if (list == null)
            {
                list = Settings.ToDictionary(c => c.Id.Trim(), c => c.SettingX);
                if (UseRuntimeSettingsCache)
                {
                    HttpRuntime.Cache.Insert(Host + "Setting", list, null,
                            DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
                }
            }
            list.Remove(name);

            var setting = Settings.SingleOrDefault(c => c.Id == name);
            if (setting != null)
            {
                Settings.DeleteOnSubmit(setting);
            }
        }

        public void LogActivity(string activity, int? oid = null, int? pid = null, int? did = null, int? uid = null)
        {
            DbUtil.LogActivity(Host, activity, oid, pid, did, uid);
        }

        public string SendGridMailUser
        {
            get
            {
                const string sendgridmailuser = "SendGridMailUser";

                var user = HttpRuntime.Cache[Host + sendgridmailuser] as string;
                if (user.HasValue())
                {
                    return user;
                }

                user = Setting(sendgridmailuser, "");
                if (!user.HasValue())
                {
                    user = ConfigurationManager.AppSettings[sendgridmailuser];
                }
                HttpRuntime.Cache.Insert(Host + sendgridmailuser, user, null, DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);

                return user;
            }
        }

        public string SendGridMailPassword
        {
            get
            {
                const string sendgridmailpassword = "SendGridMailPassword";

                var user = HttpRuntime.Cache[Host + sendgridmailpassword] as string;
                if (user.HasValue())
                {
                    return user;
                }

                user = Setting(sendgridmailpassword, "");
                if (!user.HasValue())
                {
                    user = ConfigurationManager.AppSettings[sendgridmailpassword];
                }
                HttpRuntime.Cache.Insert(Host + sendgridmailpassword, user, null, DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);

                return user;
            }
        }

        public bool UseRuntimeSettingsCache => Util.PickFirst(WebConfigurationManager.AppSettings["UseRuntimeSettingsCache"], "true").ToBool();

        public bool RegistrationsConverted()
        {
            var converted = (bool?)HttpRuntime.Cache[Host + "-RegistrationsConverted"];
            if (converted.HasValue)
            {
                return converted.Value;
            }

            var b = Setting("RegistrationsConverted");
            if (!b)
            {
                b = Connection.ExecuteScalar(
                    @"SELECT CASE WHEN EXISTS(
                        		SELECT NULL
                        		FROM dbo.Organizations
                        		WHERE RegistrationTypeId > 0
                        		AND LEN(ISNULL(RegSetting,'')) > 0
                        		AND RegSettingXml IS NULL
                        	) THEN 1 ELSE 0 END").ToInt() == 0; // 0 == already converted
            }
            return b;
        }

        public void SetRegistrationsConverted()
        {
            SetSetting("RegistrationsConverted", "true");
            SubmitChanges();
            if (UseRuntimeSettingsCache)
            {
                HttpRuntime.Cache.Insert(Host + "-RegistrationsConverted", true, null,
                    DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }
        }

        public string RenderTemplate(string source, object data)
        {
            var template = PythonModel.RegisterHelpers(this).Compile(source);
            var result = template(data);
            return result;
        }
    }
}
