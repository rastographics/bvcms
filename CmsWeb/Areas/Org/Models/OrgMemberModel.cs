using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgMemberModel
    {
        private OrganizationMember om;
        public CmsData.Organization Organization;
        public List<OrgMemMemTag> OrgMemMemTags;
        public bool IsMissionTrip;

        public OrgMemberModel()
        {
        }

        public OrgMemberModel(int oid, int pid)
        {
            OrgId = oid;
            PeopleId = pid;
            Populate();
        }
        private void Populate()
        {
            var i = (from mm in DbUtil.Db.OrganizationMembers
                     where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                     select new
                         {
                             mm,
                             mm.Person.Name,
                             mm.Organization.OrganizationName,
                             mm.Organization.RegSetting,
                             mm.Organization,
                             mm.OrgMemMemTags,
                             mm.Organization.IsMissionTrip
                         }).SingleOrDefault();
            if (i == null)
                throw new Exception("missing OrgMember at oid={0}, pid={0}".Fmt(OrgId, PeopleId));
            om = i.mm;
            this.CopyPropertiesFrom(om);
            Name = i.Name;
            AmountPaid = om.TotalPaid(DbUtil.Db);
            OrgName = i.OrganizationName;
            Organization = i.Organization;
            OrgMemMemTags = i.OrgMemMemTags.ToList();
            IsMissionTrip = i.IsMissionTrip ?? false;
            Setting = new Settings(i.RegSetting, DbUtil.Db, OrgId.Value);
        }


        public int? OrgId { get; set; }

        [SkipFieldOnCopyProperties]
        public int? PeopleId { get; set; }

        public string Name { get; set; }
        public string OrgName { get; set; }

        public string AttendStr { get; set; }

        public Settings Setting { get; set; }

        public CodeInfo MemberType { get; set; }

        public DateTime? InactiveDate { get; set; }

        [DisplayName("Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }

        public bool Pending { get; set; }

        public string RegisterEmail { get; set; }

        public string Request  { get; set; }

        public int? Grade { get; set; }

        public int? Tickets { get; set; }

        [DisplayName("Total Amount")]
        public decimal? Amount { get; set; }

        [DisplayName("Amount Paid")]
        public decimal? AmountPaid { get; set; }

        public decimal? AmountDue { get { return om.AmountDue(DbUtil.Db); } }

        public string PayLink { get; set; }

        public string ShirtSize { get; set; }

        [DisplayName("Extra Member Info")]
        public string UserData { get; set; }

        public void UpdateModel()
        {
            om = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);
            this.CopyPropertiesTo(om);
            DbUtil.Db.SubmitChanges();
            Populate();
        }
    }
}
