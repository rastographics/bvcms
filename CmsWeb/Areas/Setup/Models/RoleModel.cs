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
            var xdoc = RoleSettingDefaults;
            var locations = new List<Location>();
            foreach (var e in xdoc.XPathSelectElements("/DefaultSettings").Elements())
            {
                var location = e.Attribute("name")?.Value;
                var settings = new List<Setting>();
                var loc = new Location()
                {
                    Name = location
                };
                foreach (var s in e.Elements())
                {
                    bool defValue = s.Attribute("value")?.Value == "true";
                    string settingName = s.Attribute("name")?.Value;
                    var setting = new Setting()
                    {
                        Name = s.Attribute("friendly")?.Value,
                        XMLName = settingName,
                        Location = location,
                        FalseLabel = s.Attribute("false")?.Value,
                        TrueLabel = s.Attribute("true")?.Value,
                        Default = defValue,
                        Active = RoleChecker.RoleHasSetting(settingName, r.RoleName, defValue),
                        ToolTip = s.Attribute("tooltip")?.Value
                    };
                    settings.Add(setting);
                }
                loc.Settings = settings;
                locations.Add(loc);
            }
            return locations;
        }

        public Setting Default(string name)
        {
            var xdoc = RoleSettingDefaults;
            foreach(var e in xdoc.XPathSelectElements("/DefaultSettings").Elements())
            {
                var location = e.Attribute("name")?.Value;
                foreach(var s in e.Elements())
                {
                    var setting = s.Attribute("name");
                    if (setting?.Value == name)
                    {
                        return new Setting()
                        {
                            Name = s.Attribute("friendly")?.Value,
                            XMLName = s.Attribute("name")?.Value,
                            Location = location,
                            FalseLabel = s.Attribute("false")?.Value,
                            TrueLabel = s.Attribute("true")?.Value,
                            Default = s.Attribute("value")?.Value == "true",
                            Active = s.Attribute("value")?.Value == "true",
                            ToolTip = s.Attribute("tooltip")?.Value
                        };
                    }
                }
            }
            return null;
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
            public string FalseLabel { get; set; }
            public string TrueLabel { get; set; }
            public bool Default { get; set; }
            public bool Active { get; set; }
            public string ToolTip { get; set; }
        }
    }
}
