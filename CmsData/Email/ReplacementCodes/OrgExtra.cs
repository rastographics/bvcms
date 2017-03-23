using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private static readonly Regex OrgExtraRe = new Regex(@"\{orgextra:(?<field>[^\]]*)\}", RegexOptions.Singleline);

        private string OrgExtraReplacement(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;
            var match = OrgExtraRe.Match(code);
            var field = match.Groups["field"].Value;
            var ev = db.OrganizationExtras.SingleOrDefault(ee => ee.Field == field && ee.OrganizationId == db.CurrentOrg.Id);
            if (ev == null || !ev.Data.HasValue())
                return null;
            return ev.Data;
        }

    }
}
