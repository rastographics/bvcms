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
        private const string MatchSupportLinkRe = "<a[^>]*?href=\"https{0,1}://supportlink/{0,1}\"[^>]*>.*?</a>";
        private static readonly Regex SupportLinkRe = new Regex(MatchSupportLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string SupportLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var oid = GetId(d, "SupportLink");
            var qs = $"{oid},{emailqueueto.PeopleId},{emailqueueto.Id},{"supportlink"}:{emailqueueto.GoerSupportId}";

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
