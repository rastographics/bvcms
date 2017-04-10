using System;
using System.Text.RegularExpressions;
using Community.CsharpSqlite;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        public const string MatchDateFormattedRe = @"\{date(?<what>[^:]*):(?<fmt>[^}]*)\}";
        public static readonly Regex DateFormattedRe = new Regex(MatchDateFormattedRe, RegexOptions.Singleline);

        private string DateFormattedReplacement(string code)
        {
            var match = DateFormattedRe.Match(code);
            var fmt = match.Groups["fmt"].Value;
            var what = match.Groups["what"].Value;
            switch (what)
            {
                case "birth":
                    return DateFormattedReplacement(person.GetBirthdate(), format:fmt);
                case "wedding":
                case "anniversary":
                    return DateFormattedReplacement(person.WeddingDate, format:fmt);
                case "joined":
                    return DateFormattedReplacement(person.JoinDate, format:fmt);
                case "dropped":
                    return DateFormattedReplacement(person.DropDate, format:fmt);
            }
            return code;
        }

        public static string DateFormattedReplacement(DateTime? date, string code = null, string format = null)
        {
            if (format == null && code != null)
            {
                var match = DateFormattedRe.Match(code);
                format = match.Groups["fmt"].Value;
            }
            return date.ToString2(format);
        }
    }
}
