using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace UtilityExtensions
{
    public static partial class Util
    {
        // this method from http://haacked.com/archive/2005/09/24/10334.aspx
        public static string[] SplitUpperCase(this string source)
        {
            if (source == null)
                return new string[] { }; //Return empty array.

            if (source.Length == 0)
                return new[] { "" };

            var words = new StringCollection();
            var wordStartIndex = 0;

            var letters = source.ToCharArray();

            // Skip the first letter. we don't care what case it is.
            for (var i = 1; i < letters.Length; i++)
            {
                if (char.IsUpper(letters[i]))
                {
                    //Grab everything before the current index.
                    words.Add(new string(letters, wordStartIndex, i - wordStartIndex));
                    wordStartIndex = i;
                }
            }

            //We need to have the last word.
            words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

            //Copy to a string array.
            var wordArray = new string[words.Count];
            words.CopyTo(wordArray, 0);
            return wordArray;
        }

        public static string SplitUpperCaseToString(this string source)
        {
            return string.Join(" ", SplitUpperCase(source));
        }
        public static string SpaceCamelCase(this string name)
        {
            var re = new Regex(@"\B\p{Lu}\p{Ll}", RegexOptions.Compiled);
            name = re.Replace(name, " $0");
            return name;
        }

        public static string JoinInts(this IEnumerable<int> ints, string sep)
        {
            if (ints == null)
                return null;
            return string.Join(sep, ints);
        }

        public static string Replace(this string str, string oldValue, string newValue, bool ignoreCase = false)
        {
            var sb = new StringBuilder();
            var comparison = StringComparison.CurrentCulture;
            if (ignoreCase)
                comparison = StringComparison.CurrentCultureIgnoreCase;

            var previousIndex = 0;
            var index = str.IndexOf(oldValue, comparison);
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

        public static string DefaultTo(this string s, string defaultstr)
        {
            if (s.HasValue())
                return s;
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
            if (s == null)
                return "".Split(delimiter.ToCharArray());
            return s.Split(delimiter.ToCharArray(), nitems);
        }

        public static string[] SplitLines(this string source, bool noblanks = false)
        {
            if (source == null)
                return new string[0];
            var option = noblanks
                ? StringSplitOptions.RemoveEmptyEntries
                : StringSplitOptions.None;
            return source.Split(new[] { "\r\n", "\n", "\r" }, option);
        }

        public static string GetAttr(this XElement e, string n, string def = null)
        {
            var a = e.Attribute(n);
            return a?.Value ?? def;
        }

        public static string Truncate(this string source, int length)
        {
            if (source.HasValue() && source.Length > length)
                source = source.Substring(0, length).Trim();
            return source;
        }

        // ReSharper disable once InconsistentNaming
        public static string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return $"{baseUrl.TrimEnd('/', '\\')}/{relativeUrl.TrimStart('/', '\\')}";
        }

        public static string GetDigits(this string zip, int maxlen = 99)
        {
            if (!zip.HasValue())
                return "";
            var digits = new StringBuilder();
            foreach (var c in zip)
                if (Char.IsDigit(c))
                    digits.Append(c);
            return digits.ToString().Truncate(maxlen);
        }

        public static string NoLeadZeros(this string s)
        {
            if (!s.HasValue())
                return "";
            var digits = new StringBuilder();
            var nonzeroseen = false;
            foreach (var c in s)
                if (!nonzeroseen && c == '0')
                    continue;
                else
                {
                    nonzeroseen = true;
                    digits.Append(c);
                }
            return digits.ToString();
        }

        public static string GetChars(this string s)
        {
            if (!s.HasValue())
                return "";
            var chars = new StringBuilder();
            foreach (var c in s)
                if (!char.IsDigit(c))
                    chars.Append(c);
            return chars.ToString();
        }

        public static string MaxString(this string s, int length)
        {
            if (s?.Length > length)
                return s.Substring(0, length);
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

            for (var i = 0; i < password.Length; i++)
            {
                var r = i % 4;
                if (r == 1 || r == 2)
                    password[i] = pdigits[random.Next(pdigits.Length - 1)];
                else
                    password[i] = pchars[random.Next(pchars.Length - 1)];
            }
            return new string(password);
        }

        public static void NameSplit(string name, out string first, out string last)
        {
            if ((name ?? "").Contains(","))
            {
                var a = (name ?? "").Split(',');
                first = "";
                if (a.Length > 1)
                {
                    first = a[1].Trim();
                    last = a[0].Trim();
                }
                else
                    last = a[0].Trim();
            }
            else
            {
                var a = (name ?? "").Split(' ');
                first = "";
                if (a.Length > 1)
                {
                    first = a[0];
                    last = a[1];
                }
                else
                    last = a[0];
            }
        }

        public static void AppendNext(this StringBuilder sb, string sep, string s)
        {
            if (sb.Length > 1 && s.HasValue())
                sb.Append(sep);
            sb.Append(s);
        }

        public static bool Contains(this string s, string c, bool ignoreCase)
        {
            var result = false;
            if (s != null && c != null)
            {
                if (ignoreCase)
                    result = s.ToLower().Contains(c.ToLower());
                else
                    result = s.Contains(c);
            }
            return result;
        }

        public static string Encode64(this string s)
        {
            var b = Encoding.ASCII.GetBytes(s);
            var r = Convert.ToBase64String(b);
            return r;
        }

        public static string Decode64(this string s)
        {
            var b = Convert.FromBase64String(s);
            var r = Encoding.ASCII.GetString(b);
            return r;
        }

        /// <summary>
        /// Given a <paramref name="source"/> string, return the last <paramref name="stringLength"/> characters from the string.
        /// </summary>
        public static string Last(this string source, int stringLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            if (stringLength >= source.Length)
                return source;
            return source.Substring(source.Length - stringLength);
        }
        public static void AppendNewLine(this StringBuilder sb, string s)
        {
            if (!s.HasValue())
                return;
            if (sb.Length > 0)
                sb.AppendLine();
            sb.Append(s);
        }
        public static void AppendSpace(this StringBuilder sb, string s)
        {
            if (!s.HasValue())
                return;
            sb.Append(" " + s);
        }

        public static string GetCsvToken(this string s, int n = 1, int ntokens = 1000, string sep = ",")
        {
            var a = s.SplitStr(sep, ntokens);
            return a.Length >= n ? a[n - 1] : "";
        }
        public static string RemoveGrammarly(this string s)
        {
            s = Regex.Replace(s, "<style type=\"text/css\">._44eb54-hoverMenu.*?</style>", "", RegexOptions.Singleline);
            return Regex.Replace(s, @"\sdata-gramm\w*?="".*?""", "", RegexOptions.Singleline);
        }

        public static string Md5Hash(this string s)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(s);
                var hash = md5.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var t in hash)
                    sb.Append(t.ToString("x2"));
                return sb.ToString();
            }
        }
        public static string Sha256Hash(this string s)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var t in hash)
                    sb.Append(t.ToString("x2"));
                return sb.ToString();
            }
        }

        public static string SlugifyString(this string original)
        {
            if (original == null)
                return string.Empty;

            // Replace all non-alphanumeric
            var rgx = new Regex("[^a-zA-Z0-9]");
            var slug = rgx.Replace(original, "");
            var lowercaseSlug = slug.ToLower();

            return lowercaseSlug;
        }

        public static string HtmlEncode(this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        public static Dictionary<string, object> ToDictionary(this string value)
        {
            if (value == null)
            {
                return new Dictionary<string, object>();
            }
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        }
    }
}
