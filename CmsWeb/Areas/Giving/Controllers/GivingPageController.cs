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

            //var givingPageHash = model.GetGivingPageHashSet();
            var givingPageList = model.GetGivingPageList();

            return Json(givingPageList, JsonRequestBehavior.AllowGet);
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
                SkinFile = newGivingPage.SkinFile,
                //PageType = pageType,
                //DefaultFundId = newGivingPage.ContributionFund.FundName
            };
            var tempDefaultFund = (from d in CurrentDatabase.ContributionFunds where d.FundId == defaultFund select d).FirstOrDefault();
            givingPageItem.DefaultFund = new FundsClass()
            {
                FundId = tempDefaultFund.FundId,
                FundName = tempDefaultFund.FundName
            };
            var newPageType = new PageTypesClass();
            switch (pageType)
            {
                case 1:
                    newPageType.id = 1;
                    newPageType.pageTypeName = "Pledge";
                    break;
                case 2:
                    newPageType.id = 2;
                    newPageType.pageTypeName = "One Time";
                    break;
                case 3:
                    newPageType.id = 3;
                    newPageType.pageTypeName = "Recurring";
                    break;
                default:
                    break;
            }
            givingPageItem.PageType = newPageType;
            newGivingPageList.Add(givingPageItem);
            return Json(newGivingPageList, JsonRequestBehavior.AllowGet);
        }

        public void UpdateGivingPage(int pageId, string pageName, string pageTitle, PageTypesClass pageType, FundsClass defaultFund, bool enabled, FundsClass[] availFundsArray = null,
            string disRedirect = null, string skinFile = null, string topText = null, string thankYouText = null, string onlineNotifyPerson = null,
            string confirmEmailPledge = null, string confirmEmailOneTime = null, string confirmEmailRecurring = null, int? entryPoint = null)
        {
            var givingPage = CurrentDatabase.GivingPages.Where(g => g.GivingPageId == pageId).FirstOrDefault();
            givingPage.PageName = pageName;
            givingPage.PageTitle = pageTitle;
            givingPage.PageType = pageType.id;
            givingPage.FundId = defaultFund.FundId;
            givingPage.Enabled = enabled;
            givingPage.DisabledRedirect = disRedirect;
            givingPage.SkinFile = skinFile;
            givingPage.TopText = topText;
            givingPage.ThankYouText = thankYouText;
            givingPage.ConfirmationEmailPledge = confirmEmailPledge;
            givingPage.ConfirmationEmailOneTime = confirmEmailOneTime;
            givingPage.ConfirmationEmailRecurring = confirmEmailRecurring;
            givingPage.CampusId = givingPage.CampusId;
            givingPage.EntryPointId = entryPoint;

            UpdateModel(givingPage);
            CurrentDatabase.SubmitChanges();
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
            var availableFundsList = (from f in CurrentDatabase.ContributionFunds where f.FundStatusId == 1 select new { f.FundId, f.FundName }).ToList();
            return Json(availableFundsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEntryPoints()
        {
            var entryPointsList = (from ep in CurrentDatabase.EntryPoints select new { ep.Id, ep.Description }).ToList();
            return Json(entryPointsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOnlineNotifyPersonList()
        {
            var onlineNotifyPersonList = (from p in CurrentDatabase.People select new { p.PeopleId, p.Name }).Take(50).ToList();
            return Json(onlineNotifyPersonList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConfirmationEmailList()
        {
            var confirmationEmailList = (from p in CurrentDatabase.People where p.EmailAddress.Length > 0 select new { p.PeopleId, p.EmailAddress }).Take(50).ToList();
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
