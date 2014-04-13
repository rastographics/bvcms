using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult Users(int id)
        {
            var q = from u in DbUtil.Db.Users
                    where u.PeopleId == id
                    select u;
            return View("System/Users", q);
        }
        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult UserEdit(int? id)
        {
            User u = null;
            if (id.HasValue)
                u = DbUtil.Db.Users.Single(us => us.UserId == id);
            else
            {
                u = CmsWeb.Models.AccountModel.AddUser(Util2.CurrentPeopleId);
                DbUtil.LogActivity("New User for: {0}".Fmt(Session["ActivePerson"]));
                ViewBag.username = u.Username;
            }
            ViewBag.sendwelcome = false;
            return View("System/UserEdit", u);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult UserUpdate(int id, string u, string p, bool sendwelcome, string[] role)
        {
            var user = DbUtil.Db.Users.Single(us => us.UserId == id);
            if (user.Username != u)
            {
                var uu = DbUtil.Db.Users.SingleOrDefault(us => us.Username == u);
                if (uu != null)
                {
                    ViewBag.ErrorMsg = "username '{0}' already exists".Fmt(u);
                    return View("System/UserEdit", user);
                }
                user.Username = u;
            }
            user.SetRoles(DbUtil.Db, role, User.IsInRole("Finance"));
            if (p.HasValue())
                user.ChangePassword(p);
            DbUtil.Db.SubmitChanges();
            var pp = DbUtil.Db.LoadPersonById(user.PeopleId.Value);
            if (sendwelcome)
                CmsWeb.Models.AccountModel.SendNewUserEmail(u);
            DbUtil.LogActivity("Update User for: {0}".Fmt(Session["ActivePerson"]));
            InitExportToolbar(user.PeopleId);
            return View("System/Users", pp.Users.AsEnumerable());
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult UserDelete(int id)
        {
            var u = DbUtil.Db.Users.Single(us => us.UserId == id);
            var p = DbUtil.Db.LoadPersonById(u.PeopleId.Value);
            DbUtil.Db.PurgeUser(id);
            InitExportToolbar(p.PeopleId);
            return View("System/Users", p.Users.AsEnumerable());
        }

        [HttpGet, Authorize(Roles = "Admin")]
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
            Util.UserPeopleId = user.PeopleId;
            Util.UserPreferredName = user.Username;
            return Redirect("/");
        }

        [HttpPost, Route("Changes/{id:int}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Changes(int id, int? page, int? size, string sort, string dir)
        {
            var m = new ChangesModel(id);
            m.Pager.Set("/Person2/Changes/" + id, page, size, sort, dir);
            return View("System/Changes", m);
        }
        [HttpPost, Route("Reverse/{id:int}/{pf}/{field}")]
        public ActionResult Reverse(int id, string field, string pf, string value)
        {
            var m = new ChangesModel(id);
            m.Reverse(field, value, pf);
            return View("System/Changes", m);
        }
        [HttpPost]
        public ActionResult Duplicates(int id)
        {
            var m = new DuplicatesModel(id);
            return View("System/Duplicates", m);
        }
    }
}
