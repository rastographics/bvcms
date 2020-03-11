using CmsData;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Setup.Models;
using System.Collections.Generic;
using System;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "DashboardWidgets"), Route("{action}/{id?}")]
    public class DashboardWidgetController : CmsStaffController
    {
        public DashboardWidgetController(IRequestManager requestManager) : base(requestManager)
        {
        }
        
        public ActionResult Index()
        {
            var widgets = CurrentDatabase.DashboardWidgets.ToList();
            var r = CmsData.User.AllRoles(CurrentDatabase);
            return View(r);
        }
        
        [Route("~/Dashboards/{id}")]
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
    }
}
