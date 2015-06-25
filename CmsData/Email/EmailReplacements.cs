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
    public class EmailReplacements
    {
        private const string RegisterLinkRe = "<a[^>]*?href=\"https{0,1}://registerlink2{0,1}/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex registerLinkRe = new Regex(RegisterLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string RegisterTagRe = "(?:<|&lt;)registertag[^>]*(?:>|&gt;).+?(?:<|&lt;)/registertag(?:>|&gt;)";
        private readonly Regex registerTagRe = new Regex(RegisterTagRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string RegisterHrefRe = "href=\"https{0,1}://registerlink2{0,1}/\\d+\"";
        private readonly Regex registerHrefRe = new Regex(RegisterHrefRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string RsvpLinkRe = "<a[^>]*?href=\"https{0,1}://(?:rsvplink|regretslink)/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex rsvpLinkRe = new Regex(RsvpLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string SendLinkRe = "<a[^>]*?href=\"https{0,1}://sendlink2{0,1}/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex sendLinkRe = new Regex(SendLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string SupportLinkRe = "<a[^>]*?href=\"https{0,1}://supportlink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex supportLinkRe = new Regex(SupportLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VolReqLinkRe = "<a[^>]*?href=\"https{0,1}://volreqlink\"[^>]*>.*?</a>";
        private readonly Regex volReqLinkRe = new Regex(VolReqLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VolSubLinkRe = "<a[^>]*?href=\"https{0,1}://volsublink\"[^>]*>.*?</a>";
        private readonly Regex volSubLinkRe = new Regex(VolSubLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VoteLinkRe = "<a[^>]*?href=\"https{0,1}://votelink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex voteLinkRe = new Regex(VoteLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly string[] stringlist;
        private readonly MailAddress from;
        private string connStr;
        private string host;
        private int? currentOrgId;
        private CMSDataContext db;

        public EmailReplacements(CMSDataContext callingContext, string text, MailAddress from)
        {
            currentOrgId = callingContext.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            this.from = from;
            if (text == null)
                text = "(no content)";
            var pattern = @"(<style.*?</style>|{{[^}}]*?}}|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9})".Fmt(
                RegisterLinkRe, RegisterTagRe, RsvpLinkRe, RegisterHrefRe,
                SendLinkRe, SupportLinkRe, VolReqLinkRe,
                VolReqLinkRe, VolSubLinkRe, VoteLinkRe);
            stringlist = Regex.Split(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public List<MailAddress> ListAddresses { get; set; }

        private Person person;

        // this will run replacements in a new dataContext
        //
        public string DoReplacements(int pid, EmailQueueTo emailqueueto)
        {
            using (db = CMSDataContext.Create(connStr, host))
            {
                if (currentOrgId.HasValue)
                    db.SetCurrentOrgId(currentOrgId);
                var p = db.LoadPersonById(pid);
                person = p;
                var pi = emailqueueto.OrgId.HasValue
                    ? (from m in db.OrganizationMembers
                       let ts = db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == m.TranId && tt.PeopleId == m.PeopleId)
                       where m.PeopleId == emailqueueto.PeopleId && m.OrganizationId == emailqueueto.OrgId
                       select new PayInfo
                       {
                           PayLink = m.PayLink2(db),
                           Amount = ts.IndAmt,
                           AmountPaid = ts.IndPaid,
                           AmountDue = ts.IndDue,
                           RegisterMail = m.RegisterEmail
                       }).SingleOrDefault()
                    : null;

                var aa = db.GetAddressList(p);
                if (emailqueueto.EmailQueue.CCParents ?? false)
                    aa.AddRange(db.GetCcList(p, emailqueueto));

                if (emailqueueto.AddEmail.HasValue())
                    foreach (var ad in emailqueueto.AddEmail.SplitStr(","))
                        Util.AddGoodAddress(aa, ad);

                if (emailqueueto.OrgId.HasValue && pi != null)
                    Util.AddGoodAddress(aa, Util.FullEmail(pi.RegisterMail, p.Name));

                ListAddresses = aa.DistinctEmails();

                var noreplacements = emailqueueto.EmailQueue.NoReplacements ?? false;
                var texta = new List<string>(stringlist);
                for (var i = 1; i < texta.Count; i += 2)
                    if (noreplacements)
                        texta[i] = "";
                    else
                        texta[i] = DoReplaceCode(texta[i], p, pi, emailqueueto);

                db.SubmitChanges();
                return string.Join("", texta);
            }
        }

        // this will run replacements in the same dataContext as the calling method
        //
        public string DoReplacements(CMSDataContext callingContext, Person p)
        {
            person = p;

            db = callingContext;
            var aa = db.GetAddressList(p);

            ListAddresses = aa.DistinctEmails();

            var texta = new List<string>(stringlist);
            for (var i = 1; i < texta.Count; i += 2)
                texta[i] = DoReplaceCode(texta[i], p);

            return string.Join("", texta);
        }

        private class OrgInfo
        {
            public string Name { get; set; }
            public string Count { get; set; }
        }
        private readonly Dictionary<int, OrgInfo> orgcount = new Dictionary<int, OrgInfo>();

        private class PayInfo
        {
            public string PayLink { get; set; }
            public decimal? Amount { get; set; }
            public decimal? AmountPaid { get; set; }
            public decimal? AmountDue { get; set; }
            public string RegisterMail { get; set; }
        }

        private string DoReplaceCode(string code, Person p, PayInfo pi = null, EmailQueueTo emailqueueto = null)
        {
            if (code.StartsWith("<style"))
                return code;
            switch (code.ToLower())
            {
                case "{address}":
                    return p.PrimaryAddress;

                case "{address2}":
                    if (p.PrimaryAddress2.HasValue())
                        return "<br>" + p.PrimaryAddress2;
                    return "";

                case "{amtdue}":
                    if (pi != null)
                        return pi.AmountDue.ToString2("c");
                    break;

                case "{amtpaid}":
                    if (pi != null)
                        return pi.AmountPaid.ToString2("c");
                    break;

                case "{amount}":
                    if (pi != null)
                        return pi.Amount.ToString2("c");
                    break;

                case "{barcode}":
                    return string.Format("<img src='{0}' />", db.ServerLink("/Track/Barcode/" + p.PeopleId));

                case "{campus}":
                    return p.CampusId != null ? p.Campu.Description : "No Campus Specified";

                case "{cellphone}":
                    return p.CellPhone.HasValue() ? p.CellPhone.FmtFone() : "no cellphone on record";

                case "{city}":
                    return p.PrimaryCity;

                case "{csz}":
                    return Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip);

                case "{country}":
                    return p.PrimaryCountry;

                case "{createaccount}":
                    if (emailqueueto != null)
                        return CreateUserTag(emailqueueto);
                    break;

                case "{church}":
                    return db.Setting("NameOfChurch", "Set this in NameOfChurch setting");

                case "{cmshost}":
                    return db.ServerLink();

                case "{dob}":
                    return p.DOB;

                case "{emailhref}":
                    if (emailqueueto != null)
                        return db.ServerLink("/EmailView/" + emailqueueto.Id);
                    break;

                case "{first}":
                    return p.PreferredName.Contains("?") || p.PreferredName.Contains("unknown", true) ? "" : p.PreferredName;

                case "{fromemail}":
                    return from.Address;

                case "{homephone}":
                    return p.HomePhone.HasValue() ? p.HomePhone.FmtFone() : "no homephone on record";

                case "{last}":
                    return p.LastName;

                case "{name}":
                    return p.Name.Contains("?") || p.Name.Contains("unknown", true) ? "" : p.Name;

                case "{nextmeetingtime}":
                    if (emailqueueto != null)
                        return NextMeetingDate(code, emailqueueto);
                    break;
                case "{nextmeetingtime0}":
                    if (emailqueueto != null)
                        return NextMeetingDate0(code, emailqueueto);
                    break;

                case "{occupation}":
                    return p.OccupationOther;

                case "{orgname}":
                case "{org}":
                    return GetOrgInfo(emailqueueto).Name;

                case "{orgmembercount}":
                    return GetOrgInfo(emailqueueto).Count;

                case "{paylink}":
                    if (pi != null && pi.PayLink.HasValue())
                        return "<a href=\"{0}\">Click this link to make a payment and view your balance.</a>".Fmt(pi.PayLink);
                    break;

                case "{peopleid}":
                    return p.PeopleId.ToString();

                case "{salutation}":
                    if (emailqueueto != null)
                        return db.GoerSupporters.Where(ee => ee.Id == emailqueueto.GoerSupportId).Select(ee => ee.Salutation).SingleOrDefault();
                    break;

                case "{state}":
                    return p.PrimaryState;

                case "{email}":
                case "{toemail}":
                    if (ListAddresses.Count > 0)
                        return ListAddresses[0].Address;
                    break;

                case "{today}":
                    return DateTime.Today.ToShortDateString();

                case "{title}":
                    if (p.TitleCode.HasValue())
                        return p.TitleCode;
                    return p.ComputeTitle();

                case "{track}":
                    if (emailqueueto != null)
                        return emailqueueto.Guid.HasValue ? "<img src=\"{0}\" />".Fmt(db.ServerLink("/Track/Key/" + emailqueueto.Guid.Value.GuidToQuerystring())) : "";
                    break;

                case "{unsubscribe}":
                    if (emailqueueto != null)
                        return UnSubscribeLink(emailqueueto);
                    break;

                default:
                    if (emailqueueto == null)
                        emailqueueto = new EmailQueueTo()
                        {
                            PeopleId = p.PeopleId,
                            OrgId = db.CurrentOrgId0
                        };

                    if (code.StartsWith("{addsmallgroup:", StringComparison.OrdinalIgnoreCase))
                        return AddSmallGroup(code, emailqueueto);

                    if (code.StartsWith("{extra", StringComparison.OrdinalIgnoreCase))
                        return ExtraValue(code, emailqueueto);

                    if (registerLinkRe.IsMatch(code))
                        return RegisterLink(code, emailqueueto);

                    if (registerHrefRe.IsMatch(code))
                        return RegisterLinkHref(code, emailqueueto);

                    if (registerTagRe.IsMatch(code))
                        return RegisterTag(code, emailqueueto);

                    if (rsvpLinkRe.IsMatch(code))
                        return RsvpLink(code, emailqueueto);

                    if (sendLinkRe.IsMatch(code))
                        return SendLink(code, emailqueueto);

                    if (code.StartsWith("{orgextra:", StringComparison.OrdinalIgnoreCase))
                        return OrgExtra(code, emailqueueto);

                    if (code.StartsWith("{orgmember:", StringComparison.OrdinalIgnoreCase))
                        return OrgMember(code, emailqueueto);

                    if (code.StartsWith("{smallgroup:", StringComparison.OrdinalIgnoreCase))
                        return SmallGroup(code, emailqueueto);

                    if (regTextRe.IsMatch(code))
                        return RegText(code, emailqueueto);

                    if (code.StartsWith("{smallgroups", StringComparison.OrdinalIgnoreCase))
                        return SmallGroups(code, emailqueueto);

                    if (supportLinkRe.IsMatch(code))
                        return SupportLink(code, emailqueueto);

                    if (volReqLinkRe.IsMatch(code))
                        return VolReqLink(code, emailqueueto);

                    if (volSubLinkRe.IsMatch(code))
                        return VolSubLink(code, emailqueueto);

                    if (voteLinkRe.IsMatch(code))
                        return VoteLink(code, emailqueueto);

                    break;
            }

            return code;
        }

        private OrgInfo GetOrgInfo(EmailQueueTo emailqueueto)
        {
            OrgInfo oi = null;
            if (emailqueueto != null && emailqueueto.OrgId.HasValue)
            {
                if (!orgcount.ContainsKey(emailqueueto.OrgId.Value))
                {
                    var q = from i in db.Organizations
                            where i.OrganizationId == emailqueueto.OrgId.Value
                            select new OrgInfo()
                            {
                                Name = i.OrganizationName,
                                Count = i.OrganizationMembers.Count().ToString()
                            };
                    oi = q.SingleOrDefault();
                    orgcount.Add(emailqueueto.OrgId.Value, oi);
                }
                else
                    oi = orgcount[emailqueueto.OrgId.Value];
            }
            return oi ?? new OrgInfo();
        }

        const string AddSmallGroupRe = @"\{addsmallgroup:\[(?<group>[^\]]*)\]\}";
        readonly Regex addSmallGroupRe = new Regex(AddSmallGroupRe, RegexOptions.Singleline);
        private string AddSmallGroup(string code, EmailQueueTo emailqueueto)
        {
            var match = addSmallGroupRe.Match(code);
            if (!match.Success || !emailqueueto.OrgId.HasValue)
                return code;

            var group = match.Groups["group"].Value;
            var om = (from mm in db.OrganizationMembers
                      where mm.OrganizationId == emailqueueto.OrgId
                      where mm.PeopleId == emailqueueto.PeopleId
                      select mm).SingleOrDefault();
            if (om != null)
                om.AddToGroup(db, @group);
            return "";
        }

        const string OrgExtraRe = @"\{orgextra:(?<field>[^\]]*)\}";
        readonly Regex orgExtraRe = new Regex(OrgExtraRe, RegexOptions.Singleline);
        private string OrgExtra(string code, EmailQueueTo emailqueueto)
        {
            var match = orgExtraRe.Match(code);
            if (!match.Success || !emailqueueto.OrgId.HasValue)
                return code;
            var field = match.Groups["field"].Value;
            var ev = db.OrganizationExtras.SingleOrDefault(ee => ee.Field == field && ee.OrganizationId == db.CurrentOrg.Id);
            if (ev == null || !ev.Data.HasValue())
                return null;
            return ev.Data;
        }

        const string OrgMemberRe = @"{orgmember:(?<type>.*?),(?<divid>.*?)}";
        readonly Regex orgMemberRe = new Regex(OrgMemberRe, RegexOptions.Singleline);
        private string OrgMember(string code, EmailQueueTo emailqueueto)
        {
            var match = orgMemberRe.Match(code);
            if (!match.Success)
                return code;
            var divid = match.Groups["divid"].Value.ToInt();
            var type = match.Groups["type"].Value;
            var org = (from om in db.OrganizationMembers
                       where om.PeopleId == emailqueueto.PeopleId
                       where om.Organization.DivOrgs.Any(dd => dd.DivId == divid)
                       select om.Organization).FirstOrDefault();

            if (org == null)
                return "?";

            switch (type.ToLower())
            {
                case "location":
                    return org.Location;
                case "pendinglocation":
                case "pendingloc":
                    return org.PendingLoc;
                case "orgname":
                case "name":
                    return org.OrganizationName;
                case "leader":
                    return org.LeaderName;
            }
            return code;
        }

        private string CreateUserTag(EmailQueueTo emailqueueto)
        {
            User user = (from u in db.Users
                         where u.PeopleId == emailqueueto.PeopleId
                         select u).FirstOrDefault();
            if (user != null)
            {
                user.ResetPasswordCode = Guid.NewGuid();
                user.ResetPasswordExpires = DateTime.Now.AddHours(db.Setting("ResetPasswordExpiresHours", "24").ToInt());
                string link = db.ServerLink("/Account/SetPassword/" + user.ResetPasswordCode.ToString());
                db.SubmitChanges();
                return @"<a href=""{0}"">Set password for {1}</a>".Fmt(link, user.Username);
            }
            var ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = emailqueueto.PeopleId.ToString()
                };
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            string url = db.ServerLink("/Account/CreateAccount/{0}".Fmt(ot.Id.ToCode()));
            return @"<a href=""{0}"">Create Account</a>".Fmt(url);
        }

        const string ExtraValueRe = @"{extra(?<type>.*?):(?<field>.*?)}";
        readonly Regex extraValueRe = new Regex(ExtraValueRe, RegexOptions.Singleline);
        private string ExtraValue(string code, EmailQueueTo emailqueueto)
        {
            var match = extraValueRe.Match(code);
            if (!match.Success)
                return code;
            var field = match.Groups["field"].Value;
            var type = match.Groups["type"].Value;
            var ev = db.PeopleExtras.SingleOrDefault(ee => ee.Field == field && emailqueueto.PeopleId == ee.PeopleId);
            if (ev == null)
                return "";

            switch (type)
            {
                case "value":
                case "code":
                    return ev.StrValue;
                case "data":
                case "text":
                    return ev.Data;
                case "date":
                    return ev.DateValue.FormatDate();
                case "int":
                    return ev.IntValue.ToString();
                case "bit":
                case "bool":
                    return ev.BitValue.ToString();
            }
            return code;
        }

        private string NextMeetingDate(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;

            var mt = (from aa in db.Attends
                      where aa.OrganizationId == emailqueueto.OrgId
                      where aa.PeopleId == emailqueueto.PeopleId
                      where AttendCommitmentCode.committed.Contains(aa.Commitment ?? 0)
                      where aa.MeetingDate > DateTime.Now
                      orderby aa.MeetingDate
                      select aa.MeetingDate).FirstOrDefault();
            return mt == DateTime.MinValue ? "none" : mt.ToString("g");
        }

        private string NextMeetingDate0(string code, EmailQueueTo emailqueueto)
        {
            if (!emailqueueto.OrgId.HasValue)
                return code;

            var mt = (from mm in db.Meetings
                      where mm.OrganizationId == emailqueueto.OrgId
                      where mm.MeetingDate > DateTime.Now
                      orderby mm.MeetingDate
                      select mm.MeetingDate).FirstOrDefault() ?? DateTime.MinValue;
            return mt == DateTime.MinValue ? "none" : mt.ToString("g");
        }

        private string RegisterLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var id = GetId(d, "RegisterLink");

            var showfamily = code.Contains("registerlink2", ignoreCase: true);
            var qs = "{0},{1},{2}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id);
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
            var url = db.ServerLink("/OnlineReg/RegisterLink/{0}".Fmt(ot.Id.ToCode()));
            if (showfamily)
                url += "?showfamily=true";
            if (d.ContainsKey("style"))
                return @"<a href=""{0}"" style=""{1}"">{2}</a>".Fmt(url, d["style"], inside);
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private const string registerHrefReId = "href=\"https{0,1}://registerlink2{0,1}/(?<id>\\d+)\"";
        readonly Regex RegisterHrefReId = new Regex(registerHrefReId, RegexOptions.Singleline);
        private string RegisterLinkHref(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var match = RegisterHrefReId.Match(code);
            if (!match.Success)
                return code;
            var id = match.Groups["id"].Value.ToInt();

            var showfamily = code.Contains("registerlink2", ignoreCase: true);
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
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            string url = db.ServerLink("/OnlineReg/RegisterLink/{0}".Fmt(ot.Id.ToCode()));
            if (showfamily)
                url += "?showfamily=true";
            return "href=\"{0}\"".Fmt(url);
        }

        private string RegisterTag(string code, EmailQueueTo emailqueueto)
        {
            var doc = new HtmlDocument();
            if (code.Contains("&lt;"))
                code = HttpUtility.HtmlDecode(code);
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.FirstChild;
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var id = ele.Id.ToInt();
            var url = RegisterLinkUrl(db, id, emailqueueto.PeopleId, emailqueueto.Id, "registerlink");
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private string RsvpLink(string code, EmailQueueTo emailqueueto)
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
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            string url = db.ServerLink("/OnlineReg/RsvpLinkSg/{0}?confirm={1}&message={2}"
                                                      .Fmt(ot.Id.ToCode(), confirm, HttpUtility.UrlEncode(msg)));

            var href = d["href"];
            if (href.Contains("regretslink", ignoreCase: true))
                url = url + "&regrets=true";

            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private string SendLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var id = GetId(d, "SendLink");

            var showfamily = code.Contains("sendlink2", ignoreCase: true);
            var qs = "{0},{1},{2},{3}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id,
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
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink("/OnlineReg/SendLink/{0}".Fmt(ot.Id.ToCode()));
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        const string SmallGroupRe = @"\{smallgroup:\[(?<prefix>[^\]]*)\](?:,(?<def>[^}]*)){0,1}\}";
        readonly Regex smallGroupRe = new Regex(SmallGroupRe, RegexOptions.Singleline);
        private string SmallGroup(string code, EmailQueueTo emailqueueto)
        {
            var match = smallGroupRe.Match(code);
            if (!match.Success || !emailqueueto.OrgId.HasValue)
                return code;

            var prefix = match.Groups["prefix"].Value;
            var def = match.Groups["def"].Value;
            var sg = (from mm in db.OrgMemMemTags
                      where mm.OrgId == emailqueueto.OrgId
                      where mm.PeopleId == emailqueueto.PeopleId
                      where mm.MemberTag.Name.StartsWith(prefix)
                      select mm.MemberTag.Name).FirstOrDefault();
            if (!sg.HasValue())
                sg = def;
            return sg;
        }

        const string RegTextRe = @"{reg(?<type>.*?):(?<field>.*?)}";
        readonly Regex regTextRe = new Regex(RegTextRe, RegexOptions.Singleline);
        private string RegText(string code, EmailQueueTo emailqueueto)
        {
            var match = regTextRe.Match(code);
            if (!match.Success)
                return code;
            var field = match.Groups["field"].Value;
            var type = match.Groups["type"].Value;
            var answer = (from qa in db.ViewOnlineRegQAs
                          where qa.Question == field
                          where qa.Type == type
                          where qa.PeopleId == emailqueueto.PeopleId
                          where qa.OrganizationId == emailqueueto.OrgId
                          select qa.Answer).SingleOrDefault();

            return answer;
        }


        private string SmallGroups(string code, EmailQueueTo emailqueueto)
        {
            const string RE = @"\{smallgroups(:\[(?<prefix>[^\]]*)\]){0,1}\}";
            var re = new Regex(RE, RegexOptions.Singleline);
            var match = re.Match(code);
            if (!match.Success || !emailqueueto.OrgId.HasValue)
                return code;

            var prefix = match.Groups["prefix"].Value;
            var q = from mm in db.OrgMemMemTags
                    where mm.OrgId == emailqueueto.OrgId
                    where mm.PeopleId == emailqueueto.PeopleId
                    where mm.MemberTag.Name.StartsWith(prefix) || prefix == null || prefix == ""
                    orderby mm.MemberTag.Name
                    select mm.MemberTag.Name.Substring(prefix.Length);
            return string.Join("<br/>\n", q);
        }

        private string SupportLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var oid = GetId(d, "SupportLink");
            var qs = "{0},{1},{2},{3}:{4}".Fmt(oid, emailqueueto.PeopleId, emailqueueto.Id, "supportlink", emailqueueto.GoerSupportId);

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
            var url = db.ServerLink("/OnlineReg/SendLink/{0}".Fmt(ot.Id.ToCode()));
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private string UnSubscribeLink(EmailQueueTo emailqueueto)
        {
            var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl("{0}|{1}".Fmt(emailqueueto.PeopleId, from.Address));
            var url = db.ServerLink(qs);
            return @"<a href=""{0}"">Unsubscribe</a>".Fmt(url);
        }

        private string VolReqLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var qs = "{0},{1},{2},{3}".Fmt(d["mid"], d["pid"], d["ticks"], emailqueueto.PeopleId);
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

            var url = db.ServerLink("/OnlineReg/VolRequestResponse/{0}/{1}".Fmt(d["ans"], ot.Id.ToCode()));
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private string VolSubLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var qs = "{0},{1},{2},{3}".Fmt(d["aid"], d["pid"], d["ticks"], emailqueueto.PeopleId);
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

            var url = db.ServerLink("/OnlineReg/ClaimVolSub/{0}/{1}".Fmt(d["ans"], ot.Id.ToCode()));
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private string VoteLink(string code, EmailQueueTo emailqueueto)
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

            var qs = "{0},{1},{2},{3},{4}".Fmt(id, emailqueueto.PeopleId, emailqueueto.Id, pre, smallgroup);
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
            var url = db.ServerLink("/OnlineReg/VoteLinkSg/{0}?confirm={1}&message={2}"
                                                      .Fmt(ot.Id.ToCode(), confirm, HttpUtility.UrlEncode(msg)));
            return @"<a href=""{0}"">{1}</a>".Fmt(url, inside);
        }

        private static string GetId(IReadOnlyDictionary<string, string> d, string from)
        {
            string id = null;
            if (d.ContainsKey("lang"))
                id = d["lang"];
            else if (d.ContainsKey("id"))
                id = d["id"];
            if (id == null)
                throw new Exception("No \"Organization Id\" attribute found on \"{0}\"".Fmt(from));
            return id;
        }

        private static List<string> SPECIAL_FORMATS = new List<string>()
        {
            "http://votelink",
            "https://votelink",

            "http://registerlink",
            "https://registerlink",

            "http://registerlink2",
            "https://registerlink2",

            "http://supportlink",
            "https://supportlink",

            "http://rsvplink",
            "https://rsvplink",

            "http://regretslink",
            "https://regretslink",

            "http://volsublink",
            "https://volsublink",

            "http://volreqlink",
            "https://volreqlink",

            "http://sendlink",
            "https://sendlink",

            "http://sendlink2",
            "https://sendlink2",

            "{emailhref}"
        };
        public static bool IsSpecialLink(string link)
        {
            return SPECIAL_FORMATS.Contains(link.ToLower());
        }

        public static string RegisterLinkUrl(CMSDataContext db, int orgid, int pid, int queueid, string linktype)
        {
            var showfamily = linktype == "registerlink2";
            var qs = "{0},{1},{2},{3}".Fmt(orgid, pid, queueid, linktype);
            var ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink("/OnlineReg/RegisterLink/{0}".Fmt(ot.Id.ToCode()));
            if (showfamily)
                url += "?showfamily=true";
            return url;
        }

        public static string CreateRegisterLink(int? orgid, string text)
        {
            if (!orgid.HasValue)
                throw new ArgumentException("null not allowed on GetRegisterLink", "orgid");
            return "<a href=\"http://registerlink\" lang=\"{0}\">{1}</a>".Fmt(orgid, text);
        }
    }
}
