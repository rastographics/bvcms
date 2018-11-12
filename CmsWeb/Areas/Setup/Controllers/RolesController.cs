using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "Roles"), Route("{action=index}/{id?}")]
    public class RolesController : CmsStaffController
    {
        public RolesController(RequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var r = CmsData.User.AllRoles(DbUtil.Db);
            return View(r);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create()
        {
            var r = new Role { RoleName = "NEW" };
            DbUtil.Db.Roles.InsertOnSubmit(r);
            DbUtil.Db.SubmitChanges();
            return Redirect($"/Roles/#{r.RoleId}");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Edit(string id, string value)
        {
            var a = id.Split('.');
            var c = new ContentResult();
            c.Content = value;
            if (a[1] == value)
            {
                return c;
            }

            var existingrole = DbUtil.Db.Roles.SingleOrDefault(m => m.RoleName == value);
            var role = DbUtil.Db.Roles.SingleOrDefault(m => m.RoleName == a[1]);
            if (role == null)
            {
                TempData["error"] = "no role";
                return Content("/Error/");
            }
            if (existingrole != null && existingrole.RoleName != role.RoleName)
            {
                TempData["error"] = "duplicate role";
                return Content("/Error/");
            }
            switch (a[0])
            {
                case "RoleName":
                    role.RoleName = value;
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return c;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(string id)
        {
            id = id.Substring(1);
            var role = DbUtil.Db.Roles.SingleOrDefault(m => m.RoleId == id.ToInt());
            if (role == null)
            {
                return new EmptyResult();
            }

            if (role.UserRoles.Any())
            {
                return Content("users have that role, not deleted");
            }

            DbUtil.Db.Roles.DeleteOnSubmit(role);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
    }
}
