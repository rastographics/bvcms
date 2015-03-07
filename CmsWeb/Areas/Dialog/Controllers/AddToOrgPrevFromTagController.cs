using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Dialog.Controllers
{
	[Authorize(Roles = "Edit")]
    [RouteArea("Dialog", AreaPrefix= "AddToOrgPrevFromTag"), Route("{action}/{id?}")]
	public class AddToOrgPrevFromTagController : CmsController
	{
        [Route("~/AddToOrgPrevFromTag/{id:int}")]
		public ActionResult Index(int id)
		{
			ViewBag.tag = OrganizationModel.Tags();
			var o = DbUtil.Db.LoadOrganizationById(id);
			ViewBag.orgid = id;
			ViewBag.orgname = o.OrganizationName;
			return View();
		}
		[HttpPost]
		public ActionResult Start(int tag, int orgid)
		{
			var runningtotals = new AddToOrgFromTagRun
			{
				Started = DateTime.Now,
				Count = 0,
				Processed = 0,
				Orgid = orgid
			};
			DbUtil.Db.AddToOrgFromTagRuns.InsertOnSubmit(runningtotals);
			DbUtil.Db.SubmitChanges();
			var host = Util.Host;
		    var qid = DbUtil.Db.FetchLastQuery().Id;
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				var Db = new CMSDataContext(Util.GetConnectionString(host));
			    var cul = Db.Setting("Culture", "en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
				IEnumerable<int> q = null;
				if (tag == -1) // (last query)
					q = Db.PeopleQuery(qid).Select(pp => pp.PeopleId);
				else
					q = from t in Db.TagPeople
						where t.Id == tag
						select t.PeopleId;
				var pids = q.ToList();
				foreach (var pid in pids)
				{
					Db.Dispose();
					Db = new CMSDataContext(Util.GetConnectionString(host));
					OrganizationMember.InsertOrgMembers(Db,
						orgid, pid, MemberTypeCode.Member, DateTime.Now, null, false);
					var r = Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == orgid).OrderByDescending(mm => mm.Id).First();
					r.Processed++;
					r.Count = pids.Count;
					Db.SubmitChanges();
				}
				var rr = Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == orgid).OrderByDescending(mm => mm.Id).First();
				rr.Completed = DateTime.Now;
				Db.SubmitChanges();
				Db.UpdateMainFellowship(orgid);
			});
			return Redirect("/AddToOrgPrevFromTag/Progress/" + orgid);
		}
		[HttpPost]
		public JsonResult Progress2(int id)
		{
			var r = DbUtil.Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == id).OrderByDescending(mm => mm.Id).First();
			return Json(new { r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running });
		}
		[HttpGet]
		public ActionResult Progress(int id)
		{
			var o = DbUtil.Db.LoadOrganizationById(id);
			ViewBag.orgname = o.OrganizationName;
			ViewBag.orgid = id;
			var r = DbUtil.Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == id).OrderByDescending(mm => mm.Id).First();
			return View(r);
		}
	}
}
