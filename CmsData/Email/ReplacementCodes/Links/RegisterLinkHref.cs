using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        // this is used when the regular <a> element is not used

        private const string MatchRegisterLinkHrefRe = "href=\"https{0,1}://registerlink2{0,1}/(?<id>\\d+)\"";
        private static readonly Regex RegisterLinkHrefRe = new Regex(MatchRegisterLinkHrefRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string RegisterLinkHrefReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var match = RegisterLinkHrefRe.Match(code);
            if (!match.Success)
                return code;
            var id = match.Groups["id"].Value.ToInt();

            var showfamily = code.Contains("registerlink2", ignoreCase: true);
            string qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id}";
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
            string url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            if (showfamily)
                url += "?showfamily=true";
            return $"href=\"{url}\"";
        }

    }
}
