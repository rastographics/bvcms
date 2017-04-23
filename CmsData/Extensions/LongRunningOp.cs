using System;
using UtilityExtensions;
using System.Linq;

namespace CmsData
{
    public partial class LongRunningOp
    {
        partial void OnCreated()
        {
            host = Util.Host;
        }
        public void UpdateLongRunningOp(CMSDataContext db, string op)
        {
            var lop = FetchLongRunningOp(db, Id, op, QueryId);
            lop?.CopyProperties2(this);
        }
        public static LongRunningOp FetchLongRunningOp(CMSDataContext db, int id, string op, Guid? queryid = null)
        {
            var lop = db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == op && m.QueryId == queryid);
            if(lop != null)
                lop.host = db.Host;
            return lop;
        }
        public void RemoveExistingLop(CMSDataContext db, int id, string op, Guid? queryid = null)
        {
            var exlop = FetchLongRunningOp(db, id, op, queryid);
            if (exlop != null)
                db.LongRunningOps.DeleteOnSubmit(exlop);
            db.SubmitChanges();
        }
        public string host { get; private set; }

        public bool Finished
        {
            get { return Completed.HasValue; }
        }

    }
}