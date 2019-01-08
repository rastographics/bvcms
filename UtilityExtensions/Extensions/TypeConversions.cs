/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CsQuery.ExtensionMethods.Internal;

namespace UtilityExtensions
{
    public static partial class Util
    {
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
            return value == null
                ? null
                : Convert.ChangeType(value, type);
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
            if (s == null)
                return null;
            if (s.AllDigits() && s.Length == 8)
            {
                var d = ParseMMddyy(s);
                if (d.HasValue)
                    return GoodDate(d);
            }
            if (s.AllDigits() && s.Length == 10)
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

        public static DateTime? ToDate(this XAttribute a)
        {
            return a?.Value.ToDate();
        }
        public static DateTime? ToDate(this object o)
        {
            if (o == null)
                return null;
            if(o is double)
                return DateTime.FromOADate((double)o);
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
        public static int? ToInt2(this XAttribute a)
        {
            if (a == null)
                return null;
            int? r = null;
            int i;
            if (int.TryParse(a.Value, out i))
                r = i;
            return r;
        }
        public static int? ToInt2(this XElement e)
        {
            if (e == null)
                return null;
            int? r = null;
            int i;
            if (int.TryParse(e.Value, out i))
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
        public static bool ToBool(this XAttribute a)
        {
            if (a == null)
                return false;
            if (a.Value.IsNullOrEmpty())
                return false;
            bool b;
            bool.TryParse(a.Value, out b);
            return b;
        }
        public static bool ToBool(this XElement e)
        {
            return e != null && e.Value.ToBool();
        }
        public static bool ToBool(this object o)
        {
            if (o is bool)
                return (bool)o;
            return o.ToString().ToBool();
        }
        public static int ToInt(this double d)
        {
            var i = Convert.ToInt32(d);
            return i;
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
            var s = o as string;
            if (s != null)
                return s.ToInt();
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
        public static decimal? ToDecimal(this XAttribute a)
        {
            return a?.Value.ToDecimal();
        }
        public static decimal? ToDecimal(this XElement e)
        {
            return e?.Value.ToDecimal();
        }
        public static float ToFloat(this string s)
        {
            var r = 0f;
            float i;
            if (float.TryParse(s, out i))
                r = i;
            return r;
        }
        public static bool DateTryParse(this string date, out DateTime dt)
        {
            return DateTime.TryParse(date, out dt);
        }
        public static bool DateTryParseUS(this string date, out DateTime dt)
        {
            return DateTime.TryParse(date, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out dt);
        }
        public static decimal? GetAmount(this string s)
        {
            if (!s.HasValue())
                return null;
            var digits = new StringBuilder();
            foreach (var c in s.ToCharArray())
                if (char.IsDigit(c) || c == '.')
                    digits.Append(c);
            var a = digits.ToString().ToDecimal();
            return a;
        }
        public static string ToSuitableId(this string s)
        {
            var v = Regex.Replace(s.Replace('/','-'), @"\[|\]|\s|\(|\)|,|=|/|'|#|:", "_").Replace("__", "_").TrimEnd('_');
            var chars = v.ToCharArray();
            var sb = new StringBuilder();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '_')
                {
                    i++;
                    chars[i] = chars[i].ToUpper();
                }
                sb.Append(chars[i]);
            }
            return sb.ToString();
        }

        public static string ToSuitableEvName(this string s)
        {
            var a = s.ToSuitableId().Split('_');
            return string.Join("", a.Select(vv => vv.ToProper())).Truncate(150);
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
        public static double? ToNullableDouble(this object o)
        {
            if (o is DBNull)
                return null;
            return Convert.ToDouble(o);
        }
        public static double ToDouble(this object o)
        {
            if (o is DBNull)
                return 0;
            return Convert.ToDouble(o);
        }
        public static decimal? ToNullableDecimal(this object o)
        {
            if (o is DBNull)
                return null;
            return Convert.ToDecimal(o);
        }
        public static DateTime? ToNullableDate(this object o)
        {
            if (o is DBNull)
                return null;
            return o.ToDate();
        }
        public static int? ToNullableInt(this object o)
        {
            if (o is DBNull)
                return null;
            return o.ToInt();
        }
        public static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> projection)
        {
            while (reader.Read())
                yield return projection(reader);
        }
    }
}

