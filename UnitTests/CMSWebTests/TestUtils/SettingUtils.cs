using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSWebTests
{
    public static class SettingUtils
    {
        public static void InsertSetting(string id, string value = null)
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;

            if (!db.Settings.Any(s => s.Id == id))
            {
                var m = new Setting { Id = id };
                db.Settings.InsertOnSubmit(m);
                db.SubmitChanges();
                db.SetSetting(id, value);
            }
            else
            {
                EditSetting(id, value);
            }
        }

        public static void EditSetting(string id, string value)
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;

            db.SetSetting(id, value);
            db.SubmitChanges();
        }

        public static void DeleteSetting(string id)
        {
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;

            var set = db.Settings.SingleOrDefault(m => m.Id == id);
            db.Settings.DeleteOnSubmit(set);
            db.SubmitChanges();
        }
    }
}
