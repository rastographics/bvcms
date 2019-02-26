using CmsData.API;
using CmsWeb.Lifecycle;
using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Routing;
using CmsData;
using CmsWeb.Common.Extensions;
using CmsWeb.Models;
using Dapper;
using MoreLinq;
using Newtonsoft.Json;
using UtilityExtensions;
using ContributionSearchModel = CmsWeb.Models.ContributionSearchModel;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance")]
    [RouteArea("Finance", AreaPrefix = "Contributions"), Route("{action}/{id?}")]
    public class ContributionsController : CmsStaffController
    {
        public ContributionsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Contributions/{id:int?}")]
        public ActionResult Index(int? id, int? year, int? fundId, DateTime? dt1, DateTime? dt2, int? campus, int? bundletype,
            bool? includeunclosedbundles = true, int online = 2, string taxnontax = "TaxDed", string fundSet = null, 
			bool filterbyactivetag = false, string setQueryTag = null)
        {
            if (setQueryTag.HasValue())
                Util2.CurrentTag = $"QueryTag:{setQueryTag}";
            var api = new ContributionSearchInfo()
            {
                PeopleId = id,
                Year = year,
                FundId = fundId,
                StartDate = dt1,
                EndDate = dt2,
                CampusId = campus,
                Online = online,
                TaxNonTax = taxnontax,
                IncludeUnclosedBundles = includeunclosedbundles ?? false,
                BundleType = bundletype,
                FilterByActiveTag = filterbyactivetag
            };

            // Only setting it like this for debug purpose
            if (fundSet != null)
            {
                api.FundSet = fundSet;
            }
            var m = new ContributionSearchModel(api);
            return View(m);
        }
        [Route("~/ContributionsJsonSearch/{file}/{name}")]
        public ActionResult ContributionsJsonSearch(string file, string name)
        {
            var text = CurrentDatabase.ContentOfTypeText(file);
            var data = JsonConvert.DeserializeObject<DynamicData>(text);
            var dd = (DynamicData)data[name];
            var json = dd["parms"].ToString();
            var cmd = new SqlCommand(Resource1.ContributionsController_ContributionsAdvancedSearch_);
            cmd.Parameters.AddWithValue("@json", json);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            return cmd.ExecuteReader().ToExcel($"Contributions-{name}.xlsx");
        }
        [Route("~/BundleTotals")]
        public ActionResult BundleTotals(int? fundId, DateTime? dt1, DateTime? dt2, int? campus, int? bundletype,
            bool? includeunclosedbundles = true, int online = 2, string taxnontax = "TaxDed")
        {
            var api = new ContributionSearchInfo()
            {
                FundId = fundId,
                StartDate = dt1,
                EndDate = dt2,
                CampusId = campus,
                Online = online,
                TaxNonTax = taxnontax,
                IncludeUnclosedBundles = includeunclosedbundles ?? false,
                BundleType = bundletype
            };
            var m = new ContributionSearchModel(api);
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(ContributionSearchModel m)
        {
            var ret = m.CheckConversion();
            if (ret.HasValue())
            {
                return Content(ret);
            }

            return View(m);
        }
        [HttpPost]
        public ActionResult Export(ContributionSearchModel m)
        {
            return m.ContributionsListAllExcel();
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
