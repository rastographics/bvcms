using System;
using CmsData;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData.Classes.RoleChecker;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace CmsWeb.Areas.Setup.Models
{
    public class RoleModel
    {
        private CMSDataContext CurrentDatabase;

        public RoleModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }

        private static XDocument RoleSettingDefaults
        {
            get { return XDocument.Parse(Resource1.RoleSettingDefaults); }
        }

        private XDocument DBRoleSettings
        {
            get
            {
                var content = DbUtil.Content(CurrentDatabase, "CustomAccessRoles.xml", "<?xml version='1.0' encoding='utf - 8'?><roles></roles>");
                return XDocument.Load(new StringReader(content));
            }
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

        public void SaveSettingsForRole(string roleName, List<Setting> settings)
        {
            var xdoc = DBRoleSettings;

            // find existing role element(s) and remove
            var existing = xdoc.Descendants("role").SingleOrDefault(r => r.Attribute("name").Value == roleName);
            existing.Remove();

            // create new role element
            var elSettings = new XElement("settings");
            foreach(Setting setting in settings)
            {
                var elSetting = new XElement("setting", new XAttribute("name", setting.XMLName), new XAttribute("value", setting.Active.ToString()));
                elSettings.Add(elSetting);
            }
            var element = new XElement("role", new XAttribute("name", roleName), elSettings);
            xdoc.Descendants("roles").First().Add(element);
            UpdateDBRoleSettings(xdoc.ToString());
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

        private void UpdateDBRoleSettings(string settingsXML)
        {
            var content = CurrentDatabase.Content("CustomAccessRoles.xml");
            if (content == null)
            {
                // todo: create db content
                return;
            }

            content.Body = settingsXML;
            CurrentDatabase.SubmitChanges();
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
