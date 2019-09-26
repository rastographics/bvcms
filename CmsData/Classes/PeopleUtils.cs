using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData
{
    public class PeopleUtils
    {
        public static IQueryable<int> GetParentsIds(IQueryable<Person> q)
        {
            var q2 = from p in q
                     from m in p.Family.People
                     where m.PositionInFamilyId == 10
                     //					 where (m.PositionInFamilyId == 10 && p.PositionInFamilyId != 10)
                     //					 || (m.PeopleId == p.PeopleId && p.PositionInFamilyId == 10)
                     where m.DeceasedDate == null
                     select m.PeopleId;

            return q2;
        }
    }
}
