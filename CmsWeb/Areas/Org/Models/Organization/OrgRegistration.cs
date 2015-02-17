using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgAttribute : Attribute { }
    public class RegAttribute : Attribute { }

    public class OrgRegistration
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

        public OrgRegistration()
        {
        }

        public OrgRegistration(int id)
        {
            Id = id;
            this.CopyPropertiesFrom(Org, typeof(OrgAttribute));
            this.CopyPropertiesFrom(RegSettings, typeof(RegAttribute));
        }
        public void Update()
        {
            this.CopyPropertiesTo(Org, typeof(OrgAttribute));
            this.CopyPropertiesTo(RegSettings, typeof(RegAttribute));
            DbUtil.Db.SubmitChanges();
        }

        private Settings RegSettings
        {
            get { return _regsettings ?? (_regsettings = new Settings(Org.RegSetting, DbUtil.Db, Id)); }
        }
        private Settings _regsettings;

        public class MasterOrgInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class OrgPickInfo
        {
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; }
        }
        public List<OrgPickInfo> OrganizationsFromIdString(Organization org)
        {
            var a = org.OrgPickList.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
            var n = 0;
            var d = a.ToDictionary(i => n++);
            var q = (from o in DbUtil.Db.Organizations
                     where a.Contains(o.OrganizationId)
                     select new OrgPickInfo
                     {
                         OrganizationId = o.OrganizationId,
                         OrganizationName = o.OrganizationName
                     }).ToList();
            var list = (from op in q
                        join i in d on op.OrganizationId equals i.Value into j
                        from i in j
                        orderby i.Key
                        select op).ToList();
            return list;
        }

        [Org, Display(Description = @"
* JoinOrganization: for a single organization registration
* AttendMeeting: NOT WORKING YET
* UserSelectsOrganization: good for a single registration for multiple orgs (classes)
* ComputeOrganizationByAge: good for recreation leagues. Uses Birthday and Gender
* ManageSubscriptions: allows to select multiple organizations
* OnlineGiving: creates contribution records
* OnlinePledge: allows making pledges and checking on pledge status, increasing pledge too.
")]
        public CodeInfo RegistrationType { get; set; }


        [Org, Display(Description = @"
You should use this when you want your organization to display the 'sorry' message.
It will also remove an organization from the available org dropdown.
")]
        public bool ClassFilled { get; set; }


        [Org, Display(Description = @"
You should use this when you need to manually end your registration.
This is the preferred way instead of changing the Registration Type.
")]
        public bool RegistrationClosed { get; set; }


        [Org, Display(Name = "Registration Start", Description = @"
Your registration will become **available** on this date and time *(Central Time Zone)*
"), UIHint("DateAndTime")]
        public DateTime? RegStart { get; set; }


        [Org, Display(Name = "Registration End", Description = @"
Your registration will become **unavailable** on this date and time *(Central Time Zone)*
"), UIHint("DateAndTime")]
        public DateTime? RegEnd { get; set; }

        [Org, Display(Name = "Registration End", Description = @"
This is used on the master organization and will become the dropdown for 'User Chooses Organization'.
")]
        public MasterOrgInfo MasterOrg
        {
            get
            {
                if (_masterOrg != null) 
                    return _masterOrg;

                var q = from o in DbUtil.Db.ViewMasterOrgs
                    where o.PickListOrgId == Id
                    select new MasterOrgInfo
                    {
                        Id = o.OrganizationId,
                        Name = o.OrganizationName
                    };
                return _masterOrg = q.FirstOrDefault() ?? new MasterOrgInfo();
            }
        }
        private MasterOrgInfo _masterOrg;

        [Org, Display(Name = "Organization Pick List", Description = @"
This is used on the master organization and will become the dropdown for 'User Chooses Organization'.
"), UIHint("OrgPickList")]
        public string OrgPickList { get; set; }

        [Org, Display(Name = "Max Limit", Description = @"
This will cause the class to go into a 'class filled' state when the number of members reaches this point.
")]
        public int? Limit { get; set; }


        [Org, Display(Name = "OnRegister Script Name", Description = @"
This is the name of a script in *Special Content > Python Scripts* that will be run when someone registers.
")]
        public string AddToSmallGroupScript { get; set; }




        [Reg, Display(Name = "Registration Title", Description = @"
Leave blank to use the name of the organization.
")]
        public string Title { get; set; }


        [Reg, Display(Description = @"
A registrant must be in one of these organizations before registering.
Use a comma separated list of OrgIds.
")]
        public string VaidateOrgs { get; set; }


        [Reg, Display(Description = @"
Joins registrant to another organization at the same time.
")]
        public string GroupToJoin { get; set; }

        [Reg, Display(Name = "", Description = @"
Does not offer the 'Add other Registrations' option.
")]
        public bool AllowOnlyOne { get; set; }


        [Reg, Display(Name = "", Description = @"
Add Registrant as a Prospect.
")]
        public bool AddAsProspect { get; set; }


        [Reg, Display(Name = "", Description = @"
Allows a person to be a member of Organization and register again.
")]
        public bool AllowReRegister { get; set; }


        [Reg, Display(Name = "", Description = @"
Allows a person to leave the registration and come back to finish later.
")]
        public bool AllowSaveProgress { get; set; }


        public bool NoReqBirthYear { get; set; }
        public bool NoReqDOB { get; set; }
        public bool NoReqAddr { get; set; }
        public bool NoReqZip { get; set; }
        public bool NoReqPhone { get; set; }
        public bool NoReqGender { get; set; }
        public bool NoReqMarital { get; set; }
        public bool MemberOnly { get; set; }

        [Reg, Display(Name = "HTML Shell", Description = @"
Enter the name of the HTML shell for this registration (stored in Special Content).
")]
        public string ShellBs { get; set; }


        [Reg, Display(Name = "", Description = @"
Enter the name of the HTML/Script file for this registration.
Only works with the Special Script type of registration (stored in Special Content).
")]
        public string SpecialScript { get; set; }

    }
}