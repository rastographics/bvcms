using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchFirstOrSubstituteRe = @"\{first\|(?<sub>[^}]*)\}";
        private static readonly Regex FirstOrSubstituteRe = new Regex(MatchFirstOrSubstituteRe, RegexOptions.Singleline);

        private string FirstOrSubstituteReplacement(string code)
        {
            var match = FirstOrSubstituteRe.Match(code);
            var sub = match.Groups["sub"].Value;
            return !person.PreferredName.HasValue() || person.PreferredName.Contains("?") || person.PreferredName.Contains("unknown", true) 
                ? sub : person.PreferredName;
        }
    }
}
