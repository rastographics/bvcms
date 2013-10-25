using System;
using System.Web.Mvc;
using CmsData.API;
using CmsWeb.Models;
using ContributionSearchModel = CmsWeb.Models.ContributionSearchModel;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance")]
    public class ContributionsController : CmsStaffController
    {
        public ActionResult Index(int? id, int? year, int? fundId, DateTime? dt1, DateTime? dt2, bool? closedbundlesonly, int? campus, int? bundletype, int online = 2)
        {
            var api = new ContributionSearchInfo()
            {
                PeopleId = id,
                Year = year,
                FundId = fundId,
                StartDate = dt1,
                EndDate = dt2,
                CampusId = campus,
                Online = online,
                ClosedBundlesOnly = closedbundlesonly ?? false,
                BundleType = bundletype
            };
            var m = new ContributionSearchModel(api);
            return View(m);
        }
        public ActionResult BundleTotals(int? fundId, DateTime? dt1, DateTime? dt2, bool? closedbundlesonly, int? campus, int? bundletype, int online = 2)
        {
            var api = new ContributionSearchInfo()
            {
                FundId = fundId,
                StartDate = dt1,
                EndDate = dt2,
                CampusId = campus,
                Online = online,
                ClosedBundlesOnly = closedbundlesonly ?? false,
                BundleType = bundletype
            };
            var m = new ContributionSearchModel(api);
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(ContributionSearchModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult Export(ContributionSearchModel m)
        {
            return new ExcelResult(m.ContributionsListAll(), "contributions.xls");
        }
        [HttpPost]
        public ActionResult Return(int id, ContributionSearchModel m)
        {
            m.Return(id);
            return RedirectToAction("Index", new { id = m.SearchInfo.PeopleId, m.SearchInfo.Year });
        }
        [HttpPost]
        public ActionResult Reverse(int id, ContributionSearchModel m)
        {
            m.Reverse(id);
            return RedirectToAction("Index", new { id = m.SearchInfo.PeopleId, m.SearchInfo.Year });
        }
    }
}
