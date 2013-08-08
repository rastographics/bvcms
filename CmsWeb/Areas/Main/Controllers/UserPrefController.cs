using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Main.Controllers
{
    public class UserPrefController : CmsStaffController
    {
        //
        // GET: /Main/UserPref/

//        public ActionResult Index()
//        {
//            return View();
//        }
        public ActionResult Set(string id, string value)
        {
            DbUtil.Db.SetUserPreference(id, value);
            return Content("set {0}: {1}".Fmt(id, value));
        }
        public ActionResult UnSet(string id, string value)
        {
            var p = DbUtil.Db.CurrentUser.Preferences.SingleOrDefault(up => up.PreferenceX == id);
            DbUtil.Db.Preferences.DeleteOnSubmit(p);
            DbUtil.Db.SubmitChanges();
            return Content("unset " + id);
        }
    }
}
