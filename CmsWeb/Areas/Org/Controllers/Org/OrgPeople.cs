using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController : CmsStaffController
    {
        [HttpPost]
        public ActionResult People(OrgPeopleModel m)
        {
            SetModelDependencies(m);
            if (m.FilterIndividuals)
            {
                if (m.NameFilter.HasValue())
                {
                    m.FilterIndividuals = false;
                }
                else if (CurrentDatabase.OrgFilterCheckedCount(m.QueryId) == 0)
                {
                    m.FilterIndividuals = false;
                }
            }

            ViewBag.OrgMemberContext = true;
            ViewBag.orgname = Session["ActiveOrganization"];
            return PartialView(m);
        }

        private void SetModelDependencies(OrgPeopleModel m)
        {
            m.Db = CurrentDatabase;
            m.User = User;
        }

        public ActionResult DialogAdd(int id, string type)
        {
            ViewBag.OrgID = id;
            return View("DialogAdd" + type);
        }

        public ActionResult ReGenPaylinks(int id)
        {
            var org = CurrentDatabase.LoadOrganizationById(id);
            var q = from om in org.OrganizationMembers
                    select om;

            foreach (var om in q)
            {
                if (!om.TranId.HasValue)
                {
                    continue;
                }

                var estr = HttpUtility.UrlEncode(Util.Encrypt(om.TranId.ToString()));
                var link = Util.ResolveUrl("/OnlineReg/PayAmtDue?q=" + estr);
                om.PayLink = link;
            }
            CurrentDatabase.SubmitChanges();
            return View("Other/ReGenPaylinks", org);
        }

        [HttpPost, Route("AddProspect/{oid:int}/{pid:int}")]
        public ActionResult AddProspect(int oid, int pid)
        {
            var org = CurrentDatabase.LoadOrganizationById(oid);
            OrganizationMember.InsertOrgMembers(CurrentDatabase,
                oid, pid, MemberTypeCode.Prospect,
                DateTime.Now, null, false);
            DbUtil.LogActivity($"Adding Prospect {org.OrganizationName}({pid})");
            return Content("ok");
        }

        [HttpPost, Route("ShowProspect/{oid:int}/{pid:int}/{show}")]
        public ActionResult ShowProspect(int oid, int pid, string show)
        {
            var om = CurrentDatabase.OrganizationMembers.SingleOrDefault(aa => aa.OrganizationId == oid && aa.PeopleId == pid);
            if (om == null)
            {
                return Content("member not found");
            }

            om.Hidden = show.Equal("hide");
            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity($"ShowProspect {oid},{pid},{show}");
            return Content("ok");
        }

        [HttpPost, Route("ShowVisitor/{oid:int}/{pid:int}/{ticks:long}/{show}")]
        public ActionResult ShowVisitor(int oid, int pid, long ticks, string show)
        {
            var dt = new DateTime(ticks); // ticks here is meeting time
            var attend = CurrentDatabase.Attends.SingleOrDefault(aa => aa.PeopleId == pid && aa.OrganizationId == oid && aa.MeetingDate == dt);
            if (attend == null)
            {
                return Content("attendance not found");
            }

            attend.NoShow = show.Equal("hide");
            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity($"ShowVisitor {oid},{pid},{attend.AttendId},{show}");
            return Content("ok");
        }

        [HttpPost, Route("Join/{oid:int}/{pid:int}")]
        public ActionResult Join(int oid, int pid)
        {
            var org = CurrentDatabase.LoadOrganizationById(oid);
            if (org.AllowAttendOverlap != true)
            {
                var om = CurrentDatabase.OrganizationMembers.FirstOrDefault(mm =>
                    mm.OrganizationId != oid
                    && mm.MemberTypeId != 230 // inactive
                    && mm.MemberTypeId != 500 // inservice
                    && mm.Organization.AllowAttendOverlap != true
                    && mm.PeopleId == pid
                    && mm.Organization.OrgSchedules.Any(ss =>
                        CurrentDatabase.OrgSchedules.Any(os =>
                            os.OrganizationId == oid
                            && os.ScheduleId == ss.ScheduleId)));
                if (om != null)
                {
                    DbUtil.LogActivity($"Same Hour Joining Org {org.OrganizationName}({pid})");
                    return Content($"Already a member of {om.OrganizationId} at this hour");
                }
            }
            OrganizationMember.InsertOrgMembers(CurrentDatabase,
                oid, pid, MemberTypeCode.Member,
                DateTime.Now, null, false);
            CurrentDatabase.UpdateMainFellowship(oid);
            DbUtil.LogActivity($"Joining Org {org.OrganizationName}({pid})");
            return Content("ok");
        }

        [HttpPost, Route("ToggleCheckbox/{qid}/{pid:int}")]
        public ActionResult ToggleCheckbox(Guid qid, int pid, bool chkd)
        {
            if (chkd)
            {
                Person.Tag(CurrentDatabase, pid, qid.ToString(), Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
            }
            else
            {
                Person.UnTag(CurrentDatabase, pid, qid.ToString(), Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
            }

            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        [HttpPost, Route("ToggleCheckboxes/{qid}")]
        public ActionResult ToggleCheckboxes(Guid qid, IList<int> pids, bool chkd)
        {
            foreach (var pid in pids)
            {
                if (chkd)
                {
                    Person.Tag(CurrentDatabase, pid, qid.ToString(), Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
                }
                else
                {
                    Person.UnTag(CurrentDatabase, pid, qid.ToString(), Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers);
                }
            }

            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult CheckAll(OrgPeopleModel m)
        {
            SetModelDependencies(m);
            var list = m.CurrentNotChecked();
            CurrentDatabase.TagAll(list, m.OrgTag);
            return PartialView("People", m);
        }

        [HttpPost]
        public ActionResult CheckNone(OrgPeopleModel m)
        {
            SetModelDependencies(m);
            var list = m.CurrentChecked();
            CurrentDatabase.UnTagAll(list, m.OrgTag);
            return PartialView("People", m);
        }
    }
}
