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
		[HttpGet]
		public ActionResult RequestReport(int mid, int pid, long ticks)
		{
			var vs = new VolunteerRequestModel(mid, pid, ticks);
			SetHeaders(vs.org.OrganizationId);
			return View(vs);
		}

		[HttpGet]
		public ActionResult RequestResponse(string ans, string guid)
		{
			try
			{
				var vs = new VolunteerRequestModel(guid);
				vs.ProcessReply(ans);
				return Content(vs.DisplayMessage);
			}
			catch (Exception ex)
			{
				return Content(ex.Message);
			}
		}

		[HttpGet]
		public ActionResult GetVolSub(int aid, int pid)
		{
			var vs = new VolSubModel(aid, pid);
			SetHeaders(vs.org.OrganizationId);
			vs.ComposeMessage();
			return View(vs);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult GetVolSub(int aid, int pid, long ticks, int[] pids, string subject, string message)
		{
			var m = new VolSubModel(aid, pid, ticks);
			m.subject = subject;
			m.message = message;
			if (pids == null)
				return Content("no emails sent (no recipients were selected)");
			m.pids = pids;
			m.SendEmails();
			return Content("Emails are being sent, thank you.");
		}

		public ActionResult VolSubReport(int aid, int pid, long ticks)
		{
			var vs = new VolSubModel(aid, pid, ticks);
			SetHeaders(vs.org.OrganizationId);
			return View(vs);
		}

		public ActionResult ClaimVolSub(string ans, string guid)
		{
			try
			{
				var vs = new VolSubModel(guid);
				vs.ProcessReply(ans);
				return Content(vs.DisplayMessage);
			}
			catch (Exception ex)
			{
				return Content(ex.Message);
			}
		}

		public ActionResult ManageVolunteer(string id, int? pid)
		{
			if (!id.HasValue())
				return Content("bad link");
			VolunteerModel m = null;

			var td = TempData["ps"];
			if (td != null)
			{
				m = new VolunteerModel(orgId: id.ToInt(), peopleId: td.ToInt());
			}
			else if (pid.HasValue)
			{
				var leader = OrganizationModel.VolunteerLeaderInOrg(id.ToInt2());
				if (leader)
					m = new VolunteerModel(orgId: id.ToInt(), peopleId: pid.Value, leader: true);
			}
			if (m == null)
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
				m = new VolunteerModel(orgId: a[0].ToInt(), peopleId: a[1].ToInt());
				id = a[0];
				ot.Used = true;
				DbUtil.Db.SubmitChanges();
			}

			SetHeaders(id.ToInt());
			DbUtil.LogActivity("Pick Slots: {0} ({1})".Fmt(m.Org.OrganizationName, m.Person.Name));
			return View(m);
		}

		[HttpPost]
		public ActionResult ConfirmVolunteerSlots(VolunteerModel m)
		{
			m.UpdateCommitments();
			if (m.SendEmail || !m.IsLeader)
			{
				List<Person> Staff = null;
				Staff = DbUtil.Db.StaffPeopleForOrg(m.OrgId);
				var staff = Staff[0];

				var summary = m.Summary(this);
				var text = Util.PickFirst(m.setting.Body, "confirmation email body not found");
				text = text.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
				text = text.Replace("{name}", m.Person.Name, ignoreCase: true);
				text = text.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
				text = text.Replace("{email}", m.Person.EmailAddress, ignoreCase: true);
				text = text.Replace("{phone}", m.Person.HomePhone.FmtFone(), ignoreCase: true);
				text = text.Replace("{contact}", staff.Name, ignoreCase: true);
				text = text.Replace("{contactemail}", staff.EmailAddress, ignoreCase: true);
				text = text.Replace("{contactphone}", m.Org.PhoneNumber.FmtFone(), ignoreCase: true);
				text = text.Replace("{details}", summary, ignoreCase: true);
				DbUtil.Db.Email(staff.FromEmail, m.Person, m.setting.Subject, text);

				DbUtil.Db.Email(m.Person.FromEmail, Staff, "Volunteer Commitments managed", @"{0} managed volunteer commitments to {1}<br/>
The following Committments:<br/>
{2}".Fmt(m.Person.Name, m.Org.OrganizationName, summary));
			}
			ViewData["Organization"] = m.Org.OrganizationName;
			SetHeaders(m.OrgId);
			if (m.IsLeader)
				return View("ManageVolunteer", m);
			return View(m);
		}

		public ActionResult ManageSubscriptions(string id)
		{
			if (!id.HasValue())
				return Content("bad link");
			ManageSubsModel m;
			var td = TempData["ms"];
			if (td != null)
				m = new ManageSubsModel(td.ToInt(), id.ToInt());
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
				m = new ManageSubsModel(a[1].ToInt(), a[0].ToInt());
				id = a[0];
				ot.Used = true;
				DbUtil.Db.SubmitChanges();
			}
			SetHeaders(id.ToInt());
			DbUtil.LogActivity("Manage Subs: {0} ({1})".Fmt(m.Description(), m.person.Name));
			return View(m);
		}

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
			return Redirect("ConfirmRecurringGiving");
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
			var text = m.setting.Body.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
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
				 m.setting.Subject, text,
				 Util.EmailAddressListFromString(contributionemail), 0, m.pid);
			Util.SendMsg(Util.SysFromEmail, Util.Host, Util.TryGetMailAddress(contributionemail),
				 "Managed Giving",
				 "Managed giving for {0} ({1})".Fmt(m.person.Name, m.pid),
				 Util.EmailAddressListFromString(DbUtil.Db.StaffEmailForOrg(m.orgid)),
				 0, m.pid);

			SetHeaders(m.orgid);
			ViewBag.Title = "Online Recurring Giving";
			var msg = m.Organization.GetExtra("ConfirmationDisplay");
			if (!msg.HasValue())
				msg = @"<p>Thank you {first}, for managing your recurring giving</p>
<p>You should receive a confirmation email shortly.</p>";
			msg = msg.Replace("{first}", m.person.PreferredName, ignoreCase: true);
			ViewBag.Message = msg;
			LogOutOfOnlineReg();
			return View(m);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ConfirmSubscriptions(ManageSubsModel m)
		{
			m.UpdateSubscriptions();
			var Staff = DbUtil.Db.StaffPeopleForOrg(m.masterorgid.Value);

			DbUtil.Db.Email(Staff.First().FromEmail, m.person,
					  "Subscription Confirmation",
@"Thank you for managing your subscriptions to {0}<br/>
You have the following subscriptions:<br/>
{1}".Fmt(m.Description(), m.Summary));

			DbUtil.Db.Email(m.person.FromEmail, Staff, "Subscriptions managed", @"{0} managed subscriptions to {1}<br/>
You have the following subscriptions:<br/>
{2}".Fmt(m.person.Name, m.Description(), m.Summary));

			SetHeaders(m.masterorgid.Value);
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
				 m.setting.DonationFundId,
				 desc, pledge: true);

			var pi = m.GetPledgeInfo();
			var body = m.setting.Body;
			if (!body.HasValue())
				return Content("There is no Confirmation Message (required)");
			body = body.Replace("{amt}", pi.Pledged.ToString("N2"), ignoreCase: true);
			body = body.Replace("{org}", m.Organization.OrganizationName, ignoreCase: true);
			body = body.Replace("{first}", m.person.PreferredName, ignoreCase: true);
			DbUtil.Db.EmailRedacted(staff[0].FromEmail, m.person, m.setting.Subject, body);

			DbUtil.Db.Email(m.person.FromEmail, staff, "Online Pledge", @"{0} made a pledge to {1}".Fmt(m.person.Name, m.Organization.OrganizationName));

			SetHeaders(m.orgid);
			return View(m);
		}

		public ActionResult VoteLinkSg(string id, string message, bool? confirm)
		{
			if (!id.HasValue())
				return Content("bad link");

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
			var a = ot.Querystring.SplitStr(",", 5);
			var oid = a[0].ToInt();
			var pid = a[1].ToInt();
			var emailid = a[2].ToInt();
			var pre = a[3];
			var smallgroup = a[4];
			var q = (from pp in DbUtil.Db.People
						where pp.PeopleId == pid
						let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
						let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
						select new { p = pp, org = org, om = om }).Single();

			if (q.org == null)
				return Content("org missing, bad link");

			if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
				return Content("votelink is no longer active");

			if (q.om == null && q.org.Limit <= q.org.MemberCount)
				return Content("sorry, maximum limit has been reached");

			if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
				return Content("sorry, registration has been closed");

			var setting = new Settings(q.org.RegSetting, DbUtil.Db, oid);
			if (IsSmallGroupFilled(setting, oid, smallgroup))
				return Content("sorry, maximum limit has been reached for " + smallgroup);

			var omb = q.om;
			omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
				 oid, pid, 220, DateTime.Now, null, false);
			//DbUtil.Db.UpdateMainFellowship(oid);

			if (q.org.AddToSmallGroupScript.HasValue())
			{
				var script = DbUtil.Db.Content(q.org.AddToSmallGroupScript);
				if (script != null && script.Body.HasValue())
				{
					try
					{
						var pe = new PythonEvents(DbUtil.Db, "RegisterEvent", script.Body);
						pe.instance.AddToSmallGroup(smallgroup, omb);
					}
					catch (Exception)
					{
					}
				}
			}
			omb.AddToGroup(DbUtil.Db, smallgroup);
			omb.AddToGroup(DbUtil.Db, "emailid:" + emailid);
			ot.Used = true;
			DbUtil.Db.SubmitChanges();
			DbUtil.LogActivity("Votelink: {0}".Fmt(q.org.OrganizationName));

			if (confirm == true)
			{
				var subject = Util.PickFirst(setting.Subject, "no subject");
				var msg = Util.PickFirst(setting.Body, "no message");
				msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationName, q.org.Location, msg);
				msg = msg.Replace("{details}", smallgroup);
				var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

				try
				{
					DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
				}
				catch (Exception ex)
				{
					DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
										 q.org.OrganizationName,
										 "There was a problem sending confirmation from org: " + ex.Message);
				}
				DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
						  q.org.OrganizationName,
						  "{0} has registered for {1}<br>{2}<br>(from votelink)".Fmt(q.p.Name, q.org.OrganizationName, smallgroup));
			}

			return Content(message);
		}

		// todo: remove the following method after Aug 1, 2013
		public ActionResult VoteLink(string id, string smallgroup, string message, bool? confirm)
		{
			if (!id.HasValue())
				return Content("bad link");

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
			var oid = a[0].ToInt();
			var pid = a[1].ToInt();
			var emailid = a[2].ToInt();
			var pre = a[3];
			var q = (from pp in DbUtil.Db.People
						where pp.PeopleId == pid
						let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
						let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
						select new { p = pp, org = org, om = om }).Single();

			if (q.org == null)
				return Content("org missing, bad link");

			if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
				return Content("votelink is no longer active");

			if (q.om == null && q.org.Limit <= q.org.MemberCount)
				return Content("sorry, maximum limit has been reached");

			if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
				return Content("sorry, registration has been closed");

			var setting = new Settings(q.org.RegSetting, DbUtil.Db, oid);
			if (IsSmallGroupFilled(setting, oid, smallgroup))
				return Content("sorry, maximum limit has been reached for " + smallgroup);

			var omb = q.om;
			omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
				 oid, pid, 220, DateTime.Now, null, false);
			//DbUtil.Db.UpdateMainFellowship(oid);

			if (q.org.AddToSmallGroupScript.HasValue())
			{
				var script = DbUtil.Db.Content(q.org.AddToSmallGroupScript);
				if (script != null && script.Body.HasValue())
				{
					try
					{
						var pe = new PythonEvents(DbUtil.Db, "RegisterEvent", script.Body);
						pe.instance.AddToSmallGroup(smallgroup, omb);
					}
					catch (Exception)
					{
					}
				}
			}
			omb.AddToGroup(DbUtil.Db, smallgroup);
			omb.AddToGroup(DbUtil.Db, "emailid:" + emailid);
			ot.Used = true;
			DbUtil.Db.SubmitChanges();
			DbUtil.LogActivity("Votelink: {0}".Fmt(q.org.OrganizationName));

			if (confirm == true)
			{
				var subject = Util.PickFirst(setting.Subject, "no subject");
				var msg = Util.PickFirst(setting.Body, "no message");
				msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationName, q.org.Location, msg);
				msg = msg.Replace("{details}", smallgroup);
				var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

				DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
				DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
						  q.org.OrganizationName,
						  "{0} has registered for {1}<br>{2}<br>(from votelink)".Fmt(q.p.Name, q.org.OrganizationName, smallgroup));
			}

			return Content(message);
		}

		public ActionResult TestVoteLink(string id, string smallgroup, string message, bool? confirm)
		{
			if (!id.HasValue())
				return Content("bad link");

			var guid = id.ToGuid();
			if (guid == null)
				return Content("not a guid");
			var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
			if (ot == null)
				return Content("cannot find link");
			if (ot.Used)
				return Content("link used");
			if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
				return Content("link expired");
			var a = ot.Querystring.SplitStr(",", 5);
			var oid = a[0].ToInt();
			var pid = a[1].ToInt();
			var emailid = a[2].ToInt();
			var pre = a[3];
			if (a.Length == 5)
				smallgroup = a[4];
			var q = (from pp in DbUtil.Db.People
						where pp.PeopleId == pid
						let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
						let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
						select new { p = pp, org, om }).SingleOrDefault();
			if (q == null)
				return Content("peopleid {0} not found".Fmt(pid));

			if (q.org == null)
				return Content("no org " + oid);

			if (q.om == null && q.org.Limit <= q.org.MemberCount)
				return Content("sorry, maximum limit has been reached");

			if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
				return Content("sorry, registration has been closed");

			var setting = new Settings(q.org.RegSetting, DbUtil.Db, oid);
			if (IsSmallGroupFilled(setting, oid, smallgroup))
				return Content("sorry, maximum limit has been reached for " + smallgroup);

			return Content(@"<pre>
looks ok
oid={0}
pid={1}
emailid={2}
</pre>".Fmt(oid, pid, emailid));
		}

		public ActionResult RsvpLinkSg(string id, string message, bool? confirm)
		{
			if (!id.HasValue())
				return Content("bad link");

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
			var a = ot.Querystring.SplitStr(",", 4);
			var meetingid = a[0].ToInt();
			var pid = a[1].ToInt();
			var emailid = a[2].ToInt();
			var smallgroup = a[3];
			var q = (from pp in DbUtil.Db.People
						where pp.PeopleId == pid
						let meeting = DbUtil.Db.Meetings.SingleOrDefault(mm => mm.MeetingId == meetingid)
						let org = meeting.Organization
						select new { p = pp, org, meeting }).Single();

			if (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive)
				return Content("sorry, registration has been closed");

			if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
				return Content("rsvp is no longer available");

			if (q.org.Limit <= q.meeting.Attends.Count(aa => aa.Commitment == 1))
				return Content("sorry, maximum limit has been reached");
			var omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
											  q.meeting.OrganizationId, pid, 220, DateTime.Now, null, false);
			if (smallgroup.HasValue())
				omb.AddToGroup(DbUtil.Db, smallgroup);
			omb.AddToGroup(DbUtil.Db, "emailid:" + emailid);


			ot.Used = true;
			DbUtil.Db.SubmitChanges();
			Attend.MarkRegistered(DbUtil.Db, pid, meetingid, 1);
			DbUtil.LogActivity("Rsvplink: {0}".Fmt(q.org.OrganizationName));
			var setting = new Settings(q.org.RegSetting, DbUtil.Db, q.meeting.OrganizationId);

			if (confirm == true)
			{
				var subject = Util.PickFirst(setting.Subject, "no subject");
				var msg = Util.PickFirst(setting.Body, "no message");
				msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationName, q.org.Location, msg);
				msg = msg.Replace("{details}", q.meeting.MeetingDate.ToString2("f"));
				var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

				DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
				DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
						  q.org.OrganizationName,
						  "{0} has registered for {1}<br>{2}".Fmt(q.p.Name, q.org.OrganizationName, q.meeting.MeetingDate.ToString2("f")));
			}
			return Content(message);

		}

		// todo: remove the following method after Aug 1, 2013
		public ActionResult RsvpLink(string id, string smallgroup, string message, bool? confirm)
		{
			if (!id.HasValue())
				return Content("bad link");

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
			var meetingid = a[0].ToInt();
			var pid = a[1].ToInt();
			var emailid = a[2].ToInt();
			var q = (from pp in DbUtil.Db.People
						where pp.PeopleId == pid
						let meeting = DbUtil.Db.Meetings.SingleOrDefault(mm => mm.MeetingId == meetingid)
						let org = meeting.Organization
						select new { p = pp, org, meeting }).Single();

			if (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive)
				return Content("sorry, registration has been closed");

			if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
				return Content("rsvp is no longer available");

			if (q.org.Limit <= q.meeting.Attends.Count(aa => aa.Commitment == 1))
				return Content("sorry, maximum limit has been reached");

			var omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
											  q.meeting.OrganizationId, pid, 220, DateTime.Now, null, false);
			if (smallgroup.HasValue())
				omb.AddToGroup(DbUtil.Db, smallgroup);
			omb.AddToGroup(DbUtil.Db, "emailid:" + emailid);

			ot.Used = true;
			DbUtil.Db.SubmitChanges();
			Attend.MarkRegistered(DbUtil.Db, pid, meetingid, 1);
			DbUtil.LogActivity("Rsvplink: {0}".Fmt(q.org.OrganizationName));
			var setting = new Settings(q.org.RegSetting, DbUtil.Db, q.meeting.OrganizationId);

			if (confirm == true)
			{
				var subject = Util.PickFirst(setting.Subject, "no subject");
				var msg = Util.PickFirst(setting.Body, "no message");
				msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationName, q.org.Location, msg);
				msg = msg.Replace("{details}", q.meeting.MeetingDate.ToString2("f"));
				var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

				DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
				DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
						  q.org.OrganizationName,
						  "{0} has registered for {1}<br>{2}".Fmt(q.p.Name, q.org.OrganizationName, q.meeting.MeetingDate.ToString2("f")));
			}
			return Content(message);
		}

		[ValidateInput(false)]
		public ActionResult RegisterLink(string id, bool? showfamily)
		{
			if (!id.HasValue())
				return Content("bad link");
			if (!Request.Browser.Cookies)
				return Content(Request.UserAgent + "<br>Your browser must support cookies");

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
			var oid = a[0].ToInt();
			var pid = a[1].ToInt();
			var q = (from pp in DbUtil.Db.People
						where pp.PeopleId == pid
						let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
						let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
						select new { p = pp, org = org, om = om }).Single();

			if (q.org == null)
				return Content("org missing, bad link");

			if (q.om == null && q.org.Limit <= q.org.MemberCount)
				return Content("sorry, maximum limit has been reached");

			if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
				return Content("sorry, registration has been closed");

			var url = "/OnlineReg/Index/{0}?registertag={1}".Fmt(oid, id);
			if (showfamily == true)
				url += "&showfamily=true";
			return Redirect(url);
		}

		private bool IsSmallGroupFilled(Settings setting, int orgid, string sg)
		{
			var GroupTags = (from mt in DbUtil.Db.OrgMemMemTags
								  where mt.OrgId == orgid
								  select mt.MemberTag.Name).ToList();
			return setting.AskItems.Where(aa => aa.Type == "AskDropdown").Any(aa => ((AskDropdown)aa).IsSmallGroupFilled(GroupTags, sg))
				 || setting.AskItems.Where(aa => aa.Type == "AskCheckboxes").Any(aa => ((AskCheckboxes)aa).IsSmallGroupFilled(GroupTags, sg));
		}
	}
}
