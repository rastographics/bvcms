using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.Finance;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Security;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public void FinishLaterNotice()
        {
            var registerLink = EmailReplacements.CreateRegisterLink(masterorgid ?? Orgid,
                $"Resume registration for {Header}");
            var msg = "<p>Hi {first},</p>\n<p>Here is the link to continue your registration:</p>\n" + registerLink;
            Debug.Assert((masterorgid ?? Orgid) != null, "m.Orgid != null");
            var notifyids = DbUtil.Db.NotifyIds((masterorg ?? org).NotifyIds);
            var p = UserPeopleId.HasValue ? DbUtil.Db.LoadPersonById(UserPeopleId.Value) : List[0].person;
            DbUtil.Db.Email(notifyids[0].FromEmail, p, $"Continue your registration for {Header}", msg);
        }

        public string CheckDuplicateGift(decimal? amt)
        {
            if (!OnlineGiving() || !amt.HasValue)
            {
                return null;
            }

            var previousTransaction =
                (from t in DbUtil.Db.Transactions
                 where t.Amt == amt
                 where t.OrgId == Orgid
                 where t.TransactionDate > DateTime.Now.AddMinutes(-20)
                 where DbUtil.Db.Contributions.Any(cc => cc.PeopleId == List[0].PeopleId && cc.TranId == t.Id)
                 select t).FirstOrDefault();

            if (previousTransaction == null)
            {
                return null;
            }

            return @"
Thank you for your gift! Our records indicate that you recently submitted a gift in this amount a short while ago.
As a safeguard against duplicate transactions we recommend that you either wait 20 minutes,
or modify the amount of this gift by a small amount so that it does not appear as a duplicate. 
Thank you.
";
        }

        public RouteModel FinishRegistration(Transaction ti)
        {
            TranId = ti.Id;
            HistoryAdd("ProcessPayment");
            var ed = DbUtil.Db.RegistrationDatas.Single(dd => dd.Id == DatumId);
            ed.Data = Util.Serialize(this);
            ed.Completed = true;
            DbUtil.Db.SubmitChanges();

            return ConfirmTransaction(ti);
        }

        public RouteModel ConfirmTransaction(Transaction ti)
        {
            try
            {
                LogOutOfOnlineReg();
                var view = ConfirmTransaction(ti.TransactionId);
                switch (view)
                {
                    case ConfirmEnum.Confirm:
                        return RouteModel.ViewAction("Confirm", this);
                    case ConfirmEnum.ConfirmAccount:
                        return RouteModel.ViewAction("ConfirmAccount", this);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RouteModel.ErrorMessage(ex.Message);
            }
            return null;
        }

        public ConfirmEnum ConfirmTransaction(string TransactionReturn)
        {
            ParseSettings();
            if (List.Count == 0)
            {
                throw new Exception(" unexpected, no registrants found in confirmation");
            }

            CreateTransactionIfNeeded();
            SetConfirmationEmailAddress();

            if (CreatingAccount())
            {
                return CreateAccount();
            }

            if (OnlineGiving())
            {
                return DoOnlineGiving(TransactionReturn);
            }

            if (ManagingSubscriptions())
            {
                return ConfirmManageSubscriptions();
            }

            if (ChoosingSlots())
            {
                return ConfirmPickSlots();
            }

            if (OnlinePledge())
            {
                return SendLinkForPledge();
            }

            if (ManageGiving())
            {
                return SendLinkToManageGiving();
            }

            SetTransactionReturn(TransactionReturn);
            EnrollAndConfirm();
            UseCoupon(Transaction.TransactionId, Transaction.Amt ?? 0);
            return ConfirmEnum.Confirm;
        }

        private ConfirmEnum CreateAccount()
        {
            List[0].CreateAccount();
            return ConfirmEnum.ConfirmAccount;
        }

        private bool CreatingAccount()
        {
            return org != null && org.RegistrationTypeId == RegistrationTypeCode.CreateAccount;
        }

        private void SetTransactionReturn(string TransactionReturn)
        {
            if (!Transaction.TransactionId.HasValue())
            {
                Transaction.TransactionId = TransactionReturn;
                if (testing == true && !Transaction.TransactionId.Contains("(testing)"))
                {
                    Transaction.TransactionId += "(testing)";
                    Log("TestingTransaction");
                }
            }
        }

        private void SetConfirmationEmailAddress()
        {
            email = IsCreateAccount() || ManagingSubscriptions()
                ? List[0].person.EmailAddress
                : List[0].EmailAddress;
        }

        private ConfirmEnum DoOnlineGiving(string transactionReturn)
        {
            var p = List[0];
            if (p.IsNew)
            {
                p.AddPerson(null, p.org.EntryPointId ?? 0);
            }

            var desc = $"{p.person.Name}; {p.person.PrimaryAddress}; {p.person.PrimaryZip}";
            var staff = DbUtil.Db.StaffPeopleForOrg(org.OrganizationId)[0];
            var body = GivingConfirmation.PostAndBuild(DbUtil.Db, staff, p.person, p.setting.Body, p.org.OrganizationId, p.FundItemsChosen(), Transaction, desc,
                p.setting.DonationFundId);

            if (!Transaction.TransactionId.HasValue())
            {
                Transaction.TransactionId = transactionReturn;
                if (testing == true && !Transaction.TransactionId.Contains("(testing)"))
                {
                    Transaction.TransactionId += "(testing)";
                }
            }
            var contributionemail = (from ex in p.person.PeopleExtras
                                     where ex.Field == "ContributionEmail"
                                     select ex.Data).SingleOrDefault();
            if (contributionemail.HasValue())
            {
                contributionemail = (contributionemail ?? "").Trim();
            }

            if (Util.ValidEmail(contributionemail))
            {
                Log("UsingSpecialEmail");
            }
            else
            {
                contributionemail = p.person.FromEmail;
            }

            MailAddress from = null;
            if (!Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(p.org.OrganizationId), out from))
            {
                from = GetAdminMailAddress();
            }

            var m = new EmailReplacements(DbUtil.Db, body, from);
            body = m.DoReplacements(DbUtil.Db, p.person);

            DbUtil.Db.EmailFinanceInformation(from, p.person, p.setting.Subject, body);
            DbUtil.Db.EmailFinanceInformation(contributionemail, DbUtil.Db.StaffPeopleForOrg(p.org.OrganizationId),
                "online giving contribution received",
                $"see contribution records for {p.person.Name} ({p.PeopleId}) {Util.Host}");
            if (p.CreatingAccount)
            {
                p.CreateAccount();
            }

            return ConfirmEnum.Confirm;
        }

        private static MailAddress GetAdminMailAddress()
        {
            return new MailAddress(DbUtil.Db.Setting("AdminMail", ConfigurationManager.AppSettings["supportemail"]));
        }

        private void CreateTransactionIfNeeded()
        {
            if (Transaction != null || ManagingSubscriptions() || ChoosingSlots())
            {
                return;
            }

            HistoryAdd("ConfirmTransaction");
            UpdateDatum(completed: true);
            var pf = PaymentForm.CreatePaymentForm(this);
            _transaction = pf.CreateTransaction(DbUtil.Db);
            TranId = _transaction.Id;
        }

        public static void ConfirmDuePaidTransaction(Transaction ti, string transactionId, bool sendmail)
        {
            var db = DbUtil.Db;
            var org = DbUtil.Db.LoadOrganizationById(ti.OrgId);
            ti.TransactionId = transactionId;
            if (ti.Testing == true && !ti.TransactionId.Contains("(testing)"))
            {
                ti.TransactionId += "(testing)";
            }

            var amt = ti.Amt;
            var due = PaymentForm.AmountDueTrans(DbUtil.Db, ti);
            foreach (var pi in ti.OriginalTrans.TransactionPeople)
            {
                var p = DbUtil.Db.LoadPersonById(pi.PeopleId);
                if (p != null)
                {
                    var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == ti.OrgId && m.PeopleId == pi.PeopleId);
                    if (om == null)
                    {
                        continue;
                    }

                    DbUtil.Db.SubmitChanges();
                    if (org.IsMissionTrip == true)
                    {
                        DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(
                            new GoerSenderAmount
                            {
                                Amount = ti.Amt,
                                GoerId = pi.PeopleId,
                                Created = DateTime.Now,
                                OrgId = org.OrganizationId,
                                SupporterId = pi.PeopleId,
                            });
                        var setting = DbUtil.Db.CreateRegistrationSettings(org.OrganizationId);
                        var fund = setting.DonationFundId;
                        p.PostUnattendedContribution(DbUtil.Db, ti.Amt ?? 0, fund,
                            $"SupportMissionTrip: org={org.OrganizationId}; goer={pi.PeopleId}", typecode: BundleTypeCode.Online);
                    }
                    var pay = amt;
                    if (org.IsMissionTrip == true)
                    {
                        ti.Amtdue = due;
                    }

                    var sb = new StringBuilder();
                    sb.AppendFormat("{0:g} ----------\n", Util.Now);
                    sb.AppendFormat("{0:c} ({1} id) transaction amount\n", ti.Amt, ti.Id);
                    sb.AppendFormat("{0:c} applied to this registrant\n", pay);
                    sb.AppendFormat("{0:c} total due all registrants\n", due);

                    om.AddToMemberDataBelowComments(sb.ToString());
                    var reg = p.SetRecReg();
                    reg.AddToComments(sb.ToString());
                    reg.AddToComments($"{org.OrganizationName} ({org.OrganizationId})");

                    amt -= pay;
                }
                else
                {
                    DbUtil.Db.Email(DbUtil.Db.StaffEmailForOrg(org.OrganizationId),
                        DbUtil.Db.PeopleFromPidString(org.NotifyIds),
                        "missing person on payment due",
                        $"Cannot find {pi.Person.Name} ({pi.PeopleId}), payment due completed of {pi.Amt:c} but no record");
                }
            }
            DbUtil.Db.SubmitChanges();

            dynamic d = new DynamicData();
            d.Name = Transaction.FullName(ti);
            d.Amt = ti.Amt;
            d.Description = ti.Description;
            d.Amtdue = PaymentForm.AmountDueTrans(DbUtil.Db, ti);
            d.names = string.Join(", ", ti.OriginalTrans.TransactionPeople.Select(i => i.Person.Name));

            var msg = DbUtil.Db.RenderTemplate(@"
<p>
    Thank you {{Name}}, for your payment of {{Fmt Amt 'c'}} on {{Description}}.<br/>
    {{#if Amtdue}}
    Your balance is {{Fmt Amtdue 'c'}}<br/>
    {{/if}}
    {{names}}
</p>", d);
            var msgstaff = DbUtil.Db.RenderTemplate(@"
<p>
    {{Name}} paid {{Fmt Amt 'c'}} on {{Description}}.<br/>
    {{#if Amtdue}}
    The balance is {{Fmt Amtdue 'c'}}<br/>
    {{/if}}
    {{names}}
</p>", d);

            var pid = ti.FirstTransactionPeopleId();
            var p0 = DbUtil.Db.LoadPersonById(pid);
            // question: should we be sending to all TransactionPeople?
            if (sendmail)
            {
                MailAddress staffEmail;
                if (!Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(org.OrganizationId), out staffEmail))
                {
                    staffEmail = GetAdminMailAddress();
                }
                if (p0 == null)
                {
                    DbUtil.Db.SendEmail(staffEmail,
                        "Payment confirmation", msg, Util.ToMailAddressList(Util.FirstAddress(ti.Emails)), pid: pid).Wait();
                }
                else
                {
                    DbUtil.Db.Email(staffEmail, p0, Util.ToMailAddressList(ti.Emails),
                        "Payment confirmation", msg, false);
                    DbUtil.Db.Email(p0.FromEmail, DbUtil.Db.PeopleFromPidString(org.NotifyIds),
                        "payment received for " + ti.Description, msgstaff);
                }
            }
        }
        public static void LogOutOfOnlineReg()
        {
            var session = HttpContextFactory.Current.Session;
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
