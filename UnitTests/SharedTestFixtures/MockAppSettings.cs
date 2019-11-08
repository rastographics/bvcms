using System.Configuration;

namespace SharedTestFixtures
{
    public class MockAppSettings
    {
        public static void Apply(params (string, string)[] settings)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var t in settings)
            {
                config.AppSettings.Settings.Remove(t.Item1);
                config.AppSettings.Settings.Add(t.Item1, t.Item2);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void Remove(params string[] settings)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            foreach (var t in settings)
            {
                config.AppSettings.Settings.Remove(t);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
