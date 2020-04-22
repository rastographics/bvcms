using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using CmsWeb.Areas.Giving.Models;

namespace CmsWeb.Areas.Giving.Controllers
{
    [Authorize(Roles = "Admin,Finance,FinanceViewOnly")]
    [RouteArea("Giving", AreaPrefix = "Giving"), Route("{action}/{id?}")]
    public class GivingPageController : CmsStaffController
    {
        public GivingPageController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Giving")]
        public ActionResult Index()
        {
            return View();
        }

        //[Route("~/CreateNewGivingPage")]
        public ActionResult CreateNewGivingPage(string pageName, string pageTitle, bool enabled)
        {
            return Json(pageName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.CurrentGivingPageId = id;
            return View();
        }

        [Route("~/GetGivingPageList")]
        public JsonResult GetGivingPageList()
        {
            var model = new GivingPageModel(CurrentDatabase);

            var givingPageHash = model.GetGivingPageHashSet();

            return Json(givingPageHash, JsonRequestBehavior.AllowGet);
        }
    }
}
