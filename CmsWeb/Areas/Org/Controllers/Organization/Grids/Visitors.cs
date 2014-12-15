using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult VisitorGrid(OrgPeopleModel m)
        {
            DbUtil.Db.CopyPropertiesFrom(m);
            m.GroupSelect = GroupSelectCode.Guest;
            DbUtil.LogActivity("Viewing Visitors for {0}".Fmt(Session["ActiveOrganization"]));
            ViewBag.orgname = Session["ActiveOrganization"] + " - Guests";
            return PartialView("Tabs/Visitors", m);
        }
        [HttpPost, Route("ShowVisitor/{oid:int}/{pid:int}/{ticks:long}/{show}")]
        public ActionResult ShowVisitor(int oid, int pid, long ticks, string show)
        {
			var dt = new DateTime(ticks); // ticks here is meeting time
            var attend = DbUtil.Db.Attends.SingleOrDefault(aa => aa.PeopleId == pid && aa.OrganizationId == oid && aa.MeetingDate == dt);
            if(attend == null)
                return Content("attendance not found");
            attend.NoShow = show.Equal("hide");
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("ShowVisitor {0},{1},{2},{3}".Fmt(oid, pid, attend.AttendId, show));
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
                    DbUtil.LogActivity("Same Hour Joining Org {0}({1})".Fmt(org.OrganizationName, pid));
                    return Content("Already a member of {0} at this hour".Fmt(om.OrganizationId));
                }
            }
            OrganizationMember.InsertOrgMembers(DbUtil.Db,
                oid, pid, MemberTypeCode.Member,
                DateTime.Now, null, false);
            DbUtil.Db.UpdateMainFellowship(oid);
            DbUtil.LogActivity("Joining Org {0}({1})".Fmt(org.OrganizationName, pid));
            return Content("ok");
        }
    }
}