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
        private const string MatchRegisterLinkRe = "<a[^>]*?href=\"https{0,1}://registerlink2{0,1}/{0,1}\"[^>]*>.*?</a>";
        private static readonly Regex RegisterLinkRe = new Regex(MatchRegisterLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string RegisterLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var id = GetId(d, "RegisterLink");

            var showfamily = code.Contains("registerlink2", ignoreCase: true);
            var qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id}";
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
            var url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            if (showfamily)
                url += "?showfamily=true";
            if (d.ContainsKey("style"))
                return $@"<a href=""{url}"" style=""{d["style"]}"">{inside}</a>";
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private static string GetId(IReadOnlyDictionary<string, string> d, string from)
        {
            string id = null;
            if (d.ContainsKey("lang"))
                id = d["lang"];
            else if (d.ContainsKey("id"))
                id = d["id"];
            if (id == null)
                throw new Exception($"No \"Organization Id\" attribute found on \"{@from}\"");
            return id;
        }
        public static string RegisterLinkUrl(CMSDataContext db, int orgid, int pid, int queueid, string linktype, DateTime? expires = null)
        {
            var showfamily = linktype == "registerlink2";
            var qs = $"{orgid},{pid},{queueid},{linktype}";
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = qs,
            };
            if (expires.HasValue)
                ot.Expires = expires.Value;
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            if (showfamily)
                url += "?showfamily=true";
            return url;
        }
        public static string CreateRegisterLink(int? orgid, string text)
        {
            if (!orgid.HasValue)
                throw new ArgumentException("null not allowed on GetRegisterLink", nameof(orgid));
            return $"<a href=\"http://registerlink\" lang=\"{orgid}\">{text}</a>";
        }
    }
}
