using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Configuration.Internal;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

namespace IronPythonUtilities
{
    /// <summary>
    /// A custom app.config injector for use with IronPython code that needs configuration files.
    /// The code was taken and modified from the great work by Tom E Stephens:
    /// http://tomestephens.com/2011/02/making-ironpython-work-overriding-the-configurationmanager/
    /// </summary>
    public sealed class ConfigurationProxy : IInternalConfigSystem
    {
        Configuration config;
        Dictionary<string, IConfigurationSectionHandler> customSections;

        // this is called filename but really it's the path as needed...
        // it defaults to checking the directory you're running in.
        public ConfigurationProxy(string fileName)
        {
            customSections = new Dictionary<string, IConfigurationSectionHandler>();

            if (!Load(fileName))
                throw new ConfigurationErrorsException(string.Format(
                    "File: {0} could not be found or was not a valid cofiguration file.",
                    config.FilePath));
        }

        private bool Load(string file)
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = file };
            config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            var xml = new XmlDocument();
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                xml.Load(stream);

            //var cfgSections = xml.GetElementsByTagName("configSections");

            //if (cfgSections.Count > 0)
            //{
            //    foreach (XmlNode node in cfgSections[0].ChildNodes)
            //    {
            //        var type = System.Activator.CreateInstance(
            //                             Type.GetType(node.Attributes["type"].Value))
            //                             as IConfigurationSectionHandler;

            //        if (type == null) continue;

            //        customSections.Add(node.Attributes["name"].Value, type);
            //    }
            //}

            return config.HasFile;
        }

        public Configuration Configuration
        {
            get { return config; }
        }

        #region IInternalConfigSystem Members

        public object GetSection(string configKey)
        {
            if (configKey == "appSettings")
                return BuildAppSettings();

            object sect = config.GetSection(configKey);

            if (customSections.ContainsKey(configKey) && sect != null)
            {
                var xml = new XmlDocument();

                xml.LoadXml(((ConfigurationSection)sect).SectionInformation.GetRawXml());
                // I have no idea what I should normally be passing through in the first
                // two params, but I never use them in my confighandlers so I opted not to
                // worry about it and just pass through something...
                sect = customSections[configKey].Create(config,
                                       config.EvaluationContext,
                                       xml.FirstChild);
            }

            return sect;
        }

        public void RefreshConfig(string sectionName)
        {
            // I suppose this will work. Reload the whole file?
            Load(config.FilePath);
        }

        public bool SupportsUserConfig
        {
            get { return false; }
        }

        #endregion

        private NameValueCollection BuildAppSettings()
        {
            var coll = new NameValueCollection();

            foreach (var key in config.AppSettings.Settings.AllKeys)
                coll.Add(key, config.AppSettings.Settings[key].Value);

            return coll;
        }

        public bool InjectToConfigurationManager()
        {
            // inject self into ConfigurationManager
            var configSystem = typeof(ConfigurationManager).GetField("s_configSystem",
                                            BindingFlags.Static | BindingFlags.NonPublic);
            configSystem.SetValue(null, this);

            // lame check, but it's something
            if (ConfigurationManager.AppSettings.Count == config.AppSettings.Settings.Count)
                return true;

            return false;
        }
    }
}
