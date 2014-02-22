using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
	public partial class OnlineRegController
	{
		// reached by the paylink in the confirmation email
		// which is produced in EnrollAndConfirm
		public ActionResult PayAmtDue(string q)
		{
			if (!q.HasValue())
				return Content("unknown");
			var id = Util.Decrypt(q).ToInt2();
			var qq = from t in DbUtil.Db.Transactions
					 where t.OriginalId == id || t.Id == id
					 orderby t.Id descending
					 select t;
			var ti = qq.FirstOrDefault();

			if (ti == null || ti.Amtdue == 0)
				return Content("no outstanding transaction");

#if DEBUG
			ti.Testing = true;
			if (!ti.Address.HasValue())
			{
				ti.Address = "235 Riveredge";
				ti.City = "Cordova";
				ti.Zip = "38018";
				ti.State = "TN";
			}
#endif
			var pf = PaymentForm.CreatePaymentFormForBalanceDue(ti);
			SetHeaders(pf.OrgId ?? 0);

			ViewBag.Url = pf.Url;
			ViewBag.timeout = INT_timeout;
			if (OnlineRegModel.GetTransactionGateway() != "serviceu")
				return View("ProcessPayment", pf);
			ViewBag.TranId = ti.Id;
			return View(pf);
		}
		public void ConfirmDuePaidTransaction(Transaction ti, string TransactionID, bool sendmail)
		{
			var Db = DbUtil.Db;
			var org = Db.LoadOrganizationById(ti.OrgId);
			ti.TransactionId = TransactionID;
			if (ti.Testing == true)
				ti.TransactionId += "(testing)";

			var amt = ti.Amt;
			foreach (var pi in ti.OriginalTransaction.TransactionPeople)
			{
				var p = Db.LoadPersonById(pi.PeopleId);
				if (p != null)
				{
					var om = Db.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == ti.OrgId && m.PeopleId == pi.PeopleId);
					if (om == null)
						continue;
				    var pay = amt;
				    if (org.IsMissionTrip == true)
				    {
				        Db.ExecuteCommand(@"
INSERT dbo.GoerSenderAmounts ( OrgId , SupporterId , GoerId , Amount , Created ) VALUES  ( {0}, {1}, {1}, {2}, {3} )", 
                             org.OrganizationId, p.PeopleId, pay, DateTime.Now );
				    }
				    else
				    {
    				    var due = om.Amount - om.TotalPaid(Db);
				        if (pay > due)
    						pay = due;
    					om.AmountPaid += pay;
				    }

				    var sb = new StringBuilder();
					sb.AppendFormat("{0:g} ----------\n", Util.Now);
					sb.AppendFormat("{0:c} ({1} id) transaction amount\n", ti.Amt, ti.Id);
					sb.AppendFormat("{0:c} applied to this registrant\n", pay);
					sb.AppendFormat("{0:c} total due all registrants\n", ti.Amtdue);

                    om.AddToMemberData(sb.ToString());
					var reg = p.RecRegs.Single();
				    reg.AddToComments(sb.ToString());
					reg.AddToComments("{0} ({1})".Fmt(org.OrganizationName, org.OrganizationId));

					amt -= pay;
				}
				else
					Db.Email(Db.StaffEmailForOrg(org.OrganizationId),
						Db.PeopleFromPidString(org.NotifyIds),
						"missing person on payment due",
						"Cannot find {0} ({1}), payment due completed of {2:c} but no record".Fmt(pi.Person.Name, pi.PeopleId, pi.Amt));
			}
			Db.SubmitChanges();
			var names = string.Join(", ", ti.OriginalTransaction.TransactionPeople.Select(i => i.Person.Name).ToArray());

			var pid = ti.FirstTransactionPeopleId();
			var p0 = Db.LoadPersonById(pid);
			if (sendmail)
			{
				if (p0 == null)
					Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(Db.StaffEmailForOrg(org.OrganizationId)),
						"Payment confirmation", "Thank you for paying {0:c} for {1}.<br/>Your balance is {2:c}<br/>{3}".Fmt(ti.Amt, ti.Description, ti.Amtdue, names), Util.ToMailAddressList(Util.FirstAddress(ti.Emails)), 0, pid);
				else
				{
					Db.Email(Db.StaffEmailForOrg(org.OrganizationId),
						p0, Util.ToMailAddressList(ti.Emails), "Payment confirmation", "Thank you for paying {0:c} for {1}.<br/>Your balance is {2:c}<br/>{3}".Fmt(ti.Amt, ti.Description, ti.Amtdue, names), false);
					Db.Email(p0.FromEmail,
						Db.PeopleFromPidString(org.NotifyIds),
						"payment received for " + ti.Description,
						"{0} paid {1:c} for {2}, balance of {3:c}\n({4})".Fmt(ti.FullName, ti.Amt, ti.Description, ti.Amtdue, names));
				}
			}
		}
		public ActionResult ConfirmDuePaid(int? id, string TransactionID, decimal Amount)
		{
			if (!id.HasValue)
				return View("Unknown");
			if (!TransactionID.HasValue())
				return Content("error no transaction");

			var ti = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == id);
			if (ti == null)
				return Content("no pending transaction");
#if DEBUG
			ti.Testing = true;
#endif
            if (OnlineRegModel.GetTransactionGateway() == "serviceu")
    			ti = PaymentForm.CreateTransaction(DbUtil.Db, ti, Amount);
			ConfirmDuePaidTransaction(ti, TransactionID, sendmail: true);
			SetHeaders(ti.OrgId ?? 0);
			ViewData["timeout"] = INT_timeout;
			ViewData["Url"] = ti.Url;
			return View(ti);
		}
		public ActionResult PayDueTest(string q)
		{
			if (!q.HasValue())
				return Content("unknown");
			var id = Util.Decrypt(q);
			var ed = DbUtil.Db.ExtraDatas.SingleOrDefault(e => e.Id == id.ToInt());
			if (ed == null)
				return Content("no outstanding transaction");
			return Content(ed.Data);
		}
	}
}
