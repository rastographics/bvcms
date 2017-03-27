using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData.View;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchRecentGiftsRe = @"{recentgifts:\s*(?<fundid>\d+),(?<dt>.*)}";
        private static readonly Regex RecentGiftsRe = new Regex(MatchRecentGiftsRe, RegexOptions.Singleline);

        //private List<PledgeBalance> pledgeinfos;
//        private PledgeBalance GetPledgeInfo(int fundid, EmailQueueTo emailqueueto)
//        {
//            var pid = emailqueueto.PeopleId;
//            if (pledgeinfos == null)
//                pledgeinfos = db.PledgeBalances(fundid).Where(vv => vv.PledgeAmt > 0).ToList();
//            return pledgeinfos.SingleOrDefault(vv => vv.CreditGiverId == pid || vv.SpouseId == pid) ?? new PledgeBalance();
//        }

        public string RecentGiftsReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = RecentGiftsRe.Match(code);
            var dt = match.Groups["dt"].Value.ToDate();
            var fundid = match.Groups["fundid"].Value.ToInt();
            var i = GetPledgeInfo(fundid, emailqueueto);
            if (i == null)
                return "";

            return code;
        }
    }
}
