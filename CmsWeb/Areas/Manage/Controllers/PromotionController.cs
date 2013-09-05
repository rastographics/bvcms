using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    public class PromotionController : CmsStaffController
    {
        public ActionResult Index()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignPending()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            m.AssignPending();
            return RedirectToAction("Index");
        }
        public ActionResult List()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            return PartialView("List", m);
        }
        public ActionResult Export()
        {
            var m = new PromotionModel();
            UpdateModel(m);
            return new ExcelResult(m.Export(), "promotion.xls");
        }
    }
}
