using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class MeetingsModel : PagedTableModel<Meeting, MeetingInfo>
    {
        public int OrgId { get; set; }
        public bool Future { get; set; }

        public override IQueryable<Meeting> DefineModelList()
        {
            var tzoffset = DbUtil.Db.Setting("TZOffset", "0").ToInt(); // positive to the east, negative to the west
            var midnight = Util.Now.Date.AddDays(1).AddHours(tzoffset);
            var meetings = from m in DbUtil.Db.Meetings
                           where m.OrganizationId == OrgId
                           select m;
            if (Future)
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
            switch (SortExpression)
            {
                //case "Organization":
                //    q = q.OrderBy(a => a.Meeting.Organization.OrganizationName).ThenByDescending(a => a.MeetingDate);
                //    break;
                //case "Organization desc":
                //    q = q.OrderByDescending(a => a.Meeting.Organization.OrganizationName).ThenByDescending(a => a.MeetingDate);
                //    break;
                //case "MemberType":
                //    q = q.OrderBy(a => a.MemberTypeId).ThenByDescending(a => a.MeetingDate);
                //    break;
                //case "MemberType desc":
                //    q = q.OrderByDescending(a => a.MemberTypeId).ThenByDescending(a => a.MeetingDate);
                //    break;
                //case "AttendType":
                //    q = q.OrderBy(a => a.AttendanceTypeId).ThenByDescending(a => a.MeetingDate);
                //    break;
                //case "AttendType desc":
                //    q = q.OrderByDescending(a => a.AttendanceTypeId).ThenByDescending(a => a.MeetingDate);
                //    break;
                //case "Meeting":
                //    q = q.OrderBy(a => a.MeetingDate);
                //    break;
                //case "Meeting desc":
                //    q = q.OrderByDescending(a => a.MeetingDate);
                //    break;
                default:
                    q = Future 
                        ? q.OrderBy(m => m.MeetingDate) 
                        : q.OrderByDescending(m => m.MeetingDate);
                    break;
            }
            return q;
        }

        public override IEnumerable<MeetingInfo> DefineViewList(IQueryable<Meeting> q)
        {
            q = q.Skip(StartRow).Take(PageSize);
            var q2 = from m in q
                     let o = m.Organization
                     select new MeetingInfo
                     {
                         MeetingId = m.MeetingId,
                         OrganizationId = m.OrganizationId,
                         MeetingDate = m.MeetingDate,
                         Location = m.Location,
                         NumPresent = m.NumPresent,
                         HeadCount = m.HeadCount,
                         NumVisitors = m.NumNewVisit + m.NumRepeatVst + m.NumVstMembers,
                         Description = m.Description
                     };
            return q2;
        }
    }
}
