using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMembers
    {
        public int orgid { get; set; }
        public bool inactives { get; set; }
        public bool pendings { get; set; }
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

        public IList<int> List { get; set; } = new List<int>();

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

        private string type()
        {
            if (pendings)
            {
                return "UpdatePending";
            }
            else if (inactives)
            {
                return "UpdateInactive";
            }

            return "UpdateMembers";
        }
        public string title()
        {
            if (pendings)
            {
                return "Update Pending Members";
            }
            else if (inactives)
            {
                return "Update Inactive Members";
            }

            return "Update Members";
        }
        public string HelpLink()
        {
            return Util.HelpLink($"UpdateOrgMember_{type()}");
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
            public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
            public int? BirthYear { get; set; }
            public int? BirthMon { get; set; }
            public int? BirthDay { get; set; }
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

            public string ToolTip => $"{Name} ({PeopleId})|Cell Phone: {CellPhone}|Work Phone: {WorkPhone}|Home Phone: {HomePhone}|BirthDate: {BirthDate:d}|Join Date: {JoinDate:d}|Status: {MemberStatus}|Email: {Email}";
        }
    }
}
