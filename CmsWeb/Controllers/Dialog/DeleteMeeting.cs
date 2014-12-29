using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class DialogController
    {
        public ActionResult DeleteMeeting(int id)
        {
            if (Request.HttpMethod == "GET")
            {
    			var r = DbUtil.Db.DeleteMeetingRuns.SingleOrDefault(m => m.Meetingid == id );
                if (r != null) 
                    DbUtil.Db.DeleteMeetingRuns.DeleteOnSubmit(r);
                DbUtil.Db.SubmitChanges();
                var mm = DbUtil.Db.Meetings.Single(m => m.MeetingId == id);
                return View(new DeleteMeetingRun { Id = id, Count = mm.Attends.Count(a => a.AttendanceFlag || a.EffAttendFlag == true) });
            }
			var rr = DbUtil.Db.DeleteMeetingRuns.SingleOrDefault(m => m.Meetingid == id );
            if (rr == null) 
            {
                // start delete process
    			DbUtil.LogActivity("Delete meeting for {0}".Fmt(Session["ActiveOrganization"]));
                var mm = DbUtil.Db.Meetings.Single(m => m.MeetingId == id);
    			var runningtotals = new DeleteMeetingRun
    			{
    				Started = DateTime.Now,
    				Count = mm.Attends.Count(a => a.AttendanceFlag || a.EffAttendFlag == true),
    				Processed = 0,
    				Meetingid = id
    			};
    			DbUtil.Db.DeleteMeetingRuns.InsertOnSubmit(runningtotals);
    			DbUtil.Db.SubmitChanges();
    			var host = Util.Host;
    			System.Threading.Tasks.Task.Factory.StartNew(() =>
    			{
    				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
    				var db = new CMSDataContext(Util.GetConnectionString(host));
    			    var cul = db.Setting("Culture", "en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

    				var q = from a in db.Attends
    						where a.MeetingId == id
    						where a.AttendanceFlag|| a.EffAttendFlag == true
    						select a.PeopleId;
    				var list = q.ToList();
    			    DeleteMeetingRun r = null;
    			    foreach (var pid in list)
    				{
    					db.Dispose();
    					db = new CMSDataContext(Util.GetConnectionString(host));
    					Attend.RecordAttendance(db, pid, id, false);
    				    r = db.DeleteMeetingRuns.SingleOrDefault(m => m.Meetingid == id);
    				    Debug.Assert(r != null, "r != null");
    				    r.Processed++;
    			        db.SubmitChanges();
    				}
    			    r = db.DeleteMeetingRuns.SingleOrDefault(m => m.Meetingid == id);
				    Debug.Assert(r != null, "r != null");
    			    r.Processed--;
    	            db.SubmitChanges();
    				db.ExecuteCommand(@"
delete dbo.SubRequest 
WHERE EXISTS(
    SELECT NULL FROM Attend a 
    WHERE a.AttendId = AttendId 
    AND a.MeetingId = {0}
)", id);
    				db.ExecuteCommand("DELETE dbo.VolRequest where MeetingId = {0}", id);
    				db.ExecuteCommand("delete attend where MeetingId = {0}", id);
    				db.ExecuteCommand("delete MeetingExtra where MeetingId = {0}", id);
    				db.ExecuteCommand("delete meetings where MeetingId = {0}", id);

			        r = db.DeleteMeetingRuns.Single(m => m.Meetingid == id);
    				r.Processed++;
    				r.Completed = DateTime.Now;
    	            db.SubmitChanges();
    			});
            }
            // Display Progress here
            rr = DbUtil.Db.DeleteMeetingRuns.SingleOrDefault(m => m.Meetingid == id);
			return View(rr);
		}
    }
}
