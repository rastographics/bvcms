using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using MarkdownDeep;
using UtilityExtensions;

namespace CmsData
{
    public static class Util2
    {
        private const string STR_CurrentTag = "CurrentTag";
        private const string STR_DefaultTag = "UnNamed";

        public static object GetSessionObj(string key, object def)
        {
            if (HttpContextFactory.Current != null && HttpContextFactory.Current.Session != null)
            {
                if (HttpContextFactory.Current.Session[key] != null)
                    return HttpContextFactory.Current.Session[key];
                return def;
            }
            return def;
        }
        public static void SetSessionObj(string key, object value)
        {
            if (HttpContextFactory.Current != null)
                HttpContextFactory.Current.Session[key] = value;
        }

        public static string CurrentTag
        {
            get { return GetSessionObj(STR_CurrentTag, STR_DefaultTag).ToString(); }
            set { SetSessionObj(STR_CurrentTag, value); }
        }
        const string STR_ActiveOrganizationId = "ActiveOrganizationId";
        public static int? CurrentOrgId
        {
            get
            {
                int? orgid = null;
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_ActiveOrganizationId] != null)
                            orgid = HttpContextFactory.Current.Session[STR_ActiveOrganizationId] as int?;
                }
                else
                    orgid = (int?) Thread.GetData(Thread.GetNamedDataSlot(STR_ActiveOrganizationId));
                return orgid;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                {
                    if (HttpContextFactory.Current.Session != null)
                        HttpContextFactory.Current.Session[STR_ActiveOrganizationId] = value;
                }
                else
                    Thread.SetData(Thread.GetNamedDataSlot(STR_ActiveOrganizationId), value);
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
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[STR_ActiveGroupId] = value;
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
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[STR_ActiveGroupPrefix] = value;
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
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[STR_ActiveGroupMode] = value;
            }
        }

        const string STR_ActivePersonId = "ActivePersonId";
        public static int CurrentPeopleId
        {
            get { return GetSessionObj(STR_ActivePersonId, 0).ToInt(); }
            set { SetSessionObj(STR_ActivePersonId, value); }
        }
        const string STR_FromMobile = "source";
        public static string FromMobile
        {
            get
            {
                return (string)GetSessionObj(STR_FromMobile, null);
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
                if (CurrentTag.StartsWith("QueryTag:"))
                {
                    return CurrentTag.GetCsvToken(2, 2, ":");
                }
                string tag = CurrentTag;
                var a = tag.Split('!');
                if(a[0].ToInt2() > 0)
                    if (a.Length == 2)
                        return a[1];
                return tag;
            }
        }
        public const string STR_OrgLeadersOnly = "OrgLeadersOnly";
        public static bool OrgLeadersOnly
        {
            get { return (bool)GetSessionObj(STR_OrgLeadersOnly, false); }
            set { SetSessionObj(STR_OrgLeadersOnly, value); }
        }
        public const string STR_OrgLeadersOnlyChecked = "OrgLeadersOnlyChecked";
        public static bool OrgLeadersOnlyChecked
        {
            get { return (bool)GetSessionObj(STR_OrgLeadersOnlyChecked, false); }
            set { SetSessionObj(STR_OrgLeadersOnlyChecked, value); }
        }
        private const string STR_VisitLookbackDays = "VisitLookbackDays";
        public static int VisitLookbackDays
        {
            get { return GetSessionObj(STR_VisitLookbackDays, 180).ToInt(); }
            set { SetSessionObj(STR_VisitLookbackDays, value); }
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
                    HttpContextFactory.Current.Session[STR_MostRecentOrgs] = mru;
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
                    HttpContextFactory.Current.Session[STR_MostRecentPeople] = mru;
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
        //                    HttpContextFactory.Current.Session[STR_MostRecentQueries] = mru;
        //                }
        //                return mru;
        //            }
        //        }
        public static bool TargetLinkPeople => DbUtil.Db.UserPreference("TargetLinkPeople", true);
        public static bool TargetLinkOrg => DbUtil.Db.UserPreference("TargetLinkOrg", true);
        public static bool OnlineRegTypeSearchAdd => GetSessionObj("OnlineRegTypeSearchAdd", false).ToBool();

        //        const string STR_ActiveOrganizationId = "ActiveOrganizationId";
        //        public static int? CurrentOrgId
        //        {
        //            get
        //            {
        //                return GetSessionObj(STR_ActiveOrganizationId, null).ToInt2();
        //            }
        //            set
        //            {
        //                if (HttpContextFactory.Current != null)
        //                    HttpContextFactory.Current.Session[STR_ActiveOrganizationId] = value;
        //            }
        //        }
        public static bool UseNewFeature
        {
            get
            {
                // this works at the database level, not as a user preference
                // useful for turning the new feature on, then having a quik way to put it back in case something goes badly
                return DbUtil.Db.Setting("UseNewFeature", true);
            }
            set
            {
                DbUtil.Db.SetSetting("UseNewFeature", value ? "true" : "false");
                // be sure to SubmitChanges
            }
        }

        public static string CampusLabel => DbUtil.Db.Setting("CampusLabel", "Campus");

        public static void Log2File(string file, string data)
        {
            string fn = ConfigurationManager.AppSettings["SharedFolder"].Replace("%USERPROFILE%", Environment.GetEnvironmentVariable("USERPROFILE"));
            fn = Util.URLCombine(fn, file);
            System.IO.File.AppendAllText(fn, data);
        }
    }
}
