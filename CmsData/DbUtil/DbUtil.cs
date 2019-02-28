/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using UtilityExtensions;

namespace CmsData
{
    public static partial class DbUtil
    {
        private const string CMSDbKEY = "CMSDbKey";

        private static CMSDataContext InternalDb
        {
            get
            {
                return (CMSDataContext)HttpContextFactory.Current.Items[CMSDbKEY];
            }
            set
            {
                HttpContextFactory.Current.Items[CMSDbKEY] = value;
            }
        }

        [Obsolete("Avoid using DbUtil.Db if at all possible")]
        public static CMSDataContext Db
        {
            get
            {
                if (HttpContextFactory.Current == null)
                {
                    return CMSDataContext.Create(Util.ConnectionString, Util.Host);
                }

                if (InternalDb == null)
                {
                    InternalDb = CMSDataContext.Create(Util.ConnectionString, Util.Host);
                    InternalDb.CommandTimeout = 1200;
                }
                return InternalDb;
            }
            set
            {
                InternalDb = value;
            }
        }
        public static CMSDataContext DbReadOnly => CMSDataContext.Create(Util.ConnectionStringReadOnly, Util.Host);

        public static CMSDataContext Create(string host)
        {
            return CMSDataContext.Create(Util.GetConnectionString(host), host);
        }

        public static CMSDataContext Create(string connstr, string host)
        {
            return CMSDataContext.Create(connstr, host);
        }

        private static void _logActivity(string host, string activity, int? orgId, int? peopleId, int? datumId, int? userId, string pageUrl = null, string clientIp = null)
        {
            var ip = HttpContextFactory.Current?.Request.UserHostAddress;
            using (var db = Create(host))
            {
                if (!userId.HasValue || userId == 0)
                {
                    userId = Util.UserId;
                }

                if (userId == 0)
                {
                    userId = null;
                }

                if (orgId.HasValue && !db.PeopleIdOk(peopleId))
                {
                    peopleId = null;
                }

                if (peopleId.HasValue && !db.OrgIdOk(orgId))
                {
                    orgId = null;
                }

                var a = new ActivityLog
                {
                    ActivityDate = Util.Now,
                    UserId = userId,
                    Activity = activity.Truncate(200),
                    Machine = Environment.MachineName,
                    OrgId = orgId,
                    PeopleId = peopleId,
                    DatumId = datumId,
                    PageUrl = pageUrl,
                    ClientIp = clientIp ?? ip
                };

                db.ActivityLogs.InsertOnSubmit(a);
                db.SubmitChanges();
            }
        }

        public static void LogActivity(string activity, int? orgid = null, int? peopleid = null, int? datumId = null, int? userId = null, string pageUrl = null, string clientIp = null)
        {
            _logActivity(Util.Host, activity, orgid, peopleid, datumId, userId, pageUrl, clientIp);
        }

        public static void LogActivity(string host, string activity, int? orgid = null, int? peopleid = null, int? datumId = null, int? userId = null)
        {
            _logActivity(host, activity, orgid, peopleid, datumId, userId);
        }

        public static void LogOrgActivity(string activity, int orgid, string name)
        {
            _logActivity(Util.Host, activity, orgid, null, null, null);
            var mru = Util2.MostRecentOrgs;
            var i = mru.SingleOrDefault(vv => vv.Id == orgid);
            if (i != null)
            {
                mru.Remove(i);
            }

            mru.Insert(0, new Util2.MostRecentItem { Id = orgid, Name = name });
            if (mru.Count > 5)
            {
                mru.RemoveAt(mru.Count - 1);
            }
        }

        public static void LogPersonActivity(string activity, int pid, string name)
        {
            _logActivity(Util.Host, activity, null, pid, null, null);
            if (pid == Util.UserPeopleId)
            {
                return;
            }

            var mru = Util2.MostRecentPeople;
            var i = mru.SingleOrDefault(vv => vv.Id == pid);
            if (i != null)
            {
                mru.Remove(i);
            }

            mru.Insert(0, new Util2.MostRecentItem { Id = pid, Name = name });
            if (mru.Count > 5)
            {
                mru.RemoveAt(mru.Count - 1);
            }
        }

        public static void DbDispose()
        {
            if (InternalDb == null)
            {
                return;
            }

            InternalDb.Dispose();
            InternalDb = null;
        }

        public static string StandardExtraValues()
        {
            var s = HttpRuntime.Cache[Db.Host + "StandardExtraValues"] as string;
            if (s == null)
            {
                s = Content("StandardExtraValues.xml", "<Fields />");
                HttpRuntime.Cache.Insert(Db.Host + "StandardExtraValues", s, null,
                     DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
            }
            return s;
        }

        public static string StandardExtraValues2(bool forceread = false)
        {
            return StandardExtraValues2(Db, forceread: forceread);
        }

        public static string StandardExtraValues2(CMSDataContext db, bool forceread = false)
        {
            if (forceread)
            {
                return db.ContentText("StandardExtraValues2", "<Views />");
            }

            var s = HttpRuntime.Cache[db.Host + "StandardExtraValues2"] as string;
            if (s != null)
            {
                return s;
            }

            s = db.ContentText("StandardExtraValues2", "<Views />");
            HttpRuntime.Cache.Insert(db.Host + "StandardExtraValues2", s, null,
                DateTime.Now.AddSeconds(Util.IsDebug() ? 0 : 10), Cache.NoSlidingExpiration);
            return s;
        }

        public static void SetStandardExtraValues2(string xml)
        {
            SetStandardExtraValues2(Db, xml);
        }

        public static void SetStandardExtraValues2(CMSDataContext db, string xml)
        {
            var c = db.Content("StandardExtraValues2");
            c.Body = xml;
            HttpRuntime.Cache.Insert(db.Host + "StandardExtraValues2", xml, null,
                 DateTime.Now.AddSeconds(Util.IsDebug() ? 0 : 15), Cache.NoSlidingExpiration);
        }

        public static string FamilyExtraValues()
        {
            var s = HttpRuntime.Cache[Db.Host + "FamilyExtraValues"] as string;
            if (s == null)
            {
                s = Content("FamilyExtraValues.xml", "<Fields />");
                HttpRuntime.Cache.Insert(Db.Host + "FamilyExtraValues", s, null,
                     DateTime.Now.AddSeconds(Util.IsDebug() ? 0 : 15), Cache.NoSlidingExpiration);
            }
            return s;
        }

        public static string LoginNotice()
        {
            var hc = HttpRuntime.Cache[Db.Host + "loginnotice"] as string;
            if (hc == null)
            {
                var h = Content("LoginNotice");
                if (h != null)
                {
                    hc = h.Body;
                }
                else
                {
                    hc = string.Empty;
                }

                HttpRuntime.Cache.Insert(Db.Host + "loginnotice", hc, null,
                     DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration);
            }
            return hc;
        }

        public static string TopNotice()
        {
            var hc = HttpRuntime.Cache[Db.Host + "topnotice"] as string;
            if (hc == null)
            {
                hc = Db.ContentHtml("TopNotice", "");
                HttpRuntime.Cache.Insert(Db.Host + "topnotice", hc, null,
                     DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration);
            }
            return hc;
        }

        public static string NoticeToAdmins()
        {
            var hc = HttpRuntime.Cache[Db.Host + "Notam"] as string;
            if (hc == null)
            {
                hc = Db.ContentHtml("Notam", "");
                HttpRuntime.Cache.Insert(Db.Host + "Notam", hc, null,
                     DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration);
            }
            return hc;
        }

        public static string HeaderImage(string def)
        {
            var hc = HttpRuntime.Cache[Db.Host + "headerimg"] as string;
            if (hc == null)
            {
                var h = Content("HeaderImg");
                if (h != null)
                {
                    hc = h.Body;
                }
                else
                {
                    hc = def;
                }

                HttpRuntime.Cache.Insert(Db.Host + "headerimg", hc, null,
                     DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration);
            }
            return hc;
        }

        public static string Header()
        {
            var hc = HttpRuntime.Cache[Db.Host + "header"] as string;
            if (hc == null)
            {
                var h = Content("Header");
                if (h != null)
                {
                    hc = h.Body;
                }
                else
                {
                    hc = @"
<div id='CommonHeaderImage'>
    <a href='/'><img src='/images/headerimage.jpg' /></a>
</div>
<div id='CommonHeaderTitleBlock'>
    <h1 id='CommonHeaderTitle'>Bellevue Baptist Church</h1>
    <h2 id='CommonHeaderSubTitle'>Feed My Sheep</h2>
</div>
";
                }

                HttpRuntime.Cache.Insert(Db.Host + "header", hc, null,
                     DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration);
            }
            return hc;
        }

        public static Content Content(string name)
        {
            return Db.Contents.SingleOrDefault(c => c.Name == name);
        }

        public static Content ContentFromID(int id)
        {
            return Db.Contents.SingleOrDefault(c => c.Id == id);
        }

        public static void ContentDeleteFromID(int id)
        {
            if (id == 0)
            {
                return;
            }

            var cDelete = ContentFromID(id);
            Db.Contents.DeleteOnSubmit(cDelete);
            Db.SubmitChanges();
        }

        public static string Content(string name, string def)
        {
            var content = Db.Contents.SingleOrDefault(c => c.Name == name);
            if (content != null)
            {
                return content.Body;
            }

            return def;
        }

        public static string AdminMail => Db.Setting("AdminMail", ConfigurationManager.AppSettings["supportemail"]);
        public static string AdminMailName => Db.Setting("AdminMailName", "TouchPoint Software");
        public static string StartAddress => Db.Setting("StartAddress", "2000+Appling+Rd,+Cordova,+Tennessee+38016");
        public static bool CheckRemoteAccessRole => Db.Setting("CheckRemoteAccessRole");

        public const string MiscTagsString = "Misc Tags";
        // ReSharper disable InconsistentNaming
        public const int TagTypeId_Personal = 1;
        public const int TagTypeId_System = 2;
        public const int TagTypeId_OrgLeadersOnly = 10;
        public const int TagTypeId_OrgMembers = 3;
        public const int TagTypeId_CouplesHelper = 4;
        public const int TagTypeId_AddSelected = 5;
        public const int TagTypeId_ExtraValues = 6;
        public const int TagTypeId_Query = 7;
        public const int TagTypeId_Emailer = 8;
        public const int TagTypeId_StatusFlags = 100;
        public const int TagTypeId_QueryTags = 101;
        // ReSharper restore InconsistentNaming

        public static void UpdateValue(this object obj, List<ChangeDetail> psb, string field, object value)
        {
            if (value is string)
            {
                value = ((string)value).TrimEnd();
            }

            var o = Util.GetProperty(obj, field);
            if (o is string)
            {
                o = ((string)o).TrimEnd();
            }

            if (o == null && value == null)
            {
                return;
            }

            if (o != null && o.Equals(value))
            {
                return;
            }

            if (o == null && value is string && !((string)value).HasValue())
            {
                return;
            }

            if (value == null && o is string && !((string)o).HasValue())
            {
                return;
            }

            if (o is int && value.ToInt().Equals(o))
            {
                return;
            }

            if (o is DateTime)
            {
                if (o.Equals(value.ToDate()))
                {
                    return;
                }

                if (!o.SameMinute(value.ToDate()))
                {
                    psb.Add(new ChangeDetail(field, o, value));
                }
            }
            else
            {
                psb.Add(new ChangeDetail(field, o, value));
            }

            var s = value as string;
            if (s != null)
            {
                Util.SetPropertyFromText(obj, field, s);
            }
            else
            {
                Util.SetProperty(obj, field, value);
            }
        }

        public static DateTime? NormalizeExpires(string expires)
        {
            if (expires == null)
            {
                return null;
            }

            expires = expires.Trim();
            var re = new Regex(@"\A(?<mm>\d\d)(/|-| )?(20)?(?<yy>\d\d)\Z");
            var m = re.Match(expires);
            if (!m.Success)
            {
                return null;
            }

            DateTime dt;
            var mm = m.Groups["mm"].Value;
            var yy = m.Groups["yy"].Value;
            var s = $"{mm}/15/{yy}";
            if (!DateTime.TryParse(s, out dt))
            {
                return null;
            }

            return dt;
        }

        public class SupportPerson
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
        }

        public static List<SupportPerson> Supporters(string supporters)
        {
            var ss = supporters.Split(',').Select(s => s.Split(':'));
            return ss.Select(a => new SupportPerson
            {
                id = a[1].ToInt(),
                name = a[0].Trim(),
                email = a[2].Trim()
            }).ToList();
        }

        public static string SupporterName(string ss, int n)
        {
            var sc = Supporters(ss);
            return sc[n].name;
        }

        public static string SupporterName(string ss, string email)
        {
            var sc = Supporters(ss);
            return sc.Single(ee => ee.email == email).name;
        }

        public static SupportPerson SupporterPerson(string ss, string email)
        {
            var sc = Supporters(ss);
            return sc.Single(ee => ee.email == email);
        }
    }
}
