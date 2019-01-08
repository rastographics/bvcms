using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Coordinator.Models;
using CmsWeb.Areas.Coordinator.Services;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Areas.Coordinator.Controllers
{
    public partial class CheckinCoordinatorController : Controller
    {
        private readonly CMSDataContext db;
        private CheckinCoordinatorService _checkinCoordinator;
        private CheckinCoordinatorService CheckinCoordinator => _checkinCoordinator ?? (_checkinCoordinator = new CheckinCoordinatorService(GetDailySchedules(), db));

        public CheckinCoordinatorController()
        {
            this.db = DbUtil.Db;
        }

        private IEnumerable<CheckinScheduleDto> GetDailySchedules()
        {
            var dailySchedulesKey = $"{db.Host}.dailyschedules";
            List<CheckinScheduleDto> list = (List<CheckinScheduleDto>)HttpContext.Cache.Get(dailySchedulesKey);
            if (list == null)
            {
                list = db.ExecuteQuery<CheckinScheduleDto>(@"
SELECT t0.Id AS OrgScheduleId, 
	t0.NextMeetingDate,
	MAX(t1.OrganizationId) OrganizationId, 
	MAX(t1.OrganizationName) OrganizationName, 
	t2.Id AS SubgroupId, 
	MAX(t2.Name) AS SubgroupName, 
	MAX(t2.CheckInCapacity) CheckInCapacity, 
	t2.CheckInOpen, 
	MAX(t2.CheckInCapacityDefault) CheckInCapacityDefault, 
	t2.CheckInOpenDefault, 
	MAX(t3.Id) AS DivisionId, 
	MAX(t3.Name) AS DivisionName, 
	MAX(t4.Id) AS ProgramId, 
	MAX(t4.Name) AS ProgramName
FROM dbo.MemberTags AS t2 
INNER JOIN dbo.OrgSchedule AS t0 ON t0.Id = t2.ScheduleId
INNER JOIN dbo.Organizations AS t1 ON t2.OrgId = t1.OrganizationId
INNER JOIN dbo.Division AS t3 ON t1.DivisionId = (t3.Id)
INNER JOIN dbo.Program AS t4 ON t3.ProgId = (t4.Id)
WHERE (t2.CheckIn = 1) AND ((COALESCE(t1.CanSelfCheckin, 0)) = 1) AND (t0.NextMeetingDate >= @p0) AND (t0.NextMeetingDate < @p1)
GROUP BY t2.Id, t2.CheckInOpen, t2.CheckInOpenDefault, t0.Id, t0.NextMeetingDate
ORDER BY t0.NextMeetingDate, OrganizationName, SubgroupName", DateTime.Now.Date, DateTime.Now.AddDays(7).Date).ToList();

                // TODO: add this to top query and get all in one pass?
                var attendeeQuery = (from a in db.Attends
                                     join at in db.AttendTypes on a.AttendanceTypeId.Value equals at.Id
                                     join p in db.People on a.PeopleId equals p.PeopleId
                                     where a.MeetingDate >= DateTime.Now.Date
                                        && a.MeetingDate < DateTime.Now.AddDays(7).Date
                                     select new CheckinAttendeeDto
                                     {
                                         MeetingDate = a.MeetingDate,
                                         OrganizationId = a.OrganizationId,
                                         SubGroupId = a.SubGroupID,
                                         SubGroupName = a.SubGroupName,
                                         IsWorker = at.Worker,
                                         Name = p.Name2,
                                         PeopleId = p.PeopleId
                                     });

                var Attendees = attendeeQuery.ToList();
                foreach (var schedule in list)
                {
                    schedule.Attendees = Attendees.Where(a => a.MeetingDate == schedule.NextMeetingDate
                                                        && a.OrganizationId == schedule.OrganizationId
                                                        && a.SubGroupId == schedule.SubgroupId
                                                        && a.SubGroupName == schedule.SubgroupName).ToList();
                }
                HttpContext.Cache.Add(dailySchedulesKey, list, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            return list;
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult TimeslotSelector()
        {
            var model = CheckinCoordinator.GetFilteredTimeslots();
            return PartialView(model);
        }

        public ActionResult ProgramSelector(string selectedTimeslot = "")
        {
            var model = CheckinCoordinator.GetFilteredPrograms(selectedTimeslot);
            return PartialView(model);
        }

        public ActionResult DivisionSelector(string selectedTimeslot = "", int programId = 0)
        {
            var model = CheckinCoordinator.GetFilteredDivisions(selectedTimeslot, programId);
            return PartialView(model);
        }

        public ActionResult OrganizationSelector(string selectedTimeslot = "", int programId = 0, int divisionId = 0, int highlightedOrg = 0, string searchQuery = null)
        {
            var model = CheckinCoordinator.GetFilteredSchedules(selectedTimeslot, programId, divisionId, searchQuery);
            ViewBag.HighlightedOrg = highlightedOrg;
            return PartialView(model);
        }

        public ActionResult Details(string selectedTimeslot, int organizationId, int subgroupId, string subgroupName)
        {
            var schedule = CheckinCoordinator.GetScheduleDetail(selectedTimeslot, organizationId, subgroupId, subgroupName);
            return PartialView("Details", schedule);
        }

        [HttpPost]
        public ActionResult ExecuteAction(CheckinActionDto checkinActionDto)
        {
            var schedule = CheckinCoordinator.GetScheduleDetail(checkinActionDto.SelectedTimeslot, checkinActionDto.OrganizationId, checkinActionDto.SubgroupId, checkinActionDto.SubgroupName);

            if (checkinActionDto.Service.Equals(CheckinActionDto.SetDefaults))
            {
                CheckinCoordinator.SetDefaults(schedule);
            }

            if (checkinActionDto.Service.Equals(CheckinActionDto.SetAllDefaults))
            {
                CheckinCoordinator.SetAllDefaults();
                return Json(new { status = "OK" });
            }

            if (checkinActionDto.Service.Equals(CheckinActionDto.IncrementCapacity))
            {
                CheckinCoordinator.IncrementCapacity(schedule);
            }

            if (checkinActionDto.Service.Equals(CheckinActionDto.DecrementCapacity))
            {
                CheckinCoordinator.DecrementCapacity(schedule);
            }

            if (checkinActionDto.Service.Equals(CheckinActionDto.ToggleCheckinOpen))
            {
                CheckinCoordinator.ToggleCheckinOpen(schedule);
            }

            return Details(checkinActionDto.SelectedTimeslot, checkinActionDto.OrganizationId, checkinActionDto.SubgroupId, checkinActionDto.SubgroupName);
        }

        [HttpPost]
        public ActionResult UpdateSmallGroup(int id, int curgrpid, int targrpid, string list)
        {
            string[] arr = list.Split(',');
            int[] selectedIds = Array.ConvertAll(arr, int.Parse);
            var m = new SubgroupModel(id);
            var a = selectedIds;

            //Add members to subgroup
            var tarsgname = db.MemberTags.Single(mt => mt.Id == targrpid).Name;
            var cursgname = db.MemberTags.Single(mt => mt.Id == curgrpid).Name;
            var q2 = from om in m.OrgMembers()
                     where om.OrgMemMemTags.All(mt => mt.MemberTag.Id == curgrpid)
                     where a.Contains(om.PeopleId)
                     select om;
            foreach (var om in q2)
            {
                om.AddToGroup(db, tarsgname);
                om.RemoveFromGroup(db, cursgname);
            }
            db.SubmitChanges();

            m.groupid = targrpid;
            m.ingroup = m.GetGroupDetails(targrpid).Name;

            return Json(m);
        }

        public ActionResult MoveSubgroupView(int id, int grpid, string list, string selectedTimeslot)
        {
            var m = new SubgroupModel(id);
            var details = m.GetGroupDetails(grpid);
            m.groupid = grpid;
            m.GroupName = details.Name;
            var date = DateTime.Parse(selectedTimeslot);
            m.TimeSlot = date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string[] arr = list.Split(',');
            int[] selectedIds = Array.ConvertAll(arr, int.Parse);
            m.SelectedPeopleIds = selectedIds;
            return PartialView(m);
        }
    }
}
