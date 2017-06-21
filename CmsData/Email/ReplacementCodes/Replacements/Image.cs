using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchImageRe = @"https{0,1}://[^\#]+\#\{image=[^""]*\}";
        public static readonly Regex ImageRe = new Regex(MatchImageRe, RegexOptions.Singleline);

        public static string ImageReplacement(CMSDataContext db, string code)
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
