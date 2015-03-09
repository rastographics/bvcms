using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class FamilyModel : PagedTableModel<CmsData.Person, FamilyMemberInfo>
    {
        public CmsData.Person Person;
        public FamilyModel(int id)
        {
            Person = DbUtil.Db.LoadPersonById(id);
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
        override public IQueryable<CmsData.Person> DefineModelList()
        {
            var mindt = DateTime.Parse("1/1/1900");
            return from m in DbUtil.Db.People
                   where m.FamilyId == Person.FamilyId
                   orderby
                       m.DeceasedDate ?? mindt,
                       m.PositionInFamilyId,
                       m.PositionInFamilyId == 10 ? m.GenderId : 0,
                       m.Age descending, m.Name2
                   select m;
        }

        override public IQueryable<CmsData.Person> DefineModelSort(IQueryable<CmsData.Person> q)
        {
            return q;
        }

        override public IEnumerable<FamilyMemberInfo> DefineViewList(IQueryable<CmsData.Person> q)
        {
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
                         isDeceased = m.Deceased,
                         MemberStatus = m.MemberStatus.Description
                     };
            var q3 = q2.ToList();
            foreach (var m in q3)
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
            return q3;
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
