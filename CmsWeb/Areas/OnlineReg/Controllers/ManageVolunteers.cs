using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using UtilityExtensions;
using System.Collections.Generic;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
	public partial class OnlineRegController
	{
		[HttpGet]
        [Route("VolRequestReport/{mid:int}/{pid:int}/{ticks:long}")]
		public ActionResult VolRequestReport(int mid, int pid, long ticks)
		{
			var vs = new VolunteerRequestModel(mid, pid, ticks);
			SetHeaders(vs.org.OrganizationId);
			return View("ManageVolunteer/VolRequestReport", vs);
		}

		[HttpGet]
        [Route("VolRequestResponse")]
        [Route("VolRequestResponse/{ans}/{guid}")]
		public ActionResult RequestResponse(string ans, string guid)
		{
            ViewBag.Answer = ans;
            ViewBag.Guid = guid;
            return View();
		}

        [HttpPost]
        [Route("VolRequestResponse")]
        [Route("VolRequestResponse/{ans}/{guid}")]
        public ActionResult RequestResponse(string ans, string guid, FormCollection formCollection)
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
        [Route("GetVolSub/{aid:int}/{pid:int}")]
		public ActionResult GetVolSub(int aid, int pid)
		{
			var vs = new VolSubModel(aid, pid);
			SetHeaders(vs.org.OrganizationId);
			vs.ComposeMessage();
			return View("ManageVolunteer/GetVolSub", vs);
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

        [Route("VolSubReport/{aid:int}/{pid:int}/{ticks:long}")]
		public ActionResult VolSubReport(int aid, int pid, long ticks)
		{
			var vs = new VolSubModel(aid, pid, ticks);
			SetHeaders(vs.org.OrganizationId);
			return View("ManageVolunteer/VolSubReport", vs);
		}

        [Route("ClaimVolSub/{ans}/{guid}")]
        public ActionResult ClaimVolSub(string ans, string guid)
        {
            ViewBag.Answer = ans;
            ViewBag.Guid = guid;
            return View();
        }

        [HttpPost]
        [Route("ClaimVolSub/{ans}/{guid}")]
		public ActionResult ClaimVolSub(string ans, string guid, FormCollection formCollection)
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

        [Route("ManageVolunteer/{id}")]
        [Route("ManageVolunteer/{id}/{pid:int}")]
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
				var leader = OrganizationMember.VolunteerLeaderInOrg(DbUtil.Db, id.ToInt2());
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
			return View("ManageVolunteer/PickSlots", m);
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
				var text = Util.PickFirst(m.Setting.Body, "confirmation email body not found");
				text = text.Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "church"), ignoreCase: true);
				text = text.Replace("{name}", m.Person.Name, ignoreCase: true);
				text = text.Replace("{date}", DateTime.Now.ToString("d"), ignoreCase: true);
				text = text.Replace("{email}", m.Person.EmailAddress, ignoreCase: true);
				text = text.Replace("{phone}", m.Person.HomePhone.FmtFone(), ignoreCase: true);
				text = text.Replace("{contact}", staff.Name, ignoreCase: true);
				text = text.Replace("{contactemail}", staff.EmailAddress, ignoreCase: true);
				text = text.Replace("{contactphone}", m.Org.PhoneNumber.FmtFone(), ignoreCase: true);
				text = text.Replace("{details}", summary, ignoreCase: true);
				DbUtil.Db.Email(staff.FromEmail, m.Person, m.Setting.Subject, text);

				DbUtil.Db.Email(m.Person.FromEmail, Staff, "Volunteer Commitments managed", @"{0} managed volunteer commitments to {1}<br/>
The following Committments:<br/>
{2}".Fmt(m.Person.Name, m.Org.OrganizationName, summary));
			}
			ViewData["Organization"] = m.Org.OrganizationName;
			SetHeaders(m.OrgId);
			if (m.IsLeader)
				return View("ManageVolunteer/PickSlots", m);
			return View("ManageVolunteer/ConfirmVolunteerSlots", m);
		}

	}
}
