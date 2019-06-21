using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Setup.Models;
using System.Collections.Generic;
using System;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "Roles"), Route("{action=index}/{id?}")]
    public class RolesController : CmsStaffController
    {
        public RolesController(IRequestManager requestManager) : base(requestManager) { }
        
        public ActionResult Index()
        {
            var r = CmsData.User.AllRoles(CurrentDatabase);
            return View(r);
        }

        [Route("~/Roles/Priorities")]
        public ActionResult Priorities()
        {
            var r = CmsData.User.AllRoles(CurrentDatabase);
            return View(r);
        }

        [Route("~/Roles/{id}")]
        public ActionResult Manage(string id)
        {
            var model = new RoleModel(CurrentDatabase);

            var role = CurrentDatabase.Roles.SingleOrDefault(m => m.RoleId == id.ToInt());
            if (role == null)
            {
                TempData["error"] = "Invalid role";
                return Content("/Error/");
            }
            else
            {
                ViewBag.Settings = model.SettingsForRole(role);
                return View(role);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create()
        {
            var existingrole = CurrentDatabase.Roles.SingleOrDefault(m => m.RoleName == "NEW");
            if (existingrole != null)
            {
                return Redirect($"/Roles/#{existingrole.RoleId}");
            }
            var r = new Role { RoleName = "NEW" };
            CurrentDatabase.Roles.InsertOnSubmit(r);
            CurrentDatabase.SubmitChanges();
            return Redirect($"/Roles/#{r.RoleId}");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Edit(string id, string value)
        {
            var a = id.Split('.');
            if (a[1] == value)
            {
                // no change
                return Json(new
                {
                    Status = "success"
                });
            }

            var existingrole = CurrentDatabase.Roles.SingleOrDefault(m => m.RoleName == value);
            var role = CurrentDatabase.Roles.SingleOrDefault(m => m.RoleName == a[1]);
            if (role == null)
            {
                return Json(new
                {
                    Status = "error",
                    Message = "Invalid role, try refreshing the page"
                });
            }
            if (existingrole != null)
            {
                return Json(new
                {
                    Status = "error",
                    Message = "Existing role with that name, try again"
                });
            }
            if (a[0] == "RoleName")
            {
                role.RoleName = value;
            }
            CurrentDatabase.SubmitChanges();
            return Json(new
            {
                Status = "success"
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(string id)
        {
            id = id.Substring(1);
            var role = CurrentDatabase.Roles.SingleOrDefault(m => m.RoleId == id.ToInt());
            if (role == null)
            {
                return new EmptyResult();
            }

            if (role.UserRoles.Any())
            {
                return Content("users have that role, not deleted");
            }

            CurrentDatabase.Roles.DeleteOnSubmit(role);
            CurrentDatabase.SubmitChanges();
            return new EmptyResult();
        }

        [Route("~/Roles/SaveSettings")]
        [HttpPost]
        public ActionResult SaveSettings(string name, List<RoleModel.Setting> settings)
        {
            var model = new RoleModel(CurrentDatabase);
            try
            {
                model.SaveSettingsForRole(name, settings);
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.ToString());
            }
        }

        [Route("~/Roles/SavePriorities")]
        [HttpPost]
        public ActionResult SavePriorities(List<int> roles)
        {
            var model = new RoleModel(CurrentDatabase);
            try
            {
                model.UpdatePriorities(roles);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.ToString());
            }
        }
    }
}
