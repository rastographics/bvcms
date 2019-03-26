using CmsData;
using CmsData.Classes.RoleChecker;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;
using Settings = CmsData.Registration.Settings;

namespace CmsWeb.Areas.Org.Models
{
    public class OrganizationModel : OrgPeopleModel
    {
        public bool IsVolunteerLeader { get; set; }

        public OrganizationModel()
        {
        }
        public OrganizationModel(int id)
        {
            var filter = DbUtil.Db.NewOrgFilter(id);
            filter.CopyPropertiesTo(this);
            QueryId = filter.QueryId;
            populate(id);
        }

        public OrgMain OrgMain { get; set; }

        private SettingsGeneralModel settingsGeneralModel;
        public SettingsGeneralModel SettingsGeneralModel
        {
            get
            {
                if (settingsGeneralModel == null && Id.HasValue)
                {
                    settingsGeneralModel = new SettingsGeneralModel(Id.Value);
                }

                return settingsGeneralModel;
            }
        }
        private SettingsRegistrationModel settingsRegistrationModel;
        public SettingsRegistrationModel SettingsRegistrationModel
        {
            get
            {
                if (settingsRegistrationModel == null && Id.HasValue)
                {
                    settingsRegistrationModel = new SettingsRegistrationModel(Id.Value, DbUtil.Db);
                }

                return settingsRegistrationModel;
            }
        }

        private void populate(int id)
        {
            Id = id;
            DbUtil.Db.SetCurrentOrgId(id);
            if (Org == null)
            {
                return;
            }

            OrgMain = new OrgMain(Org);
            GroupSelect = GroupSelectCode.Member;
            IsVolunteerLeader = OrganizationMember.VolunteerLeaderInOrg(DbUtil.Db, Id);
        }

        public IEnumerable<SelectListItem> Groups()
        {
            var q = from g in DbUtil.Db.MemberTags
                    where g.OrgId == Id
                    orderby g.Name
                    select new SelectListItem
                    {
                        Text = g.Name,
                        Value = g.Id.ToString()
                    };
            return q;
        }
        public static IEnumerable<SearchDivision> Divisions(int? id)
        {
            var q = from d in DbUtil.Db.SearchDivisions(id, null)
                    where d.IsChecked == true
                    orderby d.IsMain descending, d.IsChecked descending, d.Program, d.Division
                    select d;
            return q;
        }

        public static string SpaceCamelCase(string s)
        {
            return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
        private Settings _RegSettings;
        public Settings RegSettings => _RegSettings ?? (_RegSettings = DbUtil.Db.CreateRegistrationSettings(Org.OrganizationId));

        internal bool? _showRegistrationTab;
        public bool ShowRegistrationTab()
        {
            if (_showRegistrationTab.HasValue)
            {
                return _showRegistrationTab.Value;
            }

            var typeName = OrgMain.OrganizationType.ToString().Replace(" ", "");

            if (HttpContextFactory.Current.User.IsInRole("OrgLeadersOnly") &&
                DbUtil.Db.Setting($"UX-HideRegistrationTabForOrgLeaders-{typeName}"))
            {
                _showRegistrationTab = false;
            }
            else
            {
                _showRegistrationTab = true;
            }

            return _showRegistrationTab.Value;
        }

        private bool? showContactsReceivedTab;
        public bool ShowContactsReceivedTab()
        {
            // Simple caching to avoid repeating all this logic since this is called twice
            if (showContactsReceivedTab.HasValue)
            {
                return showContactsReceivedTab.Value;
            }

            // Check and see if org visits are turned on at all
            showContactsReceivedTab = DbUtil.Db.Setting("UseContactVisitedOrgs");
            if (!showContactsReceivedTab.Value)
            {
                return false;
            }

            // Check and see if this is the wrong type of org
            var orgType = DbUtil.Db.Setting("UX-ContactedOrgType", string.Empty);
            if (!string.IsNullOrEmpty(orgType))
            {
                showContactsReceivedTab = OrgMain.OrganizationType.ToString() == orgType;
                if (!showContactsReceivedTab.Value)
                {
                    return false;
                }
            }

            // Check and see if the user doesn't have access to the tab
            var accessTypeList = DbUtil.Db.Setting("UX-VisitedOrgTabMemberTypes", null);
            if (!string.IsNullOrEmpty(accessTypeList))
            {
                var user = DbUtil.Db.Users.FirstOrDefault(x => x.Username == HttpContextFactory.Current.User.Identity.Name);
                if (user?.PeopleId == null)
                {
                    showContactsReceivedTab = false;
                    return false;
                }

                var memberTypes = accessTypeList.Split(',').Select(x => x.Trim()).ToList();

                showContactsReceivedTab = ShowContactsCheckOrgAndParents(Org, user.PeopleId.Value, memberTypes);
            }
            // Finally check to see if user is in CG role
            if (!showContactsReceivedTab.Value)
            {
                showContactsReceivedTab = HttpContextFactory.Current.User.IsInRole("CG");
            }
            return showContactsReceivedTab.Value;
        }

        public bool ShowBlueToolbar => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbar, true);
        public bool ShowBlueToolbarFullEmail => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarFullEmailMenu, true);
        public bool ShowBlueToolbarEmailMembers => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarEmailMembers, true);
        public bool ShowBlueToolbarEmailProspects => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarEmailProspects, true);
        public bool ShowBlueToolbarEmailMembersAndProspects => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarEmailMembersAndProspects, true);
        public bool ShowBlueToolbarExports => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarExportMenu, true);
        public bool ShowBlueToolbarCustomReports => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarCustomReportsMenu, true);
        public bool ShowBlueToolbarAdminGear => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarAdminGearMenu, true);
        public bool ShowBlueToolbarMembersOnlyPage => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarMembersOnlyPage, true);
        public bool ShowBlueToolbarVolunteerCalendar => RoleChecker.HasSetting(SettingName.Organization_ShowBlueToolbarVolunteerCalendar, true);

        public bool ShowSettingsTab => RoleChecker.HasSetting(SettingName.Organization_ShowSettingsTab, true);

        public bool ShowMeetingsTab
        {
            get
            {
                if (!HttpContextFactory.Current.User.IsInRole("OrgLeadersOnly"))
                {
                    return true;
                }

                var typeName = OrgMain.OrganizationType.ToString().Replace(" ", "");
                return !DbUtil.Db.Setting($"UX-HideMeetingsTabForOrgLeaders-{typeName}");
            }
        }

        private bool ShowContactsCheckOrgAndParents(Organization org, int peopleId, List<string> memberTypes)
        {
            var om = DbUtil.Db.OrganizationMembers
                    .FirstOrDefault(x => x.OrganizationId == org.OrganizationId && x.PeopleId == peopleId);

            bool result = false;
            if (om != null)
            {
                result = memberTypes.Contains(om.MemberType.Description);
            }

            if (!result && org.ParentOrgId.HasValue)
            {
                result = ShowContactsCheckOrgAndParents(org.ParentOrg, peopleId, memberTypes);
            }

            return result;
        }

        private bool? showCommunityGroupTab;
        public bool ShowCommunityGroupTab()
        {
            if (showCommunityGroupTab.HasValue)
            {
                return showCommunityGroupTab.Value;
            }

            showCommunityGroupTab = DbUtil.Db.Setting("ShowCommunityGroupTab");
            if (!showCommunityGroupTab.Value)
            {
                return false;
            }

            var orgTypes = DbUtil.Db.Setting("UX-CommunityGroupTabOrgTypes", "");
            if (!string.IsNullOrEmpty(orgTypes))
            {
                showCommunityGroupTab = Org.OrganizationType != null && orgTypes.Split(',').Select(x => x.Trim()).Contains(Org.OrganizationType.Description);
            }

            return showCommunityGroupTab.Value;
        }
    }
}
