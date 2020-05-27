using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Giving.Models;
using CmsData.Codes;
using CmsData.Classes.Giving;
using System.Text.RegularExpressions;
using System;

namespace CmsWeb.Areas.Giving.Controllers
{
    [Authorize(Roles = "Admin,Finance,FinanceViewOnly")]
    [RouteArea("Giving", AreaPrefix = "Giving"), Route("{action}/{id?}")]
    public class GivingManagementController : CmsStaffController
    {
        public GivingManagementController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        [Route("~/Giving")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("~/Giving/{id}")]
        public ActionResult Manage(string id)
        {
            var model = new GivingPageModel(CurrentDatabase);
            var page = new GivingPageItem();
            if (id.ToLower() == "new")
            {
                page.GivingPageId = 0;
            }
            else
            {
                page = model.GetGivingPages(id.ToInt()).SingleOrDefault();
            }
            if (page == null)
            {
                Util.TempError = "Invalid page";
                return Content("/Error/");
            }
            else
            {
                return View(page);
            }
        }

        [HttpGet]
        public JsonResult List()
        {
            var model = new GivingPageModel(CurrentDatabase);
            var givingPageList = model.GetGivingPages();
            return Json(givingPageList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNew(GivingPageViewModel viewModel)
        {
            var model = new GivingPageModel(CurrentDatabase);
            var newGivingPageList = model.AddNewGivingPage(viewModel);
            return Json(newGivingPageList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(GivingPageViewModel viewModel)
        {
            var givingPage = (from gp in CurrentDatabase.GivingPages where gp.GivingPageId == viewModel.pageId select gp).FirstOrDefault();
            if(givingPage == null)
            {
                return new HttpNotFoundResult();
            }
            else
            {
                var model = new GivingPageModel(CurrentDatabase);
                var returnList = model.UpdateGivingPage(viewModel, givingPage);
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckUrlAvailability(string url)
        {
            bool result = false;
            Regex regex = new Regex("^([a-z0-9-])+$");
            if (regex.IsMatch(url) && url.Length > 1 && CurrentDatabase.GivingPages.Where(p => p.PageUrl == url).Count() == 0) {
                result = true;
            }
            return Json(new
            {
                result = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPageTypes()
        {
            var pageTypesList = GivingPageTypes.GetGivingPageTypes();
            return Json(pageTypesList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableFunds()
        {
            var availableFundsList = (from f in CurrentDatabase.ContributionFunds where f.FundStatusId == 1 orderby f.FundName select new { Id = f.FundId, Name = f.FundName }).ToList();
            return Json(availableFundsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEntryPoints()
        {
            var entryPointsList = (from ep in CurrentDatabase.EntryPoints orderby ep.Description select new { ep.Id, Name = ep.Description }).ToList();
            return Json(entryPointsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOnlineNotifyPersonList()
        {
            var onlineNotifyPersonList = (from p in CurrentDatabase.People
                                          join u in CurrentDatabase.Users on p.PeopleId equals u.PeopleId
                                          join ur in CurrentDatabase.UserRoles on u.UserId equals ur.UserId
                                          join r in CurrentDatabase.Roles on ur.RoleId equals r.RoleId
                                          where r.RoleName == "Access"
                                          where u.EmailAddress != null
                                          orderby p.Name
                                          select new { Id = p.PeopleId, p.Name }).Distinct().ToList();
            return Json(onlineNotifyPersonList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConfirmationEmailList()
        {
            var confirmationEmailList = (from c in CurrentDatabase.Contents
                                         where ContentTypeCode.EmailTemplates.Contains(c.TypeID)
                                         orderby c.Name
                                         select new { c.Id, Name = c.Title }).ToList();
            return Json(confirmationEmailList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShellList()
        {
            var shellList = (from c in CurrentDatabase.Contents
                             where c.TypeID == ContentTypeCode.TypeHtml
                             where c.ContentKeyWords.Any(vv => vv.Word == "Shell")
                             orderby c.Name
                             select new { c.Id, Name = c.Title }).ToList();
            return Json(shellList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveGivingPageEnabled(bool value, int currentGivingPageId)
        {
            var givingPage = CurrentDatabase.GivingPages.Where(g => g.GivingPageId == currentGivingPageId).FirstOrDefault();
            givingPage.Enabled = value;
            UpdateModel(givingPage);
            CurrentDatabase.SubmitChanges();
            return Json(new { givingPage.GivingPageId, givingPage.PageName, givingPage.Enabled });
        }
    }
}
