using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Coordinator.Models;
using CmsWeb.Areas.Coordinator.Services;
using UtilityExtensions;

namespace CmsWeb.Areas.Coordinator.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly CheckinCoordinator _checkinCoordinator;

        public CoordinatorController()
        {
            _checkinCoordinator = new CheckinCoordinator(GetDailySchedules());
        }

        private IEnumerable<CheckinScheduleDto> GetDailySchedules(int timeslotId = 0, int programId = 0, int divisionId = 0, int organizationId = 0)
        {
            var query = (from os in DbUtil.Db.OrgSchedules
                         join o in DbUtil.Db.Organizations on os.OrganizationId equals o.OrganizationId
                         join mt in DbUtil.Db.MemberTags on o.OrganizationId equals mt.OrgId
                         join d in DbUtil.Db.Divisions on o.DivisionId equals d.Id
                         join p in DbUtil.Db.Programs on d.ProgId equals p.Id
                         where mt.CheckIn
                         && (o.CanSelfCheckin ?? false)
                         && os.NextMeetingDate >= DateTime.Now.Date
                         && os.NextMeetingDate < DateTime.Now.AddDays(7).Date
                         && (os.Id == timeslotId || timeslotId == 0)
                         && (d.Id == divisionId || divisionId == 0)
                         && (p.Id == programId || programId == 0)
                         && (o.OrganizationId == organizationId || organizationId == 0)
                         select new CheckinScheduleDto
                         {
                             CheckInCapacity = mt.CheckInCapacity,
                             CheckInOpen = mt.CheckInOpen,
                             DivisionId = d.Id,
                             DivisionName = d.Name,
                             NextMeetingDate = os.NextMeetingDate,
                             OrganizationId = o.OrganizationId,
                             OrganizationName = o.OrganizationName,
                             OrgScheduleId = os.Id,
                             ProgramId = p.Id,
                             ProgramName = p.Name,
                             SubgroupId = mt.Id,
                             SubgroupName = mt.Name
                         });

            return query.ToList();
        }

        public ActionResult Dashboard()
        {            
            return View(_checkinCoordinator);
        }

        public ActionResult TimeslotSelector()
        {
            var model = _checkinCoordinator.GetFilteredTimeslots();
            return PartialView(model);
        }

        public ActionResult ProgramSelector(string selectedTimeslot = "")
        {
            var model = _checkinCoordinator.GetFilteredPrograms(selectedTimeslot);
            return PartialView(model);
        }

        public ActionResult DivisionSelector(string selectedTimeslot = "", int programId = 0)
        {
            var model = _checkinCoordinator.GetFilteredDivisions(selectedTimeslot, programId);
            return PartialView(model);
        }

        public ActionResult OrganizationSelector(string selectedTimeslot = "", int programId = 0, int divisionId = 0)
        {
            var model = _checkinCoordinator.GetFilteredSchedules(selectedTimeslot, programId, divisionId);
            return PartialView(model);
        }

        public ActionResult Details(int organizationId, int subgroupId, string subgroupName)
        {
            var test = new SubgroupModel(organizationId) { ingroup = subgroupName, groupid = subgroupId };
            return PartialView("Details", test);
        }


        public ActionResult MoveSubgroupView(int id, int grpid, string list)
        {
            var m = new SubgroupModel(id);
            m.groupid = grpid;
            string[] arr = list.Split(',');
            int[] selectedIds = Array.ConvertAll(arr, int.Parse);           
            m.SelectedPeopleIds = selectedIds;
            return View(m);
        }

        [HttpPost]
        public ActionResult UpdateCapacity(PostTargetInfo i)
        {
            var smgroup = (from e in DbUtil.Db.MemberTags
                           where e.OrgId == i.id
                           where e.Id == i.grpid
                           select e).SingleOrDefault();

            if (smgroup != null)
            {
                int oldcapacity = smgroup.CheckInCapacity;

                if (i.addremove == 1)
                {
                    smgroup.CheckInCapacity = oldcapacity + 1;
                }
                else
                {
                    smgroup.CheckInCapacity = smgroup.CheckInCapacity - 1;
                }
            }

            DbUtil.Db.SubmitChanges();
            var m = new SubgroupModel(i.id);
            m.groupid = i.grpid;
            m.ingroup = m.GetGroupDetails(i.grpid).Name;
            //return View("SubgroupDataView", m);
            return Details(i.id, m.groupid.Value, m.ingroup);
        }

        [HttpPost]
        public ActionResult UpdateCheckInOpen(PostTargetInfo i)
        {
            var item = DbUtil.Db.MemberTags.SingleOrDefault(mt => mt.OrgId == i.id && mt.Id == i.grpid);
            item.CheckInOpen = !item.CheckInOpen;

            DbUtil.Db.SubmitChanges();

            var m = new SubgroupModel(i.id);
            m.groupid = i.grpid;
            m.ingroup = m.GetGroupDetails(i.grpid).Name;

            return Details(i.id, m.groupid.Value, m.ingroup);
        }

        public ActionResult UpdateSmallGroup(int id, int curgrpid, int targrpid, string list)
        {
            string[] arr = list.Split(',');
            int[] selectedIds = Array.ConvertAll(arr, int.Parse);
            var m = new SubgroupModel(id);
            var a = selectedIds;

            //Add members to subgroup
            var tarsgname = DbUtil.Db.MemberTags.Single(mt => mt.Id == targrpid).Name;
            var cursgname = DbUtil.Db.MemberTags.Single(mt => mt.Id == curgrpid).Name;
            var q2 = from om in m.OrgMembers()
                where om.OrgMemMemTags.All(mt => mt.MemberTag.Id == curgrpid)
                where a.Contains(om.PeopleId)
                select om;
            foreach (var om in q2)
            {                
                om.AddToGroup(DbUtil.Db, tarsgname);
                om.RemoveFromGroup(DbUtil.Db, cursgname);
            }
            DbUtil.Db.SubmitChanges();

            m.groupid = targrpid;
            m.ingroup = m.GetGroupDetails(targrpid).Name;
            return RedirectToAction("SubgroupView", m);
        }
    }
}
