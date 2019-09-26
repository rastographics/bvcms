using CmsData.Codes;
using System.Linq;

namespace CmsData
{
    public class PeopleUtils
    {
        public static IQueryable<int> GetParentsIds(IQueryable<Person> q)
        {
            var q2 = from p in q
                     from m in p.Family.People
                     where p.PositionInFamilyId == PositionInFamily.Child
                     where m.PositionInFamilyId == PositionInFamily.PrimaryAdult
                     where m.DeceasedDate == null
                     select m.PeopleId;

            return q2;
        }
    }
}
