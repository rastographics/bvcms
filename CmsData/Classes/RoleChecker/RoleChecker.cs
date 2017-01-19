using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using IronPython.Modules;

namespace CmsData.Classes.RoleChecker
{
    public static class RoleChecker
    {
        private static XDocument xdoc
        {
            get
            {
                try
                {
                    return XDocument.Load(new StringReader(DbUtil.Content("CustomAccessRoles.xml", "")));
                }
                catch
                {
                    return new XDocument();
                }
            }
        }
        private static XElement roles => xdoc.Element("roles");

        private static XElement Settings(XElement role)
        {
            return role.Element("settings");
        }

        public static bool HasSetting(SettingName setting, bool defaultValue)
        {
            if (roles != null)
            {
                foreach (var r in roles.Elements("role"))
                {
                    var roleName = r.Attribute("name");
                    if (!HttpContext.Current.User.IsInRole(roleName?.Value)) continue;

                    foreach (var s in Settings(r).Elements())
                    {
                        var nameAttribute = s.Attribute("name");
                        var valueAttribute = s.Attribute("value");
                        if (nameAttribute?.Value == SettingNameAsString(setting))
                        {
                            bool value;
                            if (bool.TryParse(valueAttribute?.Value, out value)) return value;
                        }
                    }
                }
            }

            return defaultValue;
        }

        private static string SettingNameAsString(SettingName setting)
        {
            return setting
                    .ToString()
                    .Replace('_', '-');
        }
    }

    public enum SettingName
    {
        // ReSharper disable InconsistentNaming
        Person_ShowBlueToolbar,
        Organization_ShowSettingsTab,
        Organization_ShowBlueToolbar,
        Organization_ShowBlueToolbarFullEmailMenu,
        Organization_ShowBlueToolbarEmailMembers,
        Organization_ShowBlueToolbarEmailProspects,
        Organization_ShowBlueToolbarEmailMembersAndProspects,
        Organization_ShowBlueToolbarExportMenu,
        Organization_ShowBlueToolbarCustomReportsMenu,
        Organization_ShowBlueToolbarAdminGearMenu,
        Organization_ShowBlueToolbarSubGroupManagement,
        Organization_ShowBlueToolbarMembersOnlyPage,
        Organization_ShowBlueToolbarVolunteerCalendar,
        Organization_ShowOptionsMenu,
        Organization_ShowFiltersBar,
        Organization_CollapseOrgDetails,
        Organization_ShowBirthday,
        Organization_ShowAddress,
        Organization_ShowCreateNewMeeting,
        Organization_ShowDeleteMeeting,
        Organization_ShowTagButtons,
        Meeting_HyperlinkNames,
        Meeting_ShowAddGuest,
        Meeting_AllowEditDescription,
        Meeting_ShowExtraValuesBox,
        Meeting_ShowWandTargetBox,
        Meeting_ShowBlueToolbar,
        Meeting_ShowBlueToolbarIpadAttendance,
        Meeting_ShowBlueToolbarRollsheet,
        Meeting_EnableEditByDefault,
        Meeting_ShowEnableBox,
        Meeting_ShowShowBox,
        Meeting_ShowAttendType,
        Meeting_ShowOtherAttend,
        Meeting_ShowCurrentMemberType,
        AutoOrgLeaderPromotion,
        DisableHomePage,
        DisablePersonLinks,
        HideEmailDetails,
        HideExtraValueEdit,
        HideGuestsOrgMembers,
        HideInactiveOrgMembers,
        HideMinistryTab,
        HideNavTabs,
        HidePendingOrgMembers,
        HideQueries,
        LeadersCanAlwaysEditOrgContent,
        LimitToolbar,
        LimitedSearchPerson,
        CanEditCGInfoEVs,
        EditMemberData,
        OrgMembersDropAdd,
        OtherGroupsContentOnly,
        ShowChildOrgsOnInvolvementTabs,
        ShowParentOrgInDetails
        // ReSharper restore InconsistentNaming
    }
}
