using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using CmsData.Classes.ExtraValues;
using CmsData.Classes.RoleChecker;
using UtilityExtensions;

namespace CmsData.ExtraValue
{
    // The specs for a single Standard Extra Value Field
    public class Value
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Type { get; set; }
        [XmlAttribute] public string VisibilityRoles { get; set; }
        [XmlAttribute] public string EditableRoles { get; set; }
        [XmlAttribute] public string Link { get; set; }

        [XmlElement("Code")]
        public List<Code> Codes { get; set; }


        [XmlIgnore] public int Order;
        [XmlIgnore] public int Id;
        [XmlIgnore] public bool Standard;

        public bool UserCanView(CMSDataContext db)
        {
            if (db.FromBatch)
                return true;
            if (!VisibilityRoles.HasValue())
                return true;
            var a = VisibilityRoles.SplitStr(",");
            var user = HttpContextFactory.Current?.User;
            if (user == null)
                return true;
            return a.Any(role => user.IsInRole(role.Trim()));
        }

        public bool UserCanEdit()
        {
            if (Type == "Attr")
                return false;
            var user = HttpContextFactory.Current?.User;
            if (user == null)
                return false;

            var path = HttpContextFactory.Current?.Request.Path;
            if (path != null && path.Contains("CommunityGroup"))
            {
                if (user.IsInRole("Edit"))
                    return true;

                if (RoleChecker.HasSetting(SettingName.CanEditCGInfoEVs, false))
                {
                    if (string.IsNullOrEmpty(EditableRoles))
                        return true;

                    var editableRoles = EditableRoles.SplitStr(",");
                    return editableRoles.Any(role => user.IsInRole(role.Trim()));
                }

                return false;
            }

            return user.IsInRole("Edit");
        }

    }
}
