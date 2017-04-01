using System.Linq;
using System.Text.RegularExpressions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchAddSmallGroupRe = @"\{addsmallgroup:\[(?<group>[^\]]*)\]\}";
        private static readonly Regex AddSmallGroupRe = new Regex(MatchAddSmallGroupRe, RegexOptions.Singleline);

        private string AddSmallGroupReplacement(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;
            var match = AddSmallGroupRe.Match(code);
            var group = match.Groups["group"].Value;
            var om = (from mm in db.OrganizationMembers
                where mm.OrganizationId == emailqueueto.OrgId
                where mm.PeopleId == emailqueueto.PeopleId
                select mm).SingleOrDefault();
            om?.AddToGroup(db, @group);
            return "";
        }

    }
}
