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
        private const string MatchVolSubLinkRe = "<a[^>]*?href=\"https{0,1}://volsublink\"[^>]*>.*?</a>";
        private static readonly Regex VolSubLinkRe = new Regex(MatchVolSubLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string VolSubLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var qs = $"{d["aid"]},{d["pid"]},{d["ticks"]},{emailqueueto.PeopleId}";
            OneTimeLink ot = null;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs,
                    Expires = Util.Now.AddHours(72)
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }

            var url = db.ServerLink($"/OnlineReg/ClaimVolSub/{d["ans"]}/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

    }
}
