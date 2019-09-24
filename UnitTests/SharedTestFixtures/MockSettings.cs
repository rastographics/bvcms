using CmsData;
using System.Linq;

namespace SharedTestFixtures
{
    public class MockSettings
    {
        public static Setting CreateSaveSetting(CMSDataContext db, string name, string value)
        {
            var setting = db.Settings.FirstOrDefault(s => s.Id == name);
            if (setting == null)
            {
                setting = new Setting { Id = name, SettingX = value };
                db.Settings.InsertOnSubmit(setting);
            }
            else
            {
                setting.SettingX = value;
            }
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
