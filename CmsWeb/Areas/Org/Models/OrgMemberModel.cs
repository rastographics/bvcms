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
        private int? _orgId;
        private int? _peopleId;

        private void Populate()
        {
            var i = (from mm in DbUtil.Db.OrganizationMembers
                     where mm.OrganizationId == _orgId && mm.PeopleId == _peopleId
                     select new
                         {
                             mm,
                             mm.Person.Name,
                             mm.Organization.OrganizationName,
                             mm.Organization.RegSetting,
                             mm.Organization,
                             mm.OrgMemMemTags
                         }).SingleOrDefault();
            if (i == null)
                throw new Exception("missing OrgMember at oid={0}, pid={0}".Fmt(_orgId, _peopleId));
            om = i.mm;
            this.CopyPropertiesFrom(om);
            Name = i.Name;
            OrgName = i.OrganizationName;
            Organization = i.Organization;
            OrgMemMemTags = i.OrgMemMemTags.ToList();
            Setting = new Settings(i.RegSetting, DbUtil.Db, _orgId.Value);
        }


        public int? OrgId
        {
            get { return _orgId; }
            set
            {
                _orgId = value;
                if(_peopleId.HasValue)
                    Populate();
            }
        }

        [SkipField]
        public int? PeopleId
        {
            get { return _peopleId; }
            set
            {
                _peopleId = value;
                if(_orgId.HasValue)
                    Populate();
            }
        }

        public string Name { get; set; }
        public string OrgName { get; set; }

        public string AttendStr { get; set; }

        public Settings Setting { get; set; }

        public CodeInfo MemberType { get; set; }

        public DateTime? InactiveDate { get; set; }

        [DisplayName("Enrollment Date")]
        public DateTime? Enrollment { get; set; }

        public bool? Pending { get; set; }

        public string RegisterEmail { get; set; }

        public string Request  { get; set; }

        public int? Grade { get; set; }

        public int? Tickets { get; set; }

        public decimal? Amount { get; set; }

        public decimal? AmountDue { get { return om.AmountDue(DbUtil.Db); } }

        public string PayLink { get; set; }

        public string ShirtSize { get; set; }

        [DisplayName("Extra Member Info")]
        public string UserData { get; set; }

        public void UpdateModel()
        {
            this.CopyPropertiesTo(om);
            DbUtil.Db.SubmitChanges();
        }
    }
}
