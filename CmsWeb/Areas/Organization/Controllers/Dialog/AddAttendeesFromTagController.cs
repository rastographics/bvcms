using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using CmsData.Codes;
using CmsWeb.Code;

namespace CmsWeb.Areas.Org2.Dialog.Controllers
{
    // todo: use bootstrap, longrunop 
	[Authorize(Roles = "Edit")]
    [RouteArea("Organization", AreaPrefix= "AddAttendeesFromTag"), Route("{action}/{id:int}")]
	public class AddAttendeesFromTagController : CmsController
	{
        [Route("~/AddAttendeesFromTag/{id:int}")]
		public ActionResult Index(int id)
		{
			ViewBag.tag = CodeValueModel.Tags();
			ViewBag.meetingid = id;
			return View();
		}
		[HttpPost]
        [Route("~/AddAttendeesFromTag/Start")]
		public ActionResult Start(int tag, int meetingid, bool addasmembers)
		{
			var runningtotals = new AddToOrgFromTagRun
			{
				Started = DateTime.Now,
				Count = 0,
				Processed = 0,
				Orgid = meetingid
			};
			DbUtil.Db.AddToOrgFromTagRuns.InsertOnSubmit(runningtotals);
			DbUtil.Db.SubmitChanges();
			var host = Util.Host;
		    var qid = DbUtil.Db.FetchLastQuery().Id;
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.BelowNormal;
				var Db = new CMSDataContext(Util.GetConnectionString(host));
				IEnumerable<int> q = null;
				if (tag == -1) // (last query)
					q = Db.PeopleQuery(qid).Select(pp => pp.PeopleId);
				else
					q = from t in Db.TagPeople
						where t.Id == tag
						select t.PeopleId;
				var pids = q.ToList();
				var meeting = Db.Meetings.SingleOrDefault(mm => mm.MeetingId == meetingid);
				var joindate = meeting.MeetingDate.Value.AddMinutes(-1);
				var orgid = meeting.OrganizationId;
				foreach (var pid in pids)
				{
					Db.Dispose();
					Db = new CMSDataContext(Util.GetConnectionString(host));
					if (addasmembers)
						OrganizationMember.InsertOrgMembers(Db,
							orgid, pid, MemberTypeCode.Member, joindate, null, false);
					Db.RecordAttendance(meetingid, pid, true);
					var r = Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == meetingid).OrderByDescending(mm => mm.Id).First();
					r.Processed++;
					r.Count = pids.Count;
					Db.SubmitChanges();
				}
				var rr = Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == meetingid).OrderByDescending(mm => mm.Id).First();
				rr.Completed = DateTime.Now;
				Db.SubmitChanges();
				Db.UpdateMainFellowship(orgid);
			});
			return Redirect("/AddAttendeesFromTag/Progress/" + meetingid);
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
			ViewBag.meetingid = id;
			var r = DbUtil.Db.AddToOrgFromTagRuns.Where(mm => mm.Orgid == id).OrderByDescending(mm => mm.Id).First();
			return View(r);
		}
	}
}
