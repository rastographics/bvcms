using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/UserEdit/{id:int?}"), Authorize(Roles = "Admin")]
        public ActionResult UserEdit(int? id)
        {
            User u = null;
            if (id.HasValue)
                u = DbUtil.Db.Users.Single(us => us.UserId == id);
            else
            {
                u = CmsWeb.Models.AccountModel.AddUser(Util2.CurrentPeopleId);
                DbUtil.LogActivity("New User for: {0}".Fmt(Session["ActivePerson"]));
            }
            ViewBag.sendwelcome = false;
            return View("EditorTemplates/UserEdit", u);
        }

        [POST("Person2/UserUpdate/{id}"), Authorize(Roles = "Admin")]
        public ActionResult UserUpdate(int id, string username, string password, bool islockedout, bool sendwelcome,
            string[] role)
        {
            var u = DbUtil.Db.Users.Single(us => us.UserId == id);
            if (u.Username != username)
            {
                var uu = DbUtil.Db.Users.SingleOrDefault(us => us.Username == username);
                if (uu != null)
                    return Content("error: username already exists");
            }
            var p = DbUtil.Db.LoadPersonById(u.PeopleId.Value);
            u.Username = username;
            u.IsLockedOut = islockedout;
            u.SetRoles(DbUtil.Db, role, User.IsInRole("Finance"));
            if (password.HasValue())
                u.ChangePassword(password);
            DbUtil.Db.SubmitChanges();
            if (sendwelcome)
                CmsWeb.Models.AccountModel.SendNewUserEmail(username);
            DbUtil.LogActivity("Update User for: {0}".Fmt(Session["ActivePerson"]));
            InitExportToolbar(u.PeopleId);
            return View("Toolbar/Toolbar", p.Users.AsEnumerable());
        }

        [POST("Person2/UserDelete/{id}"), Authorize(Roles = "Admin")]
        public ActionResult UserDelete(int id)
        {
            var u = DbUtil.Db.Users.Single(us => us.UserId == id);
            var p = DbUtil.Db.LoadPersonById(u.PeopleId.Value);
            DbUtil.Db.PurgeUser(id);
            InitExportToolbar(p.PeopleId);
            return View("Toolbar/Toolbar", p.Users.AsEnumerable());
        }

        [GET("Person2/Impersonate/{id}"), Authorize(Roles = "Admin")]
        public ActionResult Impersonate(int id)
        {
            var user = DbUtil.Db.Users.SingleOrDefault(uu => uu.UserId == id);
            if (user == null)
                return Content("no user");
            if (user.Roles.Contains("Finance") && !User.IsInRole("Finance"))
                return Content("cannot impersonate finance");
            Session.Remove("CurrentTag");
            FormsAuthentication.SetAuthCookie(user.Username, false);
            CmsWeb.Models.AccountModel.SetUserInfo(user.Username, Session);
            Util.FormsBasedAuthentication = true;
            Util.UserPeopleId = user.PeopleId;
            Util.UserPreferredName = user.Username;
            return Redirect("/");
        }
    }
}