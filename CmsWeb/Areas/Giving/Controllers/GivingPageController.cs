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
    [RouteArea("Giving", AreaPrefix = "GivingPage"), Route("{action}/{id?}")]
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

        //[Route("~/Giving/Edit")]
        public ActionResult Edit(int givingPageId)
        {
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
