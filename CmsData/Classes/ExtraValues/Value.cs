using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using UtilityExtensions;

namespace CmsData.ExtraValue
{
    // The specs for a single Standard Extra Value Field
    public class Value
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Type { get; set; }
        [XmlAttribute] public string VisibilityRoles { get; set; }
        [XmlAttribute] public string Link { get; set; }

        [XmlElement("Code")]
        public List<string> Codes { get; set; }


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
            var user = HttpContext.Current.User;
            return a.Any(role => user.IsInRole(role.Trim()));
        }
        public bool UserCanEdit()
        {
            var user = HttpContext.Current.User;
            return user.IsInRole("Edit");
        }

    }
}