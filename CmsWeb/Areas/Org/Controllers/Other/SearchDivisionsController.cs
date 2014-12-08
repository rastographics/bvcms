using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Org", AreaPrefix= "SearchDivisions"), Route("{action}/{id?}")]
    public class SearchDivisionsController : CmsStaffController
    {
        [HttpPost, Route("~/SearchDivisions/{id:int}")]
        public ActionResult Index(int id)
        {
            var m = new SearchDivisionsModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult ModalDialog(SearchDivisionsModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult MoveToTop(SearchDivisionsModel m)
        {
            DbUtil.Db.SetMainDivision(m.Id, m.TargetDivision);
            return View("ModalDialog", m);
        }
        [HttpPost]
        public ActionResult AddRemoveDiv(SearchDivisionsModel m)
        {
            m.AddRemoveDiv();
            return View("ModalDialog", m);
        }

    }
}
