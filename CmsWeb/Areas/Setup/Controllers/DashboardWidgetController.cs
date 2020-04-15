using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Setup.Models;
using CmsData;
using UtilityExtensions;
using System.Collections.Generic;
using System;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "HomeWidgets"), Route("{action=index}/{id?}")]
    public class DashboardWidgetController : CmsStaffController
    {
        public DashboardWidgetController(IRequestManager requestManager) : base(requestManager) { }
        
        public ActionResult Index()
        {
            var widgets = CurrentDatabase.DashboardWidgets.OrderBy(w => w.Order).ToList();
            return View(widgets);
        }

        [HttpGet, Route("~/HomeWidgets/{id}")]
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

        [HttpPost]
        public ActionResult Update(DashboardWidgetModel m)
        {
            m.UpdateModel();
            CurrentDatabase.SubmitChanges();

            return Redirect("/HomeWidgets");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var widget = CurrentDatabase.DashboardWidgets.SingleOrDefault(w => w.Id == id);
            if (widget == null)
            {
                return new EmptyResult();
            }

            if (widget.System)
            {
                return Content("This widget can't be deleted. Try disabling it instead.");
            }
            CurrentDatabase.DashboardWidgetRoles.DeleteAllOnSubmit(widget.DashboardWidgetRoles);
            CurrentDatabase.DashboardWidgets.DeleteOnSubmit(widget);
            CurrentDatabase.SubmitChanges();
            return Content("success");
        }

        [HttpPost]
        public ActionResult Reorder(List<int> widgets)
        {
            try
            {
                DashboardWidgetModel.UpdateWidgetOrder(CurrentDatabase, widgets);
                return Content("Widget order has been updated");
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult Toggle(int id, bool status)
        {
            var widget = CurrentDatabase.DashboardWidgets.SingleOrDefault(w => w.Id == id);
            if (widget == null)
            {
                return new EmptyResult();
            }
            widget.Enabled = status;
            CurrentDatabase.SubmitChanges();
            return Content(widget.Name + " has been " + (widget.Enabled ? "enabled" : "disabled"));
        }

        public ActionResult Embed(string id, bool preview = false)
        {
            try
            {
                var widget = new DashboardWidgetModel(id, CurrentDatabase);
                if (preview == true)
                {
                    widget.CachePolicy = DashboardWidgetModel.CachePolicies.NeverCache.ToInt();
                }
                if (widget.CachePolicy != DashboardWidgetModel.CachePolicies.NeverCache.ToInt())
                {
                    Response.SetCacheMinutes(widget.CacheHours * 60);
                }
                string html = widget.Embed();
                return Content(html, "text/html");
            }
            catch(Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }
    }
}
