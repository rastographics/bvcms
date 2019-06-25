using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using IntegrationTests.Resources;
using System.Linq;

namespace IntegrationTests.Support
{
    internal class Settings
    {
        private static TestContext testContext;

        public static string RootUrl => AppSetting("RootUrl", $"http://localhost:{HostPort}/");

        public static int HostPort => Convert.ToInt32(AppSetting("HostPort", _hostport ?? GetHostPort()));

        public static string ScreenShotLocation => AppSetting("ScreenShotLocation", Environment.CurrentDirectory);

        public static string ScreenShotUrl => AppSetting("ScreenShotUrl", ScreenShotLocation);

        public static string ApplicationHostConfig => Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\.vs\config\applicationhost.config"));

        private static string AppSetting(string name, string defaultValue = null)
        {
            return ContextSetting(name) ?? ConfigurationManager.AppSettings[name] ?? defaultValue;
        }

        private static string ContextSetting(string name, string defaultValue = null)
        {
            if (testContext != null && testContext.Properties[name] != null)
            {
                return Convert.ToString(testContext.Properties[name]);
            }
            return defaultValue;
        }

        private static string _hostport;
        private static string GetHostPort()
        {
            EnsureApplicationHostConfig();
            var config = new XmlDocument();
            config.Load(ApplicationHostConfig);
            var binding = config.SelectSingleNode(@"/configuration/system.applicationHost/sites/site[@name='CMSWeb']/bindings/binding[@protocol='http']");
            var info = binding.Attributes["bindingInformation"].Value;
            return _hostport = info.Split(':')[1]; //usually "*:port:localhost"
        }

        internal static void EnsureApplicationHostConfig()
        {
            if (!File.Exists(ApplicationHostConfig))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ApplicationHostConfig));
                File.WriteAllText(ApplicationHostConfig, R.applicationhostconfig);
                SetCmsWebDirectory();
            }
        }

        private static void SetCmsWebDirectory()
        {
            var config = new XmlDocument();
            config.Load(ApplicationHostConfig);
            var virtualDirectory = config.SelectSingleNode(@"/configuration/system.applicationHost/sites/site[@name='CMSWeb']/application/virtualDirectory");
            virtualDirectory.Attributes["physicalPath"].Value = Path.GetFullPath(@"..\..\..\CmsWeb");
            config.Save(ApplicationHostConfig);
        }

        public static void Initialize(TestContext context)
        {
            testContext = context;
        }
    }
}
