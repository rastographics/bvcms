using System.Linq;
using System.Text.RegularExpressions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSubGroupsRe = @"\{(smallgroups|subgroups)(:\[(?<prefix>[^\]]*)\]){0,1}\}";
        private static readonly Regex SubGroupsRe = new Regex(MatchSubGroupsRe, RegexOptions.Singleline);

        private string SubGroupsReplacement(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;

            var match = SubGroupsRe.Match(code);
            var prefix = match.Groups["prefix"].Value;
            var q = from mm in db.OrgMemMemTags
                    where mm.OrgId == emailqueueto.OrgId
                    where mm.PeopleId == emailqueueto.PeopleId
                    where mm.MemberTag.Name.StartsWith(prefix) || prefix == null || prefix == ""
                    orderby mm.MemberTag.Name
                    select mm.MemberTag.Name.Substring(prefix.Length);
            return string.Join("<br/>\n", q);
        }
    }
}
