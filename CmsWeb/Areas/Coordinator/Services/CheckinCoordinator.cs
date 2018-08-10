using System;
using System.Collections.Generic;
using System.Linq;
using CmsWeb.Areas.Coordinator.Controllers;
using CmsWeb.Areas.Coordinator.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Coordinator.Services
{
    public class CheckinCoordinator
    {
        public IEnumerable<CheckinScheduleDto> Schedules;

        public CheckinCoordinator(IEnumerable<CheckinScheduleDto> scheduleCollection)
        {
            Schedules = scheduleCollection;
        }

        public IEnumerable<CheckinTimeslotDto> GetFilteredTimeslots()
        {
            return Schedules
                .AsQueryable()
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
            DateTime date = ConvertToDate(selectedTimeslot);

            return Schedules
                .AsQueryable()
                .Where(s => (s.NextMeetingDate == date || date == DateTime.MinValue))
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
            var date = ConvertToDate(selectedTimeslot);
            return Schedules
                .AsQueryable()
                .Where(s => (s.NextMeetingDate == date || string.IsNullOrWhiteSpace(selectedTimeslot)) && (s.ProgramId == programId || programId == 0))
                .DistinctBy(s => s.DivisionId)
                .Select(s => new CheckinDivisionDto
                {
                    DivisionId = s.DivisionId,
                    DivisionName = s.DivisionName
                })
                .ToList();
        }

        public IEnumerable<CheckinScheduleDto> GetFilteredSchedules(string selectedTimeslot = "", int programId = 0, int divisionId = 0)
        {
            var date = ConvertToDate(selectedTimeslot);

            return Schedules
                .AsQueryable()
                .Where(s => (s.NextMeetingDate == date || string.IsNullOrWhiteSpace(selectedTimeslot)) && (s.ProgramId == programId || programId == 0) && (s.DivisionId == divisionId || divisionId == 0))
                .ToList();
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
    }
}
