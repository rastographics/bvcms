using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class PersonAttendHistoryModel : PagedTableModel<Attend, AttendInfo>
    {
        public readonly int PeopleId;
        public PersonAttendHistoryModel(int id, bool future)
            : base("Meeting", future ? "asc" : "desc")
        {
            PeopleId = id;
            Future = future;
        }
        public bool Future { get; set; }
        private IQueryable<Attend> attends;
        override public IQueryable<Attend> ModelList()
        {
            if (attends == null)
            {
                var midnight = Util.Now.Date.AddDays(1);
                var roles = DbUtil.Db.CurrentRoles();
                attends = from a in DbUtil.Db.Attends
                          let org = a.Meeting.Organization
                          where a.PeopleId == PeopleId
                          where !(org.SecurityTypeId == 3 && (Util2.OrgMembersOnly || Util2.OrgLeadersOnly))
                          where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                          select a;
                if (!HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.Session["showallmeetings"] == null)
                    attends = attends.Where(a => a.EffAttendFlag == null || a.EffAttendFlag == true);
                if (Future)
                    attends = attends.Where(aa => aa.MeetingDate >= midnight);
                else
                    attends = attends.Where(aa => aa.MeetingDate < midnight);
            }
            return attends;
        }
        override public IEnumerable<AttendInfo> ViewList()
        {
            var q = ApplySort().Skip(Pager.StartRow).Take(Pager.PageSize);
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
        override public IQueryable<Attend> ApplySort()
        {
            var q = ModelList();
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
                    q = Future ? q.OrderBy(a => a.MeetingDate) : q.OrderByDescending(a => a.MeetingDate);
                    break;
                case "Meeting desc":
                    q = !Future ? q.OrderBy(a => a.MeetingDate) : q.OrderByDescending(a => a.MeetingDate);
                    break;
            }
            return q;
        }
    }
}
