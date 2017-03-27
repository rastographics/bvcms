using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchPledgeFundRe = @"{pledgefund\[(?<fundid>\d+)\]\.(?<value>[^}]*)}";
        private Dictionary<int, View.PledgeReport> pledgefunds;
        private static readonly Regex PledgeFundRe = new Regex(MatchPledgeFundRe, RegexOptions.Singleline);

        private string PledgeFundReplacement(string code)
        {
            var match = PledgeFundRe.Match(code);
            var fundid = match.Groups["fundid"].Value.ToInt();
            var value = match.Groups["value"].Value;

            if (pledgefunds == null)
                pledgefunds = db.PledgeReport(DateTime.Parse("1/1/1900"), DateTime.Parse("1/1/3000"), 0)
                    .ToDictionary(vv => vv.FundId, vv => vv);
            if(!pledgefunds.ContainsKey(fundid))
                    return "PledgeFund not found";
            switch (value)
            {
                case "Name":
                    return pledgefunds[fundid].FundName;
                case "Pledged":
                    return pledgefunds[fundid].Plg.ToString2("N");
                case "ToPledge":
                    return pledgefunds[fundid].ToPledge.ToString2("N");
                case "NotToPledge":
                    return pledgefunds[fundid].NotToPledge.ToString2("N");
                case "ToFund":
                    return pledgefunds[fundid].ToFund.ToString2("N");
                default:
                    return "PledgeFund Value not found";
            }
        }
    }
}
