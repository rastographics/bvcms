using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchPythonDataRe = @"{pythondata:\s*(?<name>[^}]*)}";
        private static readonly Regex PythonDataRe = new Regex(MatchPythonDataRe, RegexOptions.Singleline);

        private string PythonDataReplacement(string code)
        {
            var match = PythonDataRe.Match(code);
            var name = match.Groups["name"].Value;
            return pythonData[name].ToString();
        }
    }
}
