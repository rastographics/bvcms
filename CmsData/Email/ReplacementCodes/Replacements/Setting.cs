using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSettingRe = @"{setting:\s*(?<name>[^},]*)(,(?<def>[^}]+))?}";
        private static readonly Regex SettingRe = new Regex(MatchSettingRe, RegexOptions.Singleline);

        private string SettingReplacement(string code)
        {
            var match = SettingRe.Match(code);
            var name = match.Groups["name"].Value;
            var def = match.Groups["def"]?.Value;
            return db.Setting(name, def ?? "no setting");
        }
    }
}
