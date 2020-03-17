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
    [RouteArea("Setup", AreaPrefix = "HomeWidgets"), Route("{action=index}/{id?}")]
    public class DashboardWidgetController : CmsStaffController
    {
        public DashboardWidgetController(IRequestManager requestManager) : base(requestManager) { }
        
        public ActionResult Index()
        {
            var widgets = CurrentDatabase.DashboardWidgets.ToList();
            return View(widgets);
        }
        
        [Route("~/HomeWidgets/{id}")]
        public ActionResult Manage(string id)
        {
            try
            {
                var model = new DashboardWidgetModel(id, CurrentDatabase);
                return View(model);
            }
            catch
            {
                return Content("Invalid widget");
            }
        }
    }
}
