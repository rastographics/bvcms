using CmsData.Codes;
using System.Collections.Generic;
using System.Linq;

namespace CmsData
{
    public class PeopleUtils
    {
        public static IQueryable<int> GetParentsIds(IQueryable<Person> q)
        {
            var q2 = (from p in q
                     from m in p.Family.People
                     where p.PositionInFamilyId == PositionInFamily.Child
                     where m.PositionInFamilyId == PositionInFamily.PrimaryAdult
                     where m.DeceasedDate == null
                     select m.PeopleId).ToList();


            var q3 = q.Where(p => p.PositionInFamilyId != PositionInFamily.Child).Select(p => p.PeopleId);

            var q4 = q3.Where(p => !q2.Any());

            q2.AddRange(q4);

            return q2.AsQueryable();
        }

        public static IQueryable<int> GetPrimarySecundaryIds(CMSDataContext db, int orgId)
        {
            return from m in db.OrganizationMembers
                   join p in db.People on m.PeopleId equals p.PeopleId
                   where m.OrganizationId == orgId
                   where p.PositionInFamilyId != PositionInFamily.Child
                   select p.PeopleId;
        }
    }
}
