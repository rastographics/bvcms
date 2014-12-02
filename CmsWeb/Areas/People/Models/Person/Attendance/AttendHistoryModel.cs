using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class PersonAttendHistoryModel : PagedTableModel<Attend, AttendInfo>
    {
        public int PeopleId { get; set; }
        public bool Future { get; set; }

        public PersonAttendHistoryModel()
            : base("Meeting", "") 
        {
        }
        override public IQueryable<Attend> DefineModelList()
        {
            var midnight = Util.Now.Date.AddDays(1);
            var roles = DbUtil.Db.CurrentRoles();
            var q = from a in DbUtil.Db.Attends
                      let org = a.Meeting.Organization
                      where a.PeopleId == PeopleId
                      where !(org.SecurityTypeId == 3 && (Util2.OrgMembersOnly || Util2.OrgLeadersOnly))
                      where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                      select a;
            if (!HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.Session["showallmeetings"] == null)
                q = q.Where(a => a.EffAttendFlag == null || a.EffAttendFlag == true);
            if (Future)
                q = q.Where(aa => aa.MeetingDate >= midnight);
            else
                q = q.Where(aa => aa.MeetingDate < midnight);
            return q;
        }
        override public IEnumerable<AttendInfo> DefineViewList(IQueryable<Attend> q)
        {
            return from a in q
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
        }
        override public IQueryable<Attend> DefineModelSort(IQueryable<Attend> q)
        {
            switch (SortExpression)
            {
                case "Organization":
                    return q.OrderBy(a => a.Meeting.Organization.OrganizationName).ThenByDescending(a => a.MeetingDate);
                case "Organization desc":
                    return q.OrderByDescending(a => a.Meeting.Organization.OrganizationName).ThenByDescending(a => a.MeetingDate);
                case "MemberType":
                    return q.OrderBy(a => a.MemberTypeId).ThenByDescending(a => a.MeetingDate);
                case "MemberType desc":
                    return q.OrderByDescending(a => a.MemberTypeId).ThenByDescending(a => a.MeetingDate);
                case "AttendType":
                    return q.OrderBy(a => a.AttendanceTypeId).ThenByDescending(a => a.MeetingDate);
                case "AttendType desc":
                    return q.OrderByDescending(a => a.AttendanceTypeId).ThenByDescending(a => a.MeetingDate);
                case "Meeting":
                default:
                    if (!Direction.HasValue())
                        Direction = Future ? "asc" : "desc";
                    if (Future)
                        return Direction == "desc"
                            ? q.OrderBy(a => a.MeetingDate) 
                            : q.OrderByDescending(a => a.MeetingDate);
                    return Direction == "asc"
                        ? q.OrderBy(a => a.MeetingDate)
                        : q.OrderByDescending(a => a.MeetingDate);
            }
        }
    }
}
