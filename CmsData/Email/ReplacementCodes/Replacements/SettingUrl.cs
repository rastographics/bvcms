using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSettingUrlRe = @"https{0,1}://[^\#]+\#\{(image|setting)=[^""]*\}";
        public static readonly Regex SettingUrlRe = new Regex(MatchSettingUrlRe, RegexOptions.Singleline);

        public static string SettingUrlReplacement(CMSDataContext db, string code)
        {
            var a = code.SplitStr("#", 2);
            var b = a[1].SplitStr("=", 2);
            var url = a[0];
            var name = b[1].TrimEnd('}');
            var setting = db.Setting(name, url);
            return setting;
        }
    }
}
