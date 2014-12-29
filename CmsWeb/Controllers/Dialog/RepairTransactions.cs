using System;
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
        public ActionResult RepairTransactions(int id)
        {
            const string repairtransactions = "RepairTransactions";
            if (Request.HttpMethod == "GET")
            {
    			var r = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == repairtransactions );
                if (r != null) 
                    DbUtil.Db.LongRunningOps.DeleteOnSubmit(r);
                DbUtil.Db.SubmitChanges();
                var count = DbUtil.Db.OrganizationMembers.Count(m => m.OrganizationId == id);
                return View(new LongRunningOp { Id = id, Count = count });
            }
			var rr = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id );
            if (rr == null) 
            {
                // start delete process
    			DbUtil.LogActivity("Repair Transactions for {0}".Fmt(Session["ActiveOrganization"]));
                var mm = DbUtil.Db.Organizations.Single(m => m.OrganizationId == id);
    			var runningtotals = new LongRunningOp
    			{
    				Started = DateTime.Now,
    				Count = mm.OrganizationMembers.Count(),
    				Processed = 0,
    				Id = id,
                    Operation = repairtransactions
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
    	            db.RepairTransactions(id);
    			});
            }
            // Display Progress here
            rr = DbUtil.Db.LongRunningOps.Single(mm => mm.Id == id && mm.Operation == repairtransactions);
			return View(rr);
		}
    }
}
