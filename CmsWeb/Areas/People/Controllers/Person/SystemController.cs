using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/Users/{id:int}")]
        public ActionResult Users(int id)
        {
            var q = from u in DbUtil.Db.Users
                    where u.PeopleId == id
                    select u;
            return View("System/Users", q);
        }
        [POST("Person2/UserEdit/{userid:int?}"), Authorize(Roles = "Admin")]
        public ActionResult UserEdit(int? userid)
        {
            User u = null;
            if (userid.HasValue)
                u = DbUtil.Db.Users.Single(us => us.UserId == userid);
            else
            {
                u = CmsWeb.Models.AccountModel.AddUser(Util2.CurrentPeopleId);
                DbUtil.LogActivity("New User for: {0}".Fmt(Session["ActivePerson"]));
            }
            ViewBag.sendwelcome = false;
            return View("System/UserEdit", u);
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
            return View("System/Users", p.Users.AsEnumerable());
        }

        [POST("Person2/UserDelete/{id}"), Authorize(Roles = "Admin")]
        public ActionResult UserDelete(int id)
        {
            var u = DbUtil.Db.Users.Single(us => us.UserId == id);
            var p = DbUtil.Db.LoadPersonById(u.PeopleId.Value);
            DbUtil.Db.PurgeUser(id);
            InitExportToolbar(p.PeopleId);
            return View("System/Users", p.Users.AsEnumerable());
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

        [POST("Person2/Changes/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Changes(int id, int? page, int? size, string sort, string dir)
        {
            var m = new ChangesModel(id);
            m.Pager.Set("/Person2/Changes/" + id, page, size, sort, dir);
            return View("System/Changes", m);
        }
        [POST("Person2/Reverse/{id:int}/{pf}/{field}")]
        public ActionResult Reverse(int id, string field, string pf, string value)
        {
            var m = new ChangesModel(id);
            m.Reverse(field, value, pf);
            return View("System/Changes", m);
        }
        [POST("Person2/Duplicates/{id:int}")]
        public ActionResult Duplicates(int id)
        {
            var m = new DuplicatesModel(id);
            return View("System/Duplicates", m);
        }
    }
}
