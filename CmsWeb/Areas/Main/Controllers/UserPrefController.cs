using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "UserPref"), Route("{action}/{id?}")]
    public class UserPrefController : CmsStaffController
    {
        public UserPrefController(RequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/UserPref")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("Set/{id}/{value}")]
        public ActionResult Set(string id, string value)
        {
            DbUtil.Db.SetUserPreference(id, value);
            return Content($"set {id}: {value}");
        }
        [Route("UnSet/{id}")]
        public ActionResult UnSet(string id)
        {
            var p =
                DbUtil.Db.Preferences.SingleOrDefault(
                    pp => pp.UserId == DbUtil.Db.CurrentUser.UserId && pp.PreferenceX == id);
            if (p == null)
            {
                return Message(id + " not found");
            }

            DbUtil.Db.Preferences.DeleteOnSubmit(p);
            DbUtil.Db.SubmitChanges();
            return Message("unset " + id);
        }
    }
}
