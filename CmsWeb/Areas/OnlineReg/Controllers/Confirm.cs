using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using CmsData;
using CmsData.Finance;
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
        //private string confirm;

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
                return Json(new {confirm = "/OnlineReg/FinishLater/" + id});
            }
            return Json(new {confirm = "/OnlineReg/Unknown"});
        }
        [HttpGet]
        public ActionResult FinishLater(int id)
        {
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == id);
            if (ed != null)
            {
                var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                return View(m);
            }
            return View("Unknown");
        }
        public ActionResult ProcessPayment(PaymentForm pf)
        {
#if DEBUG
#else
			if (Session["FormId"] != null)
				if ((Guid)Session["FormId"] == pf.FormId)
					return Content("Already submitted");                    
#endif
            OnlineRegModel m = null;
            var ed = DbUtil.Db.RegistrationDatas.SingleOrDefault(e => e.Id == pf.DatumId);
            if (ed != null)
                m = Util.DeSerialize<OnlineRegModel>(ed.Data);

#if DEBUG
#else
            if(m != null && m.History.Contains("ProcessPayment") && !pf.PayBalance)
					return Content("Already submitted");                    
#endif

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
                if (pf.Type == "B")
                    Payments.ValidateBankAccountInfo(ModelState, pf.Routing, pf.Account);
                if (pf.Type == "C")
                    Payments.ValidateCreditCardInfo(ModelState, pf);

                if (!ModelState.IsValid)
                    return View("Payment/Process", pf);

                if (m != null && pf.IsLoggedIn == true && pf.SavePayInfo)
                {
                    var gw = DbUtil.Db.Gateway(m.testing ?? false);
                    gw.StoreInVault(m.UserPeopleId ?? 0,
                            pf.Type,
                            pf.CreditCard,
                            DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"),
                            pf.MaskedCCV != null && pf.MaskedCCV.StartsWith("X") ? pf.CCV : pf.MaskedCCV,
                            pf.Routing,
                                      pf.Account,
                                      pf.IsGiving == true);

                }
                if (pf.UseBootstrap && pf.AddressChecked < 2)
                {
                    var r = AddressVerify.LookupAddress(pf.Address, "", "", "", pf.Zip);
                    var z = DbUtil.Db.ZipCodes.SingleOrDefault(zc => zc.Zip == pf.Zip.Zip5());
                    if (z != null && !z.State.HasValue())
                    {
                        pf.State = r.State = z.State;
                        pf.City = r.City = z.City;
                    }
                    if (r.Line1 != "error" && r.Line1.HasValue())
                    {
                        if (r.found == false)
                        {
                            ModelState.Clear(); // so the form will bind to the object again
                            pf.AddressChecked++;
                            ModelState.AddModelError("Zip",
                                "Address not found: " + r.address);
                            return View("Payment/Process", pf);
                        }
                        if (r.Line1 != pf.Address)
                            pf.Address = r.Line1;
                        if (r.City != (pf.City ?? ""))
                            pf.City = r.City;
                        if (r.State != (pf.State ?? ""))
                            pf.State = r.State;
                        if (r.Zip != (pf.Zip ?? ""))
                            pf.Zip = r.Zip;
                    }
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
                    ed.Data = Util.Serialize<OnlineRegModel>(m);
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
                ModelState.AddModelError("form", ex.Message);
                return View("Payment/Process", pf);
            }
        }
        private Transaction ProcessPaymentTransaction(OnlineRegModel m, PaymentForm pf)
        {
            Transaction ti = null;
            if (m != null && m.Transaction != null)
                ti = PaymentForm.CreateTransaction(DbUtil.Db, m.Transaction, pf.AmtToPay);
            else
                ti = pf.CreateTransaction(DbUtil.Db);

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
                    if (pf.Type == "B")
                    tinfo = gw.PayWithCheck(pid ?? 0, pf.AmtToPay ?? 0, pf.Routing, pf.Account, pf.Description, ti.Id,
                        pf.Email, pf.First, pf.MiddleInitial, pf.Last, pf.Suffix, pf.Address, pf.City, pf.State, pf.Zip, pf.Phone);
                    else
                    tinfo = gw.PayWithCreditCard(pid ?? 0, pf.AmtToPay ?? 0, pf.CreditCard, 
                            DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"),
                        pf.Description, ti.Id,
                        pf.CCV, pf.Email, pf.First, pf.Last, pf.Address, pf.City, pf.State, pf.Zip, pf.Phone);

            ti.TransactionId = tinfo.TransactionId;
            if (ti.Testing == true && !ti.TransactionId.Contains("(testing)"))
                ti.TransactionId += "(testing)";
            ti.Approved = tinfo.Approved;
            if (ti.Approved == false)
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

        public enum ConfirmEnum
        {
            Confirm,
            ConfirmAccount,
        }
        private ConfirmEnum ConfirmTransaction(OnlineRegModel m, string TransactionID)
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
                text = text.Replace("{name}", p.person.Name);
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
                    t.TransactionId = TransactionID;
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

                Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(p.org.OrganizationId)),
                    p.setting.Subject, sb.ToString(),
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
                t.TransactionId = TransactionID;
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
                    t.TransactionId = TransactionID;
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
