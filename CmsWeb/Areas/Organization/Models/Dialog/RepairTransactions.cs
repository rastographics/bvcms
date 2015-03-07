using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CmsData;
using UtilityExtensions;
using Tasks = System.Threading.Tasks;

namespace Areas.Org.Dialog.Models
{
    public class RepairTransactions : LongRunningOp
    {
        public const string Op = "RepairTransactions";

        public RepairTransactions() { }
        public RepairTransactions(int id)
        {
            Id = id;
            Count = DbUtil.Db.OrganizationMembers.Count(m => m.OrganizationId == Id);
        }

        public void Process(CMSDataContext db)
        {
            var lop = new LongRunningOp
            {
                Started = DateTime.Now,
                Count = db.OrganizationMembers.Count(m => m.OrganizationId == Id),
                Processed = 0,
                Id = Id,
                Operation = Op
            };
            db.LongRunningOps.InsertOnSubmit(lop);
            db.SubmitChanges();

            Tasks.Task.Run(() => DoWork(host, Id));
        }

        public static void DoWork(string host, int id)
        {
            var db = new CMSDataContext(Util.GetConnectionString(host));
            db.RepairTransactions(id);
            // finished
            var lop = FetchLongRunningOp(db, id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
		}
    }
}
