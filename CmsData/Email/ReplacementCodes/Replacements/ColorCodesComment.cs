using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchColorCodesRe = @"<!--replacecolors:\s*(?<content>.*?)-->";
        internal string ColorCodesReplacement(string text)
        {
            var match = Regex.Match(text, MatchColorCodesRe, RegexOptions.Singleline);
            if (!match.Success)
                return text;
            var content = match.Groups["content"].Value;
            var codes = db.ContentOfTypeText(content);
            return ReplaceColorCodeStr(text, codes);
        }
        private string ReplaceColorCodeStr(string text, string codes)
        {
            codes = Regex.Replace(codes, @"//\w*", ","); // replace comments
            codes = Regex.Replace(codes, @"\s*", ""); // remove spaces
            foreach (var pair in codes.SplitStr(","))
            {
                if(!pair.Contains("="))
                    continue;
                var a = pair.SplitStr("=", 2);
                text = text.Replace(a[0], a[1]);
            }
            return text;
        }
    }
}
