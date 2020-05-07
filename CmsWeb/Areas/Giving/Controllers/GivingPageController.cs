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
using CmsData.Codes;

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
            var givingPageList = model.GetGivingPageList();

            return Json(givingPageList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNewGivingPage(string pageName, string pageTitle, bool enabled, PageTypesClass[] pageType, FundsClass defaultFund = null, ShellClass skin = null, FundsClass[] availFundsArray = null, string disRedirect = null, EntryPointClass entryPoint = null)
        {
            var newGivingPage = new GivingPage()
            {
                PageName = pageName,
                PageTitle = pageTitle,
                Enabled = enabled,
                DisabledRedirect = disRedirect,
            };
            var pageTypeBitWise = 0;
            foreach(var item in pageType)
            {
                if(item.id == 1)
                {
                    pageTypeBitWise = pageTypeBitWise + 1;
                }
                if (item.id == 2)
                {
                    pageTypeBitWise = pageTypeBitWise + 2;
                }
                if (item.id == 3)
                {
                    pageTypeBitWise = pageTypeBitWise + 4;
                }
            }
            newGivingPage.PageType = pageTypeBitWise;
            if (defaultFund != null)
            {
                newGivingPage.FundId = defaultFund.FundId;
            }
            if (entryPoint != null)
            {
                newGivingPage.EntryPointId = entryPoint.Id;
            }
            CurrentDatabase.GivingPages.InsertOnSubmit(newGivingPage);
            CurrentDatabase.SubmitChanges();

            if(skin != null)
            {
                newGivingPage.SkinFile = skin.Id;
            }

            if(availFundsArray != null)
            {
                foreach (var item in availFundsArray)
                {
                    var newGivingPageFund = new GivingPageFund()
                    {
                        GivingPageId = newGivingPage.GivingPageId,
                        FundId = item.FundId
                    };
                    CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
                }
                CurrentDatabase.SubmitChanges();
            }

            #region
            var newGivingPageList = new List<GivingPageItem>();
            var givingPageItem = new GivingPageItem()
            {
                GivingPageId = newGivingPage.GivingPageId,
                PageName = newGivingPage.PageName,
                PageTitle = newGivingPage.PageTitle,
                Enabled = newGivingPage.Enabled,
                SkinFile = skin
            };

            if(defaultFund != null)
            {
                var tempDefaultFund = (from d in CurrentDatabase.ContributionFunds where d.FundId == defaultFund.FundId select d).FirstOrDefault();
                givingPageItem.DefaultFund = new FundsClass()
                {
                    FundId = tempDefaultFund.FundId,
                    FundName = tempDefaultFund.FundName
                };
            }

            givingPageItem.PageType = pageType;
            givingPageItem.PageTypeString = "";
            foreach (var item in pageType)
            {
                if (givingPageItem.PageTypeString.Length > 0)
                {
                    givingPageItem.PageTypeString += ", " + item.pageTypeName;
                }
                else
                {
                    givingPageItem.PageTypeString += item.pageTypeName;
                }
            }

            givingPageItem.AvailableFunds = availFundsArray;

            if(entryPoint != null)
            {
                var tempEntryPoint = (from ep in CurrentDatabase.EntryPoints where ep.Id == entryPoint.Id select new { ep.Id, ep.Description }).FirstOrDefault();
                givingPageItem.EntryPoint = new EntryPointClass()
                {
                    Id = (int)entryPoint.Id,
                    Description = tempEntryPoint.Description
                };
            }
            
            newGivingPageList.Add(givingPageItem);
            #endregion
            return Json(newGivingPageList, JsonRequestBehavior.AllowGet);
        }

        public void UpdateGivingPage(int pageId, string pageName, string pageTitle, PageTypesClass pageType, bool enabled, FundsClass defaultFund = null, FundsClass[] availFundsArray = null,
            string disRedirect = null, ShellClass skinFile = null, string topText = null, string thankYouText = null, PeopleClass[] onlineNotifyPerson = null,
            ConfirmEmailClass confirmEmailPledge = null, ConfirmEmailClass confirmEmailOneTime = null, ConfirmEmailClass confirmEmailRecurring = null, int? campusId = null, EntryPointClass entryPoint = null)
        {
            var givingPage = CurrentDatabase.GivingPages.Where(g => g.GivingPageId == pageId).FirstOrDefault();
            givingPage.PageName = pageName;
            givingPage.PageTitle = pageTitle;
            //givingPage.PageType = pageType.id;
            givingPage.Enabled = enabled;
            givingPage.ContributionFund = (from cf in CurrentDatabase.ContributionFunds where cf.FundId == defaultFund.FundId select cf).FirstOrDefault();
            givingPage.FundId = defaultFund.FundId;
            givingPage.DisabledRedirect = disRedirect;
            if(skinFile != null)
            {
                givingPage.SkinFile = skinFile.Id;
            }
            givingPage.TopText = topText;
            givingPage.ThankYouText = thankYouText;
            givingPage.ConfirmationEmailPledge = confirmEmailPledge.Id;
            givingPage.ConfirmationEmailOneTime = confirmEmailOneTime.Id;
            givingPage.ConfirmationEmailRecurring = confirmEmailRecurring.Id;
            campusId = 1;
            if (campusId != null)
            {
                givingPage.Campu = (from c in CurrentDatabase.Campus where c.Id == campusId select c).FirstOrDefault();
            }
            givingPage.CampusId = givingPage.CampusId;
            givingPage.EntryPoint = (from ep in CurrentDatabase.EntryPoints where ep.Id == entryPoint.Id select ep).FirstOrDefault();
            givingPage.EntryPointId = entryPoint.Id;

            foreach (var item in availFundsArray)
            {
                var newGivingPageFund = new GivingPageFund()
                {
                    GivingPageId = pageId,
                    FundId = item.FundId
                };
                CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
            }
            CurrentDatabase.SubmitChanges();

            //var onlineNotifyPersonString = "";
            //foreach (var item in onlineNotifyPerson)
            //{
            //    onlineNotifyPersonString += item.PeopleId + ",";
            //}
            //onlineNotifyPersonString = onlineNotifyPersonString.Remove(onlineNotifyPersonString.Length - 1, 1);

            //UpdateModel(givingPage);
            //CurrentDatabase.SubmitChanges();
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
            var onlineNotifyPersonList = (from p in CurrentDatabase.People
                                          join u in CurrentDatabase.Users on p.PeopleId equals u.PeopleId
                                          join ur in CurrentDatabase.UserRoles on u.UserId equals ur.UserId
                                          where ur.RoleId == 3
                                          select new { p.PeopleId, p.Name }).Distinct().ToList();
            return Json(onlineNotifyPersonList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConfirmationEmailList()
        {
            var confirmationEmailList = (from c in CurrentDatabase.Contents
                   where ContentTypeCode.EmailTemplates.Contains(c.TypeID)
                   orderby c.Name
                   select new { c.Id, c.Title }).ToList();
            return Json(confirmationEmailList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShellList()
        {
            var shellList = (from c in CurrentDatabase.Contents
                        where c.TypeID == ContentTypeCode.TypeHtml
                        where c.ContentKeyWords.Any(vv => vv.Word == "Shell")
                        orderby c.Name
                        select new { c.Id, c.Title }).ToList();
            return Json(shellList, JsonRequestBehavior.AllowGet);
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
