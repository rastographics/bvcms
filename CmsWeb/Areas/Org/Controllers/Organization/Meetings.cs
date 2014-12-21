using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult Meetings(MeetingsModel m)
        {
            //var m = new MeetingsModel(id, future ?? false);
            DbUtil.LogActivity("Viewing Meetings for {0}".Fmt(Session["ActiveOrganization"]));
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult NewMeeting(string d, string t, int AttendCredit, bool group)
        {
            var organization = DbUtil.Db.LoadOrganizationById(Util2.CurrentOrganization.Id);
            if (organization == null)
                return Content("error: no org");
            DateTime dt;
            if (!DateTime.TryParse(d + " " + t, out dt))
                return Content("error: bad date");
            var mt = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingDate == dt
                    && m.OrganizationId == organization.OrganizationId);

            if (mt != null)
                return Content("/Meeting/" + mt.MeetingId);

            mt = new CmsData.Meeting
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                OrganizationId = organization.OrganizationId,
                GroupMeetingFlag = group,
                Location = organization.Location,
                MeetingDate = dt,
                AttendCreditId = AttendCredit
            };
            DbUtil.Db.Meetings.InsertOnSubmit(mt);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Creating new meeting for {0}".Fmt(organization.OrganizationName));
            return Content("/Meeting/" + mt.MeetingId);
        }
        [Route("GotoMeetingForDate/{oid:int}/{ticks:long}")]
        public ActionResult GotoMeetingForDate(int oid, long ticks)
        {
			var dt = new DateTime(ticks); // ticks here is meeting time
            var q = from m in DbUtil.Db.Meetings
                where m.OrganizationId == oid
                where m.MeetingDate == dt
                select m;
            var meeting = q.FirstOrDefault();
            if (meeting != null)
                return Redirect("/Meeting/" + meeting.MeetingId);
            return Message("no meeting at " + dt.FormatDateTm());
        }

    }
}