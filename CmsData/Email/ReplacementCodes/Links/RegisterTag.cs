using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchRegisterTagRe = "(?:<|&lt;)registertag[^>]*(?:>|&gt;).+?(?:<|&lt;)/registertag(?:>|&gt;)";
        private static readonly Regex RegisterTagRe = new Regex(MatchRegisterTagRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string RegisterTagReplacement(string code, EmailQueueTo emailqueueto)
        {
            var doc = new HtmlDocument();
            if (code.Contains("&lt;"))
                code = HttpUtility.HtmlDecode(code);
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.FirstChild;
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var id = ele.Id.ToInt();
            var url = RegisterLinkUrl(db, id, emailqueueto.PeopleId, emailqueueto.Id, "registerlink");
            return $@"<a href=""{url}"">{inside}</a>";
        }
    }
}
