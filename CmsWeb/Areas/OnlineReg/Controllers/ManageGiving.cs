using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Models;
using UtilityExtensions;
using System.Collections.Generic;
using CmsData.Codes;
using CmsWeb.Models.OrganizationPage;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
	public partial class OnlineRegController
	{
		public ActionResult ManagePledge(string id)
		{
			if (!id.HasValue())
				return Content("bad link");
			ManagePledgesModel m = null;
			var td = TempData["mp"];
			if (td != null)
				m = new ManagePledgesModel(td.ToInt(), id.ToInt());
			else
			{
				var guid = id.ToGuid();
				if (guid == null)
					return Content("invalid link");
				var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
				if (ot == null)
					return Content("invalid link");
				if (ot.Used)
					return Content("link used");
				if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
					return Content("link expired");
				var a = ot.Querystring.Split(',');
				m = new ManagePledgesModel(a[1].ToInt(), a[0].ToInt());
				ot.Used = true;
				DbUtil.Db.SubmitChanges();
			}
			SetHeaders(m.orgid);
			DbUtil.LogActivity("Manage Pledge: {0} ({1})".Fmt(m.Organization.OrganizationName, m.person.Name));
			return View(m);
		}

		[HttpGet]
		public ActionResult ManageGiving(string id, bool? testing)
		{
			if (!id.HasValue())
				return Content("bad link");
			ManageGivingModel m = null;
			var td = TempData["mg"];
			if (td != null)
				m = new ManageGivingModel(td.ToInt(), id.ToInt());
			else
			{
				var guid = id.ToGuid();
				if (guid == null)
					return Content("invalid link");
				var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
				if (ot == null)
					return Content("invalid link");
#if DEBUG2
#else
				if (ot.Used)
					return Content("link used");
#endif
				if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
					return Content("link expired");
				var a = ot.Querystring.Split(',');
				m = new ManageGivingModel(a[1].ToInt(), a[0].ToInt());
				ot.Used = true;
				DbUtil.Db.SubmitChanges();
			}
			if (!m.testing)
				m.testing = testing ?? false;
			SetHeaders(m.orgid);
			DbUtil.LogActivity("Manage Giving: {0} ({1})".Fmt(m.Organization.OrganizationName, m.person.Name));
			return View(m);
		}

		[HttpPost]
		public ActionResult ManageGiving(ManageGivingModel m)
		{
			SetHeaders(m.orgid);
			RemoveNonDigitsIfNecessary(m);
			m.ValidateModel(ModelState);
			if (!ModelState.IsValid)
				return View(m);
			try
			{
				var gateway = OnlineRegModel.GetTransactionGateway();
				if (gateway == "authorizenet")
				{
					var au = new AuthorizeNet(DbUtil.Db, m.testing);
					au.AddUpdateCustomerProfile(m.pid,
						 m.Type,
						 m.Cardnumber,
						 m.Expires,
						 m.Cardcode,
						 m.Routing,
						 m.Account);

				}
				else if (gateway == "sage")
				{
					var sg = new SagePayments(DbUtil.Db, m.testing);
					sg.storeVault(m.pid,
						 m.Type,
						 m.Cardnumber,
						 m.Expires,
						 m.Cardcode,
						 m.Routing,
						 m.Account,
						 giving: true);
				}
				else
					throw new Exception("ServiceU not supported");

				var mg = m.person.ManagedGiving();
				if (mg == null)
				{
					mg = new ManagedGiving();
					m.person.ManagedGivings.Add(mg);
				}
				mg.SemiEvery = m.SemiEvery;
				mg.Day1 = m.Day1;
				mg.Day2 = m.Day2;
				mg.EveryN = m.EveryN;
				mg.Period = m.Period;
				mg.StartWhen = m.StartWhen;
				mg.StopWhen = m.StopWhen;
				mg.NextDate = mg.FindNextDate(DateTime.Today);

				var pi = m.person.PaymentInfo();
				pi.FirstName = m.firstname.Truncate(50);
				pi.MiddleInitial = m.middleinitial.Truncate(10);
				pi.LastName = m.lastname.Truncate(50);
				pi.Suffix = m.suffix.Truncate(10);
				pi.Address = m.address.Truncate(50);
				pi.City = m.city.Truncate(50);
				pi.State = m.state.Truncate(10);
				pi.Zip = m.zip.Truncate(15);
				pi.Phone = m.phone.Truncate(25);

				var q = from ra in DbUtil.Db.RecurringAmounts
						  where ra.PeopleId == m.pid
						  select ra;
				DbUtil.Db.RecurringAmounts.DeleteAllOnSubmit(q);
				DbUtil.Db.SubmitChanges();
				foreach (var c in m.FundItemsChosen())
				{
					var ra = new RecurringAmount
					{
						PeopleId = m.pid,
						FundId = c.fundid,
						Amt = c.amt
					};
					DbUtil.Db.RecurringAmounts.InsertOnSubmit(ra);
				}
				DbUtil.Db.SubmitChanges();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("form", ex.Message);
			}
			if (!ModelState.IsValid)
				return View(m);
			TempData["managegiving"] = m;
			return Redirect("/OnlineReg/ConfirmRecurringGiving");
		}

		private static void RemoveNonDigitsIfNecessary(ManageGivingModel m)
		{
			bool dorouting = false;
			bool doaccount = m.Account.HasValue() && !m.Account.StartsWith("X");

			if (m.Routing.HasValue() && !m.Routing.StartsWith("X"))
				dorouting = true;

			if (doaccount || dorouting)
			{
				if (doaccount)
					m.Account = m.Account.GetDigits();
				if (dorouting)
					m.Routing = m.Routing.GetDigits();
			}
		}

		public ActionResult ConfirmRecurringGiving()
		{
			var m = TempData["managegiving"] as ManageGivingModel;
			if (m == null)
				return Content("No active registration");
#if DEBUG2
			m.testing = true;
#else
#endif
			var details = ViewExtensions2.RenderPartialViewToString(this, "ManageGiving2", m);

			var staff = DbUtil.Db.StaffPeopleForOrg(m.orgid)[0];
			var text = m.Setting.Body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
			text = text.Replace("{name}", m.person.Name, ignoreCase: true);
			text = text.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
			text = text.Replace("{email}", m.person.EmailAddress, ignoreCase: true);
			text = text.Replace("{phone}", m.person.HomePhone.FmtFone(), ignoreCase: true);
			text = text.Replace("{contact}", staff.Name, ignoreCase: true);
			text = text.Replace("{contactemail}", staff.EmailAddress, ignoreCase: true);
			text = text.Replace("{contactphone}", m.Organization.PhoneNumber.FmtFone(), ignoreCase: true);
			text = text.Replace("{details}", details, ignoreCase: true);

			var contributionemail = (from ex in DbUtil.Db.PeopleExtras
											 where ex.Field == "ContributionEmail"
											 where ex.PeopleId == m.person.PeopleId
											 select ex.Data).SingleOrDefault();
			if (!Util.ValidEmail(contributionemail))
				contributionemail = m.person.FromEmail;
			Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(m.orgid)),
				 m.Setting.Subject, text,
				 Util.EmailAddressListFromString(contributionemail), 0, m.pid);
			Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(contributionemail),
				 "Managed Giving",
				 "Managed giving for {0} ({1})".Fmt(m.person.Name, m.pid),
				 Util.EmailAddressListFromString(DbUtil.Db.StaffEmailForOrg(m.orgid)),
				 0, m.pid);

			SetHeaders(m.orgid);
			ViewBag.Title = "Online Recurring Giving";
			var msg = m.Organization.GetExtra(DbUtil.Db, "ConfirmationDisplay");
			if (!msg.HasValue())
				msg = @"<p>Thank you {first}, for managing your recurring giving</p>
<p>You should receive a confirmation email shortly.</p>";
			msg = msg.Replace("{first}", m.person.PreferredName, ignoreCase: true);
			ViewBag.Message = msg;
			LogOutOfOnlineReg();
			return View(m);
		}

		[HttpPost]
		public ActionResult ConfirmPledge(ManagePledgesModel m)
		{
			var staff = DbUtil.Db.StaffPeopleForOrg(m.orgid);

			var desc = "{0}; {1}; {2}, {3} {4}".Fmt(
				 m.person.Name,
				 m.person.PrimaryAddress,
				 m.person.PrimaryCity,
				 m.person.PrimaryState,
				 m.person.PrimaryZip);

			m.person.PostUnattendedContribution(DbUtil.Db,
				 m.pledge ?? 0,
				 m.Setting.DonationFundId,
				 desc, pledge: true);

			var pi = m.GetPledgeInfo();
			var body = m.Setting.Body;
			if (!body.HasValue())
				return Content("There is no Confirmation Message (required)");
			body = body.Replace("{amt}", pi.Pledged.ToString("N2"), ignoreCase: true);
			body = body.Replace("{org}", m.Organization.OrganizationName, ignoreCase: true);
			body = body.Replace("{first}", m.person.PreferredName, ignoreCase: true);
			DbUtil.Db.EmailRedacted(staff[0].FromEmail, m.person, m.Setting.Subject, body);

			DbUtil.Db.Email(m.person.FromEmail, staff, "Online Pledge", @"{0} made a pledge to {1}".Fmt(m.person.Name, m.Organization.OrganizationName));

			SetHeaders(m.orgid);
			return View(m);
		}
	}
}
