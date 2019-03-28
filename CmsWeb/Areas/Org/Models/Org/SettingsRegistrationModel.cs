using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Code;
using MarkdownDeep;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgAttribute : Attribute { }
    public class RegAttribute : Attribute { }

    public class SettingsRegistrationModel
    {
        public Organization Org;
        private CMSDataContext Db;
        public int Id
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                    Org = Db.LoadOrganizationById(value);
            }
        }

        public List<OrgPickInfo> OrganizationsFromIdString
        {
            get
            {
                return organizationsFromIdString(Org, Db);
            }
        }           

        public SettingsRegistrationModel()
        {
        }

        public SettingsRegistrationModel(int id, CMSDataContext db)
        {
            Db = db;
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
            var os = Db.CreateRegistrationSettings(RegSettings.ToString(), Id);
            if (Org.RegistrationTypeId > 0)
                if (!Org.NotifyIds.HasValue())
                    Org.NotifyIds = Util.UserPeopleId.ToString();
            Org.UpdateRegSetting(os);
            Db.SubmitChanges();
        }

        private Settings RegSettings => _regsettings ?? (_regsettings = Db.CreateRegistrationSettings(Id));
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

        private static List<OrgPickInfo> organizationsFromIdString(Organization Org, CMSDataContext Db)
        {            
            var a = Org.OrgPickList.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
            var n = 0;
            var d = a.ToDictionary(i => n++);
            var q = (from o in Db.Organizations
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

        [Org, Display(Description = RegistrationTypeDescription)]
        public CodeInfo RegistrationType { get; set; }


        [Org, Display(Description = ClassFilledDescription)]
        public bool ClassFilled { get; set; }


        [Org, Display(Description = RegistrationClosedDescription)]
        public bool RegistrationClosed { get; set; }


        [Org, Display(Name = "Registration Start", Description = RegStartDescription), UIHint("DateAndTime")]
        public DateTime? RegStart { get; set; }


        [Org, Display(Name = "Registration End", Description = RegEndDescription), UIHint("DateAndTime")]
        public DateTime? RegEnd { get; set; }

        [Display(Description = MasterOrgDescription), UIHint("MasterOrgInfo")]
        public MasterOrgInfo MasterOrg
        {
            get
            {
                if (masterOrg != null)
                    return masterOrg;

                var q = from o in Db.ViewMasterOrgs
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

        [Org, Display(Name = "Organization Pick List", Description = PickListDescription), UIHint("OrgPickList")]
        public SettingsRegistrationModel PickList { get { return this; } }

        [Org, Display(Name = "Max Limit", Description = LimitDescription)]
        public int? Limit { get; set; }

        [Org, Display(Name = "AddToSmallGroup Script Name", Description = AddToSmallGroupScriptDescription)]
        public string AddToSmallGroupScript { get; set; }

        [Reg, Display(Name = "OnEnroll Script Name", Description = OnEnrollScriptDescription)]
        public string OnEnrollScript { get; set; }

        [Org, Display(Description = TitleDescription)]
        public string RegistrationTitle { get; set; }

        [Reg, Display(Name= "Validate Orgs", Description = ValidateOrgsDescription)]
        public string ValidateOrgs { get; set; }

        [Reg, Display(Description = AgeGroupsDescription), UIHint("AgeGroups")]
        public List<Settings.AgeGroup> AgeGroups
        {
            get { return ageGroups ?? new List<Settings.AgeGroup>(); }
            set { ageGroups = value; }
        }
        private List<Settings.AgeGroup> ageGroups;

        [Reg, Display(Name="Other Org Id to Join", Description = GroupToJoinDescription)]
        public string GroupToJoin { get; set; }

        [Reg, Display(Description = AllowOnlyOneDescription)]
        public bool AllowOnlyOne { get; set; }

        [Reg, Display(Description = AddAsProspectDescription)]
        public bool AddAsProspect { get; set; }

        [Reg, Display(Name = "Allow Re-Register", Description = AllowReRegisterDescription)]
        public bool AllowReRegister { get; set; }

        [Reg, Display(Description = AllowSaveProgressDescription)]
        public bool AllowSaveProgress { get; set; }

        [Reg, Display(Description = NoReqBirthYearDescription)]
        public bool NoReqBirthYear { get; set; }

        [Reg, Display(Description = NotReqDobDescription)]
        public bool NotReqDOB { get; set; }

        [Reg, Display(Description = NotReqAddrDescription)]
        public bool NotReqAddr { get; set; }

        [Reg, Display(Description = NotReqZipDescription)]
        public bool NotReqZip { get; set; }

        [Reg, Display(Description = NotReqPhoneDescription)]
        public bool NotReqPhone { get; set; }

        [Reg, Display(Description = NotReqGenderDescription)]
        public bool NotReqGender { get; set; }

        [Reg, Display(Description = NotReqMaritalDescription)]
        public bool NotReqMarital { get; set; }

        [Reg, Display(Description = NotReqCampusDescription)]
        public bool NotReqCampus { get; set; }

        [Reg, Display(Description = MemberOnlyDescription)]
        public bool MemberOnly { get; set; }

        [Reg, Display(Name = "HTML Shell", Description = ShellBsDescription)]
        public string ShellBs { get; set; }

        [Reg, Display(Description = SpecialScriptDescription)]
        public string SpecialScript { get; set; }

        [Reg, Display(Description = TimeOutDescription)]
        public int? TimeOut { get; set; }

        [Reg, Display(Description = DisallowAnonymousDescription)]
        public bool DisallowAnonymous { get; set; }

        [Reg, Display(Description = FinishRegistrationButtonDescription)]
        public string FinishRegistrationButton { get; set; }

        #region Descriptions

        private const string RegistrationTypeDescription = @"
* JoinOrganization: for a single organization registration
* AttendMeeting: NOT WORKING YET
* UserSelectsOrganization: good for a single registration for multiple orgs (classes)
* ComputeOrganizationByAge: good for recreation leagues. Uses Birthday and Gender
* ManageSubscriptions: allows to select multiple organizations
* OnlineGiving: creates contribution records
* OnlinePledge: allows making pledges and checking on pledge status, increasing pledge too.
";
        private const string ClassFilledDescription = @"
You should use this when you want your organization to display the 'sorry' message.
It will also remove an organization from the available org dropdown.
";
        private const string RegistrationClosedDescription = @"
You should use this when you need to manually end your registration.
This is the preferred way instead of changing the Registration Type.
";
        private const string RegStartDescription = @"
Your registration will become **available** on this date and time *(Central Time Zone)*
";
        private const string RegEndDescription = @"
Your registration will become **unavailable** on this date and time *(Central Time Zone)*
";
        private const string MasterOrgDescription = @"
This is the master organization which serves as the entrypoint for a group of sub-registrations like this one
";
        private const string PickListDescription = @"
This is used on the master organization and will become the dropdown for 'User Chooses Organization'.
";
        private const string LimitDescription = @"
This will cause the class to go into a 'class filled' state when the number of members reaches this point.
";
        private const string AddToSmallGroupScriptDescription = @"
This is the name of a script in *Special Content > Python Scripts* that will be run when someone is added to a sub-group during a registration.
";
        private const string OnEnrollScriptDescription = @"
This is the name of a script in *Special Content > Python Scripts* that will be run when someone is enrolled during a registration.
";
        private const string TitleDescription = @"Leave blank to use the name of the organization.";
        private const string ValidateOrgsDescription = @"
A registrant must be in one of these organizations before registering.
Use a comma separated list of OrgIds.
";
        private const string AgeGroupsDescription = @"
This will put registrant in a sub-group based on their age,
with an optional age-based fee
";
        private const string GroupToJoinDescription = @"Joins registrant to another organization at the same time. ";
        private const string AllowOnlyOneDescription = @"Does not offer the 'Add other Registrations' option. ";
        private const string AddAsProspectDescription = @"Add Registrant as a Prospect. ";

        private const string AllowReRegisterDescription =
            @"Allows a person to be a member of Organization and register again. ";

        private const string AllowSaveProgressDescription =
            @"Allows a person to leave the registration and come back to finish later. ";

        private const string NoReqBirthYearDescription =
            @"Registration does not require a Birth Year, just month and day ";

        private const string NotReqDobDescription = @"Regisration does not require a birthday ";
        private const string NotReqAddrDescription = @"Regisration does not require an address ";
        private const string NotReqZipDescription = @"Regisration does not require a a zipcode ";
        private const string NotReqPhoneDescription = @"Registration does not require any phone number ";
        private const string NotReqGenderDescription = @"Registration does not reqire a gender ";
        private const string NotReqMaritalDescription = @"Registration does not require a Marital status ";
        private const string NotReqCampusDescription = @"
Registration does not require Campus

For this to work, your database must have the setting ShowCampusOnRegistration=true
";
        private const string MemberOnlyDescription = @"You must be a member of the church to register ";

        private const string ShellBsDescription =
            @"Enter the name of the HTML shell for this registration (stored in Special Content). ";

        private const string SpecialScriptDescription = @"
Enter the name of the HTML/Script file for this registration.
Only works with the Special Script type of registration (stored in Special Content).
";

        private const string TimeOutDescription = @"
Enter number of milliseconds (e.g. 600000 = 600 seconds = 10 minutes).

This overrides the default on just this organization.
The default is either the RegTimeout database setting if it exists 
or the system wide default of 180000 (3 minutes).
";

        private const string DisallowAnonymousDescription = @"
Does not allow Anonymous Registrations.
Must have an accont and be logged in
";

        private const string FinishRegistrationButtonDescription = @"
Allows you to set the text of the button used to Finish, Continue or Complete the registation.
Please keep it short because the text of the button is not the place to put help.
Long Buttons do not look good.
";

        #endregion
    }
}
