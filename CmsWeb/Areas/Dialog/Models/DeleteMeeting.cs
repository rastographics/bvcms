using CmsData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;

namespace CmsWeb.Areas.Dialog.Models
{
    public class DeleteMeeting : LongRunningOperation
    {
        public const string Op = "deletemeeting";

        public int MeetingId { get; set; }
        public int OrgId { get; set; }
        public DeleteMeeting() { }
        public DeleteMeeting(int id)
        {
            QueryId = Guid.NewGuid();
            MeetingId = id;
            var mm = DbUtil.Db.Meetings.Single(m => m.MeetingId == id);
            OrgId = mm.OrganizationId;
            Count = mm.Attends.Count(a => a.AttendanceFlag || a.EffAttendFlag == true);
        }

        internal List<int> pids;

        public void Process(CMSDataContext db)
        {
            var q = from a in db.Attends
                    where a.MeetingId == MeetingId
                    where a.AttendanceFlag || a.EffAttendFlag == true
                    select a.PeopleId;
            pids = q.ToList();
            var lop = new LongRunningOperation()
            {
                Started = DateTime.Now,
                Count = pids.Count,
                Processed = 0,
                QueryId = QueryId,
                Operation = Op,
            };
            db.LongRunningOperations.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(DeleteMeeting model)
        {
            var db = DbUtil.Create(model.Host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var pid in model.pids)
            {
                //db.Dispose();
                //db = DbUtil.Create(model.Host);
                Attend.RecordAttendance(db, pid, model.MeetingId, false);
                lop = FetchLongRunningOperation(db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
            }
            db.ExecuteCommand(@"
DELETE dbo.SubRequest 
FROM dbo.SubRequest sr
JOIN dbo.Attend a ON a.AttendId = sr.AttendId
WHERE a.MeetingId = {0}
", model.MeetingId);
            db.ExecuteCommand("DELETE dbo.VolRequest WHERE MeetingId = {0}", model.MeetingId);
            db.ExecuteCommand("DELETE dbo.attend WHERE MeetingId = {0}", model.MeetingId);
            db.ExecuteCommand("DELETE dbo.MeetingExtra WHERE MeetingId = {0}", model.MeetingId);
            db.ExecuteCommand("DELETE dbo.meetings WHERE MeetingId = {0}", model.MeetingId);

            db.SubmitChanges();

            // finished
            lop = FetchLongRunningOperation(db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}
