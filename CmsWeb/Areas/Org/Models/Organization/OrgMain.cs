using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgMain
    {
        public Organization Org;
        public int Id 
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                    Org = DbUtil.Db.LoadOrganizationById(value);
            }
        }
        public int? LeaderId { get { return Org.LeaderId; }  }
        public string LeaderName { get { return Org.LeaderName; } }

        public string OrganizationName { get; set; }
        public CodeInfo Campus { get; set; }
        public CodeInfo LeaderMemberType { get; set; }
        public CodeInfo OrganizationType { get; set; }
        public CodeInfo SecurityType { get; set; }
        public CodeInfo OrganizationStatus { get; set; }
        public bool? IsBibleFellowshipOrg { get; set; }

        [NoUpdate]
        public string Schedule { get; set; }

        public void Update()
        {
            this.CopyPropertiesTo(Org);
            DbUtil.Db.SubmitChanges();
        }
    }
}