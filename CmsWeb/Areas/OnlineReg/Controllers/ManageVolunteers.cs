using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        private const string Fromcalendar = "fromcalendar";

        public OnlineRegController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
            return View("ManageVolunteer/RequestResponse");
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
            var token = TempData[Fromcalendar] as bool?;
            if (token == true)
            {
                var vs = new VolSubModel(aid, pid);
                SetHeaders(vs.org.OrganizationId);
                vs.ComposeMessage();
                return View("ManageVolunteer/GetVolSub", vs);
            }
            return Message("Must come to GetVolSub from calendar");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetVolSub(int aid, int pid, long ticks, int[] pids, string subject, string message)
        {
            var m = new VolSubModel(aid, pid, ticks);
            m.subject = subject;
            m.message = message;
            if (pids == null)
            {
                return Content("no emails sent (no recipients were selected)");
            }

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
            try
            {
                var vs = new VolSubModel();
                vs.PrepareToClaim(ans, guid);
                ViewBag.Answer = ans;
                ViewBag.Guid = guid;
                return View("ManageVolunteer/ClaimVolSub");
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
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
                return Message(ex.Message);
            }
        }

        [Route("ManageVolunteer/{id}")]
        [Route("ManageVolunteer/{id}/{pid:int}")]
        public ActionResult ManageVolunteer(string id, int? pid)
        {
            if (!id.HasValue())
            {
                return Content("bad link");
            }

            VolunteerModel m = null;

            var td = TempData["PeopleId"];
            if (td != null)
            {
                m = new VolunteerModel(id.ToInt(), td.ToInt());
            }
            else if (pid.HasValue)
            {
                var leader = OrganizationMember.VolunteerLeaderInOrg(CurrentDatabase, id.ToInt2());
                if (leader)
                {
                    m = new VolunteerModel(id.ToInt(), pid.Value, true);
                }
            }
            if (m == null)
            {
                var guid = id.ToGuid();
                if (guid == null)
                {
                    return Content("invalid link");
                }

                var ot = CurrentDatabase.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                if (ot == null)
                {
                    return Content("invalid link");
                }
#if DEBUG2
#else
                if (ot.Used)
                {
                    return Content("link used");
                }
#endif
                if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                {
                    return Content("link expired");
                }

                var a = ot.Querystring.Split(',');
                m = new VolunteerModel(a[0].ToInt(), a[1].ToInt());
                id = a[0];
                ot.Used = true;
                CurrentDatabase.SubmitChanges();
            }

            SetHeaders(id.ToInt());
            DbUtil.LogActivity($"Pick Slots: {m.Org.OrganizationName} ({m.Person.Name})");
            TempData[Fromcalendar] = true;
            return View("ManageVolunteer/PickSlots", m);
        }

        [HttpPost]
        public ActionResult ConfirmVolunteerSlots(VolunteerModel m)
        {
            m.UpdateCommitments();
            if (m.SendEmail || !m.IsLeader)
            {
                List<Person> Staff = null;
                Staff = CurrentDatabase.StaffPeopleForOrg(m.OrgId);
                var staff = Staff[0];

                var summary = m.Summary(this);
                var text = Util.PickFirst(m.Setting.Body, "confirmation email body not found");
                text = text.Replace("{church}", CurrentDatabase.Setting("NameOfChurch", "church"), true);
                text = text.Replace("{name}", m.Person.Name, true);
                text = text.Replace("{date}", DateTime.Now.ToString("d"), true);
                text = text.Replace("{email}", m.Person.EmailAddress, true);
                text = text.Replace("{phone}", m.Person.HomePhone.FmtFone(), true);
                text = text.Replace("{contact}", staff.Name, true);
                text = text.Replace("{contactemail}", staff.EmailAddress, true);
                text = text.Replace("{contactphone}", m.Org.PhoneNumber.FmtFone(), true);
                text = text.Replace("{details}", summary, true);
                CurrentDatabase.Email(staff.FromEmail, m.Person, m.Setting.Subject, text);

                CurrentDatabase.Email(m.Person.FromEmail, Staff, "Volunteer Commitments managed", $@"{m.Person.Name} managed volunteer commitments to {m.Org.OrganizationName}<br/>
The following Commitments:<br/>
{summary}");
            }
            ViewData["Organization"] = m.Org.OrganizationName;
            SetHeaders(m.OrgId);
            if (m.IsLeader)
            {
                TempData[Fromcalendar] = true;
                return View("ManageVolunteer/PickSlots", m);
            }
            return View("ManageVolunteer/ConfirmVolunteerSlots", m);
        }
    }
}
