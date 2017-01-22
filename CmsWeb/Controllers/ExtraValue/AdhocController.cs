using System;
using System.Web.Mvc;
using CmsWeb.Models.ExtraValues;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [HttpPost, Route("ExtraValue/NewAdhoc/{table}/{location}/{id:int}")]
        public ActionResult NewAdhoc(string table, string location, int id)
        {
            var m = new NewExtraValueModel(id, table, location);
            return View(m);
        }

        [HttpPost, Route("ExtraValue/SaveNewAdhoc")]
        [ValidateInput(false)]
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

        [HttpPost, Route("ExtraValue/NewAdhocFromQuery/{id:guid}")]
        public ActionResult NewAdhocFromQuery(Guid id)
        {
            var m = new NewExtraValueModel(id);
            return View("NewAdhoc", m);
        }

        [HttpPost, Route("ExtraValue/DeleteAdhoc/{table}/{id:int}")]
        public ActionResult DeleteAdhoc(string table, int id, string name)
        {
            var m = new ExtraValueModel(id, table, "Adhoc");
            m.Delete(name);
            return View("AdHoc", m);
        }

        [HttpPost, Route("ExtraValue/DeleteFromQuery/{id:guid}")]
        public ActionResult DeleteFromQuery(Guid id)
        {
            var m = new NewExtraValueModel(id);
            return View("DeleteFromQuery", m);
        }

        [HttpPost, Route("ExtraValue/ExecDeleteFromQuery")]
        public ActionResult ExecDeleteFromQuery(NewExtraValueModel m)
        {
            m.DeleteFromQuery();
            return new EmptyResult();
        }
   }
}
