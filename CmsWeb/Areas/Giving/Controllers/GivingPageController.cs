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

        public ActionResult CreateNewGivingPage(string pageName, string pageTitle, bool enabled)
        {
            return Json(pageName, JsonRequestBehavior.AllowGet);
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
            for(var i = 1; i < 11; i++)
            {
                var pageType = new PageTypesClass
                {
                    id = i,
                    pageTypeName = "Page Type " + (i).ToString()
                };
                pageTypesList.Add(pageType);
            }
            return Json(pageTypesList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableFunds()
        {
            var availableFundsList = new List<PageTypesClass>();
            for (var i = 0; i < 10; i++)
            {
                var pageType = new PageTypesClass
                {
                    id = i,
                    pageTypeName = "Available Fund " + (i + 1).ToString()
                };
                availableFundsList.Add(pageType);
            }
            return Json(availableFundsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEntryPoints()
        {
            var entryPointsList = new List<PageTypesClass>();
            for (var i = 0; i < 10; i++)
            {
                var pageType = new PageTypesClass
                {
                    id = i,
                    pageTypeName = "Entry Point " + (i + 1).ToString()
                };
                entryPointsList.Add(pageType);
            }
            return Json(entryPointsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOnlineNotifyPersonList()
        {
            var onlineNotifyPersonList = new List<PageTypesClass>();
            for (var i = 0; i < 10; i++)
            {
                var pageType = new PageTypesClass
                {
                    id = i,
                    pageTypeName = "Online Notify Person " + (i + 1).ToString()
                };
                onlineNotifyPersonList.Add(pageType);
            }
            return Json(onlineNotifyPersonList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConfirmationEmailList()
        {
            var confirmationEmailList = new List<PageTypesClass>();
            for (var i = 0; i < 10; i++)
            {
                var pageType = new PageTypesClass
                {
                    id = i,
                    pageTypeName = "Confirmation Email Person " + (i + 1).ToString()
                };
                confirmationEmailList.Add(pageType);
            }
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
