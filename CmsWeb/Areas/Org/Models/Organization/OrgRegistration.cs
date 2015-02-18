using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Code;
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
            RegSettings.AgeGroups.Clear();
            if (Org.OrgPickList.HasValue() && Org.RegistrationTypeId == RegistrationTypeCode.JoinOrganization)
                Org.OrgPickList = null;
            this.CopyPropertiesTo(RegSettings, typeof(RegAttribute));
            var os = new Settings(RegSettings.ToString(), DbUtil.Db, Id);
            Org.RegSetting = os.ToString();
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
        public List<OrgPickInfo> OrganizationsFromIdString()
        {
            var a = Org.OrgPickList.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
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

        [Display(Description = @"
This is the parent Organization for a group of sub-registrations like this one
"), UIHint("MasterOrgInfo")]
        public MasterOrgInfo MasterOrg
        {
            get
            {
                if (masterOrg != null) 
                    return masterOrg;

                var q = from o in DbUtil.Db.ViewMasterOrgs
                    where o.PickListOrgId == Id
                    select new MasterOrgInfo
                    {
                        Id = o.OrganizationId,
                        Name = o.OrganizationName
                    };
                return masterOrg = q.FirstOrDefault() ?? new MasterOrgInfo();
            }
        }
        private MasterOrgInfo masterOrg;

        [Org, Display(Name = "Organization Pick List", Description = @"
This is used on the master organization and will become the dropdown for 'User Chooses Organization'.
"), UIHint("OrgPickList")]
        public OrgRegistration PickList { get { return this; } }

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
This will put registrant in a small group based on their age,
with an optional age-based fee
"), UIHint("AgeGroups")]
        public List<Settings.AgeGroup> AgeGroups
        {
            get { return ageGroups ?? new List<Settings.AgeGroup>(); }
            set { ageGroups = value; }
        }
        private List<Settings.AgeGroup> ageGroups;

        [Reg, Display(Description = @"
Joins registrant to another organization at the same time.
")]
        public string GroupToJoin { get; set; }

        [Reg, Display(Description = @"
Does not offer the 'Add other Registrations' option.
")]
        public bool AllowOnlyOne { get; set; }


        [Reg, Display(Description = @"
Add Registrant as a Prospect.
")]
        public bool AddAsProspect { get; set; }


        [Reg, Display(Name = "Allow re-Register", Description = @"
Allows a person to be a member of Organization and register again.
")]
        public bool AllowReRegister { get; set; }


        [Reg, Display(Description = @"
Allows a person to leave the registration and come back to finish later.
")]
        public bool AllowSaveProgress { get; set; }

        [Reg, Display(Description = @"
Registration does not require a Birth Year, just month and day
")]
        public bool NoReqBirthYear { get; set; }

        [Reg, Display(Description = @"
Regisration does not require a birthday
")]
        public bool NotReqDOB { get; set; }

        [Reg, Display(Description = @"
Regisration does not require an address
")]
        public bool NotReqAddr { get; set; }

        [Reg, Display(Description = @"
Regisration does not require a a zipcode
")]
        public bool NotReqZip { get; set; }

        [Reg, Display(Description = @"
Registration does not require any phone number
")]
        public bool NotReqPhone { get; set; }

        [Reg, Display(Description = @"
Registration does not reqire a gender
")]
        public bool NotReqGender { get; set; }

        [Reg, Display(Description = @"
Regisration does not require a Marital status
")]
        public bool NotReqMarital { get; set; }

        [Reg, Display(Description = @"
You must be a member of the church to register
")]
        public bool MemberOnly { get; set; }

        [Reg, Display(Name = "HTML Shell", Description = @"
Enter the name of the HTML shell for this registration (stored in Special Content).
")]
        public string ShellBs { get; set; }


        [Reg, Display(Description = @"
Enter the name of the HTML/Script file for this registration.
Only works with the Special Script type of registration (stored in Special Content).
")]
        public string SpecialScript { get; set; }

    }
}