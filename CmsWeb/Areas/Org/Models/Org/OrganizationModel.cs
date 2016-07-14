using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text.RegularExpressions;
using CmsData.Codes;

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
            populate(id);
        }
        public OrgMain OrgMain { get; set; }

        private SettingsGeneralModel settingsGeneralModel;
        public SettingsGeneralModel SettingsGeneralModel
        {
            get
            {
                if(settingsGeneralModel == null && Id.HasValue)
                    settingsGeneralModel = new SettingsGeneralModel(Id.Value);
                return settingsGeneralModel;
            }
        }
        private SettingsRegistrationModel settingsRegistrationModel;
        public SettingsRegistrationModel SettingsRegistrationModel
        {
            get
            {
                if(settingsRegistrationModel == null && Id.HasValue)
                    settingsRegistrationModel = new SettingsRegistrationModel(Id.Value);
                return settingsRegistrationModel;
            }
        }

        private void populate(int id)
        {
            Id = id;
            DbUtil.Db.SetCurrentOrgId(id);
            if (Org == null)
                return;
            OrgMain = new OrgMain(Org);
            GroupSelect = GroupSelectCode.Member;
            IsVolunteerLeader = OrganizationMember.VolunteerLeaderInOrg(DbUtil.Db, Id);
        }

        private CodeValueModel cv = new CodeValueModel();

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

//        public IEnumerable<SelectListItem> CampusList()
//        {
//            return CodeValueModel.ConvertToSelect(cv.AllCampuses0(), "Id");
//        }
//        public IEnumerable<SelectListItem> EntryPointList()
//        {
//            return CodeValueModel.ConvertToSelect(cv.EntryPoints(), "Id");
//        }
//        public IEnumerable<SelectListItem> GenderList()
//        {
//            return CodeValueModel.ConvertToSelect(cv.GenderCodes(), "Id");
//        }
        public static string SpaceCamelCase(string s)
        {
            return Regex.Replace(s, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
        private Settings _RegSettings;
        public Settings RegSettings => _RegSettings ?? (_RegSettings = DbUtil.Db.CreateRegistrationSettings(Org.OrganizationId));

        internal bool? _showRegistrationTab;
        public bool ShowRegistrationTab()
        {
            if (_showRegistrationTab.HasValue) return _showRegistrationTab.Value;

            var typeName = OrgMain.OrganizationType.ToString().Replace(" ", "");

            if (OrgMain.OrganizationType.ToString() != "Community Group")
                _showRegistrationTab = true;
            else if (HttpContext.Current.User.IsInRole("OrgLeadersOnly") &&
                     DbUtil.Db.Setting($"UX-HideRegistrationTabForOrgLeaders-{typeName}", false))
                _showRegistrationTab = false;
            else
                _showRegistrationTab = true;

            return _showRegistrationTab.Value;
        }

        internal bool? _showContactsReceivedTab;
        public bool ShowContactsReceivedTab()
        {
            // Simple caching to avoid repeating all this logic since this is called twice
            if (_showContactsReceivedTab.HasValue) return _showContactsReceivedTab.Value;

            // Check and see if org visits are turned on at all
            _showContactsReceivedTab = DbUtil.Db.Setting("UseContactVisitedOrgs", false);
            if (!_showContactsReceivedTab.Value) return false;

            // Check and see if this is the wrong type of org
            var orgType = DbUtil.Db.Setting("UX-ContactedOrgType", string.Empty);
            if (!string.IsNullOrEmpty(orgType))
            {
                _showContactsReceivedTab = OrgMain.OrganizationType.ToString() == orgType;
                if (!_showContactsReceivedTab.Value) return false;
            }

            // Check and see if the user doesn't have access to the tab
            var accessTypeList = DbUtil.Db.Setting("UX-VisitedOrgTabMemberTypes", null);
            if (!string.IsNullOrEmpty(accessTypeList))
            {
                var user = DbUtil.Db.Users.FirstOrDefault(x => x.Username == HttpContext.Current.User.Identity.Name);
                if (user?.PeopleId == null)
                {
                    _showContactsReceivedTab = false;
                    return false;
                }

                var memberTypes = accessTypeList.Split(',').Select(x => x.Trim()).ToList();

                _showContactsReceivedTab = ShowContactsCheckOrgAndParents(Org, user.PeopleId.Value, memberTypes);
            }
            else
            {
                _showContactsReceivedTab = !HttpContext.Current.User.IsInRole("CG");
            }

            return _showContactsReceivedTab.Value;
        }

        public bool ShowMeetingsTab
        {
            get
            {
                if (!HttpContext.Current.User.IsInRole("OrgLeadersOnly"))
                    return true;

                var typeName = OrgMain.OrganizationType.ToString().Replace(" ", "");
                return !DbUtil.Db.Setting($"UX-HideMeetingsTabForOrgLeaders-{typeName}", false);
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

        internal bool? _showCommunityGroupTab;
        public bool ShowCommunityGroupTab()
        {
            if (_showCommunityGroupTab.HasValue) return _showCommunityGroupTab.Value;

            _showCommunityGroupTab = DbUtil.Db.Setting("ShowCommunityGroupTab", false);
            if (!_showCommunityGroupTab.Value) return false;

            var orgTypes = DbUtil.Db.Setting("UX-CommunityGroupTabOrgTypes", "");
            if (!string.IsNullOrEmpty(orgTypes))
            {
                _showCommunityGroupTab = Org.OrganizationType != null && orgTypes.Split(',').Select(x => x.Trim()).Contains(Org.OrganizationType.Description);
            }

            return _showCommunityGroupTab.Value;
        }
    }
}
