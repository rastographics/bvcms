using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using CmsData.Codes;
using HtmlAgilityPack;
using UtilityExtensions;

namespace CmsData
{
    public partial class CMSDataContext
    {
        public List<MailAddress> FindReplacements(ref string text, Person p, EmailQueueTo emailqueueto)
        {
            if (text == null)
                text = "(no content)";

            var texta = Regex.Split(text, "({.*?})");
            for (var i = 0; i < texta.Length; i++)
                if (i % 2 == 1)
                    texta[i] = FindReplacements2(texta[i], p, emailqueueto);
            List<MailAddress> aa = GetAddressList(p);

            if (emailqueueto.AddEmail.HasValue())
                foreach (string ad in emailqueueto.AddEmail.SplitStr(","))
                    Util.AddGoodAddress(aa, ad);

            if (emailqueueto.OrgId.HasValue)
            {
                var qm = (from m in OrganizationMembers
                          where m.PeopleId == emailqueueto.PeopleId && m.OrganizationId == emailqueueto.OrgId
                          select new { m.PayLink, m.Amount, AmountPaid = TotalPaid(emailqueueto.OrgId, emailqueueto.PeopleId), m.RegisterEmail }).SingleOrDefault();
                if (qm != null)
                {
                    if (qm.PayLink.HasValue())
                        if (text.Contains("{paylink}", ignoreCase: true))
                            text = text.Replace("{paylink}", "<a href=\"{0}\">payment link</a>".Fmt(qm.PayLink),
                                                ignoreCase: true);
                    if (text.Contains("{amtdue}", ignoreCase: true))
                        text = text.Replace("{amtdue}", (qm.Amount - qm.AmountPaid).ToString2("c"), ignoreCase: true);
                    Util.AddGoodAddress(aa, Util.FullEmail(qm.RegisterEmail, p.Name));
                }
            }
            return aa.DistinctEmails();
        }
        public string FindReplacements2(string text, Person p, EmailQueueTo emailqueueto)
        {
            switch (text.ToLower())
            {
                case "{name}":
                    if (p.Name.Contains("?") || p.Name.Contains("unknown", true))
                        return "";
                    return p.Name;
                case "{first}":
                    if (p.PreferredName.Contains("?") || p.PreferredName.Contains("unknown", true))
                        return "";
                    return p.PreferredName;
                case "{last}":
                    return p.LastName;
                case "{occupation}":
                    return p.OccupationOther;
                case "{city}":
                    return p.PrimaryCity;
                case "{state}":
                    return p.PrimaryState;
                case "{emailhref}":
                    return Util.URLCombine(CmsHost, "Manage/Emails/View/" + emailqueueto.Id);
                case "{nextmeetingtime}":
                    return FindMeetingDate(text, emailqueueto);
                case "{today}":
                    return DateTime.Today.ToShortDateString();
                case "{createaccount}":
                    return FindCreateUserTag(emailqueueto);
                case "{peopleid}":
                    return p.PeopleId.ToString();
                case "http://votelink":
                    return FindVoteLink(text, emailqueueto);
                case "http://sendlink":
                    return FindSendLink(text, emailqueueto);
                case "http://supportlink":
                    return FindSupportLink(text, emailqueueto);
                case "http://registerlink":
                    return FindRegisterLink(text, emailqueueto);
                case "http://rsvplink":
                    return FindRsvpLink(text, emailqueueto);
                case "http://volsublink":
                    return FindVolSubLink(text, emailqueueto);
                case "http://volreqlink":
                    return FindVolReqLink(text, emailqueueto);
                case "{barcode}":
                    return string.Format("<img src='{0}' />", Util.URLCombine(CmsHost, "/Track/Barcode/" + emailqueueto.PeopleId));
                case "{cellphone}":
                    return p.CellPhone.HasValue() ? p.CellPhone.FmtFone() : "no cellphone on record";
                case "{campus}":
                    return p.CampusId != null ? p.Campu.Description : "No Campus Specified";
                case "{track}":
                    return emailqueueto.Guid.HasValue ? "<img src=\"{0}\" />".Fmt(Util.URLCombine(CmsHost, "/Track/Key/" + emailqueueto.Guid.Value.GuidToQuerystring())) : "";
                case "{saluation}":
                    return GoerSupporters.Where(ee => ee.Id == emailqueueto.GoerSupportId).Select(ee => ee.Salutation).Single();



                case "{paylink}":
                    return "";
                case "{amtdue}":
                    return "";
            }
            if (text.StartsWith("{smallgrouop:"))
                return FindSmallGroupData(text, emailqueueto);

            if (text.StartsWith("{addsmallgroup:"))
                return FindAddSmallGroup(text, emailqueueto);

            if (text.StartsWith("{smallgroups"))
                return FindSmallGroups(text, emailqueueto); ;

            if (text.StartsWith("{extra"))
                return FindExtraValueData(text, emailqueueto);

            return text;
        }

        private string FindMeetingDate(string text, EmailQueueTo emailqueueto)
        {
            if (emailqueueto.OrgId.HasValue)
            {
                DateTime mt = (from aa in Attends
                               where aa.OrganizationId == emailqueueto.OrgId
                               where aa.PeopleId == emailqueueto.PeopleId
                               where aa.Commitment == AttendCommitmentCode.Attending
                               where aa.MeetingDate > DateTime.Now
                               orderby aa.MeetingDate
                               select aa.MeetingDate).FirstOrDefault();
                text = mt.ToString("g");
            }
            return text;
        }

        private string FindSmallGroups(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"\{smallgroups(:\[(?<prefix>[^\]]*)\]){0,1}\}";
            var re = new Regex(RE, RegexOptions.Singleline);
            Match match = re.Match(text);
            if (match.Success && emailqueueto.OrgId.HasValue)
            {
                string tag = match.Value;
                string prefix = match.Groups["prefix"].Value;
                var q = from mm in OrgMemMemTags
                        where mm.OrgId == emailqueueto.OrgId
                        where mm.PeopleId == emailqueueto.PeopleId
                        where mm.MemberTag.Name.StartsWith(prefix) || prefix == null || prefix == ""
                        orderby mm.MemberTag.Name
                        select mm.MemberTag.Name.Substring(prefix.Length);
                return string.Join("<br/>\n", q);
            }
            return text;
        }

        private string FindSmallGroupData(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"\{smallgroup:\[(?<prefix>[^\]]*)\](?:,(?<def>[^}]*)){0,1}\}";
            var re = new Regex(RE, RegexOptions.Singleline);
            var match = re.Match(text);
            if (match.Success && emailqueueto.OrgId.HasValue)
            {
                string tag = match.Value;
                string prefix = match.Groups["prefix"].Value;
                string def = match.Groups["def"].Value;
                string sg = (from mm in OrgMemMemTags
                             where mm.OrgId == emailqueueto.OrgId
                             where mm.PeopleId == emailqueueto.PeopleId
                             where mm.MemberTag.Name.StartsWith(prefix)
                             select mm.MemberTag.Name).FirstOrDefault();
                if (!sg.HasValue())
                    sg = def;
                return sg;
            }
            return text;
        }

        private string FindAddSmallGroup(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"\{addsmallgroup:\[(?<group>[^\]]*)\]\}";
            var re = new Regex(RE, RegexOptions.Singleline);
            Match match = re.Match(text);
            if (match.Success && emailqueueto.OrgId.HasValue)
            {
                string tag = match.Value;
                string group = match.Groups["group"].Value;
                OrganizationMember om = (from mm in OrganizationMembers
                                         where mm.OrganizationId == emailqueueto.OrgId
                                         where mm.PeopleId == emailqueueto.PeopleId
                                         select mm).SingleOrDefault();
                if (om != null)
                    om.AddToGroup(this, group);
                return "";
            }
            return text;
        }

        private string FindExtraValueData(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"{extra(?<type>.*?):(?<field>.*?)}";
            var re = new Regex(RE, RegexOptions.Singleline);
            var match = re.Match(text);
            if (match.Success)
            {
                string field = match.Groups["field"].Value;
                string type = match.Groups["type"].Value;
                var ev = PeopleExtras.SingleOrDefault(ee => ee.Field == field && emailqueueto.PeopleId == ee.PeopleId);
                switch (type)
                {
                    case "value":
                        return ev.StrValue;
                    case "data":
                        return ev.Data;
                    case "date":
                        return ev.DateValue.FormatDate();
                    case "int":
                        return ev.IntValue.ToString();
                }
            }
            return text;
        }

        private string FindCreateUserTag(EmailQueueTo emailqueueto)
        {
            User user = (from u in Users
                         where u.PeopleId == emailqueueto.PeopleId
                         select u).FirstOrDefault();
            if (user != null)
            {
                user.ResetPasswordCode = Guid.NewGuid();
                user.ResetPasswordExpires = DateTime.Now.AddHours(Setting("ResetPasswordExpiresHours", "24").ToInt());
                string link = Util.URLCombine(CmsHost, "/Account/SetPassword/" + user.ResetPasswordCode.ToString());
                SubmitChanges();
                return @"<a href=""{0}"">Set password for {1}</a>".Fmt(link, user.Username);
            }
            var ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = emailqueueto.PeopleId.ToString()
                };
            OneTimeLinks.InsertOnSubmit(ot);
            SubmitChanges();
            string url = Util.URLCombine(CmsHost, "/Account/CreateAccount/{0}".Fmt(ot.Id.ToCode()));
            return @"<a href=""{0}"">Create Account</a>".Fmt(url);
        }

        private string FindVoteLink(string text, EmailQueueTo emailqueueto)
        {
            //<a dir="ltr" href="http://votelink" id="798" rel="smallgroup" title="This is a message">test</a>
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = "<a[^>]*?href=\"https{0,1}://votelink/{0,1}\"[^>]*>.*?</a>";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string msg = "Thank you for responding.";
                if (d.ContainsKey("title"))
                    msg = d["title"];

                string confirm = "false";
                if (d.ContainsKey("dir") && d["dir"] == "ltr")
                    confirm = "true";

                if (!d.ContainsKey("rel"))
                    throw new Exception("Votelink: no smallgroup attribute");
                string smallgroup = d["rel"];
                string pre = "";
                string[] a = smallgroup.SplitStr(":");
                if (a.Length > 1)
                    pre = a[0];

                var id = GetId(d, "VoteLink");

                string url = VoteLinkUrl(text, emailqueueto, list, tag, id, msg, confirm, smallgroup, pre);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        private string FindRsvpLink(string text, EmailQueueTo emailqueueto)
        {
            //<a dir="ltr" href="http://rsvplink" id="798" rel="meetingid" title="This is a message">test</a>
            var list = new Dictionary<string, OneTimeLink>();
            const string RsvpLinkRE = "<a[^>]*?href=\"https{0,1}://rsvplink/{0,1}\"[^>]*>.*?</a>";
            var re = new Regex(RsvpLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string msg = "Thank you for responding.";
                if (d.ContainsKey("title"))
                    msg = d["title"];

                string confirm = "false";
                if (d.ContainsKey("dir") && d["dir"] == "ltr")
                    confirm = "true";

                string smallgroup = null;
                if (d.ContainsKey("rel"))
                    smallgroup = d["rel"];

                var id = GetId(d, "RsvpLink");

                string url = RsvpLinkUrl(emailqueueto, list, id, smallgroup, msg, confirm);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        private string FindSendLink(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string SendLinkRE = "<a[^>]*?href=\"https{0,1}://(?<slink>sendlink2{0,1})/{0,1}\"[^>]*>.*?</a>";
            var re = new Regex(SendLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string slink = match.Groups["slink"].Value.ToLower();

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                var id = GetId(d, "SendLink");

                string url = SendLinkUrl(emailqueueto, list, id, showfamily: slink == "sendlink2");
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }
        private string FindSupportLink(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string SendLinkRE = "<a[^>]*?href=\"https{0,1}://(?<slink>supportlink)/{0,1}\"[^>]*>.*?</a>";
            var re = new Regex(SendLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string slink = match.Groups["slink"].Value.ToLower();

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string url = SendSupportLinkUrl(emailqueueto, list);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }
        private string FindRegisterLink(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = "<a[^>]*?href=\"https{0,1}://(?<rlink>registerlink2{0,1})/{0,1}\"[^>]*>.*?</a>";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string rlink = match.Groups["rlink"].Value.ToLower();

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                var id = GetId(d, "RegisterLink");

                string url = RegisterTagUrl(emailqueueto, list, id, showfamily: rlink == "registerlink2");
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        public string FindVolSubLink(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VolSubLinkRE = "<a[^>]*?href=\"https{0,1}://volsublink\"[^>]*>.*?</a>";
            var re = new Regex(VolSubLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string qs = "{0},{1},{2},{3}"
                    .Fmt(d["aid"], d["pid"], d["ticks"], emailqueueto.PeopleId);
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
                    OneTimeLinks.InsertOnSubmit(ot);
                    SubmitChanges();
                    list.Add(qs, ot);
                }

                string url = Util.URLCombine(CmsHost, "/OnlineReg/ClaimVolSub/{0}/{1}".Fmt(d["ans"], ot.Id.ToCode()));
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        public string FindVolReqLink(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VolSubLinkRE = "<a[^>]*?href=\"https{0,1}://volreqlink\"[^>]*>.*?</a>";
            var re = new Regex(VolSubLinkRE, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("a");
                string inside = ele.InnerHtml;
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string qs = "{0},{1},{2},{3}"
                    .Fmt(d["mid"], d["pid"], d["ticks"], emailqueueto.PeopleId);
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
                    OneTimeLinks.InsertOnSubmit(ot);
                    SubmitChanges();
                    list.Add(qs, ot);
                }

                string url = Util.URLCombine(CmsHost,
                                             "/OnlineReg/RequestResponse?ans={0}&guid={1}".Fmt(d["ans"], ot.Id.ToCode()));
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

    }
}