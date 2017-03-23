using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchOrgMemberRe = @"{orgmember:(?<type>.*?),(?<divid>.*?)}";
        private static readonly Regex OrgMemberRe = new Regex(MatchOrgMemberRe, RegexOptions.Singleline);

        private string OrgMemberReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = OrgMemberRe.Match(code);
            var divid = match.Groups["divid"].Value.ToInt();
            var type = match.Groups["type"].Value;
            var org = (from om in db.OrganizationMembers
                where om.PeopleId == emailqueueto.PeopleId
                where om.Organization.DivOrgs.Any(dd => dd.DivId == divid)
                select om.Organization).FirstOrDefault();

            if (org == null)
                return "?";

            switch (type.ToLower())
            {
                case "location":
                    return org.Location;
                case "pendinglocation":
                case "pendingloc":
                    return org.PendingLoc;
                case "orgname":
                case "name":
                    return org.OrganizationName;
                case "leader":
                    return org.LeaderName;
            }
            return code;
        }
    }
}
