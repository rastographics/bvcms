using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Models
{
    public class VisitorModel : CurrentOrg
    {
        public int OrganizationId { get; set; }
        public PagerModel2 Pager { get; set; }

        public VisitorModel(CurrentOrg currorg, PagerModel2 pager = null)
        {
            if(!currorg.Id.HasValue)
                throw new ArgumentException("missing currorg.Id");
            OrganizationId = currorg.Id.Value;
            Pager = pager ?? new PagerModel2() {Direction = "asc", Sort = "Name"};
            Pager.GetCount = Count;
        }

        private IQueryable<Person> visitors;

        private IQueryable<Person> FetchVisitors()
        {
            var mindt = Util.Now.AddDays(-Util2.VisitLookbackDays).Date;
            return visitors ??
                (visitors = from p in DbUtil.Db.People
                            join g in DbUtil.Db.GuestList(OrganizationId, mindt, ShowHidden, First, Last) 
                                on p.PeopleId equals g.PeopleId
                            select p);
        }

        public bool isFiltered
        {
            get { return NameFilter.HasValue(); }
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchVisitors().Count();
            return _count.Value;
        }
        public IEnumerable<PersonMemberInfo> Visitors()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
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
        public IQueryable<Person> ApplySort()
        {
            var q = FetchVisitors();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
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
                switch (Pager.Sort)
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

    }
}
