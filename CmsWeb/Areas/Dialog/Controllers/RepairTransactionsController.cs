using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
	[Authorize(Roles = "Edit")]
    [RouteArea("Dialog", AreaPrefix= "RepairTransactions"), Route("{action}/{id}")]
	public class RepairTransactionsController : CmsController
	{
        [Authorize(Roles="Admin")]
        [Route("~/RepairTransactions/{id:int}")]
        public ActionResult Index(int id)
        {
			var host = Util.Host;
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.BelowNormal;
				var Db = new CMSDataContext(Util.GetConnectionString(host));
	            Db.PopulateComputedEnrollmentTransactions(id);
			});
			return Redirect("/RepairTransactions/Progress/" + id);
		}
		[HttpPost]
		public JsonResult Progress2(int id)
		{
			var r = DbUtil.Db.RepairTransactionsRuns.Where(mm => mm.Orgid == id).OrderByDescending(mm => mm.Id).First();
			return Json(new { r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running });
		}
		[HttpGet]
		public ActionResult Progress(int id)
		{
			var o = DbUtil.Db.LoadOrganizationById(id);
			ViewBag.orgname = o.OrganizationName;
			ViewBag.orgid = id;
			var r = DbUtil.Db.RepairTransactionsRuns.Where(mm => mm.Orgid == id).OrderByDescending(mm => mm.Id).First();
			return View(r);
		}
	}
}
