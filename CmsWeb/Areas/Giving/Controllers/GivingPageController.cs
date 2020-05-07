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

        public ActionResult CreateNewGivingPage(GivingPageViewModel viewModel)
        {
            var model = new GivingPageModel(CurrentDatabase);
            var newGivingPageList = model.AddNewGivingPage(viewModel);
            #region
            //#region
            //var newGivingPage = new GivingPage()
            //{
            //    PageName = viewModel.pageName,
            //    PageTitle = viewModel.pageTitle,
            //    Enabled = viewModel.enabled,
            //    DisabledRedirect = viewModel.disRedirect
            //};
            //newGivingPage.PageType = 0;
            //foreach (var item in viewModel.pageType)
            //{
            //    if (item.id == 1)
            //    {
            //        newGivingPage.PageType = newGivingPage.PageType + 1;
            //    }
            //    if (item.id == 2)
            //    {
            //        newGivingPage.PageType = newGivingPage.PageType + 2;
            //    }
            //    if (item.id == 3)
            //    {
            //        newGivingPage.PageType = newGivingPage.PageType + 4;
            //    }
            //}
            //if (viewModel.defaultFund != null)
            //{
            //    newGivingPage.FundId = viewModel.defaultFund.FundId;
            //}
            //if (viewModel.entryPoint != null)
            //{
            //    newGivingPage.EntryPointId = viewModel.entryPoint.Id;
            //}
            //if (viewModel.skinFile != null)
            //{
            //    newGivingPage.SkinFile = viewModel.skinFile.Id;
            //}
            //CurrentDatabase.GivingPages.InsertOnSubmit(newGivingPage);
            //CurrentDatabase.SubmitChanges();
            //#endregion

            //#region
            //if (viewModel.availFundsArray != null)
            //{
            //    foreach (var item in viewModel.availFundsArray)
            //    {
            //        var newGivingPageFund = new GivingPageFund()
            //        {
            //            GivingPageId = newGivingPage.GivingPageId,
            //            FundId = item.FundId
            //        };
            //        CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
            //    }
            //    CurrentDatabase.SubmitChanges();
            //}
            //#endregion

            //#region
            //var newGivingPageList = new List<GivingPageItem>();
            //var givingPageItem = new GivingPageItem()
            //{
            //    GivingPageId = newGivingPage.GivingPageId,
            //    PageName = newGivingPage.PageName,
            //    PageTitle = newGivingPage.PageTitle,
            //    Enabled = newGivingPage.Enabled,
            //    SkinFile = viewModel.skinFile
            //};

            //if (viewModel.defaultFund != null)
            //{
            //    var tempDefaultFund = (from d in CurrentDatabase.ContributionFunds where d.FundId == viewModel.defaultFund.FundId select d).FirstOrDefault();
            //    givingPageItem.DefaultFund = new FundsClass()
            //    {
            //        FundId = tempDefaultFund.FundId,
            //        FundName = tempDefaultFund.FundName
            //    };
            //}

            //givingPageItem.PageType = viewModel.pageType;
            //givingPageItem.PageTypeString = "";
            //foreach (var item in viewModel.pageType)
            //{
            //    if (givingPageItem.PageTypeString.Length > 0)
            //    {
            //        givingPageItem.PageTypeString += ", " + item.pageTypeName;
            //    }
            //    else
            //    {
            //        givingPageItem.PageTypeString += item.pageTypeName;
            //    }
            //}

            //givingPageItem.AvailableFunds = viewModel.availFundsArray;

            //if (viewModel.entryPoint != null)
            //{
            //    var tempEntryPoint = (from ep in CurrentDatabase.EntryPoints where ep.Id == viewModel.entryPoint.Id select new { ep.Id, ep.Description }).FirstOrDefault();
            //    givingPageItem.EntryPoint = new EntryPointClass()
            //    {
            //        Id = (int)viewModel.entryPoint.Id,
            //        Description = tempEntryPoint.Description
            //    };
            //}

            //newGivingPageList.Add(givingPageItem);
            //#endregion
            #endregion
            return Json(newGivingPageList, JsonRequestBehavior.AllowGet);
        }

        public void UpdateGivingPage(GivingPageViewModel viewModel)
        {
            var givingPage = (from gp in CurrentDatabase.GivingPages where gp.GivingPageId == viewModel.pageId select gp).FirstOrDefault();

            #region
            givingPage.PageName = viewModel.pageName;

            givingPage.PageTitle = viewModel.pageTitle;

            givingPage.PageType = 0;
            foreach (var item in viewModel.pageType)
            {
                if (item.id == 1)
                {
                    givingPage.PageType = givingPage.PageType + 1;
                }
                if (item.id == 2)
                {
                    givingPage.PageType = givingPage.PageType + 2;
                }
                if (item.id == 3)
                {
                    givingPage.PageType = givingPage.PageType + 4;
                }
            }

            givingPage.Enabled = viewModel.enabled;

            if (viewModel.defaultFund != null)
            {
                givingPage.ContributionFund = (from cf in CurrentDatabase.ContributionFunds where cf.FundId == viewModel.defaultFund.FundId select cf).FirstOrDefault();
                givingPage.FundId = viewModel.defaultFund.FundId;
            }
            else
            {
                givingPage.FundId = null;
            }

            givingPage.DisabledRedirect = viewModel.disRedirect;

            if (viewModel.skinFile != null)
            {
                givingPage.SkinFile = viewModel.skinFile.Id;
            }
            else
            {
                givingPage.SkinFile = null;
            }

            givingPage.TopText = viewModel.topText;

            givingPage.ThankYouText = viewModel.thankYouText;

            if(viewModel.confirmEmailPledge != null)
            {
                givingPage.ConfirmationEmailPledge = viewModel.confirmEmailPledge.Id;
            }
            else
            {
                givingPage.ConfirmationEmailPledge = null;
            }

            if (viewModel.confirmEmailOneTime != null)
            {
                givingPage.ConfirmationEmailOneTime = viewModel.confirmEmailOneTime.Id;
            }
            else
            {
                givingPage.ConfirmationEmailOneTime = null;
            }

            if (viewModel.confirmEmailRecurring != null)
            {
                givingPage.ConfirmationEmailRecurring = viewModel.confirmEmailRecurring.Id;
            }
            else
            {
                givingPage.ConfirmationEmailRecurring = null;
            }

            if (viewModel.campusId != null)
            {
                givingPage.Campu = (from c in CurrentDatabase.Campus where c.Id == viewModel.campusId select c).FirstOrDefault();
                givingPage.CampusId = viewModel.campusId;
            }
            givingPage.CampusId = viewModel.campusId;

            if (viewModel.entryPoint != null)
            {
                givingPage.EntryPoint = (from ep in CurrentDatabase.EntryPoints where ep.Id == viewModel.entryPoint.Id select ep).FirstOrDefault();
                givingPage.EntryPointId = viewModel.entryPoint.Id;
            }
            else
            {
                givingPage.EntryPointId = null;
            }
            #endregion

            #region
            var tempAnonymousArray = (from gpf in CurrentDatabase.GivingPageFunds
                                      join cf in CurrentDatabase.ContributionFunds on gpf.FundId equals cf.FundId
                                      where gpf.GivingPageId == givingPage.GivingPageId
                                      select new { cf.FundId, cf.FundName }).ToArray();
            var tempGivingPagesList = (from gpf in CurrentDatabase.GivingPageFunds
                                       join cf in CurrentDatabase.ContributionFunds on gpf.FundId equals cf.FundId
                                       where gpf.GivingPageId == givingPage.GivingPageId
                                       select gpf).ToList();
            if (tempAnonymousArray.Length > 0)
            {
                var tempAvailFundsArray = new FundsClass[tempAnonymousArray.Length];
                var tempAvailFundsList = new List<FundsClass>();
                foreach (var item in tempAnonymousArray)
                {
                    var t = new FundsClass()
                    {
                        FundId = item.FundId,
                        FundName = item.FundName
                    };
                    tempAvailFundsList.Add(t);
                }
                tempAvailFundsArray = tempAvailFundsList.ToArray();
                if (viewModel.availFundsArray != tempAvailFundsArray)
                {
                    foreach (var item in tempGivingPagesList)
                    {
                        CurrentDatabase.GivingPageFunds.DeleteOnSubmit(item);
                    }
                }
                foreach (var item in viewModel.availFundsArray)
                {
                    var newGivingPageFund = new GivingPageFund()
                    {
                        GivingPageId = viewModel.pageId,
                        FundId = item.FundId
                    };
                    CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
                }
            }
            else
            {
                foreach (var item in viewModel.availFundsArray)
                {
                    var newGivingPageFund = new GivingPageFund()
                    {
                        GivingPageId = viewModel.pageId,
                        FundId = item.FundId
                    };
                    CurrentDatabase.GivingPageFunds.InsertOnSubmit(newGivingPageFund);
                }
            }
            #endregion

            #region
            var onlineNotifyPersonString = "";
            if(viewModel.onlineNotifyPerson != null)
            {
                foreach (var item in viewModel.onlineNotifyPerson)
                {
                    onlineNotifyPersonString += item.PeopleId + ",";
                }
                givingPage.OnlineNotifyPerson = onlineNotifyPersonString.Remove(onlineNotifyPersonString.Length - 1, 1);
            }
            else
            {
                givingPage.OnlineNotifyPerson = onlineNotifyPersonString;
            }
            #endregion

            CurrentDatabase.SubmitChanges();
            #region
            //var returningGivingPageList = new List<GivingPageViewModel>();
            //returningGivingPageList.Add(givingPage);
            //return Json(givingPage, JsonRequestBehavior.AllowGet);
            #endregion
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
            var availableFundsList = (from f in CurrentDatabase.ContributionFunds where f.FundStatusId == 1 orderby f.FundName select new { f.FundId, f.FundName }).ToList();
            return Json(availableFundsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEntryPoints()
        {
            var entryPointsList = (from ep in CurrentDatabase.EntryPoints orderby ep.Description select new { ep.Id, ep.Description }).ToList();
            return Json(entryPointsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOnlineNotifyPersonList()
        {
            var onlineNotifyPersonList = (from p in CurrentDatabase.People
                                          join u in CurrentDatabase.Users on p.PeopleId equals u.PeopleId
                                          join ur in CurrentDatabase.UserRoles on u.UserId equals ur.UserId
                                          where ur.RoleId == 3
                                          orderby p.Name
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
