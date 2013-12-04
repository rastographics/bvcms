using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Models.OrganizationPage
{
    public class VisitorModel
    {
        private int queryid;
        public int OrganizationId { get; set; }
        public PagerModel2 Pager { get; set; }
        private string NameFilter;
        public VisitorModel(int id, int qid, string name)
        {
            OrganizationId = id;
            queryid = qid;
            Pager = new PagerModel2(Count);
            Pager.Direction = "asc";
            Pager.Sort = "Name";
            NameFilter = name;
        }
        private IQueryable<Person> _visitors;
        private IQueryable<Person> FetchVisitors()
        {
            if (_visitors == null)
            {
                _visitors = DbUtil.Db.PeopleQuery(queryid);
                if (NameFilter.HasValue())
                {
                    string First, Last;
                    Util.NameSplit(NameFilter, out First, out Last);
                    if (First.HasValue())
                        _visitors = from p in _visitors
                                    where p.LastName.StartsWith(Last)
                                    where p.FirstName.StartsWith(First) || p.NickName.StartsWith(First)
                                    select p;
                    else
                        _visitors = from p in _visitors
                                    where p.LastName.StartsWith(Last)
                                    select p;
                }
            }
            return _visitors;
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
                     let prospect = DbUtil.Db.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == p.PeopleId && mm.OrganizationId == OrganizationId && mm.MemberTypeId == MemberTypeCode.Prospect)
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
                         LastAttended = DbUtil.Db.LastAttended(OrganizationId, p.PeopleId),
                         HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName && t.Tag.PeopleId == tagownerid),
                         MemberTypeId = prospect != null ? 311 : 0
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
