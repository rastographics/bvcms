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
        private static List<string> SPECIAL_FORMATS = new List<string>() 
        { 
            "http://votelink", 
            "http://registerlink", 
            "http://registerlink2", 
            "http://supportlink", 
            "http://rsvplink", 
            "http://volsublink", 
            "http://volreqlink", 
            "http://sendlink", 
            "http://sendlink2", 
            "{emailhref}" 
        };

        public static bool IsSpecialLink(string link)
        {
            return SPECIAL_FORMATS.Contains(link.ToLower());
        }

        public List<MailAddress> DoReplacements(ref string text, Person p, EmailQueueTo emailqueueto)
        {
            if (text == null)
                text = "(no content)";
            if (text.Contains("{name}", ignoreCase: true))
                if (p.Name.Contains("?") || p.Name.Contains("unknown", true))
                    text = text.Replace("{name}", string.Empty);
                else
                    text = text.Replace("{name}", p.Name, ignoreCase: true);

            if (text.Contains("{first}", ignoreCase: true))
                if (p.PreferredName.Contains("?", true) || (p.PreferredName.Contains("unknown", true)))
                    text = text.Replace("{first}", string.Empty, ignoreCase: true);
                else
                    text = text.Replace("{first}", p.PreferredName, ignoreCase: true);
            text = text.Replace("{last}", p.LastName, ignoreCase: true);
            if (emailqueueto.GoerSupportId.HasValue)
            {
                var salutation = GoerSupporters.Where(ee => ee.Id == emailqueueto.GoerSupportId).Select(ee => ee.Salutation).Single();
                text = text.Replace("{salutation}", salutation, ignoreCase: true);
            }
            if (text.Contains("{occupation}", ignoreCase: true))
                text = text.Replace("{occupation}", p.OccupationOther, ignoreCase: true);
            if (text.Contains("{city}", ignoreCase: true))
                text = text.Replace("{city}", p.PrimaryCity, ignoreCase: true);
            if (text.Contains("{state}", ignoreCase: true))
                text = text.Replace("{state}", p.PrimaryState, ignoreCase: true);

            if (text.Contains("{emailhref}", ignoreCase: true))
            {
                string eurl = Util.URLCombine(CmsHost, "Manage/Emails/View/" + emailqueueto.Id);
                text = text.Replace("{emailhref}", eurl, ignoreCase: true);
            }

            text = DoVoteLinkAnchorStyle(text, emailqueueto);
            text = DoVoteTag(text, emailqueueto);
            text = DoVoteTag2(text, emailqueueto);
            text = DoRegisterTag(text, emailqueueto);
            text = DoRegisterTag2(text, emailqueueto);
            text = DoExtraValueData(text, emailqueueto);
            if (emailqueueto.OrgId.HasValue)
            {
                if (text.Contains("{smallgroup:", ignoreCase: true))
                    text = DoSmallGroupData(text, emailqueueto);
                if (text.Contains("{addsmallgroup:", ignoreCase: true))
                    text = DoAddSmallGroup(text, emailqueueto);
                if (text.Contains("{nextmeetingtime}", ignoreCase: true))
                    text = DoMeetingDate(text, emailqueueto);
                if (text.Contains("{smallgroups", ignoreCase: true))
                    text = DoSmallGroups(text, emailqueueto);
            }
            if (text.Contains("{today}", ignoreCase: true))
                text = text.Replace("{today}", DateTime.Today.ToShortDateString());
            if (text.Contains("{createaccount}", ignoreCase: true))
                text = text.Replace("{createaccount}", DoCreateUserTag(emailqueueto));
            if (text.Contains("{peopleid}", ignoreCase: true))
                text = text.Replace("{peopleid}", emailqueueto.PeopleId.ToString(), ignoreCase: true);
            if (text.Contains("http://votelink", ignoreCase: true))
                text = DoVoteLink(text, emailqueueto);
            if (text.Contains("http://sendlink", ignoreCase: true))
                text = DoSendLink(text, emailqueueto);
            if (text.Contains("http://supportlink", ignoreCase: true))
                text = DoSupportLink(text, emailqueueto);
            if (text.Contains("http://registerlink", ignoreCase: true))
                text = DoRegisterLink(text, emailqueueto);
            if (text.Contains("http://rsvplink", ignoreCase: true))
                text = DoRsvpLink(text, emailqueueto);
            if (text.Contains("http://volsublink", ignoreCase: true))
                text = DoVolSubLink(text, emailqueueto);
            if (text.Contains("http://volreqlink", ignoreCase: true))
                text = DoVolReqLink(text, emailqueueto);
            if (text.Contains("{barcode}", ignoreCase: true))
            {
                string link = Util.URLCombine(CmsHost, "/Track/Barcode/" + emailqueueto.PeopleId);
                text = text.Replace("{barcode}", "<img src='" + link + "' />", ignoreCase: true);
            }
            if (text.Contains("{cellphone}", ignoreCase: true))
                text = text.Replace("{cellphone}", p.CellPhone.HasValue() ? p.CellPhone.FmtFone() : "no cellphone on record", true);

            if (text.Contains("{campus}", ignoreCase: true))
                text = text.Replace("{campus}", p.CampusId != null ? p.Campu.Description : "No Campus Specified", true);

            if (emailqueueto.Guid.HasValue)
            {
                string turl = Util.URLCombine(CmsHost, "/Track/Key/" + emailqueueto.Guid.Value.GuidToQuerystring());
                text = text.Replace("{track}", "<img src=\"{0}\" />".Fmt(turl), ignoreCase: true);
            }

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

        private string DoMeetingDate(string text, EmailQueueTo emailqueueto)
        {
            DateTime mt = (from aa in Attends
                           where aa.OrganizationId == emailqueueto.OrgId
                           where aa.PeopleId == emailqueueto.PeopleId
                           where aa.Commitment == AttendCommitmentCode.Attending
                           where aa.MeetingDate > DateTime.Now
                           orderby aa.MeetingDate
                           select aa.MeetingDate).FirstOrDefault();
            text = text.Replace("{nextmeetingtime}", mt.ToString("g"));
            return text;
        }

        private string DoSmallGroups(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"\{smallgroups(:\[(?<prefix>[^\]]*)\]){0,1}\}";
            var re = new Regex(RE, RegexOptions.Singleline);
            Match match = re.Match(text);
            while (match.Success && emailqueueto.OrgId.HasValue)
            {
                string tag = match.Value;
                string prefix = match.Groups["prefix"].Value;
                var q = from mm in OrgMemMemTags
                        where mm.OrgId == emailqueueto.OrgId
                        where mm.PeopleId == emailqueueto.PeopleId
                        where mm.MemberTag.Name.StartsWith(prefix) || prefix == null || prefix == ""
                        orderby mm.MemberTag.Name
                        select mm.MemberTag.Name.Substring(prefix.Length);
                text = text.Replace(tag, string.Join("<br/>\n", q));
                match = match.NextMatch();
            }
            return text;
        }

        private string DoSmallGroupData(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"\{smallgroup:\[(?<prefix>[^\]]*)\](?:,(?<def>[^}]*)){0,1}\}";
            var re = new Regex(RE, RegexOptions.Singleline);
            Match match = re.Match(text);
            while (match.Success && emailqueueto.OrgId.HasValue)
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
                text = text.Replace(tag, sg);
                match = match.NextMatch();
            }
            return text;
        }

        private string DoAddSmallGroup(string text, EmailQueueTo emailqueueto)
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
                text = text.Replace(tag, "");
            }
            return text;
        }

        private string DoExtraValueData(string text, EmailQueueTo emailqueueto)
        {
            const string RE = @"{extra(?<type>.*?):(?<field>.*?)}";
            var re = new Regex(RE, RegexOptions.Singleline);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string field = match.Groups["field"].Value;
                string type = match.Groups["type"].Value;
                var ev =
                    PeopleExtras.SingleOrDefault(ee => ee.Field == field && emailqueueto.PeopleId == ee.PeopleId);
                string value = "";
                switch (type)
                {
                    case "value":
                        value = ev.StrValue;
                        break;
                    case "data":
                        value = ev.Data;
                        break;
                    case "date":
                        value = ev.DateValue.FormatDate();
                        break;
                    case "int":
                        value = ev.IntValue.ToString();
                        break;
                }
                text = text.Replace(tag, value);
                match = match.NextMatch();
            }
            return text;
        }

        private string DoVoteLinkAnchorStyle(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = @"{votelink(?<inside>[^}]*)}";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline);
            Match match = re.Match(text);
            while (match.Success)
            {
                string votelink = match.Value;
                string anchor = "<a " + match.Groups["inside"].Value + ">text</a>";
                anchor = anchor.Replace("&quot;", "\"");
                anchor = anchor.Replace("&rdquo;", "\"");
                anchor = anchor.Replace("&ldquo;", "\"");

                var doc = new HtmlDocument();
                doc.LoadHtml(anchor);
                HtmlNode ele = doc.DocumentNode.Element("a");
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);
                string txt = "click here";
                if (d.ContainsKey("text"))
                    txt = d["text"];

                string msg = "Thank you for responding.";
                if (d.ContainsKey("message"))
                    msg = d["message"];

                string confirm = "false";
                if (d.ContainsKey("confirm"))
                    confirm = d["confirm"];

                if (!d.ContainsKey("smallgroup"))
                    throw new Exception("Votelink: no smallgroup attribute");
                string smallgroup = d["smallgroup"];
                string pre = "";
                string[] a = smallgroup.SplitStr(":");
                if (a.Length > 1)
                    pre = a[0];

                var id = GetId(d, "VoteLink");

                string url = VoteLinkUrl(text, emailqueueto, list, votelink, id, msg, confirm, smallgroup, pre);
                text = text.Replace(votelink, @"<a href=""{0}"">{1}</a>".Fmt(url, txt));

                match = match.NextMatch();
            }
            return text;
        }

        //&lt;votetag .*?&gt;(?<inside>.+?)&lt;/votetag&gt;
        private string DoVoteTag2(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = @"&lt;votetag .*?&gt;(?<inside>.+?)&lt;/votetag&gt;";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string inside = HttpUtility.HtmlDecode(match.Groups["inside"].Value);

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("votetag");
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string msg = "Thank you for responding.";
                if (d.ContainsKey("message"))
                    msg = d["message"];

                string confirm = "false";
                if (d.ContainsKey("confirm"))
                    confirm = d["confirm"];

                if (!d.ContainsKey("smallgroup"))
                    throw new Exception("Votelink: no smallgroup attribute");
                string smallgroup = d["smallgroup"];
                string pre = "";
                string[] a = smallgroup.SplitStr(":");
                if (a.Length > 1)
                    pre = a[0];

                var id = GetId(d, "VoteTag");

                string url = VoteLinkUrl(text, emailqueueto, list, tag, id, msg, confirm, smallgroup, pre);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        private string DoCreateUserTag(EmailQueueTo emailqueueto)
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

        private string DoVoteLink(string text, EmailQueueTo emailqueueto)
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

        private string DoRsvpLink(string text, EmailQueueTo emailqueueto)
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

        private string DoSendLink(string text, EmailQueueTo emailqueueto)
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
        private string DoSupportLink(string text, EmailQueueTo emailqueueto)
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
        private string DoRegisterLink(string text, EmailQueueTo emailqueueto)
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

        public string DoVolSubLink(string text, EmailQueueTo emailqueueto)
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

        public string DoVolReqLink(string text, EmailQueueTo emailqueueto)
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

        private string DoVoteTag(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = @"<votetag[^>]*>(?<inside>.+?)</votetag>";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string inside = match.Groups["inside"].Value;

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("votetag");
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                string msg = "Thank you for responding.";
                if (d.ContainsKey("message"))
                    msg = d["message"];

                string confirm = "false";
                if (d.ContainsKey("confirm"))
                    confirm = d["confirm"];

                if (!d.ContainsKey("smallgroup"))
                    throw new Exception("Votelink: no smallgroup attribute");
                string smallgroup = d["smallgroup"];
                string pre = "";
                string[] a = smallgroup.SplitStr(":");
                if (a.Length > 1)
                    pre = a[0];

                var id = GetId(d, "VoteTag");

                string url = VoteLinkUrl(text, emailqueueto, list, tag, id, msg, confirm, smallgroup, pre);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        private string DoRegisterTag2(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = @"&lt;registertag .*?&gt;(?<inside>.+?)&lt;/registertag&gt;";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string inside = HttpUtility.HtmlDecode(match.Groups["inside"].Value);

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("registertag");
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                var id = GetId(d, "RegisterTag");

                string url = RegisterTagUrl(emailqueueto, list, id);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        private string DoRegisterTag(string text, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();
            const string VoteLinkRE = @"<registertag[^>]*>(?<inside>.+?)</registertag>";
            var re = new Regex(VoteLinkRE, RegexOptions.Singleline | RegexOptions.Multiline);
            Match match = re.Match(text);
            while (match.Success)
            {
                string tag = match.Value;
                string inside = match.Groups["inside"].Value;

                var doc = new HtmlDocument();
                doc.LoadHtml(tag);
                HtmlNode ele = doc.DocumentNode.Element("registertag");
                Dictionary<string, string> d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

                var id = GetId(d, "RegisterTag");

                string url = RegisterTagUrl(emailqueueto, list, id);
                text = text.Replace(tag, @"<a href=""{0}"">{1}</a>".Fmt(url, inside));
                match = match.NextMatch();
            }
            return text;
        }

        private static string GetId(Dictionary<string, string> d, string from)
        {
            string id = null;
            if (d.ContainsKey("lang"))
                id = d["lang"];
            else if (d.ContainsKey("id"))
                id = d["id"];
            if (id == null)
                throw new Exception("{0}: no id attribute".Fmt(from));
            return id;
        }

        private string RsvpLinkUrl(
            EmailQueueTo emailqueueto,
            Dictionary<string, OneTimeLink> list,
            string id,
            string smallgroup,
            string msg,
            string confirm)
        {
            string qs = "{0},{1},{2},{3}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id, smallgroup);
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
                OneTimeLinks.InsertOnSubmit(ot);
                SubmitChanges();
                list.Add(qs, ot);
            }
            string url = Util.URLCombine(CmsHost, "/OnlineReg/RsvpLinkSg/{0}?confirm={1}&message={2}"
                                                      .Fmt(ot.Id.ToCode(), confirm, HttpUtility.UrlEncode(msg)));
            return url;
        }

        private string VoteLinkUrl(string text,
                                   EmailQueueTo emailqueueto,
                                   Dictionary<string, OneTimeLink> list,
                                   string votelink,
                                   string id,
                                   string msg,
                                   string confirm,
                                   string smallgroup,
                                   string pre)
        {
            string qs = "{0},{1},{2},{3},{4}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id, pre, smallgroup);
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
                OneTimeLinks.InsertOnSubmit(ot);
                SubmitChanges();
                list.Add(qs, ot);
            }
            string url = Util.URLCombine(CmsHost, "/OnlineReg/VoteLinkSg/{0}?confirm={1}&message={2}"
                                                      .Fmt(ot.Id.ToCode(), confirm, HttpUtility.UrlEncode(msg)));
            return url;
        }

        private string RegisterTagUrl(EmailQueueTo emailqueueto,
                                      Dictionary<string, OneTimeLink> list,
                                      string id,
                                      bool showfamily = false)
        {
            string qs = "{0},{1},{2}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id);
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
                OneTimeLinks.InsertOnSubmit(ot);
                SubmitChanges();
                list.Add(qs, ot);
            }
            string url = Util.URLCombine(CmsHost, "/OnlineReg/RegisterLink/{0}".Fmt(ot.Id.ToCode()));
            if (showfamily)
                url += "?showfamily=true";
            return url;
        }
        public string RegisterLinkUrl(int orgid, int pid, int queueid, string linktype)
        {
            var showfamily = linktype == "registerlink2";
            string qs = "{0},{1},{2},{3}".Fmt(orgid, pid, queueid, linktype);
            var ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
            OneTimeLinks.InsertOnSubmit(ot);
            SubmitChanges();
            string url = Util.URLCombine(CmsHost, "/OnlineReg/RegisterLink/{0}".Fmt(ot.Id.ToCode()));
            if (showfamily)
                url += "?showfamily=true";
            return url;
        }
        private string SendLinkUrl(EmailQueueTo emailqueueto,
                                   Dictionary<string, OneTimeLink> list,
                                   string id,
                                   bool showfamily = false)
        {
            string qs = "{0},{1},{2},{3}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id,
                showfamily ? "registerlink2" : "registerlink");

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
                OneTimeLinks.InsertOnSubmit(ot);
                SubmitChanges();
                list.Add(qs, ot);
            }
            string url = Util.URLCombine(CmsHost, "/OnlineReg/SendLink/{0}".Fmt(ot.Id.ToCode()));
            return url;
        }
        private string SendSupportLinkUrl(EmailQueueTo emailqueueto, Dictionary<string, OneTimeLink> list)
        {
            string qs = "{0},{1},{2},{3},{4}".Fmt(emailqueueto.OrgId, emailqueueto.PeopleId, emailqueueto.Id, "supportlink", emailqueueto.GoerSupportId);

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
                OneTimeLinks.InsertOnSubmit(ot);
                SubmitChanges();
                list.Add(qs, ot);
            }
            string url = Util.URLCombine(CmsHost, "/OnlineReg/SendLink/{0}".Fmt(ot.Id.ToCode()));
            return url;
        }
    }
}