using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult People(OrgPeopleModel m)
        {
            if (m.FilterIndividuals)
                if (m.NameFilter.HasValue())
                    m.FilterIndividuals = false;
                else if (DbUtil.Db.OrgCheckedCount(m.Id, m.GroupSelect, Util.UserPeopleId) == 0)
                    m.FilterIndividuals = false;
            if (DbUtil.Db.CurrentOrg == null)
                DbUtil.Db.SetCurrentOrgId(m.Id);
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            ViewBag.OrgMemberContext = true;
            ViewBag.orgname = Session["ActiveOrganization"];
            return PartialView(m);
        }

        public ActionResult DialogAdd(int id, string type)
        {
            ViewBag.OrgID = id;
            return View("DialogAdd" + type);
        }

        public ActionResult ReGenPaylinks(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var q = from om in org.OrganizationMembers
                    select om;

            foreach (var om in q)
            {
                if (!om.TranId.HasValue) continue;
                var estr = HttpUtility.UrlEncode(Util.Encrypt(om.TranId.ToString()));
                var link = Util.ResolveUrl("/OnlineReg/PayAmtDue?q=" + estr);
                om.PayLink = link;
            }
            DbUtil.Db.SubmitChanges();
            return View("Other/ReGenPaylinks", org);
        }

        [HttpPost, Route("AddProspect/{oid:int}/{pid:int}")]
        public ActionResult AddProspect(int oid, int pid)
        {
            var org = DbUtil.Db.LoadOrganizationById(oid);
            OrganizationMember.InsertOrgMembers(DbUtil.Db,
                oid, pid, MemberTypeCode.Prospect,
                DateTime.Now, null, false);
            DbUtil.LogActivity($"Adding Prospect {org.OrganizationName}({pid})");
            return Content("ok");
        }

        [HttpPost, Route("ShowProspect/{oid:int}/{pid:int}/{show}")]
        public ActionResult ShowProspect(int oid, int pid, string show)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(aa => aa.OrganizationId == oid && aa.PeopleId == pid);
            if (om == null)
                return Content("member not found");
            om.Hidden = show.Equal("hide");
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"ShowProspect {oid},{pid},{show}");
            return Content("ok");
        }

        [HttpPost, Route("ShowVisitor/{oid:int}/{pid:int}/{ticks:long}/{show}")]
        public ActionResult ShowVisitor(int oid, int pid, long ticks, string show)
        {
            var dt = new DateTime(ticks); // ticks here is meeting time
            var attend = DbUtil.Db.Attends.SingleOrDefault(aa => aa.PeopleId == pid && aa.OrganizationId == oid && aa.MeetingDate == dt);
            if (attend == null)
                return Content("attendance not found");
            attend.NoShow = show.Equal("hide");
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"ShowVisitor {oid},{pid},{attend.AttendId},{show}");
            return Content("ok");
        }

        [HttpPost, Route("Join/{oid:int}/{pid:int}")]
        public ActionResult Join(int oid, int pid)
        {
            var org = DbUtil.Db.LoadOrganizationById(oid);
            if (org.AllowAttendOverlap != true)
            {
                var om = DbUtil.Db.OrganizationMembers.FirstOrDefault(mm =>
                    mm.OrganizationId != oid
                    && mm.MemberTypeId != 230 // inactive
                    && mm.MemberTypeId != 500 // inservice
                    && mm.Organization.AllowAttendOverlap != true
                    && mm.PeopleId == pid
                    && mm.Organization.OrgSchedules.Any(ss =>
                        DbUtil.Db.OrgSchedules.Any(os =>
                            os.OrganizationId == oid
                            && os.ScheduleId == ss.ScheduleId)));
                if (om != null)
                {
                    DbUtil.LogActivity($"Same Hour Joining Org {org.OrganizationName}({pid})");
                    return Content($"Already a member of {om.OrganizationId} at this hour");
                }
            }
            OrganizationMember.InsertOrgMembers(DbUtil.Db,
                oid, pid, MemberTypeCode.Member,
                DateTime.Now, null, false);
            DbUtil.Db.UpdateMainFellowship(oid);
            DbUtil.LogActivity($"Joining Org {org.OrganizationName}({pid})");
            return Content("ok");
        }

        [HttpPost, Route("ToggleCheckbox/{oid:int}/{pid:int}")]
        public ActionResult ToggleCheckbox(int oid, int pid, bool chkd)
        {
            if (chkd)
                Person.Tag(DbUtil.Db, pid, "Org-" + oid, Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
            else
                Person.UnTag(DbUtil.Db, pid, "Org-" + oid, Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }

        [HttpPost, Route("ToggleCheckboxes/{id:int}")]
        public ActionResult ToggleCheckboxes(int id, IList<int> pids, bool chkd)
        {
            foreach (var pid in pids)
                if (chkd)
                    Person.Tag(DbUtil.Db, pid, "Org-" + id, Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
                else
                    Person.UnTag(DbUtil.Db, pid, "Org-" + id, Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult CheckAll(OrgPeopleModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            var list = m.CurrentNotChecked();
            DbUtil.Db.TagAll(list, m.OrgTag);
            return PartialView("People", m);
        }

        [HttpPost]
        public ActionResult CheckNone(OrgPeopleModel m)
        {
            DbUtil.Db.CurrentOrg.CopyPropertiesFrom(m);
            var list = m.CurrentChecked();
            DbUtil.Db.UnTagAll(list, m.OrgTag);
            return PartialView("People", m);
        }
    }
}
