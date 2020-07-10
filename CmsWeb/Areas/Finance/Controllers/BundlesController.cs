using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Finance.Models;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance,FinanceDataEntry")]
    [RouteArea("Finance", AreaPrefix = "Bundles"), Route("{action=index}")]
    public class BundlesController : CmsStaffController
    {
        public BundlesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            var m = new BundlesModel(CurrentDatabase);
            return View(m);
        }

        [HttpPost]
        public ActionResult Results(BundlesModel m)
        {
            return View(m);
        }

        [HttpGet]
        public ActionResult NewBundle()
        {
            var dt = Util.Now.Date;
            var dw = (int)dt.DayOfWeek;
            dt = dt.AddDays(-dw);
            var b = new BundleHeader
            {
                BundleHeaderTypeId = CurrentDatabase.Setting("DefaultBundleTypeId", BundleTypeCode.PreprintedEnvelope.ToString()).ToInt(),
                BundleStatusId = BundleStatusCode.Open,
                ChurchId = 1,
                ContributionDate = dt,
                CreatedBy = CurrentDatabase.UserId1,
                CreatedDate = Util.Now,
                RecordStatus = false,
                FundId = GetDefaultFundId(),
            };
            if (User.IsInRole("FinanceDataEntry"))
            {
                b.BundleStatusId = BundleStatusCode.OpenForDataEntry;
            }

            CurrentDatabase.BundleHeaders.InsertOnSubmit(b);
            CurrentDatabase.SubmitChanges();

            return Redirect($"/Bundle/Edit/{b.BundleHeaderId}");
        }

        private int GetDefaultFundId()
        {
            var fundId = CurrentDatabase.Setting("DefaultFundId", "0").ToInt();
            if (fundId == 0)
            {
                fundId = CurrentDatabase.ContributionFunds
                    .OrderBy(f => f.FundStatusId)
                    .OrderBy(f => f.FundId).Select(f => f.FundId).FirstOrDefault();
            }
            return fundId;
        }
    }
}
