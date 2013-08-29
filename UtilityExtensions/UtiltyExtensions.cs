/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Configuration;
using System.Net.Mail;
using System.Web.UI;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;
using System.Net;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Diagnostics;
using System.Web.Caching;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace UtilityExtensions
{
    public static partial class Util
    {

        public static T QueryString<T>(this System.Web.UI.Page page, string param)
        {
            return QueryString<T>(HttpContext.Current.Request, param);
        }
        public static T QueryString<T>(this System.Web.HttpRequest req, string param)
        {
            if (req.QueryString[param].HasValue())
                return (T)req.QueryString[param].ChangeType(typeof(T));
            return default(T);
        }
        public static object ChangeType(this object value, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                var conv = new NullableConverter(type);
                type = conv.UnderlyingType;
            }
            return Convert.ChangeType(value, type);
        }
        public static int ToInt(this string s)
        {
            int i = 0;
            int.TryParse(s, out i);
            return i;
        }
        public static long ToLong(this string s)
        {
            long i = 0;
            long.TryParse(s, out i);
            return i;
        }
        public static long? ToLong2(this string s)
        {
            long ii;
            if (long.TryParse(s, out ii))
                return ii;
            return null;
        }
        public static DateTime? ToDate(this string s)
        {
            if (s != null && s.AllDigits() && s.Length == 8)
            {
                var d = ParseMMddyy(s);
                if (d.HasValue)
                    return GoodDate(d);
            }
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return GoodDate(dt);
            return null;
        }
        private static DateTime? GoodDate(DateTime? dt)
        {
            if (!dt.HasValue)
                return null;
            while (dt.Value.Year < 1900)
                dt = dt.Value.AddYears(100);
            return dt;
        }
        public static DateTime? ToDate(this object o)
        {
            return o.ToString().ToDate();
        }
        public static int? ToInt2(this string s)
        {
            int? r = null;
            int i;
            if (int.TryParse(s, out i))
                r = i;
            return r;
        }
        public static bool? ToBool2(this string s)
        {
            bool b;
            bool.TryParse(s, out b);
            return b;
        }
        public static bool ToBool(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            bool b;
            bool.TryParse(s, out b);
            return b;
        }
        public static int? ToInt2(this object o)
        {
            int? r = null;
            if (o == null || o == DBNull.Value)
                return r;
            if (o.ToString() == "")
                return r;
            return (int)o.ChangeType(typeof(int));
        }
        public static int ToInt(this object o)
        {
            if (o == null || o == DBNull.Value)
                return 0;
            return (int)o.ChangeType(typeof(int));
        }
        public static decimal? ToDecimal(this string s)
        {
            decimal? r = null;
            decimal i;
            if (decimal.TryParse(s, out i))
                r = i;
            return r;
        }
        public static float ToFloat(this string s)
        {
            var r = 0f;
            float i;
            if (float.TryParse(s, out i))
                r = i;
            return r;
        }
        public static bool IsNull(this object o)
        {
            return o == null;
        }
        public static bool IsNotNull(this object o)
        {
            return o != null;
        }
        public static string DefaultTo(this string s, string defaultstr)
        {
            if (s.HasValue())
                return s;
            else
                return defaultstr;
        }
        public static string[] SplitStr(this string s, string delimiter)
        {
            if (s == null)
                return "".Split(delimiter.ToCharArray());
            return s.Split(delimiter.ToCharArray());
        }
        public static string[] SplitStr(this string s, string delimiter, int nitems)
        {
            return s.Split(delimiter.ToCharArray(), nitems);
        }
        public static int? IntOrNull(this string s)
        {
            int? i = null;
            if (s.HasValue())
                i = int.Parse(s);
            return i;
        }
        public static bool HasValue(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }
        public static string Fmt(this string fmt, params object[] p)
        {
            return string.Format(fmt, p);
        }
        public static bool Equal(this string s, string s2)
        {
            return string.Compare(s, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool NotEqual(this string s, string s2)
        {
            return !s.Equal(s2);
        }

        public static string Truncate(this string source, int length)
        {
            if (source.HasValue() && source.Length > length)
                source = source.Substring(0, length).Trim();
            return source;
        }
        public static string trim(this string source)
        {
            if (source != null)
                source = source.Trim();
            return source;
        }

        public static string EmailHref(this string addr, string name)
        {
            if (!addr.HasValue())
                return "";
            if (name.HasValue())
                return "mailto:{0} <{1}>".Fmt(name, addr);
            return "mailto:" + addr;
        }

        public static string FormatBirthday(int? y, int? m, int? d)
        {
            return FormatBirthday(y, m, d, "");
        }

        public static string FormatBirthday(int? y, int? m, int? d, string def)
        {
            try
            {
                if (m.HasValue && d.HasValue)
                    if (!y.HasValue)
                        return new DateTime(2000, m.Value, d.Value).ToString("m");
                    else
                        return new DateTime(y.Value, m.Value, d.Value).ToString("d");
                if (y.HasValue)
                    if (m.HasValue && !d.HasValue)
                        return new DateTime(y.Value, m.Value, 1).ToString("y");
                    else
                        return y.ToString();
            }
            catch (Exception)
            {
                return "bad date {0}/{1}/{2}".Fmt(m ?? 0, d ?? 0, y ?? 0);
            }
            return def;
        }
        public static string FormatDate(this DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToString("d");
            return "";
        }
        public static string FormatDate(dynamic dyndt)
        {
            var dt = dyndt as DateTime?;
            if (dt.HasValue)
                return dt.Value.ToString("d");
            return "";
        }

        public static string ToSortableDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        public static string ToSortableDate(this DateTime? dt)
        {
            return dt.ToString2("yyyy-MM-dd");
        }

        public static string FormatDate(this DateTime? dt, string def = "")
        {
            if (dt.HasValue)
                return dt.Value.ToString("d");
            return def;
        }
        public static string FormatDateTm(this DateTime dt)
        {
            return dt.ToString("g");
        }
        public static string FormatDateTm(this DateTime? dt)
        {
            return dt.FormatDateTm(null);
        }
        public static string FormatDateTm(this DateTime? dt, string def)
        {
            if (dt.HasValue)
                return dt.ToString2("g");
            return def;
        }
        public static string ToString2(this int? i, string fmt)
        {
            if (i.HasValue)
                return i.Value.ToString(fmt);
            return "";
        }
        public static string ToString2(this decimal? d, string fmt)
        {
            if (d.HasValue)
                return d.Value.ToString(fmt);
            return "";
        }
        public static string ToString2(this DateTime? d, string fmt)
        {
            if (d.HasValue)
                return d.Value.ToString(fmt);
            return "";
        }
        public static bool DateTryParse(this string date, out DateTime dt)
        {
            return DateTime.TryParse(date, out dt);
        }

        public static string Age(this string birthday)
        {
            DateTime bd;
            if (!birthday.DateTryParse(out bd))
                return "?";
            DateTime td = Now;
            int age = td.Year - bd.Year;
            if (td.Month < bd.Month || (td.Month == bd.Month && td.Day < bd.Day))
                age--;
            return age.ToString();
        }
        public static int Age0(this string birthday)
        {
            DateTime bd;
            if (!birthday.DateTryParse(out bd))
                return -1;
            DateTime td = Now;
            int age = td.Year - bd.Year;
            if (td.Month < bd.Month || (td.Month == bd.Month && td.Day < bd.Day))
                age--;
            return age;
        }
        public static string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }
        public static int AgeAsOf(this DateTime bd, DateTime dt)
        {
            int y = bd.Year;
            if (y < 1000)
                if (y < 50)
                    y = y + 2000;
                else y = y + 1900;
            int age = dt.Year - y;
            if (dt.Month < bd.Month || (dt.Month == bd.Month && dt.Day < bd.Day))
                age--;
            return age;
        }
        public static DateTime Now
        {
            get { return DateTime.Now; }
        }
        public static string FormatCSZ(string city, string st, string zip)
        {
            string csz = city ?? string.Empty;
            if (st.HasValue())
                csz += ", " + st;
            if (zip.HasValue())
                csz += " " + zip.FmtZip();
            return csz.Trim();
        }
        public static string FormatCSZ4(string city, string st, string zip)
        {
            string csz = city ?? string.Empty;
            if (st.HasValue())
                csz += ", " + st;
            csz += " " + FmtZip(zip);
            return csz.Trim();
        }
        public static string FormatCSZ5(string city, string st, string zip)
        {
            string csz = city ?? string.Empty;
            if (st.HasValue())
                csz += ", " + st;
            csz += " " + Zip5(zip);
            return csz.Trim();
        }
        public static string FmtFone(this string phone, string prefix)
        {
            phone = phone.FmtFone();
            if (phone.HasValue())
                return prefix + " " + phone;
            return "";
        }
        public static string FmtFone7(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return "";
            var ph = GetDigits(phone).PadLeft(10, '0');
            var p7 = ph.Substring(3);
            var t = new StringBuilder(p7);
            if (t.Length >= 4)
                t.Insert(3, "-");
            return t.ToString();
        }
        public static string FmtFone7(this string phone, string prefix)
        {
            phone = phone.FmtFone7();
            if (phone.HasValue())
                return prefix + phone;
            return "";
        }
        public static bool AllDigits(this string str)
        {
            if (!str.HasValue())
                return false;
            var patt = new Regex("[^0-9]");
            return !(patt.IsMatch(str));
        }
        public static string FmtFone(this string phone)
        {
            var ph = phone.GetDigits();
            if (!ph.HasValue())
                return "";
            if (ph.Length == 7)
                return Regex.Replace(ph, @"(\d{3})(\d{4})", "$1-$2").Trim();
            if (ph.Length == 10)
                return Regex.Replace(ph, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3").Trim();
            if (ph.Length > 10)
                return Regex.Replace(ph, @"(\d{3})(\d{3})(\d{4})(\d*)", "$1-$2-$3 $4").Trim();
                return phone;
        }
        public static string GetDigits(this string zip, int maxlen = 99)
        {
            if (!zip.HasValue())
                return "";
            var digits = new StringBuilder();
            foreach (var c in zip.ToCharArray())
                if (Char.IsDigit(c))
                    digits.Append(c);
            return digits.ToString().Truncate(maxlen);
        }
        public static decimal? GetAmount(this string s)
        {
            if (!s.HasValue())
                return null;
            var digits = new StringBuilder();
            foreach (var c in s.ToCharArray())
                if (Char.IsDigit(c) || c == '.')
                    digits.Append(c);
            var a = digits.ToString().ToDecimal();
            return a;
        }
        public static string FmtZip(this string zip)
        {
            if (!zip.HasValue())
                return "";
            var t = new StringBuilder(zip.GetDigits());
            if (t.Length != 9 && t.Length != 5)
                return zip;
            if (t.Length > 5)
                t.Insert(5, "-");
            return t.ToString();
        }
        public static string Zip5(this string zip)
        {
            if (!zip.HasValue())
                return "";
            var t = zip.GetDigits();
            if (t.Length != 9 && t.Length != 5)
                return zip;
            if (t.Length > 5)
                return t.Substring(0, 5);
            return t;
        }

        public static bool DateValid(string dt)
        {
            DateTime dt2;
            return DateValid(dt, out dt2);
        }

        public static bool DateValid(string dt, out DateTime dt2)
        {
            dt2 = DateTime.MinValue;
            if (!dt.HasValue())
                return false;

            if (DateTime.TryParse(dt, out dt2))
                return true;
            if (!Regex.IsMatch(dt, @"\A(?:\A(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])((19|20)?[0-9]{2}))\Z"))
                return false;
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var styles = DateTimeStyles.None;
            var s = dt.Substring(0, 2) + "/" + dt.Substring(2, 2) + "/" + dt.Substring(4);
            if (DateTime.TryParse(s, culture, styles, out dt2))
                return true;
            return false;
        }
        public static bool BirthDateValid(string dt, out DateTime dt2)
        {
            dt2 = DateTime.MinValue;
            if (!DateValid(dt, out dt2))
                return false;
            if (dt2 > DateTime.Now)
                dt2 = dt2.AddYears(-100);
            return true;
        }

        public static string FmtAttendStr(this string attendstr)
        {
            if (!attendstr.HasValue())
                return " ";
            return attendstr;
        }
        private const string STR_Host = "HostString";
        public static string Host
        {
            get
            {
                var h = ConfigurationManager.AppSettings["host"];
                if (h.HasValue())
                    return h;
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.Url.Authority.SplitStr(".:")[0];
                return null;
            }
        }

        public static string CmsHost2
        {
            get
            {
                var h = ConfigurationManager.AppSettings["cmshost"];
                return h.Replace("{church}", Host, ignoreCase: true);
            }
        }
        private const string STR_ConnectionString = "ConnectionString";
        public static string ConnectionString
        {
            get
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_ConnectionString] != null)
                            return HttpContext.Current.Session[STR_ConnectionString].ToString();
                var cs = ConfigurationManager.ConnectionStrings["CMSHosted"];
                if (cs == null)
                    return ConfigurationManager.ConnectionStrings["CMS"].ConnectionString;

                var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
                cb.InitialCatalog = "CMS_{0}".Fmt(Host);
                return cb.ConnectionString;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_ConnectionString] = value;
            }
        }
        public static bool IsHosted
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["CMSHosted"];
                return cs != null;
            }
        }
        private const string STR_IsSage = "IsSage";
        public static bool? IsSage
        {
            get
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Items[STR_IsSage] != null)
                        return (bool)HttpContext.Current.Items[STR_IsSage];
                return null;
            }
            set
            {
                HttpContext.Current.Items[STR_IsSage] = value;
            }
        }
        public static string jQueryDateFormat
        {
            get
            {
                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                if (s.StartsWith("30"))
                    return "d/m/yy";
                return "m/d/yy";
            }
        }
        public static string jQueryDateValidation
        {
            get
            {
                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                if (s.StartsWith("30"))
                    return @"^\d\d?[-/](0?[1-9]|1[012])[-/]\d\d(\d\d)?$";
                return @"^(0?[1-9]|1[012])[-/]\d\d?[-/]\d\d(\d\d)?$";
            }
        }
        public static string jQueryDateFormat2
        {
            get
            {

                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                if (s.StartsWith("30"))
                    return "D/M/YYYY";
                return "M/D/YYYY";
            }
        }
        public static string jQueryNumberValidation
        {
            get
            {
                const decimal c = 1.23M;
                if (c.ToString("c").StartsWith("1,23"))
                    return @"^-?(?:\d+)?(?:,\d+)?$";
                return @"^-?(?:\d+)?(?:\.\d+)?$";
            }
        }
        public static string GetConnectionString(string Host)
        {
            var cs = ConfigurationManager.ConnectionStrings["CMSHosted"];
            if (cs == null)
                cs = ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            var a = Host.SplitStr(".:");
            cb.InitialCatalog = "CMS_{0}".Fmt(a[0]);
            return cb.ConnectionString;
        }
        public static string GetMasterConnectionString()
        {
            var cs = ConfigurationManager.ConnectionStrings["CMSHosted"];
            if (cs == null)
                cs = ConfigurationManager.ConnectionStrings["CMS"];
            var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
            cb.InitialCatalog = "master";
            return cb.ConnectionString;
        }

        public static string ConnectionStringImage
        {
            get
            {
#if DEBUG
                var dcs = ConfigurationManager.ConnectionStrings["CMSImage"];
                if (dcs != null)
                    return dcs.ConnectionString;
#endif
                var cs = ConfigurationManager.ConnectionStrings["CMSHosted"];
                if (cs == null)
                    cs = ConfigurationManager.ConnectionStrings["CMS"];
                var cb = new SqlConnectionStringBuilder(cs.ConnectionString);
                var a = Host.SplitStr(".:");
                cb.InitialCatalog = "CMSi_{0}".Fmt(a[0]);
                return cb.ConnectionString;
            }
        }
        public static string UserName
        {
            get
            {
                if (HttpContext.Current != null)
                    return GetUserName(HttpContext.Current.User.Identity.Name);
                return ConfigurationManager.AppSettings["TestName"];
            }
        }
        private const string STR_UserId = "UserId";
        public static int UserId
        {
            get
            {
                int id = 0;
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_UserId] != null)
                            id = HttpContext.Current.Session[STR_UserId].ToInt();
                if (id == 0)
                    id = ConfigurationManager.AppSettings["TestId"].ToInt();
                return id;
            }
            set
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session[STR_UserId] = value;
            }
        }
        private const string STR_UserPreferredName = "UserPreferredName";
        public static string UserPreferredName
        {
            get
            {
                string name = null;
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_UserPreferredName] != null)
                            name = HttpContext.Current.Session[STR_UserPreferredName] as String;
                return name;
            }
            set
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session[STR_UserPreferredName] = value;
            }
        }
        private const string STR_UserFullName = "UserFullName";
        public static string UserFullName
        {
            get
            {
                string name = "-";
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_UserFullName] != null)
                            name = HttpContext.Current.Session[STR_UserFullName] as String;
                return name;
            }
            set
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session[STR_UserFullName] = value;
            }
        }
        public static int UserId1
        {
            get { return UserId == 0 ? 1 : UserId; }
        }
        public static bool IsInRole(string role)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.User.IsInRole(role);
            return false;
        }
        private const string STR_UserPeopleId = "UserPeopleId";
        public static int? UserPeopleId
        {
            get
            {
                int? id = null;
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_UserPeopleId] != null)
                            id = HttpContext.Current.Session[STR_UserPeopleId].ToInt();
                return id ?? UserId1;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_UserPeopleId] = value;
            }
        }

        public static string GetUserName(string name)
        {
            if (name == null)
                return null;
            var a = name.Split('\\');
            if (a.Length == 2)
                return a[1];
            return a[0];
        }
        public static string ScratchPad
        {
            get { return "<--ScratchPad-->"; }
        }
        const string STR_QBScratchPad = "QBScratchPad";
        public static int QueryBuilderScratchPadId
        {
            get
            {
                int id = 0;
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_QBScratchPad] != null)
                            id = HttpContext.Current.Session[STR_QBScratchPad].ToInt();
                return id;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_QBScratchPad] = value;
            }
        }
        public static int CreateAccountCode = -1952;
        public static string SessionId
        {
            get
            {
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        return HttpContext.Current.Session.SessionID;
                return (string)HttpRuntime.Cache["SessionId"];
            }
            set { HttpRuntime.Cache["SessionId"] = value; }
        }
        private const string STR_SessionStarting = "SessionStarting";
        public static bool SessionStarting
        {
            get
            {
                bool tf = false;
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_SessionStarting] != null)
                            tf = (bool)HttpContext.Current.Session[STR_SessionStarting];
                return tf;
            }
            set { HttpContext.Current.Session[STR_SessionStarting] = value; }
        }
        private const string STR_FormsBasedAuthentication = "FormsBasedAuthentication";
        public static bool FormsBasedAuthentication
        {
            get
            {
                object tf = HttpContext.Current.Session[STR_FormsBasedAuthentication];
                if (tf.IsNotNull())
                    return (bool)tf;
                else return false;
            }
            set { HttpContext.Current.Session[STR_FormsBasedAuthentication] = value; }
        }
        private const string STR_Auditing = "Auditing";
        public static bool Auditing
        {
            get
            {
                object tf = HttpContext.Current.Items[STR_Auditing];
                if (tf.IsNotNull())
                    return (bool)tf;
                else return true;
            }
            set { HttpContext.Current.Items[STR_Auditing] = value; }
        }
        private const string STR_Helpfile = "Helpfile";
        public static string Helpfile
        {
            get
            {
                var tag = "MainPage";
                if (HttpContext.Current.Session[STR_Helpfile] != null)
                    tag = HttpContext.Current.Session[STR_Helpfile].ToString();
                return tag;
            }
            set
            {
                HttpContext.Current.Session[STR_Helpfile] = value;
            }
        }
        public static string HelpLink(string page)
        {
            var h = ConfigurationManager.AppSettings["helpurl"];
            return h.Fmt(page);
        }

        public static string SafeFormat(string s)
        {
            if (s == null)
                return null;
            s = HttpUtility.HtmlEncode(s);


            //s = Regex.Replace(s, "([^ ]*)=(https?://[^ ]*)", "<a target=\"_new\" href=\"$2\">$1</a>", RegexOptions.Singleline);
            //s = Regex.Replace(s, "[^=](https?://[^\\s]*)", "<a target=\"_new\" href=\"$1\">$1</a>", RegexOptions.Singleline);

            s = Regex.Replace(s, "(http://([^\\s]*))", "<a target=\"_new\" href=\"$1\">$1</a>", RegexOptions.Singleline);
            s = Regex.Replace(s, "(https://([^\\s]*))", "<a target=\"_new\" href=\"$1\">$1</a>", RegexOptions.Singleline);
            s = HtmlFormat(s, "\\*\\*\\*", "u");
            s = HtmlFormat(s, "\\*\\*", "b");
            s = HtmlFormat(s, "\\*", "i");
            s = Regex.Replace(s, @"&quot;(.*?)&quot;\s*=\s*&quot;(.*?)&quot;",
                "<a target=\"_new\" href=\"http://$2\">$1</a>", RegexOptions.Singleline);
            s = Regex.Replace(s, "&gt;&gt;&gt;(?:\r\n)?(.*?)(?:\r\n)?&lt;&lt;&lt;(?:\r\n)?",
                "<blockquote>$1</blockquote>", RegexOptions.Singleline);
            s = s.Replace(System.Environment.NewLine, "\n");
            return s.Replace("\n", "<br>\r\n");
        }

        private static string HtmlFormat(string s, string lookfor, string htmlcode)
        {
            return Regex.Replace(s, "{0}(.*?){0}".Fmt(lookfor),
                "<{0}>$1</{0}>".Fmt(htmlcode), RegexOptions.Singleline);
        }
        public const string STR_Preferences = "Preferences";
        public const string STR_PageSize = "PageSize";
        public static void SetPageSizeCookie(int value)
        {
            var cookie = new HttpCookie(STR_Preferences);
            cookie.Values[STR_PageSize] = value.ToString();
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        public static int GetPageSizeCookie()
        {
            HttpRequest r = null;
            if (HttpContext.Current != null)
                r = HttpContext.Current.Request;
            if (r != null && r.Cookies[STR_Preferences] != null)
            {
                var cookie = r.Cookies[STR_Preferences];
                if (cookie != null && cookie.Values[STR_PageSize] != null)
                    return cookie.Values[STR_PageSize].ToInt();
            }
            return 10;
        }
        public static string IpAddress()
        {
            string strIpAddress;
            strIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (strIpAddress == null)
                strIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return strIpAddress;
        }
        public static void Cookie(string name, string value, int days)
        {
            if (Cookie(name) == value)
                return;
            var c = new HttpCookie(name, value);
            c.Expires = DateTime.Now.AddDays(days);
            HttpContext.Current.Response.Cookies.Add(c);
            HttpContext.Current.Items["tCookie-" + name] = value;
        }
        public static string Cookie(string name)
        {
            return Cookie(name, null);
        }
        public static string Cookie(string name, string defaultValue)
        {
            var v = (string)HttpContext.Current.Items["tCookie-" + name];
            if (v.HasValue())
                return v;
            var c = HttpContext.Current.Request.Cookies[name];
            if (c != null && c.Value.HasValue())
                return c.Value;
            return defaultValue;
        }
        public static void EndShowMessage(this HttpResponse Response, string message)
        {
            Response.EndShowMessage(message, "javascript: history.go(-1)", "Go Back");
        }
        public static void EndShowMessage(this HttpResponse Response, string message, string href, string text)
        {
            Response.Clear();
            Response.Write(EndShowMessage(message, href, text));
            Response.End();
        }
        public static string EndShowMessage(string message, string href, string text)
        {
            return "<h3 style='color:red'>{0}</h3>\n<a href='{1}'>{2}</a>".Fmt(message, href, text);
        }
        public static string FullEmail(string Email, string Name)
        {
            if (Email.HasValue())
                if (Name.Contains("?"))
                    return Email;
                else
                {
                    var a = Email.SplitStr(",;");
                    var q = from ad in a
                            select Name.Replace(",", "").Replace(";", "") + " <" + ad.Trim() + ">";
                    return string.Join(";", q);
                }
            return String.Empty;
        }
        public static List<MailAddress> DistinctEmails(this List<MailAddress> list)
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null)
                    for (var j = i + 1; j < list.Count; j++)
                        if (list[j] != null)
                            if (string.Compare(list[i].Address, list[j].Address, ignoreCase: true) == 0)
                                list[j] = null;
            return list.Where(ll => ll != null).ToList();
        }
        private class MailAddressComparer : IEqualityComparer<MailAddress>
        {
            public bool Equals(MailAddress x, MailAddress y)
            {
                if (x == null)
                    return y == null;
                if (y == null)
                    return false;
                var eq = string.Compare(x.Address, y.Address, ignoreCase: true) == 0;
                return eq;
            }
            public int GetHashCode(MailAddress obj)
            {
                return obj.Address.ToLower().GetHashCode();
            }
        }
        public static List<MailAddress> ToMailAddressList(string addresses)
        {
            var list = new List<MailAddress>();
            foreach (var ad in addresses.SplitStr(",;"))
                AddGoodAddress(list, ad);
            return list;
        }
        public static List<MailAddress> ToMailAddressList(string address, string name)
        {
            return ToMailAddressList(Util.TryGetMailAddress(address, name));
        }
        public static List<MailAddress> ToMailAddressList(MailAddress ma)
        {
            return new List<MailAddress> { ma };
        }
        public static void AddGoodAddress(List<MailAddress> list, string a)
        {
            var ma = Util.TryGetMailAddress(a);
            if (ma != null)
                if (!list.Any(mm => mm.Address == a))
                    list.Add(ma);
        }
        public static string EmailAddressListToString(this List<MailAddress> list)
        {
            var addrs = string.Join(", ", list.Select(tt => tt.ToString()));
            return addrs;
        }
        public static List<MailAddress> SendErrorsTo()
        {
            var a = ConfigurationManager.AppSettings["senderrorsto"];
            return EmailAddressListFromString(a);
        }
        public static List<MailAddress> EmailAddressListFromString(string addresses)
        {
            var a = addresses.SplitStr(",;");
            var list = new List<MailAddress>();
            foreach (var ad in a)
                AddGoodAddress(list, ad);
            return list;
        }
        public static MailAddress TryGetMailAddress(string address)
        {
            if (address.HasValue())
                address = address.Trim();
            try
            {
                var ma = new MailAddress(address);
                if (ValidEmail(ma.Address))
                    return ma;
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static MailAddress TryGetMailAddress(string address, string name)
        {
            if (address.HasValue())
                address = address.Trim();
            if (name.HasValue())
                name = name.Replace("\"", "");
            if (ValidEmail(address))
                return Util.FirstAddress(address, name);
            else
                return null;
        }
        public static bool ValidEmail(string email)
        {
            if (!email.HasValue())
                return false;
            var re1 = new Regex(@"^(.*\b(?=\w))\b[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+\.[A-Z]{2,4}\b\b(?!\w)$", RegexOptions.IgnoreCase);
            var re2 = new Regex(@"^[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
            var a = email.SplitStr(",;");
            foreach (var m in a)
            {
                var b = re1.IsMatch(m) || re2.IsMatch(m);
                if (b)
                    return true; // at least one good email address
            }
            return false;
        }
        public static bool IsLocalNetworkRequest
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.IsLocal)
                        return true;
                    string hostPrefix = HttpContext.Current.Request.UserHostAddress;
                    string[] ipClass = hostPrefix.Split(new char[] { '.' });
                    int classA = Convert.ToInt16(ipClass[0]);
                    int classB = Convert.ToInt16(ipClass[1]);
                    if (classA == 10 || classA == 127)
                        return true;
                    else if (classA == 192 && classB == 168)
                        return true;
                    else if (classA == 172 && (classB > 15 && classB < 33))
                        return true;
                    return false;
                }
                return false;
            }
        }
        public static void NoCache(this HttpResponse Response)
        {
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetValidUntilExpires(false);
        }
        public static void NoCache(this HttpResponseBase Response)
        {
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetValidUntilExpires(false);
        }

        public static string AppRoot
        {
            get
            {
                var approot = Util.ResolveUrl("~");
                if (approot == "/")
                    approot = "";
                return approot;
            }
        }
        public static void ShowError(string message)
        {
            HttpContext.Current.Response.Redirect(
                "/Home/ShowError/?error={0}&url={1}".Fmt(HttpContext.Current.Server.UrlEncode(message),
                HttpContext.Current.Request.Url.OriginalString));
        }
        public static string ResolveUrl(string originalUrl)
        {
            if (originalUrl == null)
                return null;
            if (originalUrl.IndexOf("://") != -1)
                return originalUrl;
            if (originalUrl.StartsWith("~"))
                return VirtualPathUtility.ToAbsolute(originalUrl);
            return originalUrl;
        }
        public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;
            var newUrl = ResolveUrl(serverUrl);
            var originalUri = HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : Scheme()) +
                     "://" + originalUri.Authority + newUrl;
            return newUrl;
        }
        public static string ResolveServerUrl(string serverUrl)
        {
            return ResolveServerUrl(serverUrl, false);
        }
        public static string ServerLink(string path)
        {
            var Request = HttpContext.Current.Request;
            return Scheme() + "://" + Request.Url.Authority + path;
        }
        public static string Scheme()
        {
            var Request = HttpContext.Current.Request;
            var scheme = Request.Url.Scheme;
            if (Request.Headers["X-Forwarded-Proto"] == "https")
                scheme = "https";
            return scheme;
        }
        public static string ToSuitableId(this string s)
        {
            return s.Replace('[', '_').Replace(']', '_').Replace(' ', '_').Replace(',', '_');
        }
        public static string PickFirst(params string[] args)
        {
            foreach (var s in args)
                if (s.HasValue())
                    return s;
            return "";
        }
        public static string Disallow(this string value, string dissallow)
        {
            var v = value ?? "";
            value = v.Trim();
            if (value.Equal(dissallow))
                return "";
            return value;
        }
        public static string ObscureEmail(string email)
        {
            if (!ValidEmail(email))
                return email;
            var a = email.Split('@');
            var rest = new string('.', 6);
            return a[0].Substring(0, 1) + rest + "@" + a[1];
        }
        public static string ObscureAccount(string acct)
        {
            var rest = new string('x', acct.Length - 4);
            return rest + acct.Substring(acct.Length - 4);
        }
        public static SmtpClient Smtp()
        {
            var smtp = new SmtpClient();
            if (ConfigurationManager.AppSettings["requiresSsl"] == "true")
                smtp.EnableSsl = true;
            if (SmtpDebug)
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = @"c:\email";
                smtp.Host = "localhost";
            }
            return smtp;
        }
        private const string STR_AdminMail = "UnNamed";
        public static string AdminMail
        {
            get
            {
                var tag = ConfigurationManager.AppSettings["senderrorsto"];
                if (HttpContext.Current != null)
                    if ((HttpContext.Current.Items[STR_AdminMail] as string).HasValue())
                        tag = HttpContext.Current.Items[STR_AdminMail].ToString();
                return tag;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items[STR_AdminMail] = value;
            }
        }
        private const string STR_SysFromEmail = "UnNamed";
        public static string SysFromEmail
        {
            get
            {
                var tag = ConfigurationManager.AppSettings["sysfromemail"];
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_SysFromEmail] != null)
                            tag = HttpContext.Current.Session[STR_SysFromEmail].ToString();
                return tag;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_SysFromEmail] = value;
            }
        }
        private const string STR_Version = "Version";
        public static string Version
        {
            get
            {
                var version = "?";
                if (HttpContext.Current != null)
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_Version] != null)
                            version = HttpContext.Current.Session[STR_Version].ToString();
                return version;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[STR_Version] = value;
            }
        }

        public static void Serialize<T>(T m, XmlWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, m);
        }
        public static string Serialize<T>(T m)
        {
            var sw = new StringWriter();
            new XmlSerializer(typeof(T)).Serialize(sw, m);
            return sw.ToString();
        }
        public static T DeSerialize<T>(string s) where T : class
        {
            var sr = new StringReader(s);
            return (new XmlSerializer(typeof(T)).Deserialize(sr) as T);
        }
        public static string MaxString(this string s, int length)
        {
            if (s != null)
                if (s.Length > length)
                    s = s.Substring(0, length);
            return s;
        }
        public static string RandomPassword(int length)
        {
            var pchars = "ABCDEFGHJKMNPQRSTWXYZ".ToCharArray();
            var pdigits = "23456789".ToCharArray();
            var b = new byte[4];
            (new RNGCryptoServiceProvider()).GetBytes(b);
            var seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            var random = new Random(seed);
            var password = new char[length];

            for (int i = 0; i < password.Length; i++)
            {
                var r = i % 4;
                if (r == 1 || r == 2)
                    password[i] = pdigits[random.Next(pdigits.Length - 1)];
                else
                    password[i] = pchars[random.Next(pchars.Length - 1)];
            }
            return new string(password);
        }
        static public string Replace(this string str, string oldValue, string newValue, bool ignoreCase = false)
        {
            var sb = new StringBuilder();
            var comparison = StringComparison.CurrentCulture;
            if (ignoreCase)
                comparison = StringComparison.CurrentCultureIgnoreCase;

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
        public static MailAddress FirstAddress(string addrs)
        {
            return FirstAddress(addrs, null);
        }
        public static MailAddress FirstAddress(string addrs, string name)
        {
            if (!addrs.HasValue())
                addrs = AdminMail;
            var a = addrs.SplitStr(",;");
            try
            {
                var ma = new MailAddress(a[0]);
                if (name.HasValue())
                    return new MailAddress(ma.Address, name);
                return ma;
            }
            catch (Exception)
            {
                if (name.HasValue())
                    return new MailAddress(AdminMail, name);
                return new MailAddress(AdminMail);
            }
        }

        public static MailAddress FirstAddress2(string addrs, string name)
        {
            if (!addrs.HasValue())
                addrs = AdminMail;
            var a = addrs.SplitStr(",;");
            try
            {
                return new MailAddress(a[0], name);
            }
            catch (Exception)
            {
                if (name.HasValue())
                    return new MailAddress(AdminMail, name);
                return new MailAddress(AdminMail);
            }
        }

        public static bool FastFileExists(string path)
        {
            var task = new Task<bool>(() =>
            {
                var fi = new FileInfo(path);
                return fi.Exists;
            });
            task.Start();
            return task.Wait(1000) && task.Result;
        }

        public static bool AppOffline
        {
            get
            {
                var path = ConfigurationManager.AppSettings["AppOfflineFile"];
                return File.Exists(path);
            }
        }

        public static string UrgentMessage
        {
            get
            {
                var path = ConfigurationManager.AppSettings["UrgentTextFile"];
                if (!path.HasValue())
                    return HttpContext.Current.Application["UrgentMessage"] as string;
                string fileContent = HttpRuntime.Cache["UrgentMessage"] as string;
                if (fileContent == null && File.Exists(path))
                {
                    fileContent = File.ReadAllText(path);
                    HttpRuntime.Cache.Insert("UrgentMessage", fileContent, new System.Web.Caching.CacheDependency(path));
                }
                return fileContent;
            }
            set
            {
                var path = ConfigurationManager.AppSettings["UrgentTextFile"];
                if (!path.HasValue())
                {
                    if (value.HasValue())
                        HttpContext.Current.Application["UrgentMessage"] = value;
                    else
                        HttpContext.Current.Application.Remove("UrgentMessage");
                    return;
                }
                File.WriteAllText(path, value);
                HttpRuntime.Cache.Insert("UrgentMessage", value, new System.Web.Caching.CacheDependency(path));
            }
        }
        public static string ToCode(this Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
              .Replace("/", "_")
              .Replace("+", "-");
            return encoded.Substring(0, 22);
        }
        public static Guid? ToGuid(this string value)
        {
            try
            {
                value = value
                  .Replace("_", "/")
                  .Replace("-", "+");
                byte[] buffer = Convert.FromBase64String(value + "==");
                return new Guid(buffer);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string GuidToQuerystring(this Guid guid)
        {
            return HttpUtility.UrlEncode(guid.ToString());
        }
        public static Guid? QuerystringToGuid(this string value)
        {
            try
            {
                value = HttpUtility.UrlDecode(value);
                return new Guid(value);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool Contains(this string s, string c, bool ignoreCase)
        {
            bool result = false;
            if (s != null && c != null)
            {
                if (ignoreCase)
                    result = s.ToLower().Contains(c.ToLower());
                else
                    result = s.Contains(c);
            }
            return result;
        }
        public static int? GetWeekNumber(this DateTime? dt)
        {
            if (!dt.HasValue)
                return null;
            return GetWeekNumber(dt.Value);
        }
        public static int GetWeekNumber(this DateTime dt)
        {
            var cc = CultureInfo.CurrentCulture;
            int wk = cc.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            return wk;
        }

        public static IEnumerable<DateTime> DaysOfMonth(DateTime dt)
        {
            var d = new DateTime(dt.Year, dt.Month, 1);
            while (d.Month == dt.Month)
            {
                yield return d;
                d = d.AddDays(1);
            }
        }
        public static int WeekOfMonth(this DateTime sunday)
        {
            var sundays = DaysOfMonth(sunday).Where(dd => dd.DayOfWeek == 0).ToList();
            var wk = 0;
            while (wk < sundays.Count && sunday.Date > sundays[wk])
                wk++;
            return wk + 1;
        }
        public static DateTime Sunday(int month, int year)
        {
            var first = new DateTime(year, month, 1);
            return new DateTime(year, month,
                1 + (7 - (int)first.DayOfWeek) % 7);
        }
        public static DateTime Sunday(this DateTime dt)
        {
            return dt.Date.AddDays(-(int)dt.DayOfWeek);
        }
        public static int SundaysInMonth(int month, int year)
        {
            var first = new DateTime(year, month, 1);
            var sun = new DateTime(year, month, 1 + (7 - (int)first.DayOfWeek) % 7);
            int n = 0;
            while (sun.Month == month)
            {
                n++;
                sun = sun.AddDays(7);
            }
            return n;
        }
        public static bool SessionTimedOut()
        {
            if (HttpContext.Current.Session != null)
                if (HttpContext.Current.Session.IsNewSession)
                {
                    string sessionCookie = HttpContext.Current.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        return true;
                }
            return false;
        }
        public static string fmtcoupon(string s)
        {
            if (s.Length == 12)
                return s.Insert(8, " ").Insert(4, " ");
            return s;
        }
        public static string GetIPAddress()
        {
            var context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipAddress.HasValue())
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                    return addresses[0];
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        public static string MaskCC(string s)
        {
            if (!s.HasValue())
                return s;
            var n = Int64.Parse(s.GetDigits());
            StringBuilder sb = null;
            switch (s[0])
            {
                case '3':
                    sb = new StringBuilder("{0:0000 00000 00000}".Fmt(n));
                    break;
                case '4': // Visa
                case '5': // Mastercard
                case '6': // Discover
                    sb = new StringBuilder("{0:0000 0000 0000 0000}".Fmt(n));
                    break;
                default:
                    return s;
            }
            return Mask(sb, 4);
        }
        public static string MaskAccount(string s)
        {
            if (!s.HasValue())
                return s;
            return Mask(new StringBuilder(s), 4);
        }
        public static string Mask(StringBuilder sb, int leave)
        {
            for (var i = 0; i < sb.Length - leave; i++)
                if (char.IsDigit(sb[i]))
                    sb[i] = 'X';
            return sb.ToString();
        }
        public static DateTime NextSemiMonthlyDate(int baseday, DateTime d)
        {
            var a = DateTime.DaysInMonth(d.Year, d.Month) / 2.0;
            a = Math.Ceiling(a);
            var d2 = d.AddDays(a);
            if (d2.Month != d.Month)
                d2 = new DateTime(d2.Year, d2.Month, baseday);
            return d2;
        }
        public static bool IsDebug()
        {
            var d = false;
#if DEBUG
            d = true;
#endif
            return d;
        }

        private const string STR_SMTPDEBUG = "SMTPDebug";
        public static bool SmtpDebug
        {
            get
            {
                bool? deb = false;
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session != null)
                        if (HttpContext.Current.Session[STR_SMTPDEBUG] != null)
                            deb = (bool)HttpContext.Current.Session[STR_SMTPDEBUG];
                }
                else
                {
                    var localDataStoreSlot = Thread.GetNamedDataSlot(STR_SMTPDEBUG);
                    deb = (bool?)Thread.GetData(localDataStoreSlot);
                }
                return deb ?? false;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session[STR_SMTPDEBUG] = value;
                }
                else
                    Thread.SetData(Thread.GetNamedDataSlot(STR_SMTPDEBUG), value);
            }
        }
        public static string ToProper(this string s)
        {
            // allow names that start with uppercase and are not all uppercase to skip the conversion
            if (s.HasValue() && char.IsUpper(s[0]) && !char.IsUpper(s.Last()))
                return s;
            var textinfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textinfo.ToTitleCase(s.ToLower());
        }
        public static DateTime? ParseMMddyy(string s)
        {
            DateTime dt;
            if (DateTime.TryParseExact(s, "MMddyyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                    out dt))
                return dt;
            if (DateTime.TryParseExact(s, "MMddyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                    out dt))
                return dt;

            return null;
        }
        public static string BuildDate()
        {
            return GetBuildDate().FormatDateTm();
        }
        private static DateTime GetBuildDate()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                    s.Close();
            }

            var i = BitConverter.ToInt32(b, cPeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(b, i + cLinkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }
        public static void NameSplit(string name, out string First, out string Last)
        {
            var a = (name ?? "").Split(' ');
            First = "";
            if (a.Length > 1)
            {
                First = a[0];
                Last = a[1];
            }
            else
                Last = a[0];
        }
        public static void AppendNext(this StringBuilder sb, string sep, string s)
        {
            if (sb.Length > 1 && s.HasValue())
                sb.Append(sep);
            sb.Append(s);
        }
        public static bool IsNullable<T>(T t) { return false; }
        public static bool IsNullable<T>(T? t) where T : struct { return true; }
    }
    public class EventArg<T> : EventArgs
    {
        private T value;
        public T Value { get { return value; } }
        public EventArg(T value)
        {
            this.value = value;
        }
    }
}

