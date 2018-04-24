/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static string FormatDate(this DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToString("d");
            return "";
        }
        public static string FormatDateUS(this DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
            return "";
        }

        public static HtmlString FormatDate2(this DateTime? dt, string prefix = null, string suffix = null)
        {
            if (dt.HasValue)
            {
                if (prefix != null)
                    prefix = prefix + " ";
                string dateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.Replace("yyyy", "yy");
                var s = dt.Value.ToString(dateFormat);
                var d = new HtmlString($"{prefix}{s}{suffix}");
                return d;
            }
            return new HtmlString("");
        }

        public static string FormatDate(dynamic dyndt)
        {
            var dt = dyndt as DateTime?;
            if (dt.HasValue)
                return dt.Value.ToString("d");
            return "";
        }
        public static string FmtBirthday(int? y, int? m, int? d, string def = "")
        {
            try
            {
                if (m.HasValue && d.HasValue)
                    if (!y.HasValue)
                        return new DateTime(2000, m.Value, d.Value).ToString("m");
                    else
                        return new DateTime(y.Value, m.Value, d.Value).ToString("d");
                if (y.HasValue)
                    if (m.HasValue)
                        return new DateTime(y.Value, m.Value, 1).ToString("y");
                    else
                        return y.ToString();
            }
            catch (Exception)
            {
                return $"bad date {m ?? 0}/{d ?? 0}/{y ?? 0}";
            }
            return def;
        }

        public static string ToSortableDateTime(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HHmmss");
        }

        public static string ToSortableDateTime(this DateTime? dt)
        {
            return dt.ToString2("yyyy-MM-dd HHmmss");
        }

        public static string ToSortableDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static string ToSortableTime(this DateTime dt)
        {
            return dt.ToString("HHmmss.ff");
        }

        public static string ToSortableDate(this DateTime? dt)
        {
            return dt.ToString2("yyyy-MM-dd");
        }

        public static DateTime? FromSortableDate(string dtstr)
        {
            return DateTime.ParseExact(dtstr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static string FormatDate(this DateTime? dt, string def = "")
        {
            if (dt.HasValue)
                return dt.Value.ToString("d");
            return def;
        }
        public static string FormatDateTime(this DateTime? dt, string def = "")
        {
            if (dt.HasValue)
                return dt.Value.TimeOfDay.TotalSeconds > 0 
                    ? FormatDateTm(dt) 
                    : dt.Value.ToString("d");
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
        public static string FormatDateTmUS(this DateTime? dt, string def = null)
        {
            if (dt.HasValue)
                return dt.Value.ToString("g", CultureInfo.CreateSpecificCulture("en-US"));
            return def;
        }

        public static string FormatDateTm(this DateTime? dt, string def)
        {
            if (dt.HasValue)
                return dt.ToString2("g");
            return def;
        }

        public static string FormatTime(this DateTime? tm)
        {
            if (tm.HasValue)
                return tm.Value.FormatTime();
            return "";
        }

        public static string FormatTime(this DateTime tm)
        {
            return tm.ToString("h:mm tt");
        }

        public static string ToString2(this int? i, string fmt)
        {
            if (i.HasValue)
                return fmt.Contains("{") ? string.Format(fmt, i.Value) : i.Value.ToString(fmt);
            return "";
        }

        public static string ToString2(this decimal? d, string fmt)
        {
            if (d.HasValue)
                return d.Value.ToString(fmt);
            return "";
        }
        public static string ToString2(this decimal? d, string fmt, string postfix)
        {
            if (d.HasValue)
                return d.Value.ToString(fmt) + postfix;
            return "";
        }
        public static string ToString2(this double? d, string fmt, string postfix = null)
        {
            if (d.HasValue)
                return d.Value.ToString(fmt) + postfix;
            return "";
        }

        public static string ToString2(this DateTime? d, string fmt)
        {
            if (d.HasValue)
                return d.Value.ToString(fmt);
            return "";
        }

        public static string ToString2(this TimeSpan? ts, string fmt)
        {
            if (ts.HasValue)
                return ts.Value.ToString(fmt);
            return "";
        }

        public static string FormatCSZ(string city, string st, string zip)
        {
            var csz = city ?? string.Empty;
            if (st.HasValue())
                csz += ", " + st;
            if (zip.HasValue())
                csz += " " + zip.FmtZip();
            return csz.Trim();
        }

        public static string FormatCSZ4(string city, string st, string zip)
        {
            var csz = city ?? string.Empty;
            if (st.HasValue())
                csz += ", " + st;
            csz += " " + FmtZip(zip);
            return csz.Trim();
        }
        public static string FormatCSZ4(object city, object st, object zip)
        {
            return FormatCSZ4(city?.ToString(), st?.ToString(), zip?.ToString());
        }

        public static string FormatCSZ5(string city, string st, string zip)
        {
            var csz = city ?? string.Empty;
            if (st.HasValue())
                csz += ", " + st;
            csz += " " + Zip5(zip);
            return csz.Trim();
        }

        public static string FmtFone(this string phone, string prefix)
        {
            phone = phone.FmtFone();
            if (!phone.HasValue())
                return "";
            if(prefix.HasValue())
                return prefix + " " + phone;
            return phone;
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

        public static string FmtAttendStr(this string attendstr)
        {
            if (!attendstr.HasValue())
                return " ";
            return attendstr;
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
            s = s.Replace(Environment.NewLine, "\n");
            return s.Replace("\n", "<br>\r\n");
        }

        private static string HtmlFormat(string s, string lookfor, string htmlcode)
        {
            return Regex.Replace(s, $"{lookfor}(.*?){lookfor}",
                $"<{htmlcode}>$1</{htmlcode}>", RegexOptions.Singleline);
        }

        public static string Disallow(this string value, string dissallow)
        {
            var v = value ?? "";
            value = v.Trim();
            if (string.Compare(value, dissallow, StringComparison.OrdinalIgnoreCase) == 0)
                return "";
            return value;
        }

        public static string ToStringNoZero(this int? value)
        {
            value = value ?? 0;
            if (value == 0)
                return "";
            return value.ToString();
        }

        public static string fmtcoupon(string s)
        {
            if (s.Length == 12)
                return s.Insert(8, " ").Insert(4, " ");
            return s;
        }

        public static string MaskCC(string s)
        {
            if (!s.HasValue())
                return s;
            var n = long.Parse(s.GetDigits());
            StringBuilder sb = null;
            switch (s[0])
            {
                case '3':
                    sb = new StringBuilder($"{n:0000 00000 00000}");
                    break;
                case '4': // Visa
                case '5': // Mastercard
                case '6': // Discover
                    sb = new StringBuilder($"{n:0000 0000 0000 0000}");
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

        public static string ToProper(this string s)
        {
            // allow names that start with uppercase and are not all uppercase to skip the conversion
            if (s.HasValue() && char.IsUpper(s[0]) && !char.IsUpper(s.Last()))
                return s;
            var textinfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textinfo.ToTitleCase(s.ToLower());
        }
    }
}
