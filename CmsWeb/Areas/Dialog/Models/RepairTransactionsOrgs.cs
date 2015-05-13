using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using CmsData;
using UtilityExtensions;
using Tasks = System.Threading.Tasks;

namespace CmsWeb.Areas.Dialog.Models
{
    public class RepairTransactionsOrgs : LongRunningOp
    {
        public const string Op = "RepairTransactionsOrgs";

        public RepairTransactionsOrgs() { }
        public RepairTransactionsOrgs(int id)
        {
            Id = id;
        }
        public string Orgs { get; set; }

        public void Process(CMSDataContext db)
        {
            var lop = new LongRunningOp
            {
                Started = DateTime.Now,
                Count = Orgs.Split(',').Length,
                Processed = 0,
                Id = Id,
                Operation = Op
            };
            db.LongRunningOps.InsertOnSubmit(lop);
            db.SubmitChanges();
            Tasks.Task.Run(() => DoWork(this));
		}
        public static void DoWork(RepairTransactionsOrgs model)
        {
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			var db = DbUtil.Create(model.host);
		    var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            LongRunningOp lop = null;
		    foreach (var oid in model.Orgs.Split(',').Select(mm => mm.ToInt()))
		    {
	            db.RepairTransactionsOrgs(oid);
                lop = FetchLongRunningOp(db, model.Id, Op);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
		    }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
		}
    }
}
