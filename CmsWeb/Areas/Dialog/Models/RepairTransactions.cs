using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CmsData;
using UtilityExtensions;
using Tasks = System.Threading.Tasks;

namespace CmsWeb.Models
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

            Util.Log("host1 " + host);
            var h = host;
            Tasks.Task.Run(() => DoWork(h, Id));
        }

        public static void DoWork(string host, int id)
        {
            Util.Log("host2 " + host);
			var cs = Util.GetConnectionString(host);
            Util.Log(cs);
			var db = new CMSDataContext(cs);
            db.RepairTransactions(id);
            Util.Log("done");
            // finished
            var lop = FetchLongRunningOp(db, id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
		}
    }
}
