using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData.View;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private static readonly Regex PledgeRe = new Regex(@"{pledge(?<type>amt|bal):\s*(?<fundid>\d+)}", RegexOptions.Singleline);

        private List<PledgeBalance> pledgeinfos;
        private PledgeBalance GetPledgeInfo(int fundid, EmailQueueTo emailqueueto)
        {
            var pid = emailqueueto.PeopleId;
            if (pledgeinfos == null)
                pledgeinfos = db.PledgeBalances(fundid).Where(vv => vv.PledgeAmt > 0).ToList();
            return pledgeinfos.SingleOrDefault(vv => vv.CreditGiverId == pid || vv.SpouseId == pid) ?? new PledgeBalance();
        }

        public string PledgeReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = PledgeRe.Match(code);
            var type = match.Groups["type"].Value;
            var fundid = match.Groups["fundid"].Value.ToInt();
            var i = GetPledgeInfo(fundid, emailqueueto);
            if (i == null)
                return "";

            switch (type.ToLower())
            {
                case "bal":
                    return (i.Balance ?? 0).ToString("c");
                case "amt":
                    return (i.PledgeAmt ?? 0).ToString("c");
            }
            return code;
        }
    }
}
