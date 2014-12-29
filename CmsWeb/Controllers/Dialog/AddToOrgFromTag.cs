using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class DialogController
    {
        public class TagInfo
        {
            [DisplayName("Choose A Tag")]
            public CodeInfo Tag { get; set; }
            public string Name { get; set; }
        }
        [Route("~/Dialog/AddToOrgFromTag/{group:int}/{id:int}")]
        public ActionResult AddToOrgFromTag(int group, int id)
        {
            const string addtoorgfromtag = "addtoorgfromtag";
            if (Request.HttpMethod == "GET")
            {
    			var r = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag );
                if (r != null) 
                    DbUtil.Db.LongRunningOps.DeleteOnSubmit(r);
                DbUtil.Db.SubmitChanges();
                return View(new LongRunningOp { Id = id });
            }
			var rr = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag );
            if (rr == null) 
            {
                // start delete process
    			DbUtil.LogActivity("Delete meeting for {0}".Fmt(Session["ActiveOrganization"]));
                var mm = DbUtil.Db.Meetings.Single(m => m.MeetingId == id);
    			var runningtotals = new LongRunningOp
    			{
    				Started = DateTime.Now,
    				Count = mm.Attends.Count(a => a.AttendanceFlag || a.EffAttendFlag == true),
    				Processed = 0,
    				Id = id,
                    Operation = addtoorgfromtag
    			};
    			DbUtil.Db.LongRunningOps.InsertOnSubmit(runningtotals);
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
    			    LongRunningOp r = null;
    			    foreach (var pid in list)
    				{
    					db.Dispose();
    					db = new CMSDataContext(Util.GetConnectionString(host));
    					Attend.RecordAttendance(db, pid, id, false);
    				    r = db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag);
    				    Debug.Assert(r != null, "r != null");
    				    r.Processed++;
    			        db.SubmitChanges();
    				}
    			    r = db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag);
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

			        r = db.LongRunningOps.Single(m => m.Id == id && m.Operation == addtoorgfromtag);
    				r.Processed++;
    				r.Completed = DateTime.Now;
    	            db.SubmitChanges();
    			});
            }
            // Display Progress here
            rr = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == addtoorgfromtag);
			return View(rr);
		}
    }
}
