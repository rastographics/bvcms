using CmsData;
using SharedTestFixtures;
using System.Linq;

namespace CMSWebTests
{
    public static class SettingUtils
    {
        public static void UpdateSetting(string id, string value = null)
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);

            if (!db.Settings.Any(s => s.Id == id))
            {
                var m = new Setting { Id = id, SettingX = value };
                db.Settings.InsertOnSubmit(m);
            }
            else
            {
                db.SetSetting(id, value);
            }
            db.SubmitChanges();
        }

        public static void DeleteSetting(string id)
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);

            var setting = db.Settings.SingleOrDefault(m => m.Id == id);
            if (setting != null)
            {
                db.Settings.DeleteOnSubmit(setting);
                db.SubmitChanges();
            }
        }
    }
}
