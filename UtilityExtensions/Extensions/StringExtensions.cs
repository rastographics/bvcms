using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static string[] SplitUpperCase(this string source) // this method from http://haacked.com/archive/2005/09/24/10334.aspx
        {
            if (source == null)
                return new string[] { }; //Return empty array.

            if (source.Length == 0)
                return new string[] { "" };

            StringCollection words = new StringCollection();
            int wordStartIndex = 0;

            char[] letters = source.ToCharArray();

            // Skip the first letter. we don't care what case it is.
            for (int i = 1; i < letters.Length; i++)
            {
                if (char.IsUpper(letters[i]))
                {
                    //Grab everything before the current index.
                    words.Add(new String(letters, wordStartIndex, i - wordStartIndex));
                    wordStartIndex = i;
                }
            }

            //We need to have the last word.
            words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

            //Copy to a string array.
            string[] wordArray = new string[words.Count];
            words.CopyTo(wordArray, 0);
            return wordArray;
        }
        public static string SplitUpperCaseToString(this string source)
        {
            return string.Join(" ", SplitUpperCase(source));
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
        public static string[] SplitLines(this string source, bool noblanks = false)
        {
            if (source == null)
                return new string[0];
            var option = noblanks
                ? StringSplitOptions.RemoveEmptyEntries
                : StringSplitOptions.None;
            return source.Split(new string[] { "\r\n", "\n" }, option);
        }

        public static string GetAttr(this XElement e, string n, string def = null)
        {
            var a = e.Attribute(n);
            return a != null ? a.Value : def;
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
        public static string URLCombine(string baseUrl, string relativeUrl)
        {
            if (baseUrl.Length == 0)
                return relativeUrl;
            if (relativeUrl.Length == 0)
                return baseUrl;
            return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
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
        public static string NoLeadZeros(this string s)
        {
            if (!s.HasValue())
                return "";
            var digits = new StringBuilder();
            var nonzeroseen = false;
            foreach (var c in s.ToCharArray())
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
            foreach (var c in s.ToCharArray())
                if (!Char.IsDigit(c))
                    chars.Append(c);
            return chars.ToString();
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
        public static void NameSplit(string name, out string First, out string Last)
        {
            if ((name ?? "").Contains(","))
            {
                var a = (name ?? "").Split(',');
                First = "";
                if (a.Length > 1)
                {
                    First = a[1].Trim();
                    Last = a[0].Trim();
                }
                else
                    Last = a[0].Trim();
            }
            else
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
        }
        public static void AppendNext(this StringBuilder sb, string sep, string s)
        {
            if (sb.Length > 1 && s.HasValue())
                sb.Append(sep);
            sb.Append(s);
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
    }
}
