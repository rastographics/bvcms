/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CmsData.View;
using Community.CsharpSqlite;
using UtilityExtensions;
using System.Xml.Linq;
using System.Web.Caching;
using System.Data.SqlClient;

namespace CmsData
{
    public static partial class DbUtil
    {
        private const string CMSDbKEY = "CMSDbKey";
        private static CMSDataContext InternalDb
        {
            get
            {
                return (CMSDataContext)HttpContext.Current.Items[CMSDbKEY];
            }
            set
            {
                HttpContext.Current.Items[CMSDbKEY] = value;
            }
        }
        public static bool DatabaseExists()
        {
            var exists = (bool?)HttpRuntime.Cache[Util.Host + "-DatabaseExists"];
            if (exists.HasValue)
                return exists.Value;

            var r = CheckDatabaseExists("CMS_" + Util.Host);
            HttpRuntime.Cache.Insert(Util.Host + "-DatabaseExists", r, null,
                DateTime.Now.AddSeconds(60), Cache.NoSlidingExpiration);
            return r;
        }

        public static bool CheckDatabaseExists(string name)
        {
            using (var cn = new SqlConnection(Util.GetConnectionString2("master")))
            {
                try
                {
                    cn.Open();
                    var cmd = new SqlCommand(
                            "SELECT CAST(CASE WHEN EXISTS(SELECT NULL FROM sys.databases WHERE name = '"
                            + name + "') THEN 1 ELSE 0 END AS BIT)", cn);
                    return (bool)cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static CMSDataContext Db
        {
            get
            {
                if (HttpContext.Current == null)
                    return new CMSDataContext(Util.ConnectionString);
                if (InternalDb == null)
                {
                    InternalDb = new CMSDataContext(Util.ConnectionString);
                    InternalDb.CommandTimeout = 1200;
                }
                return InternalDb;
            }
            set
            {
                InternalDb = value;
            }
        }
        public static void LogActivity(string activity, string name = null, int? orgid = null, int? pid = null)
        {
            var db = new CMSDataContext(Util.ConnectionString);
            int? uid = Util.UserId;
            if (uid == 0)
                uid = null;
            var a = new ActivityLog
            {
                ActivityDate = Util.Now,
                UserId = uid,
                Activity = activity,
                Machine = System.Environment.MachineName,
                OrgId = orgid,
                PeopleId = pid,
            };
            db.ActivityLogs.InsertOnSubmit(a);
            db.SubmitChanges();
            db.Dispose();
            if (orgid.HasValue)
            {
                var mru = Util2.MostRecentOrgs;
                var i = mru.SingleOrDefault(vv => vv.Id == orgid);
                if (i != null)
                    mru.Remove(i);
                mru.Insert(0, new Util2.MostRecentItem() { Id = orgid.Value, Name = name });
                if (mru.Count > 5)
                    mru.RemoveAt(mru.Count - 1);
            }
            else if (pid.HasValue && pid != Util.UserPeopleId)
            {
                var mru = Util2.MostRecentPeople;
                var i = mru.SingleOrDefault(vv => vv.Id == pid);
                if (i != null)
                    mru.Remove(i);
                mru.Insert(0, new Util2.MostRecentItem() { Id = pid.Value, Name = name });
                if (mru.Count > 5)
                    mru.RemoveAt(mru.Count - 1);
            }
            //		    else if (qid.HasValue && pid != Util.UserPeopleId)
            //		    {
            //		        var mru = Util2.MostRecentQueries;
            //		        var i = mru.SingleOrDefault(vv => vv.Id == pid);
            //		        if (i != null)
            //		            mru.Remove(i);
            //		        mru.Insert(0, new Util2.MostRecentItem() { Id = pid.Value, Name = name });
            //                if (mru.Count > 5)
            //    	            mru.RemoveAt(mru.Count-1);
            //		    }
        }
        public static void DbDispose()
        {
            if (InternalDb != null)
            {
                InternalDb.Dispose();
                InternalDb = null;
            }
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
            return StandardExtraValues2(DbUtil.Db, forceread: forceread);
        }
        public static string StandardExtraValues2(CMSDataContext db, bool forceread = false)
        {
            if(forceread)
                return db.ContentText("StandardExtraValues2", "<Views />");
            var s = HttpRuntime.Cache[db.Host + "StandardExtraValues2"] as string;
            if (s != null) 
                return s;
            s = db.ContentText("StandardExtraValues2", "<Views />");
            HttpRuntime.Cache.Insert(db.Host + "StandardExtraValues2", s, null,
                DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
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
                 DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
        }
        public static string FamilyExtraValues()
        {
            var s = HttpRuntime.Cache[Db.Host + "FamilyExtraValues"] as string;
            if (s == null)
            {
                s = Content("FamilyExtraValues.xml", "<Fields />");
                HttpRuntime.Cache.Insert(Db.Host + "FamilyExtraValues", s, null,
                     DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
            }
            return s;
        }
        public static string TopNotice()
        {
            var hc = HttpRuntime.Cache[Db.Host + "topnotice"] as string;
            if (hc == null)
            {
                var h = Content("TopNotice");
                if (h != null)
                    hc = h.Body;
                else
                    hc = string.Empty;
                HttpRuntime.Cache.Insert(Db.Host + "topnotice", hc, null,
                     DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration);
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
                    hc = h.Body;
                else
                    hc = def;
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
                    hc = h.Body;
                else
                    hc = @"
<div id='CommonHeaderImage'>
    <a href='/'><img src='/images/headerimage.jpg' /></a>
</div>
<div id='CommonHeaderTitleBlock'>
    <h1 id='CommonHeaderTitle'>Bellevue Baptist Church</h1>
    <h2 id='CommonHeaderSubTitle'>Feed My Sheep</h2>
</div>
";
                HttpRuntime.Cache.Insert(Db.Host + "header", hc, null,
                     DateTime.Now.AddMinutes(3), Cache.NoSlidingExpiration);
            }
            return hc;
        }

        public static Content Content(string name)
        {
            return DbUtil.Db.Contents.SingleOrDefault(c => c.Name == name);
        }

        public static Content ContentFromID(int id)
        {
            return DbUtil.Db.Contents.SingleOrDefault(c => c.Id == id);
        }

        public static void ContentDeleteFromID(int id)
        {
            if (id == 0) return;

            Content cDelete = ContentFromID(id);
            DbUtil.Db.Contents.DeleteOnSubmit(cDelete);
            DbUtil.Db.SubmitChanges();
        }

        public static string Content(string name, string def)
        {
            var content = DbUtil.Db.Contents.SingleOrDefault(c => c.Name == name);
            if (content != null)
                return content.Body;
            return def;
        }

        public static string SystemEmailAddress { get { return Db.Setting("SystemEmailAddress", ""); } }
        public static string AdminMail { get { return Db.Setting("AdminMail", SystemEmailAddress); } }
        public static string StartAddress { get { return Db.Setting("StartAddress", "2000+Appling+Rd,+Cordova,+Tennessee+38016"); } }
        public static bool CheckRemoteAccessRole { get { return Db.Setting("CheckRemoteAccessRole", "") == "true"; } }

        public const string MiscTagsString = "Misc Tags";
        public const int TagTypeId_Personal = 1;
        public const int TagTypeId_OrgMembersOnly = 3;
        public const int TagTypeId_OrgLeadersOnly = 6;
        public const int TagTypeId_CouplesHelper = 4;
        public const int TagTypeId_AddSelected = 5;
        public const int TagTypeId_ExtraValues = 6;
        public const int TagTypeId_Query = 7;
        public const int TagTypeId_Emailer = 8;
        public const int TagTypeId_StatusFlags = 9;

        public static void UpdateValue(this object obj, StringBuilder psb, string field, object value)
        {
            if (value is string)
                value = ((string)value).TrimEnd();
            var o = Util.GetProperty(obj, field);
            if (o is string)
                o = ((string)o).TrimEnd();
            if (o == null && value == null)
                return;
            if (o != null && o.Equals(value))
                return;
            if (o == null && value is string && !((string)value).HasValue())
                return;
            if (value == null && o is string && !((string)o).HasValue())
                return;
            if (o is int && value.ToInt().Equals(o))
                return;
            if (o is DateTime && o.Equals(value.ToDate()))
                return;
            psb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", field, o, value ?? "(null)");
            if (value is string)
                Util.SetPropertyFromText(obj, field, (string)value);
            else
                Util.SetProperty(obj, field, value);
        }
        public static void CreateDatabase()
        {
            var Server = HttpContext.Current.Server;
            HttpRuntime.Cache.Remove(Util.Host + "-DatabaseExists");
            string cs = Util.GetConnectionString2("master");
            RunScripts(cs, "create database CMS_" + Util.Host);
            var needimage = !CheckDatabaseExists("CMSi_" + Util.Host);
            if (needimage)
                RunScripts(cs, "create database CMSi_" + Util.Host);
            var needelmah = !DbUtil.CheckDatabaseExists("Elmah");
            if (needelmah)
                RunScripts(cs, "create database Elmah");
            var path = Server.MapPath("/");
            RunScripts(Util.ConnectionString, Util.Host == "testdb"
                    ? File.ReadAllText(path + @"..\SqlScripts\BuildTestDatabase.sql")
                    : File.ReadAllText(path + @"..\SqlScripts\BuildStarterDatabase.sql"));
            RunScripts(Util.ConnectionString, File.ReadAllText(path + @"..\SqlScripts\InsertZipCodes.sql"));
            if (needimage)
                RunScripts(Util.ConnectionStringImage, File.ReadAllText(path + @"..\SqlScripts\BuildImageDatabase.sql"));
            if (needelmah)
                RunScripts(Util.GetConnectionString2("Elmah"), File.ReadAllText(path + @"..\SqlScripts\BuildElmahDb.sql"));
        }

        private static void RunScripts(string cs, string script)
        {
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                var cmd = new SqlCommand { Connection = cn };
                var scripts = Regex.Split(script, "\r\nGO\r\n", RegexOptions.Multiline);
                foreach (var s in scripts)
                    if(s.HasValue())
                    { 
                        cmd.CommandText = s;
                        cmd.ExecuteNonQuery();
                    }
            }
        }
        public static DateTime? NormalizeExpires(string expires)
        {
            if (expires == null)
                return null;
            expires = expires.Trim();
            var re = new Regex(@"\A(?<mm>\d\d)(/|-| )?(20)?(?<yy>\d\d)\Z");
            var m = re.Match(expires);
            if (!m.Success)
                return null;
            DateTime dt;
            var mm = m.Groups["mm"].Value;
            var yy = m.Groups["yy"].Value;
            var s = "{0}/15/{1}".Fmt(mm, yy);
            if (!DateTime.TryParse(s, out dt))
                return null;
            return dt;
        }
    }
}
