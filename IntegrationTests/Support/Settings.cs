using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;

namespace IntegrationTests.Support
{
    internal class Settings
    {
        private static TestContext testContext;

        public static string RootUrl { get { return AppSetting("RootUrl", "http://localhost:54454/"); } }

        public static int HostPort { get { return Convert.ToInt32(AppSetting("HostPort", "54454")); } }

        public static string ScreenShotLocation { get { return AppSetting("ScreenShotLocation", Environment.CurrentDirectory); } }

        public static string ScreenShotUrl { get { return AppSetting("ScreenShotUrl", ScreenShotLocation); } }

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

        public static void Initialize(TestContext context)
        {
            testContext = context;
        }
    }
}
