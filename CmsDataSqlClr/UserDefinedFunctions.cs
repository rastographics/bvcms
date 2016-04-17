using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CmsDataSqlClr
{
    public class UserDefinedFunctions
    {
        public static bool IsValidEmail(string addr)
        {
            var email = addr?.Trim();
            if (string.IsNullOrEmpty(email))
                return true;
            var re1 = new Regex(@"^(.*\b(?=\w))\b[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9._-]+\.[A-Z]{2,4}\b\b(?!\w)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var re2 = new Regex(@"^[A-Z0-9._%+-]+(?<=[^.])@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return re1.IsMatch(email) || re2.IsMatch(email);
        }

        public static string RegexMatch(string subject, string pattern)
        {
            var m = Regex.Match(subject ?? "", pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            if (!m.Success)
                return null;
            var g = m.Groups["group"];
            return g.Success ? g.Value : m.Value;
        }

        public static string AllRegexMatchs(string subject, string pattern)
        {
            var list = new List<string>();
            var re = new Regex(pattern);
            var match = re.Match(subject);
            while (match.Success)
            {
                list.Add(match.Value);
                match = match.NextMatch();
            }
            return string.Join("<br>\n", list.ToArray());
        }
    }
}