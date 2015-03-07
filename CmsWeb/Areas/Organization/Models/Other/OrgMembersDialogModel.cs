using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org2.Models
{
    public class OrgMembersDialogModel
    {
        public int orgid { get; set; }
        public bool inactives { get; set; }
        public bool pendings { get; set; }
        public bool prospects { get; set; }
        public int? sg { get; set; }

        public int memtype { get; set; }
        public int tag { get; set; }
        public DateTime? inactivedt { get; set; }

        public int MemberType { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public bool Pending { get; set; }
	    public bool MemTypeOriginal { get; set; }
	    public decimal? addpmt { get; set; }
    	public string addpmtreason { get; set; }

        private IList<int> list = new List<int>();
        public IList<int> List
        {
            get { return list; }
            set { list = value; }
        }
        public IEnumerable<SelectListItem> Tags()
        {
            var cv = new CodeValueModel();
            var tg = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Id").ToList();
            tg.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return tg;
        }
        private List<SelectListItem> mtypes;
        private List<SelectListItem> MemberTypes()
        {
            if (mtypes == null)
            {
                var q = from mt in DbUtil.Db.MemberTypes
                        where mt.Id != MemberTypeCode.Visitor
                        where mt.Id != MemberTypeCode.VisitingMember
                        orderby mt.Description
                        select new SelectListItem
                        {
                            Value = mt.Id.ToString(),
                            Text = mt.Description,
                        };
                mtypes = q.ToList();
            }
            return mtypes;
        }
        public IEnumerable<SelectListItem> MemberTypeCodesWithDrop()
        {
            var mt = MemberTypes().ToList();
            mt.Insert(0, new SelectListItem { Value = "-1", Text = "Drop" });
            mt.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return mt;
        }
        public IEnumerable<SelectListItem> MemberTypeCodesWithNotSpecified()
        {
            var mt = MemberTypes().ToList();
            mt.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return mt;
        }

        public int count;
        public IEnumerable<MemberSearchInfo> FetchOrgMemberList()
        {
            var q = OrgMembers();
            if (memtype != 0)
                q = q.Where(om => om.MemberTypeId == memtype);
            if (tag > 0)
                q = q.Where(om => om.Person.Tags.Any(t => t.Id == tag));
            if (inactivedt.HasValue)
                q = q.Where(om => om.InactiveDate == inactivedt);

            count = q.Count();
            var q1 = q.OrderBy(m => m.Person.Name2);
            var q2 = from m in q1
                     let p = m.Person
                     select new MemberSearchInfo
                     {
                         PeopleId = m.PeopleId,
                         Name = p.Name,
                         LastName = p.LastName,
                         JoinDate = m.EnrollmentDate,
                         InactiveDt = m.InactiveDate,
                         MemberType = m.MemberType.Description,
                         BirthDate = p.DOB,
                         Address = p.PrimaryAddress,
                         CityStateZip = p.CityStateZip,
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         Email = p.EmailAddress,
                         Age = p.Age,
                         MemberStatus = p.MemberStatus.Description,
                         ischecked = list.Contains(m.PeopleId)
                     };
            return q2;
        }
        public IQueryable<OrganizationMember> OrgMembers()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                where om.OrganizationId == orgid
                where om.OrgMemMemTags.Any(g => g.MemberTagId == sg) || (sg ?? 0) == 0
                select om;
            if (pendings)
                q = from om in q
                    where (om.Pending ?? false) == pendings
                    select om;
            else if (inactives)
                q = from om in q
                    where (inactives && om.MemberTypeId == MemberTypeCode.InActive)
                    select om;
            else if (prospects)
                q = from om in q
                    where (prospects && om.MemberTypeId == MemberTypeCode.Prospect)
                    select om;
            else // regular active members
                q = from om in q
                    where om.MemberTypeId != MemberTypeCode.InActive
                    where om.MemberTypeId != MemberTypeCode.Prospect
                    where (om.Pending ?? false) == false
                    select om;
            return q;
        }
        private string type()
        {
            if (pendings)
                return "UpdatePending";
            if (inactives)
                return "UpdateInactive";
            return "UpdateMembers";
        }
        public string title()
        {
            if (pendings)
                return "Update Pending Members";
            if (inactives)
                return "Update Inactive Members";
            return "Update Members";
        }
        public string HelpLink()
        {
            return Util.HelpLink("UpdateOrgMember_{0}".Fmt(type()));
        }

        public class MemberSearchInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public DateTime? JoinDate { get; set; }
            public DateTime? InactiveDt { get; set; }
            public string MemberType { get; set; }
            public string Email { get; set; }
            public string BirthDate { get; set; }
            public string Address { get; set; }
            public string CityStateZip { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public int? Age { get; set; }
            public string MemberStatus { get; set; }
            public bool ischecked { get; set; }
            public string Checked()
            {
                return ischecked ? "checked='checked'" : "";
            }

            public string ToolTip
            {
                get
                {
                    return "{0} ({1})|Cell Phone: {2}|Work Phone: {3}|Home Phone: {4}|BirthDate: {5:d}|Join Date: {6:d}|Status: {7}|Email: {8}"
                        .Fmt(Name, PeopleId, CellPhone, WorkPhone, HomePhone, BirthDate, JoinDate, MemberStatus, Email);
                }
            }

        }
    }
}
