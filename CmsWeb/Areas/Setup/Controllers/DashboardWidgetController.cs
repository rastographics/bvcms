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
            var r = CmsData.User.AllRoles(CurrentDatabase);
            return View(r);
        }
        
        public ActionResult Manage(string id)
        {
            return View();
        }
    }
}
