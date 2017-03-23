using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CmsData.Email.ReplacementCodes;
using Novacode;
using UtilityExtensions;
using CmsData.View;

namespace CmsData
{
    public partial class EmailReplacements
    {
        public List<MailAddress> ListAddresses { get; set; }
        public List<OptOut> OptOuts;

        private readonly string[] stringlist;
        private readonly MailAddress from;
        private readonly string connStr;
        private readonly string host;
        private int? currentOrgId;
        private CMSDataContext db;
        private Person person;
        private PayInfo pi;
        private OrgInfos orgInfos;
        private OrgInfos OrgInfos => orgInfos ?? (orgInfos = new OrgInfos(db));

        public EmailReplacements(CMSDataContext callingContext, string text, MailAddress from, int? queueid = null, bool noPremailer = false)
        {
            currentOrgId = callingContext.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
            this.from = from;
            if (queueid > 0)
                OptOuts = db.OptOuts(queueid, from.Address).ToList();

            if (text == null)
                text = "(no content)";


            var pattern =
                $@"(<style.*?</style>|{CodeRe}|{RegisterLinkRe}|{RegisterTagRe}|{RsvpLinkRe}|{RegisterHrefRe}|
                    {SendLinkRe}|{SupportLinkRe}|{MasterLinkRe}|{VolReqLinkRe}|{VolReqLinkRe}|{VolSubLinkRe}|{VoteLinkRe})";

            // we do the InsertDrafts replacement code here so that it is only inserted once before the replacements
            // and so that there can be replacement codes in the draft itself and they will get replaced.
            text = DoInsertDrafts(text);

            text = MapUrlEncodedReplacementCodes(text, new[] { "emailhref" });
            if (!noPremailer)
                try
                {
                    var result = PreMailer.Net.PreMailer.MoveCssInline(text);
                    text = result.Html;
                }
                catch
                {
                    // ignore Premailer exceptions
                }

            stringlist = Regex.Split(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        public EmailReplacements(CMSDataContext callingContext, DocX doc)
        {
            currentOrgId = callingContext.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
            DocXDocument = doc;

            var text = doc.Text;

            var pattern =
                $@"({{[^}}]*?}}|{RegisterLinkRe}|{RegisterTagRe}|{RsvpLinkRe}|{RegisterHrefRe}|
                    {SendLinkRe}|{SupportLinkRe}|{MasterLinkRe}|{VolReqLinkRe}|{VolReqLinkRe}|{VolSubLinkRe}|{VoteLinkRe})";

            stringlist = Regex.Split(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        public EmailReplacements(CMSDataContext callingContext)
        {
            currentOrgId = callingContext.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
        }

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

                pi = GetPayInfo(emailqueueto.OrgId ?? currentOrgId, p.PeopleId);

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

                if (OptOuts != null && emailqueueto.EmailQueue.CCParents == true)
                {
                    var pp = OptOuts.SingleOrDefault(vv => vv.PeopleId == emailqueueto.PeopleId);
                    if (pp != null)
                    {
                        if (pp.HhPeopleId.HasValue && Util.ValidEmail(pp.HhEmail))
                        {
                            aa.Add(new MailAddress(pp.HhEmail, pp.HhName));
                            emailqueueto.Parent1 = pp.HhPeopleId;
                        }
                        if (pp.HhSpPeopleId.HasValue && Util.ValidEmail(pp.HhSpEmail))
                        {
                            aa.Add(new MailAddress(pp.HhSpEmail, pp.HhSpName));
                            emailqueueto.Parent2 = pp.HhSpPeopleId;
                        }
                    }
                }
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
                        texta[i] = DoReplaceCode(texta[i], emailqueueto);

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
                texta[i] = DoReplaceCode(texta[i]);

            return string.Join("", texta);
        }

        private DocX DocXDocument { get; set; }

        public DocX DocXReplacements(Person p)
        {
            person = p;
            var texta = new List<string>(stringlist);
            var dict = new Dictionary<string, string>();
            for (var i = 1; i < texta.Count; i += 2)
                if (!dict.ContainsKey(texta[i]))
                    dict.Add(texta[i], DoReplaceCode(texta[i]));
            var doc = DocXDocument.Copy();
            foreach (var d in dict)
                doc.ReplaceText(d.Key, Util.PickFirst(d.Value, "____"));
            return doc;
        }
        public string RenderCode(string code, Person p, int? orgId = null)
        {
            person = p;
            if (!code.StartsWith("{"))
                code = $"{{{code}}}";

            pi = GetPayInfo(orgId, p.PeopleId);

            return DoReplaceCode(code, new EmailQueueTo() { OrgId = orgId, PeopleId = p.PeopleId });
        }


        private string DoReplaceCode(string code, EmailQueueTo emailqueueto = null)
        {
            if (code.StartsWith("<style"))
                return code;
            switch (code.ToLower())
            {
                case "{address}":
                    return person.PrimaryAddress;

                case "{address2}":
                    if (person.PrimaryAddress2.HasValue())
                        return "<br>" + person.PrimaryAddress2;
                    return "";

                case "{amtdue}":
                    return pi?.AmountDue.ToString2("c") ?? code;

                case "{amtpaid}":
                    return pi?.AmountPaid.ToString2("c") ?? code;

                case "{amount}":
                    if (pi != null)
                        return pi.Amount.ToString2("c");
                    break;

                case "{barcode}":
                    return $"<img src='{db.ServerLink("/Track/Barcode/" + person.PeopleId)}' />";

                case "{birthdate}":
                    return Util.FormatBirthday(person.BirthYear, person.BirthMonth, person.BirthDay, "not available");

                case "{campus}":
                    return person.CampusId != null ? person.Campu.Description : $"No {Util2.CampusLabel} Specified";

                case "{cellphone}":
                    return person.CellPhone.HasValue() ? person.CellPhone.FmtFone() : "no cellphone on record";

                case "{city}":
                    return person.PrimaryCity;

                case "{csz}":
                    return Util.FormatCSZ(person.PrimaryCity, person.PrimaryState, person.PrimaryZip);

                case "{country}":
                    return person.PrimaryCountry;

                case "{createaccount}":
                    if (emailqueueto != null)
                        return CreateUserTag(emailqueueto);
                    break;

                case "{church}":
                    return db.Setting("NameOfChurch", "No NameOfChurch in Settings");

                case "{churchphone}":
                    return db.Setting("ChurchPhone", "No ChurchPhone in Settings");

                case "{cmshost}":
                    return db.ServerLink();

                case "{dob}":
                    return person.DOB;

                case "{estatement}":
                    if (person.ElectronicStatement == true)
                        return "Online Electronic Statement Only";
                    return "Printed Statement in Addition to Online Option";

                case "{emailhref}":
                    if (emailqueueto != null)
                        return db.ServerLink("/EmailView/" + emailqueueto.Id);
                    break;

                case "{first}":
                    return person.PreferredName.Contains("?") || person.PreferredName.Contains("unknown", true) ? "" : person.PreferredName;

                case "{fromemail}":
                    return from.Address;

                case "{homephone}":
                    return person.HomePhone.HasValue() ? person.HomePhone.FmtFone() : "no homephone on record";

                case "{last}":
                    return person.LastName;

                case "{name}":
                    return person.Name.Contains("?") || person.Name.Contains("unknown", true) ? "" : person.Name;

                case "{nextmeetingtime}":
                    if (emailqueueto != null)
                        return NextMeetingDate(emailqueueto.OrgId, emailqueueto.PeopleId) ?? code;
                    break;
                case "{nextmeetingtime0}":
                    if (emailqueueto != null)
                        return NextMeetingDate0(emailqueueto.OrgId) ?? code;
                    break;

                case "{occupation}":
                    return person.OccupationOther;

                case "{orgname}":
                case "{org}":
                    return OrgInfos.Name(emailqueueto?.OrgId);

                case "{orgmembercount}":
                    return OrgInfos.Count(emailqueueto?.OrgId);

                case "{paylink}":
                    if (pi != null && pi.PayLink.HasValue())
                        return $"<a href=\"{pi.PayLink}\">Click this link to make a payment and view your balance.</a>";
                    break;

                case "{peopleid}":
                    return person.PeopleId.ToString();

                case "{receivesms}":
                    return person.ReceiveSMS ? "Yes" : "No";

                case "{salutation}":
                    if (emailqueueto != null)
                        return db.GoerSupporters.Where(ee => ee.Id == emailqueueto.GoerSupportId)
                                .Select(ee => ee.Salutation).SingleOrDefault();
                    break;

                case "{state}":
                    return person.PrimaryState;

                case "{statementtype}":
                    var stmtcode = person.ContributionOptionsId;
                    switch (stmtcode)
                    {
                        case Codes.StatementOptionCode.Individual:
                            return "Individual";

                        case Codes.StatementOptionCode.Joint:
                            return "Joint";

                        case Codes.StatementOptionCode.None:
                        default:
                            return "None";
                    }

                case "{email}":
                case "{toemail}":
                    if (ListAddresses.Count > 0)
                        return ListAddresses[0].Address;
                    break;

                case "{today}":
                    return Util.Today.ToShortDateString();

                case "{title}":
                    if (person.TitleCode.HasValue())
                        return person.TitleCode;
                    return person.ComputeTitle();

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
                    return DoSpecialCode(code, emailqueueto);
            }
            return code;
        }

        private string DoSpecialCode(string code, EmailQueueTo emailqueueto)
        {
            if (emailqueueto == null)
                emailqueueto = new EmailQueueTo()
                {
                    PeopleId = person.PeopleId,
                    OrgId = db.CurrentOrgId0
                };

            if (AddSmallGroupRe.IsMatch(code))
                return AddSmallGroupReplacement(code, emailqueueto);

            if (PledgeRe.IsMatch(code))
                return PledgeReplacement(code, emailqueueto);

            if (FundnameRe.IsMatch(code))
                return FundNameReplacement(code);

            if (SettingRe.IsMatch(code))
                return SettingReplacement(code);

            if (ExtraValueRe.IsMatch(code))
                return ExtraValueReplacement(code, emailqueueto);

            if (FirstOrSubstituteRe.IsMatch(code))
                return FirstOrSubstituteReplacement(code);

            if (SubGroupsRe.IsMatch(code))
                return SubGroupsReplacement(code, emailqueueto);

            if (OrgExtraRe.IsMatch(code))
                return OrgExtraReplacement(code, emailqueueto);

            if (SmallGroupRe.IsMatch(code))
                return SmallGroupReplacement(code, emailqueueto);

            if (OrgMemberRe.IsMatch(code))
                return OrgMemberReplacement(code, emailqueueto);

            if (OrgBarCodeRe.IsMatch(code))
                return OrgBarCodeReplacement(code, emailqueueto);

            if (RegTextRe.IsMatch(code))
                return RegTextReplacement(code, emailqueueto);
            



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

            return code;
        }
    }
}