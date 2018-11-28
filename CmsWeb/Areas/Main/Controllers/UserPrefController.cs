using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "UserPref"), Route("{action}/{id?}")]
    public class UserPrefController : CmsStaffController
    {
        public UserPrefController(IRequestManager requestManager) : base(requestManager)
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
            CurrentDatabase.SetUserPreference(id, value);
            return Content($"set {id}: {value}");
        }
        [Route("UnSet/{id}")]
        public ActionResult UnSet(string id)
        {
            var p =
                CurrentDatabase.Preferences.SingleOrDefault(
                    pp => pp.UserId == CurrentDatabase.CurrentUser.UserId && pp.PreferenceX == id);
            if (p == null)
            {
                return Message(id + " not found");
            }

            CurrentDatabase.Preferences.DeleteOnSubmit(p);
            CurrentDatabase.SubmitChanges();
            return Message("unset " + id);
        }
    }
}
