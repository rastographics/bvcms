using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public class FamilyModel
    {
        public CmsData.Person Person;
        public PagerModel2 Pager { get; set; }
        public FamilyModel(int id)
        {
            Person = DbUtil.Db.LoadPersonById(id);
            Pager = new PagerModel2(Count);
            Pager.pagesize = 10;
            Pager.ShowPageSize = false;
        }
        private Family family;
        public Family Family
        {
            get
            {
                if (family == null)
                    family = DbUtil.Db.Families.SingleOrDefault(ff => ff.FamilyId == Person.FamilyId);
                return family;
            }
        }
        private IQueryable<CmsData.Person> members;
        private IQueryable<CmsData.Person> FetchMembers()
        {
            if (members == null)
            {
                var mindt = DateTime.Parse("1/1/1900");
                members = from m in DbUtil.Db.People
                          where m.FamilyId == Person.FamilyId
                          orderby
                               m.DeceasedDate ?? mindt,
                               m.PositionInFamilyId,
                               m.PositionInFamilyId == 10 ? m.GenderId : 0,
                               m.Age descending, m.Name2
                          select m;
            }
            return members;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchMembers().Count();
            return _count.Value;
        }
        public IEnumerable<FamilyMemberInfo> Members()
        {
            var q = FetchMembers();
            var q2 = from m in q
                     select new FamilyMemberInfo
                     {
                         Id = m.PeopleId,
                         Pictures = m.Picture,
                         Name = m.Name,
                         Age = m.Age,
                         Color = m.DeceasedDate != null ? "red" : "auto",
                         PositionInFamily = m.PositionInFamilyId == CmsData.Codes.PositionInFamily.PrimaryAdult ? 
                            (m.FamiliesHeaded.Any() ? "Head" : (m.PeopleId == m.Family.HeadOfHouseholdSpouseId ? "Spouse" : "Head2")) :
                            m.FamilyPosition.Description,
                         SpouseIndicator = m.PeopleId == Person.SpouseId ? "*" : "&nbsp;",
                         Email = m.EmailAddress,
                         MemberStatus = m.MemberStatus.Description
                     };
            var list = q2.ToList();
            foreach (var m in list)
            {
                if (m.Pictures == null)
                    m.Pictures = new Picture();
                if (!m.Pictures.ThumbId.HasValue && m.Pictures.LargeId.HasValue)
                {
                    var i = ImageData.DbUtil.Db.Images.SingleOrDefault(im => m.Pictures.LargeId == im.Id);
                    if (i != null)
                    {
                        var th = ImageData.Image.NewImageFromBits(i.Bits, 50, 50);
                        m.Pictures.ThumbId = th.Id;
                    }
                }
            }
            DbUtil.Db.SubmitChanges();
            return list.Skip(Pager.StartRow).Take(Pager.PageSize);
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
