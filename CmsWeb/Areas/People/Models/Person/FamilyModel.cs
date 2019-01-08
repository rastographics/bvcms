using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.People.Models
{
    public class FamilyModel : PagedTableModel<FamilyMember, FamilyMember>
    {
        public CmsData.Person Person;
        public FamilyModel() { }
        public FamilyModel(int id)
        {
            Person = DbUtil.Db.LoadPersonById(id);
            pagesize = 100;
        }
        private Family family;
        public Family Family
        {
            get
            {
                if (family != null)
                {
                    return family;
                }

                return family = DbUtil.Db.Families.SingleOrDefault(ff => ff.FamilyId == Person.FamilyId);
            }
        }
        public override IQueryable<FamilyMember> DefineModelList()
        {
            return from m in DbUtil.Db.FamilyMembers(Person.PeopleId)
                   select m;
        }

        public override IQueryable<FamilyMember> DefineModelSort(IQueryable<FamilyMember> q)
        {
            var mindt = DateTime.Parse("1/1/1900");
            return from m in q
                   orderby m.DeceasedDate ?? mindt,
                       m.PositionInFamilyId,
                       m.PositionInFamilyId == 10 ? m.GenderId : 0,
                       m.Age descending,
                       m.Name
                   select m;
        }

        public override IEnumerable<FamilyMember> DefineViewList(IQueryable<FamilyMember> q)
        {
            return q;
        }
        public class RelatedFamilyInfo
        {
            public int Id { get; set; }
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public int PeopleId { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
        }
        public IEnumerable<RelatedFamilyInfo> RelatedFamilies()
        {
            var rf1 = from rf in Family.RelatedFamilies1
                      let hh = rf.RelatedFamily2.HeadOfHousehold
                      select new RelatedFamilyInfo
                      {
                          Id = Person.FamilyId,
                          Id1 = rf.FamilyId,
                          Id2 = rf.RelatedFamilyId,
                          PeopleId = hh != null ? hh.PeopleId : 0,
                          Name = "The " + (hh != null ? hh.Name : "?") + " Family",
                          Description = rf.FamilyRelationshipDesc
                      };
            var rf2 = from rf in Family.RelatedFamilies2
                      let hh = rf.RelatedFamily1.HeadOfHousehold
                      select new RelatedFamilyInfo
                      {
                          Id = Person.FamilyId,
                          Id1 = rf.FamilyId,
                          Id2 = rf.RelatedFamilyId,
                          PeopleId = hh != null ? hh.PeopleId : 0,
                          Name = "The " + (hh != null ? hh.Name : "?") + " Family",
                          Description = rf.FamilyRelationshipDesc
                      };
            var q = rf1.Union(rf2);
            return q;
        }
    }
}
