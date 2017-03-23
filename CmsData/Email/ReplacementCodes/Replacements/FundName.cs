using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchFundnameRe = @"{fundname:\s*(?<fundid>\d+)}";
        private readonly Dictionary<int, string> fundnames = new Dictionary<int, string>();
        private static readonly Regex FundnameRe = new Regex(MatchFundnameRe, RegexOptions.Singleline);

        private string FundNameReplacement(string code)
        {
            var match = FundnameRe.Match(code);
            var fundid = match.Groups["fundid"].Value.ToInt();

            if (fundnames.ContainsKey(fundid))
                return fundnames[fundid];
            var q = from i in db.ContributionFunds
                    where i.FundId == fundid
                    select i.FundDescription;
            var name = q.FirstOrDefault() ?? "No Fund Found";
            fundnames.Add(fundid, name);
            return name;
        }
    }
}
