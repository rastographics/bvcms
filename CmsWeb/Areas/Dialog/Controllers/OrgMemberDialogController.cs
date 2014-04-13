using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using CmsWeb.Models.OrganizationPage;
using CmsData.Codes;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix= "OrgMemberDialog")]
    public class OrgMemberDialogController : CmsStaffController
    {
        [Route("~/OrgMemberDialog/{from}/{id:int}/{pid:int}")]
        [Route("~/OrgMemberDialog/{id:int}/{pid:int}")]
        public ActionResult Index(int id, int pid, string from = "na", string page = null)
        {
            var m = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == id && om.PeopleId == pid);
            ViewData["from"] = from;
            if (m == null)
                return Content("cannot find membership: id={0} pid={1}".Fmt(id, pid));
            return View(m);

        }
        [HttpPost, Route("CheckBoxChanged/{id:int}/{pid:int}/{sgid:int}")]
        public ActionResult CheckBoxChanged(int id, int pid, int sgid, bool ck)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == id);
            if (om == null)
                return Content("error");
            if (ck)
                om.OrgMemMemTags.Add(new OrgMemMemTag { MemberTagId = sgid });
            else
            {
                var mt = om.OrgMemMemTags.SingleOrDefault(t => t.MemberTagId == sgid);
				if (mt == null)
					return Content("not found");
                DbUtil.Db.OrgMemMemTags.DeleteOnSubmit(mt);
            }
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
        
        [HttpPost, Route("Edit/{id:int}/{pid:int}")]
        public ActionResult Edit(int id, int pid)
        {
            ViewData["MemberTypes"] = CodeValueModel.ConvertToSelect(CodeValueModel.MemberTypeCodes(), "Id");
            var om = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == id);
            return View(om);
        }
        [HttpPost, Route("Display/{id:int}/{pid:int}")]
        public ActionResult Display(int id, int pid)
        {
            var om = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == id);
            return View(om);
        }
        [HttpPost, Route("Update/{id:int}/{pid:int}")]
        public ActionResult Update(int id, int pid)
        {
            var om = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == id);
            try
            {
                UpdateModel(om);
                om.ShirtSize = om.ShirtSize.MaxString(20);
                var rr = om.Person.SetRecReg();
                if(om.ShirtSize.HasValue())
                    rr.ShirtSize = om.ShirtSize;
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception)
            {
                ViewData["MemberTypes"] = CodeValueModel.ConvertToSelect(CodeValueModel.MemberTypeCodes(), "Id");
                return View("Edit", om);
            }
            return View("Display", om);
        }
        [HttpPost, Route("Drop/{id:int}/{pid:int}")]
        public ActionResult Drop(int id, int pid)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == id);
            if (om != null)
            {
                om.Drop(DbUtil.Db, addToHistory:true);
                DbUtil.Db.SubmitChanges();
            }
            return Content("dropped");
        }
        [HttpPost, Route("Move/{id:int}/{pid:int}")]
        public ActionResult Move(int id, int pid)
        {
            var om = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == id);
            ViewData["name"] = om.Person.Name;
            ViewData["oid"] = id;
            ViewData["pid"] = pid;
            if (om.Organization.DivisionId == null)
                return View((IEnumerable<OrgMove>)null);
			var divorgs = om.Organization.DivOrgs.Select(mm => mm.DivId).ToList();
            var q = from o in DbUtil.Db.Organizations
                    where o.DivOrgs.Any(dd => divorgs.Contains(dd.DivId))
                    where o.OrganizationId != id
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    orderby o.OrganizationName
                    select new OrgMove
                    {
                         OrgName = o.OrganizationName,
                         toorg = o.OrganizationId,
                         pid = pid,
                         frorg = id,
                         Program = o.Division.Program.Name,
                         Division = o.Division.Name,
                         orgSchedule = o.OrgSchedules.First()
                    };
            return View(q.ToList());
        }
        [HttpPost, Route("MoveSelect/{frorg:int}/{toorg:int}/{pid:int}")]
        public ActionResult MoveSelect(int frorg, int toorg, int pid)
        {
            var om1 = DbUtil.Db.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == frorg);
            var om2 = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                toorg, om1.PeopleId, om1.MemberTypeId, DateTime.Now, om1.InactiveDate, om1.Pending ?? false);
			DbUtil.Db.UpdateMainFellowship(om2.OrganizationId);
			om2.EnrollmentDate = om1.EnrollmentDate;
			if (om2.EnrollmentDate.Value.Date == DateTime.Today)
				om2.EnrollmentDate = DateTime.Today; // force it to be midnight, so you can check them in.
            om2.Request = om1.Request;
            om2.Amount = om1.Amount;
            om2.UserData = om1.UserData;
            om1.Drop(DbUtil.Db, addToHistory:true);
            DbUtil.Db.SubmitChanges();
            return Content("moved");
        }
        public class OrgMove
        {
            public string OrgName { get; set; }
            public int pid { get; set; }
            public int frorg { get; set; }
            public int toorg { get; set; }
            public string Program { get; set; }
            public string Division { get; set; }
            public OrgSchedule orgSchedule { get; set; }
            public string Tip
            {
                get
                {
                    var si = new ScheduleInfo(orgSchedule);
                    return "{0} ({1})|Program:{2}|Division: {3}|Schedule: {4}".Fmt(OrgName, toorg, Program, Division, si.Display);
                }
            }
        }
        public string HelpLink()
        {
            return "";
        }

    }
}
