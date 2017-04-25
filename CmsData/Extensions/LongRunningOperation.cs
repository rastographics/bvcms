using System;
using UtilityExtensions;
using System.Linq;

namespace CmsData
{
    public partial class LongRunningOperation
    {
        partial void OnCreated()
        {
            Host = Util.Host;
        }

        public void UpdateLongRunningOp(CMSDataContext db, string op)
        {
            var lop = FetchLongRunningOperation(db, op, QueryId);
            lop?.CopyProperties2(this);
        }
        public static LongRunningOperation FetchLongRunningOperation(CMSDataContext db, string op, Guid queryid)
        {
            var lop = db.LongRunningOperations.SingleOrDefault(m => m.Operation == op && m.QueryId == queryid);
            if(lop != null)
                lop.Host = db.Host;
            return lop;
        }
        public void RemoveExistingLop(CMSDataContext db, string op, Guid queryid)
        {
            var exlop = FetchLongRunningOperation(db, op, queryid);
            if (exlop != null)
                db.LongRunningOperations.DeleteOnSubmit(exlop);
            db.SubmitChanges();
        }
        public string Host { get; private set; }

        public bool Finished => Completed.HasValue;

//        partial void OnQueryIdChanged()
//        {
//            QueryIdChanged();
//        }
//
//        public virtual void QueryIdChanged()
//        {
//            
//        }
    }
}