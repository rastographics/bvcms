using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using System.Web.Mvc;

namespace CmsWeb.Areas.Org.Models
{
    public class MemberModel : PagedTableModel<OrganizationMember, PersonMemberInfo>, ICurrentOrg
    {
        public int? OrganizationId { get; set; }
        public Organization Org { get; set; }
        public int GroupSelect { get; set; }

        public MemberModel()
            : base("Name", "asc")
        {
            if(ClearFilter)
                this.ClearCurrentOrg();
            this.CopyPropertiesFrom(DbUtil.Db.CurrentOrg);
            OrganizationId = Id;
            Org = DbUtil.Db.LoadOrganizationById(Id);
        }

        public bool IsFiltered
        {
            get { return NameFilter.HasValue() || SgFilter.HasValue(); }
        }
        public IEnumerable<SelectListItem> SmallGroups()
        {
            var q = from mt in DbUtil.Db.MemberTags
                    where mt.OrgId == OrganizationId
                    orderby mt.Name
                    select new SelectListItem
                    {
                        Text = mt.Name,
                        Value = mt.Id.ToString()
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "-1", Text = "(not assigned)" });
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return list;
        }

        public override IQueryable<OrganizationMember> DefineModelList()
        {
            return from om in DbUtil.Db.OrganizationMembers
                   where om.OrganizationId == OrganizationId
                   where DbUtil.Db.OrgMember(OrganizationId, GroupSelect, this.First(), this.Last(), SgFilter, ShowHidden).Any(mm => mm.PeopleId == om.PeopleId)
                   select om;
        }

        public override IQueryable<OrganizationMember> DefineModelSort(IQueryable<OrganizationMember> q)
        {
            if (Direction == "asc")
                switch (Sort)
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
                            orderby om.AttendPct
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
                    case "Last Att.":
                        q = from om in q
                            let p = om.Person
                            orderby om.LastAttended, p.LastName, p.FirstName
                            select om;
                        break;
                    case "Joined":
                        q = from om in q
                            let p = om.Person
                            orderby om.EnrollmentDate, p.LastName, p.FirstName
                            select om;
                        break;
                }
            else
                switch (Sort)
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
                            orderby om.AttendPct descending
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
                            p.Name2 descending
                            select om;
                        break;
                    case "Last Att.":
                        q = from om in q
                            let p = om.Person
                            orderby om.LastAttended descending, p.LastName descending, p.FirstName descending
                            select om;
                        break;
                    case "Joined":
                        q = from om in q
                            let p = om.Person
                            orderby om.EnrollmentDate descending, p.LastName descending, p.FirstName descending
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending
                            select om;
                        break;
                }
            return q;
        }

        public override IEnumerable<PersonMemberInfo> DefineViewList(IQueryable<OrganizationMember> q)
        {
            var q0 = q.Skip(StartRow).Take(PageSize);

            var tagownerid = Util2.CurrentTagOwnerId;
            var q1 = from om in q0
                    let p = om.Person
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
                        InactiveDate = om.InactiveDate,
                        AttendPct = om.AttendPct,
                        LastAttended = om.LastAttended,
                        HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName && t.Tag.PeopleId == tagownerid),
                        Joined = om.EnrollmentDate,
                        Hidden = om.Hidden ?? false
                    };
            return q1;
        }

        public int? Id { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool ClearFilter { get; set; }
    }
}
