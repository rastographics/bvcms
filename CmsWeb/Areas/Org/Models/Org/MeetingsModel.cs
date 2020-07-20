using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData.Classes.RoleChecker;
using System;
using System.Diagnostics.Eventing.Reader;
using CmsData.Codes;
using CmsWeb.Constants;

namespace CmsWeb.Areas.Org.Models
{
    public class MeetingsModel : PagedTableModel<Meeting, MeetingInfo>
    {
        public int Id { get; set; }
        public bool Future { get; set; }

        public bool ShowCreateNewMeeting { get; } = RoleChecker.HasSetting(SettingName.Organization_ShowCreateNewMeeting, true);
        public bool ShowDeleteMeeting { get; } = RoleChecker.HasSetting(SettingName.Organization_ShowDeleteMeeting, true);
        public bool ShowESpaceSyncMeetings
        {
            get
            {
                if (!_ShowESpaceSyncMeetings.HasValue)
                {
                    _ShowESpaceSyncMeetings = ShowCreateNewMeeting &&
                        CurrentDatabase.Setting("eSpaceEnabled") &&
                        OrgHasEspaceEventId();
                }
                return _ShowESpaceSyncMeetings.Value;
            }
        }

        private bool? _ShowESpaceSyncMeetings;

        public bool IsTicketing
        {
            get
            {
                if (!isTicketing.HasValue)
                {
                    var regtype = (from o in CurrentDatabase.Organizations
                                   where o.OrganizationId == Id
                                   select o.RegistrationTypeId).SingleOrDefault();
                    isTicketing = regtype == RegistrationTypeCode.Ticketing;
                }
                return isTicketing.Value;
            }
        }
        private bool? isTicketing;

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public MeetingsModel()
        {
            Sort = "";
            Direction = "";
            AjaxPager = true;
        }

        public override IQueryable<Meeting> DefineModelList()
        {
            var tzoffset = CurrentDatabase.Setting("TZOffset", "0").ToInt(); // positive to the east, negative to the west
            var midnight = Util.Now.Date.AddDays(1).AddHours(tzoffset);
            var meetings = from m in CurrentDatabase.Meetings
                           where m.OrganizationId == Id
                           select m;
            var future = IsTicketing ? !Future : Future;
            if (future)
                meetings = from m in meetings
                           where m.MeetingDate >= midnight
                           select m;
            else
                meetings = from m in meetings
                           where m.MeetingDate < midnight
                           select m;
            return meetings;
        }

        public override IQueryable<Meeting> DefineModelSort(IQueryable<Meeting> q)
        {
            var future = IsTicketing ? !Future : Future;
            if (Direction == "asc")
                switch (SortExpression)
                {
                    case "Present":
                        q = q.OrderByDescending(m => m.NumPresent);
                        break;
                    case "Count":
                        q = q.OrderBy(m => m.HeadCount).ThenBy(m => m.NumPresent);
                        break;
                    case "Guests":
                        q = q.OrderByDescending(m => (m.NumNewVisit + m.NumRepeatVst + m.NumVstMembers));
                        break;
                    case "Meeting Time":
                        q = q.OrderBy(m => m.MeetingDate.Value.TimeOfDay).ThenBy(m => m.MeetingDate);
                        break;
                    case "Location":
                        q = q.OrderBy(m => m.Location ?? "zzz").ThenByDescending(m => m.MeetingDate);
                        break;
                    case "Description":
                        q = q.OrderBy(m => (m.Description ?? "") + "zzz").ThenByDescending(m => m.MeetingDate);
                        break;
                    default:
                        q = !future
                            ? q.OrderBy(m => m.MeetingDate)
                            : q.OrderByDescending(m => m.MeetingDate);
                        break;
                }
            else
                switch (SortExpression)
                {
                    case "Present":
                        q = q.OrderBy(m => m.NumPresent);
                        break;
                    case "Count":
                        q = q.OrderByDescending(m => m.HeadCount).ThenByDescending(m => m.NumPresent);
                        break;
                    case "Guests":
                        q = q.OrderBy(m => (m.NumNewVisit + m.NumRepeatVst + m.NumVstMembers)).ThenBy(m => m.NumPresent);
                        break;
                    case "Meeting Time":
                        q = q.OrderByDescending(m => m.MeetingDate.Value.TimeOfDay).ThenByDescending(m => m.MeetingDate);
                        break;
                    case "Location":
                        q = q.OrderBy(m => m.Location ?? "zzz").ThenByDescending(m => m.MeetingDate);
                        break;
                    case "Description":
                        q = q.OrderBy(m => (m.Description ?? "") + "zzz").ThenByDescending(m => m.MeetingDate);
                        break;
                    default:
                        q = future
                            ? q.OrderBy(m => m.MeetingDate)
                            : q.OrderByDescending(m => m.MeetingDate);
                        break;
                }
            return q;
        }

        public override IEnumerable<MeetingInfo> DefineViewList(IQueryable<Meeting> q)
        {
            var future = IsTicketing ? !Future : Future;
            var q2 = from m in q
                     let o = m.Organization
                     let mc = future && CurrentDatabase.ViewMeetingConflicts.Any(mm =>
                         mm.MeetingDate == m.MeetingDate
                         && (mm.OrgId1 == m.OrganizationId || mm.OrgId2 == m.OrganizationId))
                     select new MeetingInfo
                     {
                         MeetingId = m.MeetingId,
                         OrganizationId = m.OrganizationId,
                         MeetingDate = m.MeetingDate,
                         Location = m.Location,
                         NumPresent = m.NumPresent,
                         HeadCount = m.HeadCount,
                         NumVisitors = m.NumNewVisit + m.NumRepeatVst + m.NumVstMembers,
                         Description = m.Description,
                         Conflict = mc
                     };
            return q2;
        }

        private bool OrgHasEspaceEventId() => CurrentDatabase.Organizations.Any(o => o.OrganizationId == Id && o.ESpaceEventId.HasValue);
    }
}
