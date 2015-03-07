using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class PrevMemberModel
    {
        public int OrganizationId { get; set; }
        public PagerModel2 Pager { get; set; }
        public PrevMemberModel(int id, string name)
        {
            Util2.CurrentOrgId = id;
            OrganizationId = id;
            Pager = new PagerModel2(Count);
            Pager.Direction = "asc";
            Pager.Sort = "Name";
            nameFilter = name;
        }
        public bool ShowProspects { get; set; }
        private readonly string nameFilter;
        private IQueryable<EnrollmentTransaction> enrollments;
        private IQueryable<EnrollmentTransaction> FetchPrevMembers()
        {
            if (enrollments == null)
            {
                enrollments = from etd in DbUtil.Db.EnrollmentTransactions
                               let mdt = DbUtil.Db.EnrollmentTransactions.Where(m =>
                                   m.PeopleId == etd.PeopleId
                                   && m.OrganizationId == OrganizationId
                                   && m.TransactionTypeId > 3
                                   && m.TransactionStatus == false).Select(m => m.TransactionDate).Max()
                               where etd.TransactionStatus == false
                               where etd.TransactionDate == mdt
                               where ShowProspects 
                                    ? etd.MemberTypeId == MemberTypeCode.Prospect 
                                    : etd.MemberTypeId != MemberTypeCode.Prospect
                               where etd.OrganizationId == OrganizationId
                               where etd.TransactionTypeId >= 4
                               where etd.Person.OrganizationMembers.All(om => om.OrganizationId != OrganizationId)
                               select etd;

                if (nameFilter.HasValue())
                {
                    string first, last;
                    Util.NameSplit(nameFilter, out first, out last);
                    if (first.HasValue())
                        enrollments = from om in enrollments
                                       let p = om.Person
                                       where p.LastName.StartsWith(last)
                                       where p.FirstName.StartsWith(first) || p.NickName.StartsWith(first)
                                       select om;
                    else
                        enrollments = from om in enrollments
                                       let p = om.Person
                                       where p.LastName.StartsWith(last)
                                       select om;
                }
            }
            return enrollments;
        }
        public bool isFiltered
        {
            get { return nameFilter.HasValue(); }
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchPrevMembers().Count();
            return _count.Value;
        }
        public IEnumerable<PersonMemberInfo> PrevMembers()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
            var tagownerid = Util2.CurrentTagOwnerId;
            var q2 = from om in q
                     let p = om.Person
                     let att = om.Person.Attends.Where(a => a.OrganizationId == om.OrganizationId && a.AttendanceFlag).Max(a => a.MeetingDate)
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
                         MemberTypeCode = om.MemberType.Code,
                         MemberType = om.MemberType.Description,
                         MemberTypeId = om.MemberTypeId,
                         AttendPct = om.AttendancePercentage,
                         LastAttended = att,
                         Joined = om.EnrollmentDate,
                         Dropped = om.TransactionDate,
                         HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName && t.Tag.PeopleId == tagownerid),
                     };
            return q2;
        }
        public IQueryable<EnrollmentTransaction> ApplySort()
        {
            var q = FetchPrevMembers();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Name":
                        q = from om in q
                            let p = om.Person
                            orderby p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Church":
                        q = from om in q
                            let p = om.Person
                            orderby p.MemberStatus.Code,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "MemberType":
                        q = from om in q
                            let p = om.Person
                            orderby om.MemberType.Code,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Primary Address":
                        q = from om in q
                            let p = om.Person
                            orderby p.Family.StateCode,
                            p.Family.CityName,
                            p.Family.AddressLineOne,
                            p.PeopleId
                            select om;
                        break;
                    case "BFTeacher":
                        q = from om in q
                            let p = om.Person
                            orderby p.BFClass.LeaderName,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "% Att.":
                        q = from om in q
                            orderby om.AttendancePercentage
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear, p.BirthMonth, p.BirthDay
                            select om;
                        break;
                    case "Bday":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthMonth, p.BirthDay,
                            p.Name2
                            select om;
                        break;
                    case "Dropped":
                        q = from om in q
                            orderby om.TransactionDate
                            select om;
                        break;
                    case "Last Att.":
                        q = from om in q
                            let att2 = om.Person.Attends.Where(a => a.OrganizationId == om.OrganizationId).Max(a => a.MeetingDate)
                            orderby att2
                            select om;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Church":
                        q = from om in q
                            let p = om.Person
                            orderby p.MemberStatus.Code descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "MemberType":
                        q = from om in q
                            let p = om.Person
                            orderby om.MemberType.Code descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Address":
                        q = from om in q
                            let p = om.Person
                            orderby p.Family.StateCode descending,
                                   p.Family.CityName descending,
                                   p.Family.AddressLineOne descending,
                                   p.PeopleId descending
                            select om;
                        break;
                    case "BFTeacher":
                        q = from om in q
                            let p = om.Person
                            orderby p.BFClass.LeaderName descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "% Att.":
                        q = from om in q
                            orderby om.AttendancePercentage descending
                            select om;
                        break;
                    case "Name":
                        q = from om in q
                            let p = om.Person
                            orderby p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Bday":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.Name2
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending
                            select om;
                        break;
                    case "Dropped":
                        q = from om in q
                            orderby om.TransactionDate descending
                            select om;
                        break;
                    case "Last Att.":
                        q = from om in q
                            let att2 = om.Person.Attends.Where(a => a.OrganizationId == om.OrganizationId).Max(a => a.MeetingDate)
                            orderby att2 descending
                            select om;
                        break;
                }
            return q;
        }

    }
}
