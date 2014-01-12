using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData.ExtraValue;
using CmsWeb.Models.ExtraValues;
using DocumentFormat.OpenXml.EMMA;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [POST("ExtraValue/NewStandard/{table}/{location}/{id:int}")]
        public ActionResult NewStandard(string location, string table, int id)
        {
            var m = new NewExtraValueModel(id, table, location);
            return View(m);
        }
        [POST("ExtraValue/EditStandard/{table}")]
        public ActionResult EditStandard(string table, string name)
        {
            var m = new NewExtraValueModel(table, name);
            return View(m);
        }
        [POST("ExtraValue/SaveEditedStandard")]
        public ActionResult SaveEditedStandard(NewExtraValueModel m)
        {
            var i = Views.GetViewsViewValue(m.ExtraValueTable, m.ExtraValueName);
            i.value.VisibilityRoles = m.VisibilityRoles;
            i.value.Codes = m.ConvertToCodes();
            i.views.Save();
            return View("EditStandard", m);
        }

        [POST("ExtraValue/ListStandard/{table}/{location}/{id:int}")]
        public ActionResult ListStandard(string table, string location, int id)
        {
            var m = new ExtraValueModel(id, table, location);
            return View(m);
        }

        [POST("ExtraValue/DeleteStandard/{table}/{location}")]
        public ActionResult DeleteStandard(string table, string location, string name, bool removedata)
        {
            var m = new ExtraValueModel(table, location);
            m.DeleteStandard(name, removedata);
            return Content("ok");
        }

        [POST("ExtraValue/SaveNewStandard")]
        public ActionResult SaveNewStandard(NewExtraValueModel m)
        {
            try
            {
                if(ModelState.IsValid)
                    m.AddAsNewStandard();
                else
                    ViewBag.Error = "not saved, errors in form";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("NewStandard", m);
        }

        [POST("ExtraValue/ApplyOrder/{table}/{location}")]
        public ActionResult ApplyOrder(string table, string location, Dictionary<string, int> orders)
        {
            var m = new ExtraValueModel(table, location);
            m.ApplyOrder(orders);
            m = new ExtraValueModel(table, location);
            return View("ListStandard", m);
        }

        [POST("ExtraValue/SwitchMultiline/{table}/{location}")]
        public ActionResult SwitchMultiline(string table, string location, string name)
        {
            var m = new ExtraValueModel(table, location);
            m.SwitchMultiline(name);
            return View("ListStandard", m);
        }

        [GET("ExtraValue/ConvertToStandard/{table}")]
        public ActionResult ConvertToStandard(string table, string name)
        {
            var m = new NewExtraValueModel(0, table, "Standard");
            m.ConvertToStandard(name);
            return Redirect("/ExtraValue/Summary");
        }
        [GET("ExtraValue/ConvertInfoCard")]
        public ActionResult ConvertInfoCard()
        {
            return Redirect("/ExtraValue/Summary");
        }
    }
}
