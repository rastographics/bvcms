using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSubGroupRe = @"\{(smallgroup|subgroup):\[(?<prefix>[^\]]*)\](?:,(?<def>[^}]*)){0,1}\}";
        private static readonly Regex SmallGroupRe = new Regex(MatchSubGroupRe, RegexOptions.Singleline);

        private string SmallGroupReplacement(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;

            var match = SmallGroupRe.Match(code);
            var prefix = match.Groups["prefix"].Value;
            var def = match.Groups["def"].Value;
            var sg = (from mm in db.OrgMemMemTags
                      where mm.OrgId == emailqueueto.OrgId
                      where mm.PeopleId == emailqueueto.PeopleId
                      where mm.MemberTag.Name.StartsWith(prefix)
                      select mm.MemberTag.Name).FirstOrDefault();
            if (!sg.HasValue())
                sg = def;
            return sg;
        }
    }
}
