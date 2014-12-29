using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class DialogController
    {
        public ActionResult RepairTransactionsOrgs(int id, OrgSearchModel osm, string postdata)
        {
            const string repairtransactions = "RepairTransactionsOrgs";
            if (Request.HttpMethod == "GET" && osm != null)
            {
    			var r = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == repairtransactions );
                if (r != null) 
                    DbUtil.Db.LongRunningOps.DeleteOnSubmit(r);
                DbUtil.Db.SubmitChanges();
                var orgIds = osm.FetchOrgs().Select(oo => oo.OrganizationId).ToArray();
                var oids = string.Join(",", orgIds);
                ViewBag.Orgs = oids;
                return View(new LongRunningOp { Id = id, Count = orgIds.Length });
            }
			var rr = DbUtil.Db.LongRunningOps.SingleOrDefault(m => m.Id == id );
            if (rr == null && postdata.HasValue())  
            {
                // start delete process
                var orgIds = postdata.Split(',').Select(m => m.ToInt()).ToArray();
    			var runningtotals = new LongRunningOp
    			{
    				Started = DateTime.Now,
    				Count = orgIds.Length,
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
    			    LongRunningOp r = null;
    			    foreach (var oid in orgIds)
    			    {
        	            db.RepairTransactionsOrgs(oid);
    				    r = db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == repairtransactions);
    				    Debug.Assert(r != null, "r != null");
    				    r.Processed++;
    			        db.SubmitChanges();
    			    }
    			    r = db.LongRunningOps.SingleOrDefault(m => m.Id == id && m.Operation == repairtransactions);
				    Debug.Assert(r != null, "r != null");
    				r.Completed = DateTime.Now;
    	            db.SubmitChanges();
    			});
            }
            // Display Progress here
            rr = DbUtil.Db.LongRunningOps.Single(mm => mm.Id == id && mm.Operation == repairtransactions);
			return View(rr);
		}
    }
}
