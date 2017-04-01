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
        private const string MatchVoteLinkRe = "<a[^>]*?href=\"https{0,1}://votelink/{0,1}\"[^>]*>.*?</a>";
        private static readonly Regex VoteLinkRe = new Regex(MatchVoteLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string VoteLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            //<a dir="ltr" href="http://votelink" id="798" rel="smallgroup" title="This is a message">test</a>
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var msg = "Thank you for responding.";
            if (d.ContainsKey("title"))
                msg = d["title"];

            var confirm = "false";
            if (d.ContainsKey("dir") && d["dir"] == "ltr")
                confirm = "true";

            if (!d.ContainsKey("rel"))
                throw new Exception("Votelink: no smallgroup attribute");
            var smallgroup = d["rel"];
            var pre = "";
            var a = smallgroup.SplitStr(":");
            if (a.Length > 1)
                pre = a[0];

            var id = GetId(d, "VoteLink");

            var qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{pre},{smallgroup}";
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
                $"/OnlineReg/VoteLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(msg)}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

    }
}
