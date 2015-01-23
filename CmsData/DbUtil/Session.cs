using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsData
{
    public static class Util2
    {
        private const string STR_CurrentTag = "CurrentTag";
        private const string STR_DefaultTag = "UnNamed";
        public static object GetSessionObj(string key, object def)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session[key] != null)
                    return HttpContext.Current.Session[key];
                return def;
            }
            return def;
        }

        public static string CurrentTag
        {
            get
            {
                return GetSessionObj(STR_CurrentTag, STR_DefaultTag).ToString();
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_CurrentTag] = value;
            }
        }
        const string STR_ActiveOrganizationId = "ActiveOrganizationId";
        public static int? CurrentOrgId
        {
            get
            {
                return GetSessionObj(STR_ActiveOrganizationId, null).ToInt2();
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ActiveOrganizationId] = value;
            }
        }
        const string STR_ActiveGroupId = "ActiveGroup";
        public static int[] CurrentGroups
        {
            get
            {
                return (int[])GetSessionObj(STR_ActiveGroupId, new int[] { 0 });
            }
            set
            {
                if (value == null)
                    value = new int[] { 0 };
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ActiveGroupId] = value;
            }
        }
        const string STR_ActiveGroupPrefix = "ActiveGroupPrefix";
        public static string CurrentGroupsPrefix
        {
            get
            {
                return (string)GetSessionObj(STR_ActiveGroupPrefix, null);
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ActiveGroupPrefix] = value;
            }
        }
        const string STR_ActiveGroupMode = "ActiveGroupMode";
        public static int CurrentGroupsMode
        {
            get
            {
                return (int)GetSessionObj(STR_ActiveGroupMode, 0);
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ActiveGroupMode] = value;
            }
        }
        const string STR_ActivePersonId = "ActivePersonId";
        public static int CurrentPeopleId
        {
            get
            {
                return GetSessionObj(STR_ActivePersonId, 0).ToInt();
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ActivePersonId] = value;
            }
        }
        public static int? CurrentTagOwnerId
        {
            get
            {
                var pid = Util.UserPeopleId;
                var a = CurrentTag.Split('!');
                if (a.Length > 1)
                    pid = a[0].ToInt2();
                return pid;
            }
        }
        public static string CurrentTagName
        {
            get
            {
                string tag = CurrentTag;
                var a = tag.Split('!');
                if (a.Length == 2)
                    return a[1];
                return tag;
            }
        }
        public const string STR_OrgMembersOnly = "OrgMembersOnly";
        public static bool OrgMembersOnly
        {
            get
            {
                return (bool)GetSessionObj(STR_OrgMembersOnly, false);
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_OrgMembersOnly] = value;
            }
        }
        public const string STR_OrgLeadersOnly = "OrgLeadersOnly";
        public static bool OrgLeadersOnly
        {
            get
            {
                return (bool)GetSessionObj(STR_OrgLeadersOnly, false);
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_OrgLeadersOnly] = value;
            }
        }
        public const string STR_OrgLeadersOnlyChecked = "OrgLeadersOnlyChecked";
        public static bool OrgLeadersOnlyChecked
        {
            get
            {
                return (bool)GetSessionObj(STR_OrgLeadersOnlyChecked, false);
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_OrgLeadersOnlyChecked] = value;
            }
        }
        private const string STR_VisitLookbackDays = "VisitLookbackDays";
        public static int VisitLookbackDays
        {
            get
            {
                return GetSessionObj(STR_VisitLookbackDays, 180).ToInt();
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_VisitLookbackDays] = value;
            }
        }
        [Serializable]
        public class MostRecentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public const string STR_MostRecentOrgs = "MostRecentOrgs";
        public static List<MostRecentItem> MostRecentOrgs
        {
            get
            {
                var mru = (List<MostRecentItem>)GetSessionObj(STR_MostRecentOrgs, null);
                if (mru == null)
                {
                    mru = (from i in DbUtil.Db.MostRecentItems(Util.UserId)
                           where i.Type == "org"
                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
                    HttpContext.Current.Session[STR_MostRecentOrgs] = mru;
                }
                return mru;
            }
        }
        public const string STR_MostRecentPeople = "MostRecentPeople";
        public static List<MostRecentItem> MostRecentPeople
        {
            get
            {
                var mru = (List<MostRecentItem>)GetSessionObj(STR_MostRecentPeople, null);
                if (mru == null)
                {
                    mru = (from i in DbUtil.Db.MostRecentItems(Util.UserId)
                           where i.Type == "per"
                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
                    HttpContext.Current.Session[STR_MostRecentPeople] = mru;
                }
                return mru;
            }
        }

//        public const string STR_MostRecentQueries = "MostRecentQueries";
//        public static List<MostRecentItem> MostRecentQueries
//        {
//            get
//            {
//                var mru = (List<MostRecentItem>)GetSessionObj(STR_MostRecentQueries, null);
//                if (mru == null)
//                {
//                    mru = (from i in DbUtil.Db.MostRecentItems(Util.UserId)
//                           where i.Type == "query"
//                           select new MostRecentItem() { Id = i.Id.Value, Name = i.Name }).ToList();
//                    HttpContext.Current.Session[STR_MostRecentQueries] = mru;
//                }
//                return mru;
//            }
//        }
        public static bool TargetLinkPeople
        {
            get
            {
                return DbUtil.Db.UserPreference("TargetLinkPeople", "true").ToBool();
            }
        }
        public static bool UseNewFeature
        {
            get
            {
                // this works at the database level, not as a user preference
                // useful for turning the new feature on, then having a quik way to put it back in case something goes badly
                return DbUtil.Db.Setting("UseNewFeature", "true").ToBool();
            }
            set
            {
                DbUtil.Db.SetSetting("UseNewFeature", value ? "false" : "true");
                // be sure to SubmitChanges
            }
        }
    }
}
