using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;
using System.Globalization;

namespace CmsWeb.Areas.People.Models.Person
{
    public class PersonAttendHistoryModel
    {
        public readonly int PeopleId;
        public CmsWeb.Models.PagerModel2 Pager { get; set; }
        public PersonAttendHistoryModel(int id, bool future)
        {
            PeopleId = id;
            Future = future;
            Pager = new CmsWeb.Models.PagerModel2(Count);
        }
        public bool Future { get; set; }
        private IQueryable<Attend> _attends;
        private IQueryable<Attend> FetchAttends()
        {
            if (_attends == null)
            {
                var midnight = Util.Now.Date.AddDays(1);
            	var roles = DbUtil.Db.CurrentRoles();
                _attends = from a in DbUtil.Db.Attends
						   let org = a.Meeting.Organization
                           where a.PeopleId == PeopleId
                           where !(org.SecurityTypeId == 3 && (Util2.OrgMembersOnly || Util2.OrgLeadersOnly))
						   where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                           select a;
                if (!HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.Session["showallmeetings"] == null)
                    _attends = _attends.Where(a => a.EffAttendFlag == null || a.EffAttendFlag == true);
                if (Future)
                    _attends = _attends.Where(aa => aa.MeetingDate >= midnight);
                else
                    _attends = _attends.Where(aa => aa.MeetingDate < midnight);
            }
            return _attends;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchAttends().Count();
            return _count.Value;
        }
        public IEnumerable<AttendInfo> Attendances()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
            var q2 = from a in q
                     let o = a.Meeting.Organization
                     select new AttendInfo
                     {
                         PeopleId = a.PeopleId,
                         MeetingId = a.MeetingId,
                         OrganizationId = a.Meeting.OrganizationId,
                         OrganizationName = CmsData.Organization
                            .FormatOrgName(o.OrganizationName, o.LeaderName, null),
                         AttendType = a.AttendType.Description ?? "(null)",
                         MeetingName = o.Division.Name + ": " + o.OrganizationName,
                         MeetingDate = a.MeetingDate,
                         MemberType = a.MemberType.Description ?? "(null)",
                         AttendFlag = a.AttendanceFlag,
                         OtherAttends = a.OtherAttends,
                     };
            return q2;
        }
        private IQueryable<Attend> ApplySort()
        {
            var q = FetchAttends();
            switch (Pager.SortExpression)
            {
                case "Organization":
                    q = q.OrderBy(a => a.Meeting.Organization.OrganizationName).ThenByDescending(a => a.MeetingDate);
                    break;
                case "Organization desc":
                    q = q.OrderByDescending(a => a.Meeting.Organization.OrganizationName).ThenByDescending(a => a.MeetingDate);
                    break;
                case "MemberType":
                    q = q.OrderBy(a => a.MemberTypeId).ThenByDescending(a => a.MeetingDate);
                    break;
                case "MemberType desc":
                    q = q.OrderByDescending(a => a.MemberTypeId).ThenByDescending(a => a.MeetingDate);
                    break;
                case "AttendType":
                    q = q.OrderBy(a => a.AttendanceTypeId).ThenByDescending(a => a.MeetingDate);
                    break;
                case "AttendType desc":
                    q = q.OrderByDescending(a => a.AttendanceTypeId).ThenByDescending(a => a.MeetingDate);
                    break;
                case "Meeting":
                    q = q.OrderBy(a => a.MeetingDate);
                    break;
                case "Meeting desc":
                    q = q.OrderByDescending(a => a.MeetingDate);
                    break;
                default:
                    if (Future)
                        q = q.OrderBy(a => a.MeetingDate);
                    else
                        q = q.OrderByDescending(a => a.MeetingDate);
                    break;
            }
            return q;
        }
    }
}
