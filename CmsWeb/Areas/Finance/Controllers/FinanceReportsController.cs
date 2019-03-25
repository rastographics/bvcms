using CmsData;
using CmsData.API;
using CmsWeb.Areas.Finance.Models.Report;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using TableStyles = OfficeOpenXml.Table.TableStyles;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance,FinanceViewOnly")]
    [RouteArea("Finance", AreaPrefix = "FinanceReports"), Route("{action}/{id?}")]
    public class FinanceReportsController : CmsStaffController
    {
        public FinanceReportsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult ContributionStatement(int id, DateTime fromDate, DateTime toDate, int typ)
        {
            DbUtil.LogActivity($"Contribution Statement for ({id})");
            return new ContributionStatementResult
            {
                PeopleId = id,
                FromDate = fromDate,
                ToDate = toDate,
                typ = typ
            };
        }

        private DynamicParameters DonorTotalSummaryParameters(DonorTotalSummaryOptionsModel model, bool useMedianMin = false)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@enddt", model.StartDate);
            queryParameters.Add("@years", model.NumberOfYears);
            queryParameters.Add("@fund", model.Fund.Value.ToInt());
            queryParameters.Add("@campus", model.Campus.Value.ToInt());

            if (useMedianMin)
            {
                queryParameters.Add("@medianMin", model.MinimumMedianTotal);
            }

            if (model?.FundSet != null) // TODO: seems like a redundant null check, if model was null, it would have errored well before this point
            {
                var fundset = APIContributionSearchModel.GetCustomStatementsList(CurrentDatabase, model.FundSet.Value).JoinInts(",");
                queryParameters.Add("@fundids", fundset);
            }
            else
            {
                var authorizedFunds = CurrentDatabase.ContributionFunds.ScopedByRoleMembership().Select(f => f.FundId).ToList();
                var authorizedFundsCsv = string.Join(",", authorizedFunds);

                queryParameters.Add("@fundids", authorizedFundsCsv);
            }

            return queryParameters;
        }

        [HttpGet]
        public EpplusResult DonorTotalSummary(DonorTotalSummaryOptionsModel model)
        {
            var excel = new ExcelPackage();
            var connection = new SqlConnection(Util.ConnectionString);

            var totalSummaryParameters = DonorTotalSummaryParameters(model, useMedianMin: true);
            var totalSummary = connection.ExecuteReader("dbo.DonorTotalSummary", totalSummaryParameters, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            excel.AddSheet(totalSummary, "MemberNon");
            totalSummary.Close();

            var totalSummaryBySizeParameters = DonorTotalSummaryParameters(model);
            var totalSummaryBySize = connection.ExecuteReader("dbo.DonorTotalSummaryBySize", totalSummaryBySizeParameters, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            excel.AddSheet(totalSummaryBySize, "BySize");
            totalSummaryBySize.Close();

            var totalSummaryByAgeParameters = DonorTotalSummaryParameters(model);
            var totalSummaryByAge = connection.ExecuteReader("dbo.DonorTotalSummaryByAge", totalSummaryByAgeParameters, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            excel.AddSheet(totalSummaryByAge, "ByAge");
            totalSummaryByAge.Close();

            return CreateExcelResult(excel, "DonorTotalSummary.xlsx");
        }

        private static EpplusResult CreateExcelResult(ExcelPackage excelPackage, string fileName)
        {
            return new EpplusResult(excelPackage, fileName);
        }

        [HttpGet]
        public ActionResult DonorTotalSummaryOptions()
        {
            var model = new DonorTotalSummaryOptionsModel
            {
                StartDate = DateTime.Today,
                NumberOfYears = 5,
                MinimumMedianTotal = 100,
                Campus = CreateCodeInfoForField("Campus0"),
                Fund = CreateCodeInfoForField("Fund")
            };

            var customFundsList = ContributionStatements.CustomFundSetSelectList(CurrentDatabase);

            if (customFundsList != null)
            {
                model.FundSet = new CodeInfo(null, customFundsList);
            }

            return View(model);
        }

        [HttpGet]
        public EpplusResult ChaiDonorsReportDownload(DonorTotalSummaryOptionsModel model)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@fund", model.Fund.Value.ToInt());

            var authorizedFunds = CurrentDatabase.ContributionFunds.ScopedByRoleMembership().Select(f => f.FundId).ToList();
            var authorizedFundsCsv = string.Join(",", authorizedFunds);
            queryParameters.Add("@authorizedFundIds", authorizedFundsCsv);

            var excel = new ExcelPackage();
            var connection = new SqlConnection(Util.ConnectionString);

            var reader = connection.ExecuteReader("dbo.CHAIDonationsReport2", queryParameters, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            excel.AddSheet(reader, "CHAIDonations");
            reader.Close();

            return CreateExcelResult(excel, "CHAIDonationsReport.xlsx");
        }

        [HttpGet]
        public ActionResult ChaiDonorsReport()
        {
            //Re-using a model, projected that this report will use most fields in this model, in next rollout.
            var model = new DonorTotalSummaryOptionsModel
            {
                StartDate = DateTime.Today,
                NumberOfYears = 5,
                MinimumMedianTotal = 100,
                Campus = CreateCodeInfoForField("Campus0"),
                Fund = CreateCodeInfoForField("Fund"),
            };

            return View(model);
        }

        private static CodeInfo CreateCodeInfoForField(string fieldName)
        {
            return new CodeInfo(fieldName);
        }

        public ActionResult PledgeReport()
        {
            var fromDate = DateTime.Parse("1/1/1900");
            var toDate = DateTime.Parse("1/1/2099");
            var queryResult = from pledgeReports in CurrentDatabase.PledgeReport(fromDate, toDate, 0)
                              join allowedFunds in CurrentDatabase.ContributionFunds.ScopedByRoleMembership() on pledgeReports.FundId equals allowedFunds.FundId
                              orderby pledgeReports.FundId descending
                              select pledgeReports;

            return View(queryResult);
        }

        [HttpGet, Route("~/PledgeFulfillment2/{fundid1:int}/{fundid2:int}")]
        public EpplusResult PledgeFulfillment2(int fundid1, int fundid2)
        {
            var excel = new ExcelPackage();
            var connection = new SqlConnection(Util.ConnectionString);
            connection.Open();

            var reader = connection.ExecuteReader("dbo.PledgeFulfillment2", new { fundid1, fundid2, }, commandTimeout: 1200, commandType: CommandType.StoredProcedure);
            excel.AddSheet(reader, "Pledges");
            reader.Close();

            return CreateExcelResult(excel, "PledgeFulfillment2.xlsx");
        }

        [HttpGet]
        public ActionResult DonorTotalsByRange()
        {
            var model = new TotalsByFundModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult DonorTotalsByRangeResults(TotalsByFundModel model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult TotalsByFund()
        {
            var model = new TotalsByFundModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult TotalsByFundExport(TotalsByFundModel model)
        {
            model.SaveAsExcel();
            return SimpleContent("done");
        }

        private ContentResult SimpleContent(string message)
        {
            return Content(message);
        }

        [HttpPost, Route("~/TotalsByFundCustomReport/{id}")]
        public ActionResult TotalsByFundCustomReport(string id, TotalsByFundModel model)
        {
            var content = CurrentDatabase.ContentOfTypeSql(id);
            if (content == null)
            {
                return SimpleContent("no content");
            }

            var p = model.GetDynamicParameters();
            ViewBag.Name = id.SpaceCamelCase();

            var linkUrl = CurrentDatabase.ServerLink($"/TotalsByFundCustomExport/{id}");
            var linkHtml = $"<a href='{linkUrl}' class='CustomExport btn btn-default' target='_blank'><i class='fa fa-file-excel-o'></i> Download as Excel</a>";

            var connection = new SqlConnection(Util.ConnectionStringReadOnlyFinance);
            connection.Open();

            var reader = connection.ExecuteReader(content, p, commandTimeout: 1200);
            var contentTable = GridResult.Table(reader, id.SpaceCamelCase(), excellink: linkHtml);

            return SimpleContent(contentTable);
        }

        [HttpPost, Route("~/TotalsByFundCustomExport/{id}")]
        public ActionResult TotalsByFundCustomExport(string id, TotalsByFundModel model)
        {
            var content = CurrentDatabase.ContentOfTypeSql(id);
            if (content == null)
            {
                return SimpleContent("no content");
            }

            var connection = new SqlConnection(Util.ConnectionStringReadOnlyFinance);
            connection.Open();
            var queryParameters = model.GetDynamicParameters();

            var s = id.SpaceCamelCase();
            return connection.ExecuteReader(content, queryParameters, commandTimeout: 1200).ToExcel(s + ".xlsx", fromSql: true);
        }

        [HttpPost, Route("~/FundList")]
        public ActionResult FundList(TotalsByFundModel model)
        {
            return SimpleContent($@"<pre>{string.Join(",", APIContributionSearchModel.GetCustomFundSetList(CurrentDatabase, model.FundSet))}</pre>");
        }

        [HttpPost]
        public ActionResult TotalsByFundResults(TotalsByFundModel model)
        {
            if (model.IncludeBundleType)
            {
                return View("TotalsByFundResults2", model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult TotalsByFundRange(bool? pledged)
        {
            var model = new TotalsByFundRangeModel { Pledged = pledged ?? false };
            return View(model);
        }

        [HttpPost]
        public ActionResult TotalsByFundRangeResults(TotalsByFundRangeModel model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult TotalsByFundAgeRange()
        {
            var model = new TotalsByFundAgeRangeModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult TotalsByFundAgeRangeResults(TotalsByFundAgeRangeModel model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult Deposits(DateTime dt)
        {
            var model = new DepositsModel(dt);
            return View(model);
        }

        [HttpGet]
        public ActionResult DepositTotalsForDates()
        {
            var model = new DepositTotalsModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult DepositTotalsForDatesResults(DepositTotalsModel model)
        {
            return View(model);
        }

        public ActionResult ManagedGiving(string sortBy, string sortDir)
        {
            var query = CurrentDatabase.ViewManagedGivingLists.AsQueryable();

            if (sortBy == "name")
            {
                if (string.IsNullOrEmpty(sortDir) || sortDir == "asc")
                {
                    query = query.OrderBy(mgl => mgl.Name2);
                }
                else
                {
                    query = query.OrderByDescending(mgl => mgl.Name2);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(sortDir) || sortDir == "asc")
                {
                    query = query.OrderBy(mgl => mgl.NextDate);
                }
                else
                {
                    query = query.OrderByDescending(mgl => mgl.NextDate);
                }
            }

            ViewBag.SortDir = sortDir == "asc" ? "desc" : "asc";
            return View(query.ToList());
        }

        [HttpGet]
        public ActionResult ManageGiving2(int id)
        {
            var model = new ManageGivingModel(id);
            model.testing = true;
            var body = ViewExtensions2.RenderPartialViewToString(this, "ManageGiving2", model);
            return SimpleContent(body);
        }

        public ActionResult PledgeFulfillments(int id)
        {
            var q = CurrentDatabase.PledgeFulfillment(id)
                .OrderByDescending(vv => vv.PledgeAmt)
                .ThenByDescending(vv => vv.TotalGiven).ToList();
            var count = q.Count;

            if (count == 0)
            {
                return Message("No Pledges to Report");
            }

            var cols = CurrentDatabase.Mapping.MappingSource.GetModel(typeof(CMSDataContext))
                .GetMetaType(typeof(CmsData.View.PledgeFulfillment)).DataMembers;

            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");

            ws.Cells["A2"].LoadFromCollection(q);

            var range = ws.Cells[1, 1, count + 1, cols.Count];
            var table = ws.Tables.Add(range, "Pledges");
            table.ShowTotal = true;
            table.ShowFilter = false;
            table.TableStyle = TableStyles.Light9;

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            for (var i = 0; i < cols.Count; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];
                switch (name)
                {
                    case "First":
                        table.Columns[i].TotalsRowLabel = "Total";
                        break;
                    case "Last":
                        table.Columns[i].TotalsRowFormula = @"CONCATENATE(""Count: "", SUBTOTAL(103,[Last]))";
                        break;
                    case "PledgeAmt":
                    case "TotalGiven":
                    case "Balance":
                        table.Columns[i].TotalsRowFormula = $"SUBTOTAL(109,[{name}])";
                        colrange.Style.Numberformat.Format = "#,##0.00;(#,##0.00)";
                        colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Column(col).Width = 12;
                        break;
                    case "PledgeDate":
                    case "LastDate":
                        colrange.Style.Numberformat.Format = "mm-dd-yy";
                        colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Column(col).Width = 12;
                        break;
                    case "Zip":
                    case "CreditGiverId":
                    case "SpouseId":
                    case "FamilyId":
                        colrange.Style.Numberformat.Format = "@";
                        colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        break;
                }
            }

            return new EpplusResult(ep, $"PledgeFulfillment - {id}.xlsx");
        }
    }
}
