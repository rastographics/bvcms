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
        private bool showHidden { get; set; }
        public VisitorModel(int id, string name, bool showHidden)
        {
            OrganizationId = id;
            Util2.CurrentOrgId = id;
            Pager = new PagerModel2(Count);
            Pager.Direction = "asc";
            Pager.Sort = "Name";
            nameFilter = name;
            this.showHidden = showHidden;
        }

        private IQueryable<Person> visitors;
        private Dictionary<int, GuestList> guestlist;

        private IQueryable<Person> FetchVisitors()
        {
            if (visitors == null)
            {
                var mindt = Util.Now.AddDays(-Util2.VisitLookbackDays).Date;
                guestlist = DbUtil.Db.GuestList(Util2.CurrentOrgId, mindt, showHidden).ToDictionary(gg => gg.PeopleId, gg => gg);
                visitors = from p in DbUtil.Db.People
                           join g in DbUtil.Db.GuestList(Util2.CurrentOrgId, mindt, showHidden) on p.PeopleId equals g.PeopleId
                           select p;
                        
                if (nameFilter.HasValue())
                {
                    string First, Last;
                    Util.NameSplit(nameFilter, out First, out Last);
                    if (First.HasValue())
                        visitors = from p in visitors
                                    where p.LastName.StartsWith(Last)
                                    where p.FirstName.StartsWith(First) || p.NickName.StartsWith(First)
                                    select p;
                    else
                        visitors = from p in visitors
                                    where p.LastName.StartsWith(Last)
                                    select p;
                }
            }
            return visitors;
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
                     let prospect = DbUtil.Db.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == p.PeopleId && mm.OrganizationId == OrganizationId && mm.MemberTypeId == MemberTypeCode.Prospect)
                     let guest = guestlist[p.PeopleId]
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
                         LastAttended = guest.MeetingDate,
                         LastMeetingId = guest.MeetingId,
                         LastAttendId = guest.AttendId,
                         GuestHidden = guest.Hidden,
                         HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName && t.Tag.PeopleId == tagownerid),
                         MemberTypeId = prospect != null ? MemberTypeCode.Prospect : 0
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
