using System;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsWeb.Models.ExtraValues;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [POST("ExtraValue/NewAdhoc/{table}/{location}/{id:int}")]
        public ActionResult NewAdhoc(string table, string location, int id)
        {
            var m = new NewExtraValueModel(id, table, location);
            return View(m);
        }

        [POST("ExtraValue/SaveNewAdhoc")]
        public ActionResult SaveNewAdhoc(NewExtraValueModel m)
        {
            try
            {
                m.AddAsNewAdhoc();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("NewAdhoc", m);
            }
            return new EmptyResult();
        }

        [POST("ExtraValue/NewAdhocFromQuery/{id:guid}")]
        public ActionResult NewAdhocFromQuery(Guid id)
        {
            var m = new NewExtraValueModel(id);
            return View("NewAdhoc", m);
        }

        [POST("ExtraValue/DeleteAdhoc/{table}/{id:int}")]
        public ActionResult DeleteAdhoc(string table, int id, string name)
        {
            var m = new ExtraValueModel(id, table);
            m.Delete(name);
            return View("AdHoc", m);
        }

        [POST("ExtraValue/DeleteFromQuery/{id:guid}")]
        public ActionResult DeleteFromQuery(Guid id)
        {
            var m = new NewExtraValueModel(id);
            return View("DeleteFromQuery", m);
        }

        [POST("ExtraValue/ExecDeleteFromQuery")]
        public ActionResult ExecDeleteFromQuery(NewExtraValueModel m)
        {
            m.DeleteFromQuery();
            return new EmptyResult();
        }
   }
}
