using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Giving.Models;
using CmsData.Codes;
using CmsData.Classes.Giving;

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

        [HttpGet]
        public JsonResult List()
        {
            var model = new GivingPageModel(CurrentDatabase);
            var givingPageList = model.GetGivingPageList();
            return Json(givingPageList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNew(GivingPageViewModel viewModel)
        {
            var model = new GivingPageModel(CurrentDatabase);
            var newGivingPageList = model.AddNewGivingPage(viewModel);
            return Json(newGivingPageList, JsonRequestBehavior.AllowGet);
        }

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
        public JsonResult GetPageTypes()
        {
            var pageTypesList = GivingPageTypes.GetGivingPageTypes();
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
        public void SaveGivingPageEnabled(bool value, int currentGivingPageId)
        {
            var givingPage = CurrentDatabase.GivingPages.Where(g => g.GivingPageId == currentGivingPageId).FirstOrDefault();
            givingPage.Enabled = value;
            UpdateModel(givingPage);
            CurrentDatabase.SubmitChanges();
        }
    }
}
