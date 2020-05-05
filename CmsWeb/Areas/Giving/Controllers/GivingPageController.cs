using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using CmsWeb.Areas.Giving.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CmsWeb.Areas.Giving.Controllers
{
    [Authorize(Roles = "Admin,Finance,FinanceViewOnly")]
    [RouteArea("Giving", AreaPrefix = "Giving"), Route("{action}/{id?}")]
    public class GivingPageController : CmsStaffController
    {
        public GivingPageController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        [Route("~/Giving")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetGivingPageList()
        {
            var model = new GivingPageModel(CurrentDatabase);

            var givingPageHash = model.GetGivingPageHashSet();

            return Json(givingPageHash, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNewGivingPage(string pageName, string pageTitle, bool enabled, string skin, int pageType, int defaultFund, FundsClass[] availFundsArray, string disRedirect, int entryPoint)
        {
            var newGivingPage = new GivingPage()
            {
                PageName = pageName,
                PageTitle = pageTitle,
                Enabled = enabled,
                SkinFile = skin,
                PageType = pageType,
                FundId = defaultFund,
                DisabledRedirect = disRedirect,
                EntryPointId = entryPoint
            };
            CurrentDatabase.GivingPages.InsertOnSubmit(newGivingPage);
            CurrentDatabase.SubmitChanges();
            var newGivingPageList = new List<GivingPageItem>();
            var givingPageItem = new GivingPageItem()
            {
                GivingPageId = newGivingPage.GivingPageId,
                PageName = newGivingPage.PageName,
                Enabled = newGivingPage.Enabled,
                Skin = newGivingPage.SkinFile,
                PageType = newGivingPage.PageType.ToString(),
                DefaultFund = newGivingPage.ContributionFund.FundName
            };
            newGivingPageList.Add(givingPageItem);
            return Json(newGivingPageList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.CurrentGivingPageId = id;
            return View();
        }

        [HttpGet]
        public JsonResult GetGivingPage(int pageId) // id = GivingPageId
        {
            var givingPage = CurrentDatabase.GivingPages.Where(g => g.GivingPageId == pageId).FirstOrDefault();
            var output = new GivingPage
            {
                GivingPageId = givingPage.GivingPageId,
                PageName = givingPage.PageName,
                PageTitle = givingPage.PageTitle,
                PageType = givingPage.PageType,
                FundId = givingPage.FundId,
                Enabled = givingPage.Enabled,
                DisabledRedirect = givingPage.DisabledRedirect,
                SkinFile = givingPage.SkinFile,
                TopText = givingPage.TopText,
                ThankYouText = givingPage.ThankYouText,
                ConfirmationEmailPledge = givingPage.ConfirmationEmailPledge,
                ConfirmationEmailOneTime = givingPage.ConfirmationEmailOneTime,
                ConfirmationEmailRecurring = givingPage.ConfirmationEmailRecurring,
                CampusId = givingPage.CampusId,
                EntryPointId = givingPage.EntryPointId
            };

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPageTypes()
        {
            var pageTypesList = new List<PageTypesClass>();
            PageTypesClass pledge = new PageTypesClass
            {
                id = 1,
                pageTypeName = "Pledge"
            };
            PageTypesClass oneTime = new PageTypesClass
            {
                id = 2,
                pageTypeName = "One Time"
            };
            PageTypesClass recurring = new PageTypesClass
            {
                id = 3,
                pageTypeName = "Recurring"
            };
            pageTypesList.Add(pledge);
            pageTypesList.Add(oneTime);
            pageTypesList.Add(recurring);

            return Json(pageTypesList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableFunds()
        {
            var availableFundsList = (from f in CurrentDatabase.ContributionFunds
                                 where f.FundStatusId == 1
                                 select new { f.FundId, f.FundName }).ToList();
            return Json(availableFundsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEntryPoints()
        {
            var entryPointsList = (from ep in CurrentDatabase.EntryPoints
                                   select new { ep.Id, ep.Description }).ToList();
            return Json(entryPointsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOnlineNotifyPersonList()
        {
            var onlineNotifyPersonList = (from p in CurrentDatabase.People
                                          select new { p.PeopleId, p.Name }).Take(50).ToList();
            return Json(onlineNotifyPersonList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConfirmationEmailList()
        {
            var confirmationEmailList = (from p in CurrentDatabase.People
                                         where p.EmailAddress != null
                                         select new { p.PeopleId, p.EmailAddress }).Take(50).ToList();
            return Json(confirmationEmailList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SaveGivingPageEnabled(bool currentValue, int currentGivingPageId)
        {
           var givingPage = CurrentDatabase.GivingPages.Where(g => g.GivingPageId == currentGivingPageId).FirstOrDefault();
           givingPage.Enabled = currentValue;
           UpdateModel(givingPage);
           CurrentDatabase.SubmitChanges();
        }
    }
}
