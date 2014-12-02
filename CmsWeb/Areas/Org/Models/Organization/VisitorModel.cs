using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Models
{
    public class VisitorModel : PagedTableModel<Person, PersonMemberInfo>, ICurrentOrg
    {
        public int OrganizationId { get; set; }

        public VisitorModel()//CurrentOrg currorg, PagerModel2 pager = null)
        {
            this.CopyPropertiesFrom(DbUtil.Db.CurrentOrg);
            if(!Id.HasValue)
                throw new ArgumentException("missing currorg.Id");
            OrganizationId = Id.Value;
//            Pager = pager ?? new PagerModel2() {Direction = "asc", Sort = "Name"};
//            Pager.GetCount = Count;
        }

        public bool IsFiltered
        {
            get { return NameFilter.HasValue(); }
        }

        public override IQueryable<Person> DefineModelList()
        {
            var mindt = Util.Now.AddDays(-Util2.VisitLookbackDays).Date;
            return from p in DbUtil.Db.People
                   join g in DbUtil.Db.GuestList(OrganizationId, mindt, ShowHidden, this.First(), this.Last())
                       on p.PeopleId equals g.PeopleId
                   select p;
        }

        public override IQueryable<Person> DefineModelSort(IQueryable<Person> q)
        {
            if (Direction == "asc")
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.Name2,
                            p.PeopleId
                            select p;
                        break;
                    case "Church":
                        q = from p in q
                            orderby p.MemberStatus.Code,
                            p.Name2,
                            p.PeopleId
                            select p;
                        break;
                    case "Primary Address":
                        q = from p in q
                            orderby p.Family.StateCode,
                            p.Family.CityName,
                            p.Family.AddressLineOne,
                            p.PeopleId
                            select p;
                        break;
                    case "Age":
                        q = from p in q
                            orderby p.BirthYear, p.BirthMonth, p.BirthDay
                            select p;
                        break;
                    case "Bday":
                        q = from p in q
                            orderby p.BirthMonth, p.BirthDay,
                            p.Name2
                            select p;
                        break;
                    case "Last Attended":
                        q = from p in q
                            orderby DbUtil.Db.LastAttended(OrganizationId, p.PeopleId)
                            select p;
                        break;
                }
            else
                switch (Sort)
                {
                    case "Church":
                        q = from p in q
                            orderby p.MemberStatus.Code descending,
                            p.Name2 descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Address":
                        q = from p in q
                            orderby p.Family.StateCode descending,
                                   p.Family.CityName descending,
                                   p.Family.AddressLineOne descending,
                                   p.PeopleId descending
                            select p;
                        break;
                    case "Name":
                        q = from p in q
                            orderby p.Name2 descending,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Bday":
                        q = from p in q
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.Name2 descending
                            select p;
                        break;
                    case "Age":
                        q = from p in q
                            orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending
                            select p;
                        break;
                    case "Last Attended":
                        q = from p in q
                            orderby DbUtil.Db.LastAttended(OrganizationId, p.PeopleId) descending
                            select p;
                        break;
                }
            return q;
        }

        public override IEnumerable<PersonMemberInfo> DefineViewList(IQueryable<Person> q)
        {
            q = q.Skip(StartRow).Take(PageSize);
            var tagownerid = Util2.CurrentTagOwnerId;
            var q2 = from p in q
                     join g in DbUtil.Db.GuestList2(OrganizationId) on p.PeopleId equals g.PeopleId
                     select new PersonMemberInfo
                     {
                         PeopleId = p.PeopleId,
                         Name = p.Name,
                         Name2 = p.Name2,
                         BirthDate = Util.FormatBirthday(
                             p.BirthYear,
                             p.BirthMonth,
                             p.BirthDay),
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         EmailAddress = p.EmailAddress,
                         PhonePref = p.PhonePrefId,
                         HomePhone = p.HomePhone,
                         CellPhone = p.CellPhone,
                         WorkPhone = p.WorkPhone,
                         MemberStatus = p.MemberStatus.Description,
                         Email = p.EmailAddress,
                         BFTeacher = p.BFClass.LeaderName,
                         BFTeacherId = p.BFClass.LeaderId,
                         Age = p.Age.ToString(),
                         LastAttended = g.LastAttendDt,
                         Hidden = g.Hidden,
                         HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName && t.Tag.PeopleId == tagownerid),
                         MemberTypeId = g.MemberTypeId ?? 0
                     };
            return q2;
        }

        public int? Id { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool ClearFilter { get; set; }
    }
}
