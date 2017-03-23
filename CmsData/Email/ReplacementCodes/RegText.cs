using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private static readonly Regex RegTextRe = new Regex(@"{reg(?<type>.*?):(?<field>.*?)}", RegexOptions.Singleline);

        private string RegTextReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = RegTextRe.Match(code);
            var field = match.Groups["field"].Value;
            var type = match.Groups["type"].Value;
            var answer = (from qa in db.ViewOnlineRegQAs
                          where qa.Question == field
                          where qa.Type == type
                          where qa.PeopleId == emailqueueto.PeopleId
                          where qa.OrganizationId == emailqueueto.OrgId
                          select qa.Answer).SingleOrDefault();

            return answer;
        }
    }
}
