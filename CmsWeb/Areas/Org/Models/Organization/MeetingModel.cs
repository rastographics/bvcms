using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class MeetingModel
    {
        private int OrgId;
        public PagerModel2 Pager { get; set; }
        public MeetingModel(int id, bool future)
        {
            OrgId = id;
            this.future = future;
            Pager = new PagerModel2(Count);
        }
        private bool future { get; set; }
        private IQueryable<CmsData.Meeting> _meetings;
        private IQueryable<CmsData.Meeting> FetchMeetings()
        {
            if (_meetings == null)
            {
                var tzoffset = DbUtil.Db.Setting("TZOffset", "0").ToInt(); // positive to the east, negative to the west
                var midnight = Util.Now.Date.AddDays(1).AddHours(tzoffset);
                _meetings = from m in DbUtil.Db.Meetings
                            where m.OrganizationId == OrgId
                            select m;
                if (future)
                    _meetings = from m in _meetings
                                where m.MeetingDate >= midnight
                                select m;
                else
                    _meetings = from m in _meetings
                                where m.MeetingDate < midnight
                                select m;
            }
            return _meetings;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchMeetings().Count();
            return _count.Value;
        }
        public IEnumerable<MeetingInfo> Meetings()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
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
        private IQueryable<CmsData.Meeting> ApplySort()
        {
            var q = FetchMeetings();
            switch (Pager.SortExpression)
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
                    if (future)
                        q = q.OrderBy(m => m.MeetingDate);
                    else
                        q = q.OrderByDescending(m => m.MeetingDate);
                    break;
            }
            return q;
        }
    }
}
