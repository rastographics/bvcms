/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        public static bool IsNotNull(this object o)
        {
            return o != null;
        }

        public static int? IntOrNull(this string s)
        {
            int? i = null;
            if (s.HasValue())
                i = int.Parse(s);
            return i;
        }

        public static string ToNull(this string value)
        {
            return value.HasValue() ? value : null;
        }

        public static bool Has(this object obj, string propertyName)
        {
            var dynamic = obj as DynamicObject;
            if (dynamic == null) return false;
            return dynamic.GetDynamicMemberNames().Contains(propertyName);
        }

        public static bool HasValue(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        public static bool Equal(this string s, string s2)
        {
            return string.Compare(s, s2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool NotEqual(this string s, string s2)
        {
            return !s.Equal(s2);
        }

        public static bool SameMinute(this DateTime dt1, DateTime dt2)
        {
            dt1 = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, 0);
            dt2 = new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, 0);
            return dt1.Equals(dt2);
        }

        public static bool SameMinute(this object o, DateTime? dt2)
        {
            if (!dt2.HasValue)
                return false;
            if (o is DateTime)
                return SameMinute((DateTime) o, dt2.Value);
            return false;
        }

        public static bool AllDigits(this string str)
        {
            if (!str.HasValue())
                return false;
            var patt = new Regex("[^0-9]");
            return !(patt.IsMatch(str));
        }
		public static bool AllDigitsCommas(this string str)
		{
            if (!str.HasValue())
                return false;
			var patt = new Regex("[^0-9,]");
			return !patt.IsMatch(str);
		}

        private const string STR_Culture = "Culture";
        public static string Culture
        {
            get
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Items[STR_Culture] != null)
                        return (string)HttpContextFactory.Current.Items[STR_Culture];
                return null;
            }
            set
            {
                HttpContextFactory.Current.Items[STR_Culture] = value;
            }
        }

        public static string CultureDateFormat
        {
            get
            {
                var cultureDateFormat = "MM/DD/YYYY";
                if (Culture != "en-US")
                {
                    var c = new CultureInfo(Culture);
                    cultureDateFormat = c.DateTimeFormat.ShortDatePattern.ToUpper();
                }
                return cultureDateFormat;
            }
        }

        // h:mm A
        public static string CultureDateFormatAlt
        {
            get
            {
                var cultureDateFormat = "MM/DD/YY";
                if (Culture != "en-US")
                {
                    cultureDateFormat = string.Empty;
                }
                return cultureDateFormat;
            }
        }

        public static string CultureDateTimeFormat => $"{CultureDateFormat} {"h:mm A"}";

        public static string CultureDateTimeFormatAlt
        {
            get
            {
                var cultureDateTimeFormat = "MM/DD/YY";
                if (Culture != "en-US")
                {
                    cultureDateTimeFormat = string.Empty;
                }
                if (!string.IsNullOrEmpty(cultureDateTimeFormat))
                    cultureDateTimeFormat = $"{cultureDateTimeFormat} {"h:mm A"}";

                return cultureDateTimeFormat;
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
                if (s.StartsWith("2002"))
                    return @"^\d\d\d\d[-/]\d\d[-/](0[1-9]|1[012])$";
                return @"^(0?[1-9]|1[012])[-/]\d\d?[-/]\d\d(\d\d)?$";
            }
        }

        public static string jQueryDateFormat2
        {
            get
            {
                var dt = new DateTime(2002, 1, 30);
                var s = dt.ToShortDateString();
                var sep = s.Contains("-") ? "-" : "/";
                if (s.StartsWith("30"))
                    return $"d{sep}m{sep}yyyy";
                if (s.StartsWith("2002"))
                    return $"yyyy{sep}mm{sep}dd";
                return $"m{sep}d{sep}yyyy";
            }
        }

        public static string jQueryDateFormat2WithTime => jQueryDateFormat2 + " H:ii P";

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

        public static bool IsInRole(string role)
        {
            if (HttpContextFactory.Current != null)
                return HttpContextFactory.Current.User.IsInRole(role);
            return false;
        }
    }
}

