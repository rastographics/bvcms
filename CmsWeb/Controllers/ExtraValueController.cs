using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Models.ExtraValues;
using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.text;
using NPOI.SS.Formula;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class ExtraValueController : CmsStaffController
    {
        [POST("ExtraValue/Display/{table}/{location}/{id}")]
        public ActionResult Display(string table, string location, int id)
        {
            var m = new ExtraValueModel(id, table, location);
            return View(location, m);
        }

        [POST("ExtraValue/Delete/{table}/{location}/{id:int}")]
        public ActionResult Delete(string table, string location, int id, string name)
        {
            var m = new ExtraValueModel(id, table, location);
            m.Delete(name);
            return View("AdHoc", m);
        }

        [GET("ExtraValue/Codes/{table}")]
        public ActionResult Codes(string table, string name)
        {
            var m = new ExtraValueModel(table);
            var j = m.CodesJson(HttpUtility.UrlDecode(name));
            return Content(j);
        }

        [POST("ExtraValue/NewStandard/{table}/{location}/{id:int}")]
        public ActionResult NewStandard(string location, string table, int id)
        {
            var m = new NewExtraValueModel(id, table, location);
            return View(m);
        }
        [POST("ExtraValue/ListStandard/{table}/{location}/{id:int}")]
        public ActionResult ListStandard(string table, string location, int id, string title)
        {
            var m = new ExtraValueModel(id, table, location);
            ViewBag.DialogTitle = title ?? "Edit Standard Extra Value";
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
            var ret = m.AddAsNewStandard();
            if (ret.HasValue())
                ViewBag.Error = ret;
            return View("NewStandard", m);
        }
        [POST("ExtraValue/NewAdhoc/{table}/{location}/{id:int}")]
        public ActionResult NewAdhoc(string table, string location, int id)
        {
            var m = new NewExtraValueModel(id, table, location);
            return View(m);
        }
        [POST("ExtraValue/NewAdhocQuery/{id:guid}")]
        public ActionResult NewAdhocQuery(Guid id)
        {
            var m = new NewExtraValueModel(id);
            return View("NewAdhoc", m);
        }
        [POST("ExtraValue/SaveNewAdhoc")]
        public ActionResult SaveNewAdhoc(NewExtraValueModel m)
        {
            var ret = m.AddAsNewAdhoc();
            if (ret.HasValue())
            {
                ViewBag.Error = ret;
                return View("NewAdhoc", m);
            }
            return Content(ret);
        }
        [POST("ExtraValue/Edit/{table}/{type}")]
        public ActionResult Edit(string table, string type, string pk, string name, string value)
        {
            var m = new ExtraValueModel(pk.ToInt(), table);
            m.EditExtra(type, HttpUtility.UrlDecode(name), value);
            return new EmptyResult();
        }

        [GET("ExtraValue/Bits/{table}/{id:int}")]
        public ActionResult Bits(string table, int id, string name)
        {
            var m = new ExtraValueModel(id, table);
            var j = m.DropdownBitsJson(HttpUtility.UrlDecode(name));
            return Content(j);
        }
        [POST("ExtraValue/ApplyOrder/{table}/{location}")]
        public ActionResult ApplyOrder(string table, string location, Dictionary<string, int> orders)
        {
            var m = new ExtraValueModel(table, location);
            m.ApplyOrder(orders);
            return View("ListStandard", m);
        }
    }
}
