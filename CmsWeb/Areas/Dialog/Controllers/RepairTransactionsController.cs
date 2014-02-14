using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Reports.Models;
using System.IO;
using CmsData;
using UtilityExtensions;
using CmsWeb.Models;
using System.Text;
using System.Web.UI;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CmsData.Codes;

namespace CmsWeb.Areas.Dialog.Controllers
{
	[Authorize(Roles = "Edit")]
	public class RepairTransactionsController : CmsController
	{
        [Authorize(Roles="Admin")]
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
