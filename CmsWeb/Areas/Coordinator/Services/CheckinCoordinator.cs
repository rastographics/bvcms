using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Areas.Coordinator.Controllers;
using CmsWeb.Areas.Coordinator.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Coordinator.Services
{
    public class CheckinCoordinatorService
    {
        public IEnumerable<CheckinScheduleDto> Schedules;
        private CMSDataContext db;

        public CheckinCoordinatorService(IEnumerable<CheckinScheduleDto> scheduleCollection, CMSDataContext dbContext)
        {
            Schedules = scheduleCollection;
            db = dbContext; 
        }

        #region Schedule queries
        public IEnumerable<CheckinTimeslotDto> GetFilteredTimeslots()
        {
            return Schedules
                .DistinctBy(s => s.NextMeetingDate.Value)
                .Select(s => new CheckinTimeslotDto
                {
                    NextMeetingDate = s.NextMeetingDate.Value
                })
                .OrderBy(s => s.NextMeetingDate)
                .ToList();
        }

        public IEnumerable<CheckinProgramDto> GetFilteredPrograms(string selectedTimeslot = "")
        {
            var emptyTime = string.IsNullOrWhiteSpace(selectedTimeslot);
            DateTime date = ConvertToDate(selectedTimeslot);

            return Schedules
                .Where(s => (emptyTime || s.NextMeetingDate == date))
                .DistinctBy(s => s.ProgramId)
                .Select(s => new CheckinProgramDto
                {
                    ProgramId = s.ProgramId,
                    ProgramName = s.ProgramName
                })
                .Distinct()
                .ToList();
        }

        public IEnumerable<CheckinDivisionDto> GetFilteredDivisions(string selectedTimeslot = "", int programId = 0)
        {
            var emptyTime = string.IsNullOrWhiteSpace(selectedTimeslot);
            var date = ConvertToDate(selectedTimeslot);
            return Schedules
                .Where(s => (emptyTime || s.NextMeetingDate == date) && (programId == 0 || s.ProgramId == programId))
                .DistinctBy(s => s.DivisionId)
                .Select(s => new CheckinDivisionDto
                {
                    DivisionId = s.DivisionId,
                    DivisionName = s.DivisionName
                })
                .ToList();
        }

        public IEnumerable<CheckinScheduleDto> GetFilteredSchedules(string selectedTimeslot = "", int programId = 0, int divisionId = 0, string searchQuery = null)
        {
            var emptyTime = string.IsNullOrWhiteSpace(selectedTimeslot);
            var date = ConvertToDate(selectedTimeslot);

            var schedules = Schedules
                .Where(s => (emptyTime || s.NextMeetingDate == date) && (programId == 0 || s.ProgramId == programId) && (divisionId == 0 || s.DivisionId == divisionId));
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                schedules = schedules.Where(s => s.OrganizationName.ToLower().Contains(searchQuery.ToLower()) || s.SubgroupName.ToLower().Contains(searchQuery.ToLower()));
            }
            return schedules.ToList();
        }

        public CheckinScheduleDto GetScheduleDetail(string selectedTimeslot, int organizationId, int subgroupId, string subgroupName)
        {
            var date = ConvertToDate(selectedTimeslot);

            return Schedules.SingleOrDefault(s => s.NextMeetingDate == date && s.OrganizationId == organizationId && s.SubgroupId == subgroupId && s.SubgroupName == subgroupName);
        }

        private static DateTime ConvertToDate(string selectedTimeslot)
        {
            DateTime date;

            if (!string.IsNullOrWhiteSpace(selectedTimeslot))
            {
                date = DateTime.Parse(selectedTimeslot);
            }
            else
            {
                date = DateTime.MinValue;
            }

            return date;
        }
        #endregion

        #region Schedule checkin methods

        public void SetDefaults(CheckinScheduleDto checkinScheduleDto)
        {
            checkinScheduleDto.CheckInCapacity = checkinScheduleDto.CheckInCapacityDefault;
            checkinScheduleDto.CheckInOpen = checkinScheduleDto.CheckInOpenDefault;
            CommitChanges(checkinScheduleDto);
        }

        public void SetAllDefaults()
        {
            db.ExecuteCommand("UPDATE dbo.MemberTags SET CheckInOpen = CheckInOpenDefault, CheckInCapacity = CheckInCapacityDefault");
        }

        public void IncrementCapacity(CheckinScheduleDto checkinScheduleDto)
        {
            checkinScheduleDto.CheckInCapacity += 1;
            CommitChanges(checkinScheduleDto);
        }

        public void DecrementCapacity(CheckinScheduleDto checkinScheduleDto)
        {
            if (checkinScheduleDto.CheckInCapacity > 0)
            {
                checkinScheduleDto.CheckInCapacity -= 1;
                CommitChanges(checkinScheduleDto);
            }
        }

        public void ToggleCheckinOpen(CheckinScheduleDto checkinScheduleDto)
        {
            checkinScheduleDto.CheckInOpen = !checkinScheduleDto.CheckInOpen;
            CommitChanges(checkinScheduleDto);
        }

        private void CommitChanges(CheckinScheduleDto checkinScheduleDto)
        {
            var dbRecord = db.MemberTags.SingleOrDefault(mt => mt.OrgId == checkinScheduleDto.OrganizationId && mt.Id == checkinScheduleDto.SubgroupId && mt.Name == checkinScheduleDto.SubgroupName);
            dbRecord.CheckInOpen = checkinScheduleDto.CheckInOpen;
            dbRecord.CheckInCapacity = checkinScheduleDto.CheckInCapacity;
            db.SubmitChanges();
        }
        #endregion
    }
}
