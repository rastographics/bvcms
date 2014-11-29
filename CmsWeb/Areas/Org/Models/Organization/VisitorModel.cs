using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Models
{
    public class VisitorModel
    {
        public int OrganizationId { get; set; }
        public PagerModel2 Pager { get; set; }
        private readonly string nameFilter;
        private bool showHiddenGuests { get; set; }
        private string first;
        private string last;

        public VisitorModel(int id, CurrentOrg currorg)
        {
            OrganizationId = id;
            Pager = new PagerModel2(Count);
            Pager.Direction = "asc";
            Pager.Sort = "Name";
            nameFilter = currorg.NameFilter;
            showHiddenGuests = currorg.ShowHidden;
        }

        private IQueryable<Person> visitors;

        private IQueryable<Person> FetchVisitors()
        {
            var mindt = Util.Now.AddDays(-Util2.VisitLookbackDays).Date;
            Util.NameSplit(nameFilter, out first, out last);
            return visitors ??
                (visitors = from p in DbUtil.Db.People
                            join g in DbUtil.Db.GuestList(OrganizationId, mindt, showHiddenGuests, first, last) 
                                on p.PeopleId equals g.PeopleId
                            select p);
        }

        public bool isFiltered
        {
            get { return nameFilter.HasValue(); }
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
