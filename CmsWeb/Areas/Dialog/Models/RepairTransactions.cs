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
            Tasks.Task.Run(() => DoWork(this));
        }

        public static void DoWork(RepairTransactions model)
        {
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			var db = new CMSDataContext(Util.GetConnectionString(model.host));
		    var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            db.RepairTransactions(model.Id);
            // finished
            var lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
		}
    }
}
