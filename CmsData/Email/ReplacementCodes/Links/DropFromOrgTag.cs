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
        private const string MatchDropFromOrgTagRe = "(?:<|&lt;)dropfromorg[^>]*(?:>|&gt;).+?(?:<|&lt;)/dropfromorg(?:>|&gt;)";
        private static readonly Regex DropFromOrgTagRe = new Regex(MatchDropFromOrgTagRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string DropFromOrgTagReplacement(string code, EmailQueueTo emailqueueto)
        {
            var doc = new HtmlDocument();
            if (code.Contains("&lt;"))
                code = HttpUtility.HtmlDecode(code);
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.FirstChild;
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var id = ele.Id.ToInt();
            var url = DropFromOrgLinkUrl(db, id, emailqueueto.PeopleId, emailqueueto.Id);
            return $@"<a href=""{url}"">{inside}</a>";
        }
        public static string DropFromOrgLinkUrl(CMSDataContext db, int orgid, int pid, int queueid)
        {
            var qs = $"{orgid},{pid},{queueid}";
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = qs,
            };
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink($"/OnlineReg/DropFromOrgLink/{ot.Id.ToCode()}");
            return url;
        }
    }
}
