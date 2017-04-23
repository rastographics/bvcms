using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchOrgExtraRe = @"\{orgextra:(?<field>[^\]]*)\}";
        private static readonly Regex OrgExtraRe = new Regex(MatchOrgExtraRe, RegexOptions.Singleline);

        private string OrgExtraReplacement(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;
            var match = OrgExtraRe.Match(code);
            var field = match.Groups["field"].Value;
            var ev = db.OrganizationExtras.SingleOrDefault(ee => ee.Field == field && ee.OrganizationId == emailqueueto.OrgId);
            if (ev == null || !ev.Data.HasValue())
                return null;
            return ev.Data;
        }

    }
}
