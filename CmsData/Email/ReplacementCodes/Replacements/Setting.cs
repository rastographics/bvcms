using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSettingRe = @"{setting:\s*(?<name>[^}]*)}";
        private static readonly Regex SettingRe = new Regex(MatchSettingRe, RegexOptions.Singleline);

        private string SettingReplacement(string code)
        {
            var match = SettingRe.Match(code);
            var name = match.Groups["name"].Value;
            return db.Setting(name, "no setting");
        }
    }
}
