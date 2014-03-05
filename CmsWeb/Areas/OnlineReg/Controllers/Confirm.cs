using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using System.Configuration;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CmsWeb.Areas.Manage.Controllers;
using CmsData.Codes;
using CmsWeb.Code;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
	public partial class OnlineRegController
	{
	    private string confirm;

	    public ActionResult ProcessPayment(PaymentForm pf)
		{
#if DEBUG
#else
			if (Session["FormId"] != null)
				if ((Guid)Session["FormId"] == pf.FormId)
					return Content("Already submitted");                    
#endif
			OnlineRegModel m = null;
			var ed = DbUtil.Db.ExtraDatas.SingleOrDefault(e => e.Id == pf.DatumId);
			if (ed != null)
				m = Util.DeSerialize<OnlineRegModel>(ed.Data);

#if DEBUG
#else
            if(m != null && m.History.Contains("ProcessPayment"))
					return Content("Already submitted");                    
#endif

			if (pf.AmtToPay < 0) pf.AmtToPay = 0;
			if (pf.Donate < 0) pf.Donate = 0;

			pf.AllowCoupon = false;

			SetHeaders(pf.OrgId ?? 0);
			ViewBag.Url = pf.Url;
			ViewBag.timeout = INT_timeout;

			if ((pf.AmtToPay ?? 0) <= 0 && (pf.Donate ?? 0) <= 0)
			{
				DbUtil.Db.SubmitChanges();
				ModelState.AddModelError("form", "amount zero");
				return View("ProcessPayment", pf);
			}

			try
			{
				if (pf.Type == "B")
					Payments.ValidateBankAccountInfo(ModelState, pf.Routing, pf.Account);
				if (pf.Type == "C")
					Payments.ValidateCreditCardInfo(ModelState, pf.CreditCard, pf.Expires, pf.CCV);
				
				if (!ModelState.IsValid)
					return View("ProcessPayment", pf);

				if (pf.IsLoggedIn == true && pf.SavePayInfo == true)
				{
					var gateway = OnlineRegModel.GetTransactionGateway();
					if (gateway == "authorizenet")
					{
						var au = new AuthorizeNet(DbUtil.Db, m.testing ?? false);
						if ((pf.Type == "B" && !pf.Routing.StartsWith("X") && !pf.Account.StartsWith("X"))
							|| (pf.Type == "C" && !pf.CreditCard.StartsWith("X")))
							au.AddUpdateCustomerProfile(m.UserPeopleId.Value,
								pf.Type,
								pf.CreditCard,
								DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"),
								pf.MaskedCCV != null && pf.MaskedCCV.StartsWith("X") ? pf.CCV : pf.MaskedCCV,
								pf.Routing,
								pf.Account);
					}
					else if (gateway == "sage")
					{
						var sg = new CmsData.SagePayments(DbUtil.Db, m.testing ?? false);
						if ((pf.Type == "B" && !pf.Routing.StartsWith("X") && !pf.Account.StartsWith("X"))
							|| (pf.Type == "C" && !pf.CreditCard.StartsWith("X")))
							sg.storeVault(m.UserPeopleId.Value,
										  pf.Type,
										  pf.CreditCard,
            							  DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"),
										  pf.MaskedCCV != null && pf.MaskedCCV.StartsWith("X") ? pf.CCV : pf.MaskedCCV,
										  pf.Routing,
										  pf.Account,
										  pf.IsGiving == true);
					}
					else
						throw new Exception("ServiceU not supported");

				}
				var ti = ProcessPaymentTransaction(m, pf);

				if (ti.Approved == false)
				{
					ModelState.AddModelError("form", ti.Message);
					return View("ProcessPayment", pf);
				}
    			if (m != null) 
    			{
    			    m.TranId = ti.Id;
                    m.History.Add("ProcessPayment");
    			    ed.Data = Util.Serialize<OnlineRegModel>(m);
    			    ed.Completed = true;
                    DbUtil.Db.SubmitChanges();
    			}
				Session["FormId"] = pf.FormId;
				if (pf.DatumId > 0)
				{
				    confirm = ConfirmTransaction(m, ti.TransactionId);
				    if (confirm.StartsWith("error:"))
				    {
				        TempData["error"] = confirm.Substring(6);
				        return Redirect("/Error");
				    }
				    return View(confirm);
				}

			    ConfirmDuePaidTransaction(ti, ti.TransactionId, sendmail: true);
				return View("ConfirmDuePaid", ti);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("form", ex.Message);
				return View("ProcessPayment", pf);
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
			var gateway = OnlineRegModel.GetTransactionGateway();
			if (gateway == "authorizenet")
				if (pf.SavePayInfo == true)
				{
					var anet = new AuthorizeNet(DbUtil.Db, pf.testing);
					tinfo = anet.createCustomerProfileTransactionRequest(
						pid ?? 0,
						pf.AmtToPay ?? 0,
						pf.Description,
						pf.TranId ?? 0);
				}
				else
					if (pf.Type == "B")
						tinfo = OnlineRegModel.PostECheck(
							pf.Routing, pf.Account,
							pf.AmtToPay ?? 0,
							ti.Id, pf.Description,
							pid ?? 0, pf.First, pf.Last,
							pf.Address, pf.City, pf.State, pf.Zip,
							pf.testing);
					else
						tinfo = OnlineRegModel.PostTransaction(
							pf.CreditCard, pf.CCV,
							DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"),
							pf.AmtToPay ?? 0,
							ti.Id, pf.Description,
							pid ?? 0, pf.Email, pf.First, pf.Last,
							pf.Address, pf.City, pf.State, pf.Zip,
							pf.testing);
			else if (gateway == "sage")
				if (pf.SavePayInfo == true)
				{
					var sage = new SagePayments(DbUtil.Db, pf.testing);
					tinfo = sage.createVaultTransactionRequest(
						pid ?? 0,
						pf.AmtToPay ?? 0,
						pf.Description,
						ti.Id,
						pf.Type);
				}
				else
					if (pf.Type == "B")
						tinfo = OnlineRegModel.PostVirtualCheckTransactionSage(
							pf.Routing, pf.Account,
							pf.AmtToPay ?? 0,
							ti.Id, pf.Description,
							pid ?? 0, pf.Email, pf.First, pf.MiddleInitial, pf.Last, pf.Suffix,
							pf.Address, pf.City, pf.State, pf.Zip, pf.Phone,
							pf.testing);
					else
						tinfo = OnlineRegModel.PostTransactionSage(
							pf.CreditCard, pf.CCV,
							DbUtil.NormalizeExpires(pf.Expires).ToString2("MMyy"),
							pf.AmtToPay ?? 0,
							ti.Id, pf.Description,
							pid ?? 0, pf.Email, pf.First, pf.MiddleInitial, pf.Last, pf.Suffix,
							pf.Address, pf.City, pf.State, pf.Zip, pf.Phone,
							pf.testing);

			else
				throw new Exception("unknown gateway " + gateway);

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

		private string ConfirmTransaction(OnlineRegModel m, string TransactionID, ExtraDatum ed = null)
		{
			m.ParseSettings();
		    if (m.List.Count == 0)
		        return "error: unexpected, no registrants found in confirmation";
			string confirm = "Confirm";
			var managingsubs = m.ManagingSubscriptions();
			var choosingslots = m.ChoosingSlots();
			var t = m.Transaction;
			if (t == null && !managingsubs && !choosingslots)
			{
				var pf = PaymentForm.CreatePaymentForm(m);
			    if (ed != null)
			        pf.DatumId = ed.Id;
			    t = pf.CreateTransaction(DbUtil.Db);
				m.TranId = t.Id;
			    if (ed != null)
			    {
                    m.History.Add("ConfirmTransaction");
			        ed.Completed = true;
			        ed.Data = Util.Serialize<OnlineRegModel>(m);
			        DbUtil.Db.SubmitChanges();
			    }
			}
            if (t != null)
    			ViewData["message"] = t.Message;

			if (m.org != null && m.org.RegistrationTypeId == RegistrationTypeCode.CreateAccount)
			{
				m.List[0].CreateAccount();
				ViewData["CreatedAccount"] = true;
				confirm = "ConfirmAccount";
			}
			else if (m.OnlineGiving())
			{
				var p = m.List[0];
				if (p.IsNew)
					p.AddPerson(null, p.org.EntryPointId ?? 0);

				var staff = DbUtil.Db.StaffPeopleForOrg(p.org.OrganizationId)[0];
				var text = p.setting.Body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase:true);
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
						p.person.PostUnattendedContribution(DbUtil.Db,
															g.amt,
															g.fundid,
															desc);
					}
				}
				t.Financeonly = true;
				if (t.Donate > 0)
				{
					var fundname = DbUtil.Db.ContributionFunds.Single(ff => ff.FundId == p.setting.DonationFundId).FundName;
					sb.AppendFormat(row, fundname, t.Donate);
					t.Fund = p.setting.DonationFund();
					p.person.PostUnattendedContribution(DbUtil.Db,
						t.Donate.Value,
						p.setting.DonationFundId,
						desc);
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
					contributionemail = contributionemail.Trim();
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
				m.URL = null;
				ViewData["ManagingSubscriptions"] = true;
				ViewData["CreatedAccount"] = m.List[0].CreatingAccount;
				confirm = "ConfirmAccount";
			}
			else if (choosingslots)
			{
				m.ConfirmPickSlots();
				m.URL = null;
				ViewData["ManagingVolunteer"] = true;
				ViewData["CreatedAccount"] = m.List[0].CreatingAccount;
				confirm = "ConfirmAccount";
			}
			else if (m.OnlinePledge())
			{
				m.ConfirmManagePledge();
				ViewData["ManagingPledge"] = true;
				ViewData["CreatedAccount"] = m.List[0].CreatingAccount;
				confirm = "ConfirmAccount";
			}
			else if (m.ManageGiving())
			{
				m.ConfirmManageGiving();
				ViewData["ManagingGiving"] = true;
				ViewData["CreatedAccount"] = m.List[0].CreatingAccount;
				confirm = "ConfirmAccount";
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
				    return "error: no person";
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
					return "error: no person";
				}
				m.UseCoupon(t.TransactionId, t.Amt ?? 0);
			}
			if (m.IsCreateAccount() || m.ManagingSubscriptions())
				ViewData["email"] = m.List[0].person.EmailAddress;
			else
				ViewData["email"] = m.List[0].email;

            if(m.masterorgid.HasValue && m.orgid.HasValue && !m.settings[m.orgid.Value].Subject.HasValue())
    		    ViewData["orgname"] = m.masterorg.OrganizationName;
            else
    		    ViewData["orgname"] = m.org != null ? m.org.OrganizationName : m.masterorg.OrganizationName;

		    LogOutOfOnlineReg();
			return confirm;
		}

	    private void LogOutOfOnlineReg()
	    {
	        if ((bool?) Session["OnlineRegLogin"] == true)
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

			var ed = DbUtil.Db.ExtraDatas.SingleOrDefault(e => e.Id == id);
			if (ed == null || ed.Completed == true)
				return Content("no pending confirmation found");

			var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
			var confirm = ConfirmTransaction(m, transactionId, ed);
            if (confirm.StartsWith("error"))
            {
                TempData["error"] = confirm.Substring(5);
                return Redirect("/Error");
            }
            ViewBag.Url = m.URL;

            ed.Completed = true;
			DbUtil.Db.SubmitChanges();

			SetHeaders(m);
			return View(confirm, m);
		}
	}
}
