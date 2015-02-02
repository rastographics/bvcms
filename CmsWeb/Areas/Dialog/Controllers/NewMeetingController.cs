using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class DialogController
    {
        [HttpPost, Route("ForNewMeeting/{orgid:int}")]
        public ActionResult ForNewMeeting(int orgid)
        {
            var oi = new OrganizationModel() { Id = orgid };
            var m = new NewMeetingInfo()
            {
                MeetingDate = oi.PrevMeetingDate,
                Schedule = new CodeInfo(0, oi.SchedulesPrev()),
                AttendCredit = new CodeInfo(0, oi.AttendCreditList()),
            };
            ViewBag.Action = "/NewMeeting/Create/" + orgid;
            ViewBag.Method = "POST";
            return View("MeetingInfo", m);
        }
        [HttpPost, Route("ForNewRollsheet/{orgid:int}")]
        public ActionResult ForNewRollsheet(int orgid)
        {
            var oi = new OrganizationModel() { Id = orgid };
            var m = new NewMeetingInfo()
            {
                MeetingDate =  oi.NextMeetingDate,
                Schedule = new CodeInfo(0, oi.SchedulesNext()),
                AttendCredit = new CodeInfo(0, oi.AttendCreditList()),
            };
            ViewBag.Action = "/Reports/RollsheetForOrg/" + orgid;
            ViewBag.Method = "POST";
            return View("MeetingInfo", m);
        }
        [HttpPost, Route("ForNewRallyRollsheet/{orgid:int}")]
        public ActionResult ForNewRallyRollsheet(int orgid)
        {
            var oi = new OrganizationModel() { Id = orgid };
            var m = new NewMeetingInfo()
            {
                MeetingDate =  oi.NextMeetingDate,
                Schedule = new CodeInfo(0, oi.SchedulesNext()),
                AttendCredit = new CodeInfo(0, oi.AttendCreditList()),
            };
            ViewBag.Action = "/Reports/RallyRollsheetForOrg/" + orgid;
            ViewBag.Method = "POST";
            return View("MeetingInfo", m);
        }
        [HttpPost, Route("ForNewRollsheets/{schedule:int}")]
        public ActionResult ForNewRollsheets(int schedule)
        {
            var m = new NewMeetingInfo()
            {
                MeetingDate =  OrgSearchModel.DefaultMeetingDate(schedule),
                Schedule = null,
                AttendCredit = null
            };
            return View("MeetingInfo", m);
        }
        [HttpPost, Route("ForNewRallyRollsheets/{schedule:int}")]
        public ActionResult ForNewRallyRollsheets(int schedule)
        {
            var m = new NewMeetingInfo()
            {
                MeetingDate =  OrgSearchModel.DefaultMeetingDate(schedule),
                Schedule = null,
                AttendCredit = null,
            };
            return View("MeetingInfo", m);
        }
        [HttpPost]
        public ActionResult Create(NewMeetingInfo model)
        {
            if (!ModelState.IsValid)
                return View("MeetingInfo", model);
            var organization = DbUtil.Db.LoadOrganizationById(Util2.CurrentOrganization.Id);
            if (organization == null)
                return Content("error: no org");
            var mt = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingDate == model.MeetingDate
                    && m.OrganizationId == organization.OrganizationId);

            if (mt != null)
                return Redirect("/Meeting/" + mt.MeetingId);

            mt = new Meeting
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                OrganizationId = organization.OrganizationId,
                GroupMeetingFlag = model.ByGroup,
                Location = organization.Location,
                MeetingDate = model.MeetingDate,
                AttendCreditId = model.AttendCredit.Value.ToInt()
            };
            DbUtil.Db.Meetings.InsertOnSubmit(mt);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Creating new meeting for {0}".Fmt(organization.OrganizationName));
            return Redirect("/Meeting/" + mt.MeetingId);
        }
    }
}
