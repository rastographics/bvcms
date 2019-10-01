using CmsData.Codes;
using System.Collections.Generic;
using System.Linq;

namespace CmsData
{
    public class PeopleUtils
    {
        public static IQueryable<int> GetParentsAndAdultsIds(IQueryable<Person> q)
        {
            var parentsAndAdults = (from p in q                           
                           from m in p.Family.People
                           where p.PositionInFamilyId == PositionInFamily.Child
                           where m.PositionInFamilyId == PositionInFamily.PrimaryAdult                           
                           where m.DeceasedDate == null
                           select m.PeopleId)
                           .Union
                           (from a in q
                            where a.PositionInFamilyId != PositionInFamily.Child
                            select a.PeopleId);

            return parentsAndAdults;
        }
    }
}
