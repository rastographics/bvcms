using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSendLinkRe = "<a[^>]*?href=\"https{0,1}://sendlink2{0,1}/{0,1}\"[^>]*>.*?</a>";
        private static readonly Regex SendLinkRe = new Regex(MatchSendLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string SendLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var id = GetId(d, "SendLink");

            var showfamily = code.Contains("sendlink2", ignoreCase: true);
            var qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{(showfamily ? "registerlink2" : "registerlink")}";

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
            var url = db.ServerLink($"/OnlineReg/SendLink/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

    }
}
