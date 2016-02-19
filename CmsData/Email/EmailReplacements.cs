using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private const string MasterLinkRe = "<a[^>]*?href=\"https{0,1}://masterlink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex masterLinkRe = new Regex(MasterLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VolReqLinkRe = "<a[^>]*?href=\"https{0,1}://volreqlink\"[^>]*>.*?</a>";
        private readonly Regex volReqLinkRe = new Regex(VolReqLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VolSubLinkRe = "<a[^>]*?href=\"https{0,1}://volsublink\"[^>]*>.*?</a>";
        private readonly Regex volSubLinkRe = new Regex(VolSubLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VoteLinkRe = "<a[^>]*?href=\"https{0,1}://votelink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex voteLinkRe = new Regex(VoteLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly string[] stringlist;
        private readonly MailAddress from;
        private readonly string connStr;
        private readonly string host;
        private int? currentOrgId;
        private CMSDataContext db;

        public EmailReplacements(CMSDataContext callingContext, string text, MailAddress from)
        {
            currentOrgId = callingContext.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
            this.from = from;

            if (text == null)
                text = "(no content)";

            var pattern =
                $@"(<style.*?</style>|{{[^}}]*?}}|{RegisterLinkRe}|{RegisterTagRe}|{RsvpLinkRe}|{RegisterHrefRe}|
                    {SendLinkRe}|{SupportLinkRe}|{MasterLinkRe}|{VolReqLinkRe}|{VolReqLinkRe}|{VolSubLinkRe}|{VoteLinkRe})";

            // we do the InsertDrafts replacement code here so that it is only inserted once before the replacements
            // and so that there can be replacement codes in the draft itself and they will get replaced.
            text = DoInsertDrafts(text);

            text = MapUrlEncodedReplacementCodes(text, new[] { "emailhref" });

            stringlist = Regex.Split(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
        private string DoInsertDrafts(string text)
        {
            var a = Regex.Split(text, "({insertdraft:.*?})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (a.Length <= 2)
                return text;
            for (var i = 1; i < a.Length; i += 2)
                if (a[i].StartsWith("{insertdraft:"))
                    a[i] = InsertDraft(a[i]);
            text = string.Join("", a);
            return text;
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

                if (emailqueueto.EmailQueue.FinanceOnly == true)
                {
                    var contributionemail = (from ex in db.PeopleExtras
                                             where ex.PeopleId == p.PeopleId
                                             where ex.Field == "ContributionEmail"
                                             select ex.Data).SingleOrDefault();
                    if (contributionemail.HasValue())
                        contributionemail = contributionemail.trim();
                    if (!Util.ValidEmail(contributionemail))
                        contributionemail = p.FromEmail;
                    aa.Clear();
                    aa.Add(Util.TryGetMailAddress(contributionemail));
                }

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
                    if (noreplacements && !texta[i].StartsWith("<style"))
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
                    return $"<img src='{db.ServerLink("/Track/Barcode/" + p.PeopleId)}' />";

                case "{campus}":
                    return p.CampusId != null ? p.Campu.Description : $"No {Util2.CampusLabel} Specified";

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

                case "{estatement}":
                    if (p.ElectronicStatement == true)
                        return "Online Electronic Statement Only";
                    else
                        return "Printed Statement in Addition to Online Option";

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
                        return $"<a href=\"{pi.PayLink}\">Click this link to make a payment and view your balance.</a>";
                    break;

                case "{peopleid}":
                    return p.PeopleId.ToString();

                case "{receivesms}":
                    if (p.ReceiveSMS == true)
                        return "Yes";
                    else
                        return "No";

                case "{salutation}":
                    if (emailqueueto != null)
                        return db.GoerSupporters.Where(ee => ee.Id == emailqueueto.GoerSupportId).Select(ee => ee.Salutation).SingleOrDefault();
                    break;

                case "{state}":
                    return p.PrimaryState;

                case "{statementtype}":
                    var stmtcode = p.ContributionOptionsId;
                    switch (stmtcode)
                    {
                        case CmsData.Codes.StatementOptionCode.Individual:
                            return "Individual";

                        case CmsData.Codes.StatementOptionCode.Joint:
                            return "Joint";

                        case CmsData.Codes.StatementOptionCode.None:
                        default:
                            return "None";
                    }

                    
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
                        return emailqueueto.Guid.HasValue ?
                            $"<img src=\"{db.ServerLink("/Track/Key/" + emailqueueto.Guid.Value.GuidToQuerystring())}\" />"
                            : "";
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

                    if (code.StartsWith("{orgbarcode"))
                        return OrgBarcode(code, emailqueueto);

                    if (code.StartsWith("{smallgroup:", StringComparison.OrdinalIgnoreCase))
                        return SmallGroup(code, emailqueueto);
                    if (code.StartsWith("{subgroup:", StringComparison.OrdinalIgnoreCase))
                        return SmallGroup(code, emailqueueto);

                    if (regTextRe.IsMatch(code))
                        return RegText(code, emailqueueto);

                    if (code.StartsWith("{smallgroups", StringComparison.OrdinalIgnoreCase))
                        return SmallGroups(code, emailqueueto);
                    if (code.StartsWith("{subgroups", StringComparison.OrdinalIgnoreCase))
                        return SmallGroups(code, emailqueueto);

                    if (supportLinkRe.IsMatch(code))
                        return SupportLink(code, emailqueueto);

                    if (masterLinkRe.IsMatch(code))
                        return MasterLink(code, emailqueueto);

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

        private string OrgBarcode(string code, EmailQueueTo emailqueueto)
        {
            var oid = code.StartsWith("{orgbarcode:")
                ? code.Substring(12).TrimEnd('}').ToInt()
                : emailqueueto.OrgId;
            return $@"<img src='{db.ServerLink($"/Track/Barcode/{oid}-{emailqueueto.PeopleId}")}' width='95%' />";
        }

        private OrgInfo GetOrgInfo(EmailQueueTo emailqueueto)
        {
            OrgInfo oi = null;
            if (emailqueueto == null && db.CurrentOrgId > 0)
                emailqueueto = new EmailQueueTo() {OrgId = db.CurrentOrgId};
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
        const string InsertDraftRe = @"\{insertdraft:(?<draft>.*?)}";
        readonly Regex insertDraftRe = new Regex(InsertDraftRe, RegexOptions.Singleline);
        private string InsertDraft(string code)
        {
            var match = insertDraftRe.Match(code);
            if (!match.Success)
                return code;

            var draft = match.Groups["draft"].Value;

            var s = db.ContentOfTypeSavedDraft(draft);
            return s;
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
                return $@"<a href=""{link}"">Set password for {user.Username}</a>";
            }
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = emailqueueto.PeopleId.ToString()
            };
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink($"/Account/CreateAccount/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">Create Account</a>";
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

        private const string RegisterHrefReId = "href=\"https{0,1}://registerlink2{0,1}/(?<id>\\d+)\"";
        readonly Regex registerHrefReId = new Regex(RegisterHrefReId, RegexOptions.Singleline);
        private string RegisterLinkHref(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var match = registerHrefReId.Match(code);
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
            return $@"<a href=""{url}"">{inside}</a>";
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

            string qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{smallgroup}";
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
                $"/OnlineReg/RsvpLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(msg)}");

            var href = d["href"];
            if (href.Contains("regretslink", ignoreCase: true))
                url = url + "&regrets=true";

            return $@"<a href=""{url}"">{inside}</a>";
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

        const string SmallGroupRe = @"\{(smallgroup|subgroup):\[(?<prefix>[^\]]*)\](?:,(?<def>[^}]*)){0,1}\}";
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


        const string SubGroupsRe = @"\{(smallgroups|subgroups)(:\[(?<prefix>[^\]]*)\]){0,1}\}";
        readonly Regex subGroupsRe = new Regex(SubGroupsRe, RegexOptions.Singleline);
        private string SmallGroups(string code, EmailQueueTo emailqueueto)
        {
            var match = subGroupsRe.Match(code);
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

        private string MasterLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var oid = GetId(d, "MasterLink");
            var qs = $"{oid},{emailqueueto.PeopleId},{emailqueueto.Id},{"masterlink"}";

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

        private string UnSubscribeLink(EmailQueueTo emailqueueto)
        {
            var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl($"{emailqueueto.PeopleId}|{@from.Address}");
            var url = db.ServerLink(qs);
            return $@"<a href=""{url}"">Unsubscribe</a>";
        }

        private string VolReqLink(string code, EmailQueueTo emailqueueto)
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

        private string VolSubLink(string code, EmailQueueTo emailqueueto)
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
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }

            var url = db.ServerLink($"/OnlineReg/ClaimVolSub/{d["ans"]}/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
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

            "http://masterlink",
            "https://masterlink",

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

        /// <summary>
        /// Depending on the WYSIWYG editor being used, the URLs (where replacement codes are set) might end up getting URL encoded.
        /// This method will replace the URL-encoded version with the normal version so that the actual replacement logic can be relatively
        /// consistent.
        /// </summary>
        private static string MapUrlEncodedReplacementCodes(string text, IEnumerable<string> codesToReplace)
        {
            foreach (var code in codesToReplace)
            {
                var codeToReplace = $"{{{code}}}";
                text = text.Replace(WebUtility.UrlEncode(codeToReplace), codeToReplace);
            }
            return text;
        }
    }
}
