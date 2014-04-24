using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {
        public class FamilyMember
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public int? Age { get; set; }
        }
        public IEnumerable<FamilyMember> FamilyMembers()
        {
            var family = from p in user.Family.People
                         where p.DeceasedDate == null
                         select new { p.PeopleId, p.Name2, p.Age, p.Name };
            var q = from m in family
                    join r in list on m.PeopleId equals r.PeopleId into j
                    from r in j.DefaultIfEmpty()
                    where r == null || r.IsValidForContinue == false
                    orderby m.PeopleId == user.Family.HeadOfHouseholdId ? 1 :
                            m.PeopleId == user.Family.HeadOfHouseholdSpouseId ? 2 :
                            3, m.Age descending, m.Name2
                    select new FamilyMember
                    {
                        PeopleId = m.PeopleId,
                        Name = m.Name,
                        Age = m.Age
                    };
            return q;
        }
    }
}