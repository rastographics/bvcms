using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Reports.Models;
using System.IO;
using CmsData;
using UtilityExtensions;
using CmsWeb.Models;
using System.Text;
using System.Web.UI;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CmsData.Codes;

namespace CmsWeb.Areas.Dialog.Controllers
{
	[Authorize(Roles = "Edit")]
	public class DeleteMeetingController : CmsController
	{
        [Authorize(Roles="Edit")]
        public ActionResult Index(int id)
        {
            var mm = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingId == id);
            if (mm == null)
                return Content("error: no meeting");
			DbUtil.LogActivity("Delete meeting for {0}".Fmt(Session["ActiveOrganization"]));

			var runningtotals = new DeleteMeetingRun
			{
				Started = DateTime.Now,
				Count = mm.Attends.Count(a => a.EffAttendFlag == true || a.AttendanceFlag == true),
				Processed = 0,
				Meetingid = id
			};
			DbUtil.Db.DeleteMeetingRuns.InsertOnSubmit(runningtotals);
			DbUtil.Db.SubmitChanges();
			var host = Util.Host;
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				var Db = new CMSDataContext(Util.GetConnectionString(host));
			    var cul = Db.Setting("Culture", "en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

	            var meeting = Db.Meetings.SingleOrDefault(m => m.MeetingId == id);
				var q = from a in Db.Attends
						where a.MeetingId == id
						where a.AttendanceFlag == true || a.EffAttendFlag == true
						select a.PeopleId;
				var list = q.ToList();
				foreach (var pid in list)
				{
					Db.Dispose();
					Db = new CMSDataContext(Util.GetConnectionString(host));
					Attend.RecordAttendance(Db, pid, id, false);
					var r = Db.DeleteMeetingRuns.Where(m => m.Meetingid == id).OrderByDescending(m => m.Id).First();
					r.Processed++;
			        Db.SubmitChanges();
				}
				var rr = Db.DeleteMeetingRuns.Where(m => m.Meetingid == id).OrderByDescending(m => m.Id).First();
				rr.Processed--;
	            Db.SubmitChanges();
				Db.ExecuteCommand(
					"delete dbo.SubRequest WHERE EXISTS(SELECT NULL FROM Attend a WHERE a.AttendId = AttendId AND a.MeetingId = {0})",
					id);
				Db.ExecuteCommand("DELETE dbo.VolRequest where MeetingId = {0}", id);
				Db.ExecuteCommand("delete attend where MeetingId = {0}", id);
				Db.ExecuteCommand("delete MeetingExtra where MeetingId = {0}", id);
				Db.ExecuteCommand("delete meetings where MeetingId = {0}", id);
				rr.Processed++;
				rr.Completed = DateTime.Now;
	            Db.SubmitChanges();
			});
			return Redirect("/DeleteMeeting/Progress/" + id);
		}
		[HttpPost]
		public JsonResult Progress2(int id)
		{
			var r = DbUtil.Db.DeleteMeetingRuns.Where(mm => mm.Meetingid == id).OrderByDescending(mm => mm.Id).First();
			return Json(new { r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running });
		}
		[HttpGet]
		public ActionResult Progress(int id)
		{
            var mm = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingId == id);
			if (mm == null)
				return View(new DeleteMeetingRun { Error = "meeting not found", Started = DateTime.Now });
			ViewBag.orgname = mm.Organization.OrganizationName;
			ViewBag.meetingid = id;
			ViewBag.meetingdate = mm.MeetingDate.Value;
			var r = DbUtil.Db.DeleteMeetingRuns.Where(m => m.Meetingid == id).OrderByDescending(m => m.Id).First();
			return View(r);
		}
	}
}
