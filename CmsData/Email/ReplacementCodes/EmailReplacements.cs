using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CmsData.API;
using CmsData.Email.ReplacementCodes;
using UtilityExtensions;
using CmsData.View;
using Xceed.Words.NET;

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
        private OrgInfo orgInfos;
        private OrgInfo OrgInfos => orgInfos ?? (orgInfos = new OrgInfo(db));
        private const string MatchCodeRe = "(?<!{){(?!{)[^}]*?}";
        private readonly DynamicData pythonData;

        private const string MatchRes =
            MatchRegisterLinkRe + "|"
            + MatchRegisterTagRe + "|"
            + MatchRsvpLinkRe + "|"
            + MatchRegisterLinkHrefRe + "|"
            + MatchSendLinkRe + "|"
            + MatchSupportLinkRe + "|"
            + MatchMasterLinkRe + "|"
            + MatchVolReqLinkRe + "|"
            + MatchVolSubLinkRe + "|"
            + MatchVoteLinkRe + "|"
            + MatchUnlayerLinkRe;

        private const string Pattern1 = "(<style.*?</style>|" + MatchSettingUrlRe + "|" + MatchCodeRe + "|" + MatchRes + "|" + MatchDropFromOrgTagRe + ")";
        private const string Pattern2 = "(" + MatchCodeRe + "|" + MatchRes + ")";

        public EmailReplacements(CMSDataContext callingContext, string text, MailAddress from, int? queueid = null, bool noPremailer = false, DynamicData pythondata = null)
        {
            currentOrgId = Util2.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
            pythonData = pythondata;
            this.from = from;
            if (queueid > 0)
                OptOuts = db.OptOuts(queueid, from.Address).ToList();

            if (text == null)
                text = "(no content)";

            // we do the InsertDrafts replacement code here so that it is only inserted once before the replacements
            // and so that there can be replacement codes in the draft itself and they will get replaced.
            text = DoInsertDrafts(text);
            text = ColorCodesReplacement(text);

            text = MapUrlEncodedReplacementCodes(text, new[] { "emailhref" });
            if (!noPremailer)
                try
                {
                    text = text.Replace("{{", "<!--{{").Replace("}}", "}}-->");
                    var result = PreMailer.Net.PreMailer.MoveCssInline(text);
                    text = result.Html;
                    text = text.Replace("<!--{{", "{{").Replace("}}-->", "}}");
                    // prevent gmail from not rendering when an empty title is autoclosed.
                    text = text.Replace("<title />", "<title></title>");
                }
                catch
                {
                    // ignore Premailer exceptions
                }

            stringlist = Regex.Split(text, Pattern1, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        public EmailReplacements(CMSDataContext callingContext, DocX doc)
        {
            currentOrgId = Util2.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
            DocXDocument = doc;

            var text = doc.Text;

            stringlist = Regex.Split(text, Pattern2, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        public EmailReplacements(CMSDataContext callingContext)
        {
            currentOrgId = Util2.CurrentOrgId;
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

                pi = GetPayInfo(emailqueueto.OrgId ?? currentOrgId ?? db.CurrentSessionOrgId, p.PeopleId);

                var aa = db.GetAddressList(p);

                if (emailqueueto.EmailQueue.FinanceOnly == true)
                {
                    var contributionemail = (from ex in db.PeopleExtras
                                             where ex.PeopleId == p.PeopleId
                                             where ex.Field == "ContributionEmail"
                                             select ex.Data).SingleOrDefault();
                    if (contributionemail.HasValue())
                        contributionemail = contributionemail.Trim();
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
            pi = GetPayInfo(currentOrgId ?? db.CurrentSessionOrgId, p.PeopleId);

            var aa = db.GetAddressList(p);

            ListAddresses = aa.DistinctEmails();

            var texta = new List<string>(stringlist);
            for (var i = 1; i < texta.Count; i += 2)
                texta[i] = DoReplaceCode(texta[i]);

            return string.Join("", texta);
        }

        private DocX DocXDocument { get; set; }

        public DocX DocXReplacements(Person p, int? orgId = null)
        {
            person = p;
            var eqto = new EmailQueueTo() {OrgId = orgId, PeopleId = p.PeopleId};
            var texta = new List<string>(stringlist);
            var dict = new Dictionary<string, string>();
            for (var i = 1; i < texta.Count; i += 2)
                if (!dict.ContainsKey(texta[i]))
                    dict.Add(texta[i], DoReplaceCode(texta[i], eqto));
            var doc = DocXDocument.Copy();
            foreach (var d in dict)
                doc.ReplaceText(d.Key, Util.PickFirst(d.Value, "____"));
            return doc;
        }

        public static List<string> TextReplacementsList(string text)
        {
            var stringlist = Regex.Split(text, Pattern2, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            var codes = new List<string>();
            for (var i = 1; i < stringlist.Length; i += 2)
                if (!codes.Contains(stringlist[i]))
                    codes.Add(stringlist[i]);
            return codes;
        }
        public Dictionary<string, string> DocXReplacementsDictionary(Person p, int? orgid = null)
        {
            person = p;
            var eqto = new EmailQueueTo() {OrgId = orgid, PeopleId = p?.PeopleId ?? 0};
            var texta = new List<string>(stringlist);
            var dict = new Dictionary<string, string>();
            for (var i = 1; i < texta.Count; i += 2)
                if (!dict.ContainsKey(texta[i]))
                    dict.Add(texta[i], p == null ? "" : DoReplaceCode(texta[i], eqto));
            return dict;
        }
        public string RenderCode(string code, Person p, int? orgId = null)
        {
            person = p;
            if (!code.StartsWith("{"))
                code = $"{{{code}}}";

            pi = GetPayInfo(orgId, p.PeopleId);

            return DoReplaceCode(code, new EmailQueueTo() { OrgId = orgId, PeopleId = p.PeopleId });
        }


    }
}
