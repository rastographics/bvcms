using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        public static string CurrentTag
        {
            get => Util.GetFromSession(STR_CurrentTag, STR_DefaultTag).ToString();
            set => Util.SetValueInSession(STR_CurrentTag, value);
        }

        public static int? CurrentOrgId
        {
            get => Util.GetFromSession(STR_ActiveOrganizationId, (int?)null);
            set => Util.SetValueInSession(STR_ActiveOrganizationId, value);
        }

        public static int[] CurrentGroups
        {
            get => (int[])Util.GetFromSession(STR_ActiveGroupId, new int[] { 0 });
            set
            {
                if (value == null)
                {
                    value = new int[] { 0 };
                }

                Util.SetValueInSession(STR_ActiveGroupId, value);
            }
        }

        public static string CurrentGroupsPrefix
        {
            get => Util.GetFromSession<string>(STR_ActiveGroupPrefix, null);
            set => Util.SetValueInSession(STR_ActiveGroupPrefix, value);
        }

        public static int CurrentGroupsMode
        {
            get => Util.GetFromSession(STR_ActiveGroupMode, "0").ToInt();
            set => Util.SetValueInSession(STR_ActiveGroupMode, value.ToString());
        }

        public static int CurrentPeopleId
        {
            get => Util.GetFromSession(STR_ActivePersonId, "0").ToInt();
            set => Util.SetValueInSession(STR_ActivePersonId, value.ToString());
        }

        public static string FromMobile => Util.GetFromSession<string>(STR_FromMobile, null);

        public static int? CurrentTagOwnerId
        {
            get
            {
                var pid = Db.UserPeopleId;
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

        public static string GetValidTagName(string tagname)
        {
            return tagname.HasValue() ? tagname : CurrentTagName;
        }

        public static bool OrgLeadersOnly
        {
            get => Util.GetFromSession(STR_OrgLeadersOnly, false);
            set => Util.SetValueInSession(STR_OrgLeadersOnly, value);
        }

        public static bool OrgLeadersOnlyChecked
        {
            get => (bool)Util.GetFromSession(STR_OrgLeadersOnlyChecked, false);
            set => Util.SetValueInSession(STR_OrgLeadersOnlyChecked, value);
        }

        public static int VisitLookbackDays
        {
            get => Util.GetFromSession(STR_VisitLookbackDays, "180").ToInt();
            set => Util.SetValueInSession(STR_VisitLookbackDays, value.ToString());
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
                var mru = Util.GetFromSession<List<MostRecentItem>>(STR_MostRecentOrgs, null);
                if (mru == null)
                {
                    mru = (from i in Db.MostRecentItems(Db.UserId)
                           where i.Type == "org"
                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
                    Util.SetValueInSession(STR_MostRecentOrgs, mru);
                }
                return mru;
            }
            set
            {
                Util.SetValueInSession(STR_MostRecentOrgs, value);
            }
        }

        public static List<MostRecentItem> MostRecentPeople
        {
            get
            {
                var mru = Util.GetFromSession<List<MostRecentItem>>(STR_MostRecentPeople, null);
                if (mru == null)
                {
                    mru = (from i in Db.MostRecentItems(Db.UserId)
                           where i.Type == "per"
                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
                    Util.SetValueInSession(STR_MostRecentPeople, mru);
                }
                return mru;
            }
            set
            {
                Util.SetValueInSession(STR_MostRecentPeople, value);
            }
        }

        public static bool TargetLinkPeople => Db.UserPreference("TargetLinkPeople", true);
        public static bool TargetLinkOrg => Db.UserPreference("TargetLinkOrg", true);
        public static bool OnlineRegTypeSearchAdd => Util.GetFromSession("OnlineRegTypeSearchAdd", false).ToBool();

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
