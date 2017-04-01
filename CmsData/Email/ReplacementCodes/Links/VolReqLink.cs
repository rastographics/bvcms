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
        private const string MatchVolReqLinkRe = "<a[^>]*?href=\"https{0,1}://volreqlink\"[^>]*>.*?</a>";
        private static readonly Regex VolReqLinkRe = new Regex(MatchVolReqLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string VolReqLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var qs = $"{d["mid"]},{d["pid"]},{d["ticks"]},{emailqueueto.PeopleId}";
            OneTimeLink ot = null;
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

            var url = db.ServerLink($"/OnlineReg/VolRequestResponse/{d["ans"]}/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

    }
}
