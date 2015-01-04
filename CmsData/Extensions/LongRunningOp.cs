using UtilityExtensions;
using System.Linq;

namespace CmsData
{
    public partial class LongRunningOp
    {
        public void UpdateLongRunningOp(CMSDataContext db, string op)
        {
            var lop = FetchLongRunningOp(db, Id, op);
            if(lop != null)
                lop.CopyProperties2(this);
        }
        public static LongRunningOp FetchLongRunningOp(CMSDataContext db, int id, string op)
        {
            return db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == op);
        }
        public void RemoveExistingLop(CMSDataContext db, int id, string op)
        {
            var exlop = FetchLongRunningOp(db, id, op);
            if (exlop != null)
                db.LongRunningOps.DeleteOnSubmit(exlop);
            db.SubmitChanges();
        }
        public string host
        {
            get { return Util.Host; }
        }
        public bool Finished
        {
            get { return Completed.HasValue; }
        }

    }
}