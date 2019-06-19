using CmsData.Codes;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;


namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance,FinanceDataEntry")]
    [RouteArea("Finance", AreaPrefix = "Bundle"), Route("{action}/{id?}")]
    public class BundleController : CmsStaffController
    {
        public BundleController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Bundle/{id:int}")]
        public ActionResult Index(int id, bool? create)
        {
            var m = new Models.BundleModel(id, CurrentDatabase);
            if (User.IsInRole("FinanceDataEntry") && m.BundleStatusId != BundleStatusCode.OpenForDataEntry)
            {
                return Redirect("/Bundles");
            }

            if (m.Bundle == null)
            {
                return Content("no bundle");
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Results(Models.BundleModel m)
        {
            return View(m);
        }

        public ActionResult Edit(int id)
        {
            TempData["editbundle"] = true;
            return RedirectToAction("Index", new { id = id });
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            var m = new Models.BundleModel(id, CurrentDatabase);
            return View(m);
        }

        [HttpPost]
        public ActionResult Update(int id)
        {
            var m = new Models.BundleModel(id, CurrentDatabase);
            UpdateModel(m);
            UpdateModel(m.Bundle, "Bundle");
            var q = from d in CurrentDatabase.BundleDetails
                    where d.BundleHeaderId == m.Bundle.BundleHeaderId
                    select d.Contribution;
            var dt = q.Select(cc => cc.ContributionDate).FirstOrDefault();
            if (m.Bundle.ContributionDateChanged && q.All(cc => cc.ContributionDate == dt))
            {
                foreach (var c in q)
                {
                    c.ContributionDate = m.Bundle.ContributionDate;
                }
            }
            var fid = q.Select(cc => cc.FundId).FirstOrDefault();
            if (m.Bundle.FundIdChanged && q.All(cc => cc.FundId == fid))
            {
                foreach (var c in q)
                {
                    c.FundId = m.Bundle.FundId ?? 1;
                }
            }
            var postingdt = Util.Now;
            if (m.Bundle.BundleStatusIdChanged && m.Bundle.BundleStatusId == BundleStatusCode.Closed)
            {
                foreach (var d in m.Bundle.BundleDetails)
                {
                    d.Contribution.PostingDate = postingdt;
                }
            }
            CurrentDatabase.SubmitChanges();
            if (User.IsInRole("FinanceDataEntry"))
            {
                return Redirect("/Bundles");
            }

            m.BundleId = id; // refresh values
            return View("Display", m);
        }

        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var m = new Models.BundleModel(id, CurrentDatabase);
            return View("Display", m);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var m = new Models.BundleModel(id, CurrentDatabase);
            var q = from d in m.Bundle.BundleDetails
                    select d.Contribution;
            CurrentDatabase.Contributions.DeleteAllOnSubmit(q);
            CurrentDatabase.BundleDetails.DeleteAllOnSubmit(m.Bundle.BundleDetails);
            CurrentDatabase.BundleHeaders.DeleteOnSubmit(m.Bundle);
            CurrentDatabase.SubmitChanges();
            return Content("/Bundles");
        }

        [HttpGet]
        public ActionResult Export(int id)
        {
            var query = CurrentDatabase.ContentOfTypeSql("BundleExportSql").DefaultTo(@"
            SELECT header.BundleHeaderId [Bundle ID]
	            ,contrib.ContributionDate [Date]
	            ,people.Name [Name]
	            ,contrib.ContributionAmount [Amount]
	            ,fund.FundIncomeAccount [Income Account]
	            ,fund.FundId [Fund ID]
            FROM dbo.BundleHeader header
            JOIN dbo.BundleDetail detail ON detail.BundleHeaderId = header.BundleHeaderId
            JOIN dbo.Contribution contrib ON contrib.ContributionId = detail.ContributionId
            JOIN dbo.ContributionFund fund ON fund.FundId = contrib.FundId
            JOIN dbo.People people ON people.PeopleId = contrib.PeopleId
            WHERE contrib.PledgeFlag = 0 AND header.BundleHeaderId = @BundleId
            ORDER BY contrib.ContributionDate
            ");

            var connection = CurrentDatabase.ReadonlyConnection();
            connection.Open();
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@BundleId", id);
            var filename = $"Bundle-Export-{id}.xlsx";

            return connection.ExecuteReader(query, queryParameters, commandTimeout: 1200).ToExcel(filename, fromSql: true);
        }
    }
}
