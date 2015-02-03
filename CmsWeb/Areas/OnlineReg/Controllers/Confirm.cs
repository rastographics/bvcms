using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;
using CmsData;
using CmsData.Finance;
using CmsData.View;
using CmsWeb.Models;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;
using CmsData.Codes;
using CmsWeb.Code;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        [HttpPost]
        public ActionResult SaveProgressPayment(int id)
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed != null)
            {
                var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                m.HistoryAdd("saveprogress");
                if (m.UserPeopleId == null)
                    m.UserPeopleId = Util.UserPeopleId;
                m.UpdateDatum();
                return Json(new { confirm = "/OnlineReg/FinishLater/" + id });
            }
            return Json(new { confirm = "/OnlineReg/Unknown" });
        }

        [HttpGet]
        public ActionResult FinishLater(int id)
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed != null)
            {
                var m = Util.DeSerialize<OnlineRegModel>(ed.Data);

                var registerLink = EmailReplacements.CreateRegisterLink(m.masterorgid ?? m.Orgid, "Resume registration for {0}".Fmt(m.Header));
                var msg = "<p>Hi {first},</p>\n<p>Here is the link to continue your registration:</p>\n" + registerLink;
                Debug.Assert((m.masterorgid ?? m.Orgid) != null, "m.Orgid != null");
                var notifyids = DbUtil.Db.NotifyIds((m.masterorgid ?? m.Orgid).Value, (m.masterorg ?? m.org).NotifyIds);
                var p = m.UserPeopleId.HasValue ? DbUtil.Db.LoadPersonById(m.UserPeopleId.Value) : m.List[0].person;
                DbUtil.Db.Email(notifyids[0].FromEmail, p, "Continue your registration for {0}".Fmt(m.Header), msg);

                /* We use Content as an ActionResult instead of Message because we want plain text sent back
                 * This is an HttpPost ajax call and will have a SiteLayout wrapping this.
                 */
                //return Content("We have saved your progress. An email with a link to finish this registration will come to you shortly.");

                return View(m);
            }
            return View("Unknown");
        }

        [HttpPost]
        public ActionResult ProcessPayment(PaymentForm pf)
        {
            Response.NoCache();

#if DEBUG
#else
			if (Session["FormId"] != null)
				if ((Guid)Session["FormId"] == pf.FormId)
					return Message("Already submitted");
#endif

            OnlineRegModel m = null;
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == pf.DatumId);
            if (ed != null)
                m = Util.DeSerialize<OnlineRegModel>(ed.Data);

            var peopleId = 0;
            if (m != null)
                peopleId = m.UserPeopleId ?? 0;

#if DEBUG
#else
            if (m != null && m.History.Contains("ProcessPayment"))
					return Content("Already submitted");
#endif

            if (m != null && m.OnlineGiving())
            {
                var previousTransaction =
                    (from t in DbUtil.Db.Transactions
                     where t.Amt == pf.AmtToPay
                     where t.OrgId == m.Orgid
                     where t.TransactionDate > DateTime.Now.AddMinutes(-60)
                     where DbUtil.Db.Contributions.Any(cc => cc.PeopleId == m.List[0].PeopleId && cc.TranId == t.Id)
                     select t).FirstOrDefault();
                if (previousTransaction != null)
                    return Message("You have already submitted a gift in this amount a short while ago. Please let us know if you saw an error and what the message said.");
            }

            if (pf.AmtToPay < 0) pf.AmtToPay = 0;
            if (pf.Donate < 0) pf.Donate = 0;

            pf.AllowCoupon = false;

            SetHeaders(pf.OrgId ?? 0);

            if ((pf.AmtToPay ?? 0) <= 0 && (pf.Donate ?? 0) <= 0)
            {
                DbUtil.Db.SubmitChanges();
                ModelState.AddModelError("form", "amount zero");
                return View("Payment/Process", pf);
            }

            try
            {
                ValidatePaymentForm(pf);

                if (!ModelState.IsValid)
                    return View("Payment/Process", pf);

                if (m != null && pf.IsLoggedIn.GetValueOrDefault() && pf.SavePayInfo)
                {
                    var gateway = DbUtil.Db.Gateway(m.testing ?? false);
                    // we need to perform a $1 auth if this is a brand new credit card that we are going to store it in the vault.
                    // otherwise we skip doing an auth just call store in vault just like normal.
                    if (!VerifyCardWithAuth(gateway, pf, peopleId))
                        return View("Payment/Process", pf);

                    InitializePaymentInfo(peopleId, pf);
                    StoreInVault(gateway, pf, peopleId);
                }

                var ti = ProcessPaymentTransaction(m, pf);

                if (ti.Approved == false)
                {
                    ModelState.AddModelError("form", ti.Message);
                    return View("Payment/Process", pf);
                }

                if (m != null)
                {
                    m.TranId = ti.Id;
                    m.HistoryAdd("ProcessPayment");
                    ed.Data = Util.Serialize(m);
                    ed.Completed = true;
                    DbUtil.Db.SubmitChanges();
                }
                Session["FormId"] = pf.FormId;

                if (pf.DatumId > 0)
                {
                    try
                    {
                        var view = ConfirmTransaction(m, ti.TransactionId);
                        switch (view)
                        {
                            case ConfirmEnum.Confirm:
                                return View("Confirm", m);
                            case ConfirmEnum.ConfirmAccount:
                                return View("ConfirmAccount");
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        TempData["error"] = ex.Message;
                        return Redirect("/Error");
                    }
                }

                ConfirmDuePaidTransaction(ti, ti.TransactionId, sendmail: true);

                ViewBag.amtdue = PaymentForm.AmountDueTrans(DbUtil.Db, ti).ToString("C");
                return View("PayAmtDue/Confirm", ti);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("form", ex.Message);
                return View("Payment/Process", pf);
            }
        }

        private void ValidatePaymentForm(PaymentForm pf)
        {
            switch (pf.Type)
            {
                case PaymentType.Ach:
                    PaymentValidator.ValidateBankAccountInfo(ModelState, pf.Routing, pf.Account);
                    break;
                case PaymentType.CreditCard:
                    PaymentValidator.ValidateCreditCardInfo(ModelState, pf);
                    break;
                default:
                    ModelState.AddModelError("Type", "Please select Bank Account or Credit Card.");
                    break;
            }

            PaymentValidator.ValidateBillingDetails(ModelState, pf);
        }

        private static void StoreInVault(IGateway gateway, PaymentForm pf, int peopleId)
        {
            gateway.StoreInVault(peopleId, pf.Type, pf.CreditCard,
                DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"), pf.CVV, pf.Routing, pf.Account,
                pf.IsGiving.GetValueOrDefault());
        }

        /// <summary>
        /// Perform a $1 authorization... the amount is randomized because some gateways will reject identical, subsequent amounts
        /// within a short period of time.
        /// </summary>
        private bool VerifyCardWithAuth(IGateway gateway, PaymentForm pf, int peopleId)
        {
            if (pf.Type != PaymentType.CreditCard) return true;

            if (pf.CreditCard.StartsWith("X")) return true;

            var random = new Random();
            var dollarAmt = (decimal) random.Next(100, 199) / 100;

            var transactionResponse = gateway.AuthCreditCard(peopleId, dollarAmt, pf.CreditCard,
                DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"), "One Time Auth", 0, pf.CVV, string.Empty,
                pf.First, pf.Last, pf.Address, pf.Address2, pf.City, pf.State, pf.Country, pf.Zip, pf.Phone);

            if (!transactionResponse.Approved)
            {
                ModelState.AddModelError("form", transactionResponse.Message);
                return false;
            }

            // if we got this far that means the auth worked so now let's do a void for that auth.
            var voidResponse = gateway.VoidCreditCardTransaction(transactionResponse.TransactionId);
            return true;
        }

        private static void InitializePaymentInfo(int peopleId, PaymentForm pf)
        {
            var person = DbUtil.Db.LoadPersonById(peopleId);
            var pi = person.PaymentInfo();
            if (pi == null)
            {
                pi = new PaymentInfo();
                person.PaymentInfos.Add(pi);
            }
            pi.SetBillingAddress(pf.First, pf.MiddleInitial, pf.Last, pf.Suffix, pf.Address, pf.Address2, pf.City,
                pf.State, pf.Country, pf.Zip, pf.Phone);
        }

        private Transaction ProcessPaymentTransaction(OnlineRegModel m, PaymentForm pf)
        {
            var ti = (m != null && m.Transaction != null)
                ? PaymentForm.CreateTransaction(DbUtil.Db, m.Transaction, pf.AmtToPay)
                : pf.CreateTransaction(DbUtil.Db);

            int? pid = null;
            if (m != null)
            {
            m.ParseSettings();
            var terms = Util.PickFirst(m.Terms, "");
            if (terms.HasValue())
                ViewData["Terms"] = terms;

                pid = m.UserPeopleId;
            if (m.TranId == null)
                m.TranId = ti.Id;
            }

            if (!pid.HasValue)
            {
                var pds = DbUtil.Db.FindPerson(pf.First, pf.Last, null, pf.Email, pf.Phone);
                if (pds.Count() == 1)
                    pid = pds.Single().PeopleId.Value;
            }

            TransactionResponse tinfo;
            var gw = DbUtil.Db.Gateway(pf.testing);

            if (pf.SavePayInfo)
                tinfo = gw.PayWithVault(pid ?? 0, pf.AmtToPay ?? 0, pf.Description, ti.Id, pf.Type);
            else
            {
                tinfo = pf.Type == PaymentType.Ach
                    ? PayWithCheck(gw, pf, pid, ti)
                    : PayWithCreditCard(gw, pf, pid, ti);
            }

            ti.TransactionId = tinfo.TransactionId;

            if (ti.Testing.GetValueOrDefault() && !ti.TransactionId.Contains("(testing)"))
                ti.TransactionId += "(testing)";

            ti.Approved = tinfo.Approved;

            if (!ti.Approved.GetValueOrDefault())
            {
                ti.Amtdue += ti.Amt;
                if (m != null && m.OnlineGiving())
                    ti.Amtdue = 0;
            }

            ti.Message = tinfo.Message;
            ti.AuthCode = tinfo.AuthCode;
            ti.TransactionDate = DateTime.Now;

            DbUtil.Db.SubmitChanges();
            return ti;
        }

        private static TransactionResponse PayWithCreditCard(IGateway gateway, PaymentForm paymentForm, int? peopleId, Transaction transaction)
        {
            return gateway.PayWithCreditCard(peopleId ?? 0, paymentForm.AmtToPay ?? 0, paymentForm.CreditCard,
                DbUtil.NormalizeExpires(paymentForm.Expires).ToString2("MMyy"), paymentForm.Description, transaction.Id,
                paymentForm.CVV, paymentForm.Email, paymentForm.First, paymentForm.Last, paymentForm.Address, paymentForm.Address2,
                paymentForm.City, paymentForm.State, paymentForm.Country, paymentForm.Zip, paymentForm.Phone);
        }

        private static TransactionResponse PayWithCheck(IGateway gw, PaymentForm pf, int? pid, Transaction ti)
        {
            return gw.PayWithCheck(pid ?? 0, pf.AmtToPay ?? 0, pf.Routing, pf.Account, pf.Description, ti.Id, pf.Email,
                pf.First, pf.MiddleInitial, pf.Last, pf.Suffix, pf.Address, pf.Address2, pf.City, pf.State, pf.Country,
                pf.Zip, pf.Phone);
        }

        public enum ConfirmEnum
        {
            Confirm,
            ConfirmAccount,
        }

        private ConfirmEnum ConfirmTransaction(OnlineRegModel m, string transactionId)
        {
            m.ParseSettings();
            if (m.List.Count == 0)
                throw new Exception(" unexpected, no registrants found in confirmation");
            var ret = ConfirmEnum.Confirm;
            var managingsubs = m.ManagingSubscriptions();
            var choosingslots = m.ChoosingSlots();
            var t = m.Transaction;
            if (t == null && !managingsubs && !choosingslots)
            {
                m.HistoryAdd("ConfirmTransaction");
                m.UpdateDatum(completed: true);
                var pf = PaymentForm.CreatePaymentForm(m);
                t = pf.CreateTransaction(DbUtil.Db);
                m.TranId = t.Id;
            }
            if (t != null)
                ViewBag.message = t.Message;

            if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
            {
                m.List[0].CreateAccount();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (m.OnlineGiving())
            {
                var p = m.List[0];
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
                    OrgId = m.Orgid,
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
                    if (m.testing == true && !t.TransactionId.Contains("(testing)"))
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
                var mm = new EmailReplacements(DbUtil.Db, body, from );
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
                m.ConfirmManageSubscriptions();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (choosingslots)
            {
                m.ConfirmPickSlots();
                m.URL = null;
                ViewBag.ManagingVolunteer = true;
                ViewBag.CreatedAccount = m.List[0].CreatingAccount;
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (m.OnlinePledge())
            {
                m.SendLinkForPledge();
                ViewBag.CreatedAccount = m.List[0].CreatingAccount;
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (m.ManageGiving())
            {
                m.SendLinkToManageGiving();
                ret = ConfirmEnum.ConfirmAccount;
            }
            else if (t.TransactionGateway.ToLower() == "serviceu")
            {
                t.TransactionId = transactionId;
                if (m.testing == true && !t.TransactionId.Contains("(testing)"))
                    t.TransactionId += "(testing)";
                t.Message = "Transaction Completed";
                t.Approved = true;
                m.EnrollAndConfirm();
                if (m.List.Any(pp => pp.PeopleId == null))
                {
                    LogOutOfOnlineReg();
                    throw new Exception("no person");
                }
                m.UseCoupon(t.TransactionId, t.Amt ?? 0);
            }
            else
            {
                if (!t.TransactionId.HasValue())
                {
                    t.TransactionId = transactionId;
                    if (m.testing == true && !t.TransactionId.Contains("(testing)"))
                        t.TransactionId += "(testing)";
                }
                m.EnrollAndConfirm();
                if (m.List.Any(pp => pp.PeopleId == null))
                {
                    LogOutOfOnlineReg();
                    throw new Exception("no person");
                }
                m.UseCoupon(t.TransactionId, t.Amt ?? 0);
            }
            if (m.IsCreateAccount() || m.ManagingSubscriptions())
                m.email = m.List[0].person.EmailAddress;
            else
                m.email = m.List[0].EmailAddress;
            ViewBag.email = m.email;

            if (m.masterorgid.HasValue && m.Orgid.HasValue && !m.settings[m.Orgid.Value].Subject.HasValue())
                ViewBag.orgname = m.masterorg.OrganizationName;
            else
                ViewBag.orgname = m.org != null ? m.org.OrganizationName : m.masterorg.OrganizationName;

            LogOutOfOnlineReg();
            return ret;
        }

        private void LogOutOfOnlineReg()
        {
            if ((bool?)Session["OnlineRegLogin"] == true)
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
            }
        }

        public ActionResult Confirm(int? id, string transactionId, decimal? amount)
        {
            if (!id.HasValue)
                return View("Unknown");
            if (!transactionId.HasValue())
                return Content("error no transaction");

            var m = OnlineRegModel.GetRegistrationFromDatum(id ?? 0);
            if (m == null || m.Completed)
                return Content("no pending confirmation found");

            if (m.List.Count == 0)
                return Content("no registrants found");
            try
            {
                var view = ConfirmTransaction(m, transactionId);
                m.UpdateDatum(completed: true);
                SetHeaders(m);
                if (view == ConfirmEnum.ConfirmAccount)
                    return View("ConfirmAccount", m);
                return View("Confirm", m);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["error"] = ex.Message;
                return Redirect("/Error");
            }
        }
    }
}
