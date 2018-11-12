using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance,FinanceDataEntry")]
    [RouteArea("Finance", AreaPrefix = "Bundles"), Route("{action=index}")]
    public class BundlesController : CmsStaffController
    {
        public BundlesController(RequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var m = new BundlesModel();
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(BundlesModel m)
        {
            return View(m);
        }
        public ActionResult NewBundle()
        {
            var dt = Util.Now.Date;
            var dw = (int)dt.DayOfWeek;
            dt = dt.AddDays(-dw);
            var b = new BundleHeader
            {
                BundleHeaderTypeId = DbUtil.Db.Setting("DefaultBundleTypeId", BundleTypeCode.PreprintedEnvelope.ToString()).ToInt(),
                BundleStatusId = BundleStatusCode.Open,
                ChurchId = 1,
                ContributionDate = dt,
                CreatedBy = Util.UserId1,
                CreatedDate = Util.Now,
                RecordStatus = false,
                FundId = DbUtil.Db.Setting("DefaultFundId", "1").ToInt(),
            };
            if (User.IsInRole("FinanceDataEntry"))
            {
                b.BundleStatusId = BundleStatusCode.OpenForDataEntry;
            }

            DbUtil.Db.BundleHeaders.InsertOnSubmit(b);
            DbUtil.Db.SubmitChanges();
            TempData["createbundle"] = true;
            return Redirect("/Bundle/" + b.BundleHeaderId);
        }
    }
}
