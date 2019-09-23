using CmsData;

namespace SharedTestFixtures
{
    public class MockSettings
    {
        public static Setting CreateSaveSetting(CMSDataContext db, string name, string value)
        {
            var setting = new Setting
            {
                Id = name,
                SettingX = value,
                System = null
            };
            db.Settings.InsertOnSubmit(setting);
            db.SubmitChanges();
            return setting;
        }

        public static void DeleteSetting(CMSDataContext db, Setting setting)
        {
            db.Settings.DeleteOnSubmit(setting);
            db.SubmitChanges();
        }
    }
}
