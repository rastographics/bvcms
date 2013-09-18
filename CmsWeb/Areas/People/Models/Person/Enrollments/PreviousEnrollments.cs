using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class PreviousEnrollments
    {
        private int PeopleId;
        public CmsData.Person person { get; set; }
        public CmsWeb.Models.PagerModel2 Pager { get; set; }
        public PreviousEnrollments(int id)
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
            Pager = new CmsWeb.Models.PagerModel2(Count);
        }
        private IQueryable<EnrollmentTransaction> _enrollments;
        private IQueryable<EnrollmentTransaction> FetchEnrollments()
        {
            if (_enrollments == null)
            {
                var limitvisibility = Util2.OrgMembersOnly || Util2.OrgLeadersOnly
                    || !HttpContext.Current.User.IsInRole("Access");
                var roles = DbUtil.Db.CurrentRoles();
                _enrollments = from etd in DbUtil.Db.EnrollmentTransactions
                               let org = etd.Organization
                               where etd.TransactionStatus == false
                               where etd.PeopleId == PeopleId
                               where etd.TransactionTypeId >= 4
                               where !(limitvisibility && etd.Organization.SecurityTypeId == 3)
                               where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                               select etd;
            }
            return _enrollments;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchEnrollments().Count();
            return _count.Value;
        }
        public IEnumerable<OrgMemberInfo> Enrollments()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
            var q2 = from om in q
                     select new OrgMemberInfo
                     {
                         OrgId = om.OrganizationId,
                         PeopleId = om.PeopleId,
                         Name = om.OrganizationName,
                         MemberType = om.MemberType.Description,
                         EnrollDate = om.FirstTransaction.TransactionDate,
                         DropDate = om.TransactionDate,
                         AttendPct = om.AttendancePercentage,
                         DivisionName = om.Organization.Division.Program.Name + "/" + om.Organization.Division.Name,
                         OrgType = om.Organization.OrganizationType.Description ?? "Other"
                     };
            return q2;
        }
        private IQueryable<EnrollmentTransaction> ApplySort()
        {
            var q = FetchEnrollments();
            switch (Pager.SortExpression)
            {
                case "Enroll Date":
                case "Enroll Date desc":
					q = from om in q
						orderby om.Organization.OrganizationType.Code ?? "z", om.EnrollmentDate
						select om;
                    break;
				default:
					q = from om in q
						orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
						select om;
                    break;
            }
            return q;
        }
    }
}
