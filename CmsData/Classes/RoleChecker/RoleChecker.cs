using System.IO;
using System.Web;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsData.Classes.RoleChecker
{
    public static class RoleChecker
    {
        private static XDocument Xdoc
        {
            get
            {
                const string CustomAccessRolesKey = "CustomAccessRoles.xml";
                try
                {
                    var doc = HttpContextFactory.Current.Items[CustomAccessRolesKey] as XDocument;
                    if (doc != null)
                    {
                        return doc;
                    }

                    var s = DbUtil.Content(CustomAccessRolesKey, "");
                    doc = s.HasValue()
                        ? XDocument.Load(new StringReader(DbUtil.Content(CustomAccessRolesKey, "")))
                        : new XDocument(); // avoid expensive catch when there there really is no error because document is empty
                    HttpContextFactory.Current.Items[CustomAccessRolesKey] = doc;
                    return doc;
                }
                catch
                {
                    var doc = new XDocument();
                    HttpContextFactory.Current.Items[CustomAccessRolesKey] = doc;
                    return doc;
                }
            }
        }
        private static XElement Roles => Xdoc.Element("roles");

        private static XElement Settings(XElement role)
        {
            return role.Element("settings");
        }

        public static bool HasSetting(SettingName setting, bool defaultValue)
        {
            var roles = Roles;

            if (roles != null)
            {
                foreach (var r in roles.Elements("role"))
                {
                    var roleName = r.Attribute("name");
                    if (!HttpContextFactory.Current.User.IsInRole(roleName?.Value))
                    {
                        continue;
                    }

                    foreach (var s in Settings(r).Elements())
                    {
                        var nameAttribute = s.Attribute("name");
                        var valueAttribute = s.Attribute("value");
                        if (nameAttribute?.Value == SettingNameAsString(setting))
                        {
                            bool value;
                            if (bool.TryParse(valueAttribute?.Value, out value))
                            {
                                return value;
                            }
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
        HidePreviousOrgMembers,
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
        OtherGroupsContentOnly
        // ReSharper restore InconsistentNaming
    }
}
