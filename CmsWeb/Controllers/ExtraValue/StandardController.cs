using CmsData.ExtraValue;
using CmsWeb.Models.ExtraValues;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [HttpPost, Route("ExtraValue/NewStandard/{table}/{location}/{id:int}")]
        public ActionResult NewStandard(string location, string table, int id)
        {
            var m = new NewExtraValueModel(id, table, location);
            return View(m);
        }
        [ValidateInput(false)]
        [HttpPost, Route("ExtraValue/EditStandard/{table}")]
        public ActionResult EditStandard(string table, int id, string location, string name)
        {
            var m = new NewExtraValueModel(id, table, name, location);
            return View(m);
        }
        [ValidateInput(false)]
        [HttpPost, Route("ExtraValue/SaveEditedStandard")]
        public ActionResult SaveEditedStandard(NewExtraValueModel m)
        {
            var i = Views.GetViewsViewValue(CurrentDatabase, m.ExtraValueTable, m.ExtraValueName, m.ExtraValueLocation);
            i.value.VisibilityRoles = m.VisibilityRoles;
            i.value.EditableRoles = m.EditableRoles;
            i.value.Codes = m.ConvertToCodes();
            i.value.Link = Server.HtmlEncode(m.ExtraValueLink);
            i.views.Save(CurrentDatabase);
            return View("ListStandard", new ExtraValueModel(m.Id, m.ExtraValueTable, m.ExtraValueLocation));
        }

        [HttpPost, Route("ExtraValue/ListStandard/{table}/{location}/{id:int}")]
        public ActionResult ListStandard(string table, string location, int id)
        {
            var m = new ExtraValueModel(id, table, location);
            return View(m);
        }

        [ValidateInput(false)]
        [HttpPost, Route("ExtraValue/DeleteStandard/{table}/{location}")]
        public ActionResult DeleteStandard(string table, string location, string name, bool removedata)
        {
            var m = new ExtraValueModel(table, location);
            m.DeleteStandard(name, removedata);
            return Content("ok");
        }
        [ValidateInput(false)]
        [HttpPost, Route("ExtraValue/Delete/{table}/{location}/{id:int}")]
        public ActionResult Delete(string table, string location, int id, string name)
        {
            var m = new ExtraValueModel(id, table, location);
            m.Delete(name);
            return View("Standard", m);
        }

        [ValidateInput(false)]
        [HttpPost, Route("ExtraValue/SaveNewStandard")]
        public ActionResult SaveNewStandard(NewExtraValueModel m)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    m.AddAsNewStandard();
                }
                else
                {
                    ViewBag.Error = "not saved, errors in form";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("NewStandard", m);
        }

        [HttpPost, Route("ExtraValue/ApplyOrder/{table}/{location}")]
        public ActionResult ApplyOrder(string table, string location, Dictionary<string, int> orders)
        {
            var m = new ExtraValueModel(table, location);
            m.ApplyOrder(orders);
            m = new ExtraValueModel(table, location);
            return View("ListStandard", m);
        }

        [HttpPost, Route("ExtraValue/SwitchMultiline/{table}/{location}")]
        public ActionResult SwitchMultiline(string table, string location, string name)
        {
            var m = new ExtraValueModel(table, location);
            m.SwitchMultiline(name);
            return View("ListStandard", m);
        }

        [HttpGet, Route("ExtraValue/ConvertToStandard/{table}")]
        public ActionResult ConvertToStandard(string table, string name)
        {
            var m = new NewExtraValueModel(0, table, "Standard");
            m.ConvertToStandard(name);
            return Redirect("/ExtraValue/Summary/" + table);
        }
        [HttpGet, Route("ExtraValue/ConvertInfoCard")]
        public ActionResult ConvertInfoCard()
        {
            return Redirect("/ExtraValue/Summary");
        }
        [HttpPost, Route("ExtraValue/Location/{table}/{location}/{id:int}")]
        public ActionResult Location(string location, string table, int id, string label)
        {
            var m = new ExtraValueModel(id, table, location);
            ViewBag.EvLocationLabel = label;
            return View(m);
        }
    }
}
