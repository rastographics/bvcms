using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchRsvpLinkRe = "<a[^>]*?href=\"https{0,1}://(?:rsvplink|regretslink)/{0,1}\"[^>]*>.*?</a>";
        private static readonly Regex RsvpLinkRe = new Regex(MatchRsvpLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string RsvpLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            //<a dir="ltr" href="http://rsvplink" id="798" rel="meetingid" title="This is a message">test</a>
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var msg = "Thank you for responding.";
            if (d.ContainsKey("title"))
                msg = d["title"];

            string confirm = "false";
            if (d.ContainsKey("dir") && d["dir"] == "ltr")
                confirm = "true";

            string smallgroup = null;
            if (d.ContainsKey("rel"))
                smallgroup = d["rel"];

            var id = GetId(d, "RsvpLink");

            string qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{smallgroup}";
            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink(
                $"/OnlineReg/RsvpLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(msg)}");

            var href = d["href"];
            if (href.Contains("regretslink", ignoreCase: true))
                url = url + "&regrets=true";

            return $@"<a href=""{url}"">{inside}</a>";
        }

    }
}
