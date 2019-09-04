using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using UtilityExtensions;

namespace CmsData
{
    public static class Util2
    {
        public const string STR_ActiveOrganizationId = "ActiveOrganizationId";
        public const string STR_ActiveGroupId = "ActiveGroup";
        public const string STR_ActiveGroupPrefix = "ActiveGroupPrefix";
        public const string STR_ActiveGroupMode = "ActiveGroupMode";
        public const string STR_ActivePersonId = "ActivePersonId";
        public const string STR_CurrentTag = "CurrentTag";
        public const string STR_DefaultTag = "UnNamed";
        public const string STR_FromMobile = "source";
        public const string STR_MostRecentOrgs = "MostRecentOrgs";
        public const string STR_MostRecentPeople = "MostRecentPeople";
        public const string STR_OrgLeadersOnly = "OrgLeadersOnly";
        public const string STR_OrgLeadersOnlyChecked = "OrgLeadersOnlyChecked";
        public const string STR_VisitLookbackDays = "VisitLookbackDays";

        private static CMSDataContext Db => CMSDataContext.Create(HttpContextFactory.Current);

        public static object GetSessionObj(string key, object def = null)
        {
            if (HttpContextFactory.Current != null && HttpContextFactory.Current.Session != null)
            {
                if (HttpContextFactory.Current.Session[key] != null)
                {
                    return HttpContextFactory.Current.Session[key];
                }

                return def;
            }
            else
            {
                var threadData = Thread.GetData(Thread.GetNamedDataSlot(key));
                if (threadData != null)
                {
                    return threadData;
                }
            }
            return def;
        }
        public static void SetSessionObj(string key, object value)
        {
            if (HttpContextFactory.Current != null && HttpContextFactory.Current.Session != null)
            {
                HttpContextFactory.Current.Session[key] = value;
            }
            else
            {
                Thread.SetData(Thread.GetNamedDataSlot(key), value);
            }
        }

        public static string CurrentTag
        {
            get => GetSessionObj(STR_CurrentTag, STR_DefaultTag).ToString();
            set => SetSessionObj(STR_CurrentTag, value);
        }

        public static int? CurrentOrgId
        {
            get => GetSessionObj(STR_ActiveOrganizationId) as int?;
            set => SetSessionObj(STR_ActiveOrganizationId, value);
        }


        public static int[] CurrentGroups
        {
            get => (int[])GetSessionObj(STR_ActiveGroupId, new int[] { 0 });
            set
            {
                if (value == null)
                {
                    value = new int[] { 0 };
                }

                SetSessionObj(STR_ActiveGroupId, value);
            }
        }

        public static string CurrentGroupsPrefix
        {
            get => (string)GetSessionObj(STR_ActiveGroupPrefix, null);
            set => SetSessionObj(STR_ActiveGroupPrefix, value);
        }

        public static int CurrentGroupsMode
        {
            get => (int)GetSessionObj(STR_ActiveGroupMode, 0);
            set => SetSessionObj(STR_ActiveGroupMode, value);
        }


        public static int CurrentPeopleId
        {
            get => GetSessionObj(STR_ActivePersonId, 0).ToInt();
            set => SetSessionObj(STR_ActivePersonId, value);
        }

        public static string FromMobile => (string)GetSessionObj(STR_FromMobile, null);
        public static int? CurrentTagOwnerId
        {
            get
            {
                var pid = Util.UserPeopleId;
                var a = CurrentTag.Split('!');
                if (a.Length > 1)
                {
                    pid = a[0].ToInt2();
                }

                return pid;
            }
        }
        public static string CurrentTagName
        {
            get
            {
                if (CurrentTag.StartsWith("QueryTag:"))
                {
                    return CurrentTag.GetCsvToken(2, 2, ":");
                }
                string tag = CurrentTag;
                var a = tag.Split('!');
                if (a[0].ToInt2() > 0)
                {
                    if (a.Length == 2)
                    {
                        return a[1];
                    }
                }

                return tag;
            }
        }

        public static bool OrgLeadersOnly
        {
            get => (bool)GetSessionObj(STR_OrgLeadersOnly, false);
            set => SetSessionObj(STR_OrgLeadersOnly, value);
        }

        public static bool OrgLeadersOnlyChecked
        {
            get => (bool)GetSessionObj(STR_OrgLeadersOnlyChecked, false);
            set => SetSessionObj(STR_OrgLeadersOnlyChecked, value);
        }

        public static int VisitLookbackDays
        {
            get => GetSessionObj(STR_VisitLookbackDays, 180).ToInt();
            set => SetSessionObj(STR_VisitLookbackDays, value);
        }

        [Serializable]
        public class MostRecentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public static List<MostRecentItem> MostRecentOrgs
        {
            get
            {
                var mru = (List<MostRecentItem>)GetSessionObj(STR_MostRecentOrgs, null);
                if (mru == null)
                {
                    mru = (from i in Db.MostRecentItems(Util.UserId)
                           where i.Type == "org"
                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
                    HttpContextFactory.Current.Session[STR_MostRecentOrgs] = mru;
                }
                return mru;
            }
        }

        public static List<MostRecentItem> MostRecentPeople
        {
            get
            {
                var mru = (List<MostRecentItem>)GetSessionObj(STR_MostRecentPeople, null);
                if (mru == null)
                {
                    mru = (from i in Db.MostRecentItems(Util.UserId)
                           where i.Type == "per"
                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
                    HttpContextFactory.Current.Session[STR_MostRecentPeople] = mru;
                }
                return mru;
            }
        }

        public static bool TargetLinkPeople => Db.UserPreference("TargetLinkPeople", true);
        public static bool TargetLinkOrg => Db.UserPreference("TargetLinkOrg", true);
        public static bool OnlineRegTypeSearchAdd => GetSessionObj("OnlineRegTypeSearchAdd", false).ToBool();

        public static string CampusLabel => Db.Setting("CampusLabel", "Campus");

        public static void Log2File(string file, string data)
        {
            string fn = ConfigurationManager.AppSettings["SharedFolder"].Replace("%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"));
            fn = Util.URLCombine(fn, file);
            System.IO.File.AppendAllText(fn, data);
        }

        public static string FetchUsername(CMSDataContext db, string first, string last)
        {
            var firstinitial = first?.Trim();
            if (firstinitial.HasValue())
            {
                firstinitial = firstinitial.ToLower();
                if (firstinitial.Length > 1)
                {
                    firstinitial = firstinitial[0].ToString();
                }
            }
            var username = firstinitial + last.Trim().ToLower().Replace(",", "").Replace(" ", "").Truncate(20);
            var uname = username;
            var i = 1;
            while (db.Users.SingleOrDefault(u => u.Username == uname) != null)
            {
                uname = username + i++;
            }
            return uname;
        }
    }
}
