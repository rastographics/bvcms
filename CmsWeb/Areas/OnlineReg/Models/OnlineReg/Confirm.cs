using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {
        public void FinishLaterNotice()
        {
            var registerLink = EmailReplacements.CreateRegisterLink(masterorgid ?? Orgid,
                "Resume registration for {0}".Fmt(Header));
            var msg = "<p>Hi {first},</p>\n<p>Here is the link to continue your registration:</p>\n" + registerLink;
            Debug.Assert((masterorgid ?? Orgid) != null, "m.Orgid != null");
            var notifyids = DbUtil.Db.NotifyIds((masterorg ?? org).NotifyIds);
            var p = UserPeopleId.HasValue ? DbUtil.Db.LoadPersonById(UserPeopleId.Value) : List[0].person;
            DbUtil.Db.Email(notifyids[0].FromEmail, p, "Continue your registration for {0}".Fmt(Header), msg);
        }

        public string CheckDuplicateGift(decimal? amt)
        {
            if (!OnlineGiving() || !amt.HasValue)
                return null;

            var previousTransaction =
                (from t in DbUtil.Db.Transactions
                 where t.Amt == amt
                 where t.OrgId == Orgid
                 where t.TransactionDate > DateTime.Now.AddMinutes(-60)
                 where DbUtil.Db.Contributions.Any(cc => cc.PeopleId == List[0].PeopleId && cc.TranId == t.Id)
                 select t).FirstOrDefault();

            if (previousTransaction == null)
                return null;

            return "You have already submitted a gift in this amount a short while ago. Please let us know if you saw an error and what the message said.";
        }

        public RouteModel FinishRegistration(Transaction ti)
        {
            TranId = ti.Id;
            HistoryAdd("ProcessPayment");
            var ed = DbUtil.Db.RegistrationDatas.Single(dd => dd.Id == DatumId);
            ed.Data = Util.Serialize(this);
            ed.Completed = true;
            DbUtil.Db.SubmitChanges();

            try
            {
                var view = ConfirmTransaction(ti.TransactionId);
                switch (view)
                {
                    case ConfirmEnum.Confirm:
                        return RouteModel.ViewAction("Confirm", this);
                    case ConfirmEnum.ConfirmAccount:
                        return RouteModel.ViewAction("ConfirmAccount");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RouteModel.ErrorMessage(ex.Message);
            }
            return null;
        }

        public ConfirmEnum ConfirmTransaction(string transactionId)
        {
            ParseSettings();
            if (List.Count == 0)
                throw new Exception(" unexpected, no registrants found in confirmation");
            var ret = ConfirmEnum.Confirm;
            var managingsubs = ManagingSubscriptions();
            var choosingslots = ChoosingSlots();
            var t = Transaction;
            if (t == null && !managingsubs && !choosingslots)
            {
                HistoryAdd("ConfirmTransaction");
                UpdateDatum(completed: true);
                var pf = PaymentForm.CreatePaymentForm(this);
                t = pf.CreateTransaction(DbUtil.Db);
                TranId = t.Id;
            }

            if (org != null && org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
            {
                List[0].CreateAccount();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (OnlineGiving())
            {
                var p = List[0];
                if (p.IsNew)
                    p.AddPerson(null, p.org.EntryPointId ?? 0);

                var staff = DbUtil.Db.StaffPeopleForOrg(p.org.OrganizationId)[0];
                var text = p.setting.Body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
                text = text.Replace("{amt}", (t.Amt ?? 0).ToString("N2"));
                text = text.Replace("{date}", DateTime.Today.ToShortDateString());
                text = text.Replace("{tranid}", t.Id.ToString());
                //text = text.Replace("{name}", p.person.Name);
                text = text.Replace("{account}", "");
                text = text.Replace("{email}", p.person.EmailAddress);
                text = text.Replace("{phone}", p.person.HomePhone.FmtFone());
                text = text.Replace("{contact}", staff.Name);
                text = text.Replace("{contactemail}", staff.EmailAddress);
                text = text.Replace("{contactphone}", p.org.PhoneNumber.FmtFone());
                var re = new Regex(@"(?<b>.*?)<!--ITEM\sROW\sSTART-->(?<row>.*?)\s*<!--ITEM\sROW\sEND-->(?<e>.*)", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
                var match = re.Match(text);
                var b = match.Groups["b"].Value;
                var row = match.Groups["row"].Value.Replace("{funditem}", "{0}").Replace("{itemamt}", "{1:N2}");
                var e = match.Groups["e"].Value;
                var sb = new StringBuilder(b);

                var desc = "{0}; {1}; {2}".Fmt(
                        p.person.Name,
                        p.person.PrimaryAddress,
                        p.person.PrimaryZip);
                foreach (var g in p.FundItemsChosen())
                {
                    if (g.amt > 0)
                    {
                        sb.AppendFormat(row, g.desc, g.amt);
                        p.person.PostUnattendedContribution(DbUtil.Db, g.amt, g.fundid, desc, tranid: t.Id);
                    }
                }
                t.TransactionPeople.Add(new TransactionPerson
                {
                    PeopleId = p.person.PeopleId,
                    Amt = t.Amt,
                    OrgId = Orgid,
                });
                t.Financeonly = true;
                if (t.Donate > 0)
                {
                    var fundname = DbUtil.Db.ContributionFunds.Single(ff => ff.FundId == p.setting.DonationFundId).FundName;
                    sb.AppendFormat(row, fundname, t.Donate);
                    t.Fund = p.setting.DonationFund();
                    p.person.PostUnattendedContribution(DbUtil.Db, t.Donate ?? 0, p.setting.DonationFundId, desc, tranid: t.Id);
                }
                sb.Append(e);
                if (!t.TransactionId.HasValue())
                {
                    t.TransactionId = transactionId;
                    if (testing == true && !t.TransactionId.Contains("(testing)"))
                        t.TransactionId += "(testing)";
                }
                var contributionemail = (from ex in p.person.PeopleExtras
                                         where ex.Field == "ContributionEmail"
                                         select ex.Data).SingleOrDefault();
                if (contributionemail.HasValue())
                    contributionemail = (contributionemail ?? "").Trim();
                if (!Util.ValidEmail(contributionemail))
                    contributionemail = p.person.FromEmail;


                var body = sb.ToString();
                var from = Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(p.org.OrganizationId));
                var mm = new EmailReplacements(DbUtil.Db, body, from);
                body = mm.DoReplacements(p.person);

                Util.SendMsg(Util.SysFromEmail, Util.Host, from, p.setting.Subject, body,
                    Util.EmailAddressListFromString(contributionemail), 0, p.PeopleId);
                DbUtil.Db.Email(contributionemail, DbUtil.Db.StaffPeopleForOrg(p.org.OrganizationId),
                    "online giving contribution received",
                    "see contribution records for {0} ({1})".Fmt(p.person.Name, p.PeopleId));
                if (p.CreatingAccount == true)
                    p.CreateAccount();
            }
            else if (managingsubs)
            {
                ConfirmManageSubscriptions();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (choosingslots)
            {
                ConfirmPickSlots();
                URL = null;
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (OnlinePledge())
            {
                SendLinkForPledge();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (ManageGiving())
            {
                SendLinkToManageGiving();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else
            {
                if (!t.TransactionId.HasValue())
                {
                    t.TransactionId = transactionId;
                    if (testing == true && !t.TransactionId.Contains("(testing)"))
                        t.TransactionId += "(testing)";
                }
                EnrollAndConfirm();
                if (List.Any(pp => pp.PeopleId == null))
                {
                    LogOutOfOnlineReg();
                    throw new Exception("no person");
                }
                UseCoupon(t.TransactionId, t.Amt ?? 0);
            }
            if (IsCreateAccount() || ManagingSubscriptions())
                email = List[0].person.EmailAddress;
            else
                email = List[0].EmailAddress;

            LogOutOfOnlineReg();
            return ret;
        }
        public static void LogOutOfOnlineReg()
        {
            var session = HttpContext.Current.Session;
            if ((bool?)session["OnlineRegLogin"] == true)
            {
                FormsAuthentication.SignOut();
                session.Abandon();
            }
        }
    }
    public enum ConfirmEnum
    {
        Confirm,
        ConfirmAccount,
    }
}
