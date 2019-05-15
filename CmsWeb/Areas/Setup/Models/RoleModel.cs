using System;
using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData.Classes.RoleChecker;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CmsWeb.Areas.Setup.Models
{
    public class RoleModel
    {
        private static XDocument RoleSettingDefaults
        {
            get { return XDocument.Parse(Resource1.RoleSettingDefaults); }
        }

        public List<Location> SettingsForRole(Role r)
        {

        }

        private Setting Default(string name)
        {
            var xdoc = RoleSettingDefaults;
            foreach(var e in xdoc.XPathSelectElements("/DefaultSettings").Elements())
            {
                return new Setting()
                {
                    Name = e.Attribute("name")?.Value
                };
            }
        }

        public class Location
        {
            public string Name { get; set; }
            public List<Setting> Settings { get; set; }
        }

        public class Setting
        {
            public string Name { get; set; }
            public string XMLName { get; set; }
            public string Location { get; set; }
            public bool Default { get; set; }
            public bool Active { get; set; }
            public string ToolTip { get; set; }
        }
    }
}
