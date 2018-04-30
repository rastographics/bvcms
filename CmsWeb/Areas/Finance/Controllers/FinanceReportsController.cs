using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Finance.Models.Report;
using CmsData;
using CmsData.API;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Code;
using Dapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UtilityExtensions;
using CmsWeb.Models;
using CmsWeb.Areas.OnlineReg.Models;
using TableStyles = OfficeOpenXml.Table.TableStyles;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance,FinanceViewOnly")]
    [RouteArea("Finance", AreaPrefix = "FinanceReports"), Route("{action}/{id?}")]
    public class FinanceReportsController : CmsStaffController
    {
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

        private DynamicParameters DonorTotalSummaryParameters(DonorTotalSummaryOptionsModel m, bool useMedianMin = false)
        {
            var p = new DynamicParameters();
            p.Add("@enddt", m.StartDate);
            p.Add("@years", m.NumberOfYears);
            if(useMedianMin)
                p.Add("@medianMin", m.MinimumMedianTotal);
            p.Add("@fund", m.Fund.Value.ToInt());
            p.Add("@campus", m.Campus.Value.ToInt());
            if (m?.FundSet != null)
            {
                var fundset = APIContributionSearchModel.GetCustomStatementsList(m.FundSet.Value).JoinInts(",");
                p.Add("@fundids", fundset);
            }
            else
                p.Add("@fundids", null);
            return p;
        }
        [HttpGet]
        public EpplusResult DonorTotalSummary(DonorTotalSummaryOptionsModel m)
        {
            var ep = new ExcelPackage();
            var cn = new SqlConnection(Util.ConnectionString);

            var p = DonorTotalSummaryParameters(m, useMedianMin: true);
            var rd = cn.ExecuteReader("dbo.DonorTotalSummary", p, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            ep.AddSheet(rd, "MemberNon");

            p = DonorTotalSummaryParameters(m);
            rd = cn.ExecuteReader("dbo.DonorTotalSummaryBySize", p, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            ep.AddSheet(rd, "BySize");

            rd = cn.ExecuteReader("dbo.DonorTotalSummaryByAge", p, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            ep.AddSheet(rd, "ByAge");

            return new EpplusResult(ep, "DonorTotalSummary.xlsx");
        }

        [HttpGet]
        public EpplusResult ChaiDonorsReportDownload(DonorTotalSummaryOptionsModel m)
        {
            var p = new DynamicParameters();
            p.Add("@fund", m.Fund.Value.ToInt());
            var ep = new ExcelPackage();
            var cn = new SqlConnection(Util.ConnectionString);

            var rd = cn.ExecuteReader("dbo.CHAIDonationsReport", p, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            ep.AddSheet(rd, "CHAIDonations");

            return new EpplusResult(ep, "CHAIDonationsReport.xlsx");
        }

        [HttpGet]
        public ActionResult ChaiDonorsReport()
        {
            //Re-using a model, projected that this report will use most fields in this model, in next rollout.
            var m = new DonorTotalSummaryOptionsModel
            {
                StartDate = DateTime.Today,
                NumberOfYears = 5,
                MinimumMedianTotal = 100,
                Campus = new CodeInfo("Campus0"),
                Fund = new CodeInfo("Fund"),
            };
            return View(m);
        }

        [HttpGet, Route("~/PledgeFulfillment2/{fundid1:int}/{fundid2:int}")]
        public EpplusResult PledgeFulfillment2(int fundid1, int fundid2)
        {
            var ep = new ExcelPackage();
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();

            var rd = cn.ExecuteReader("dbo.PledgeFulfillment2", new { fundid1, fundid2, },
                commandTimeout: 1200, commandType: CommandType.StoredProcedure);
            ep.AddSheet(rd, "Pledges");
            return new EpplusResult(ep, "PledgeFulfillment2.xlsx");
        }

        [HttpGet]
        public ActionResult DonorTotalSummaryOptions()
        {
            var m = new DonorTotalSummaryOptionsModel
            {
                StartDate = DateTime.Today,
                NumberOfYears = 5,
                MinimumMedianTotal = 100,
                Campus = new CodeInfo("Campus0"),
                Fund = new CodeInfo("Fund"),
            };
            var customfunds = ContributionStatements.CustomFundSetSelectList();
            if(customfunds != null)
                m.FundSet = new CodeInfo(null, customfunds);
            return View(m);
        }
        [HttpGet]
        public ActionResult DonorTotalsByRange()
        {
            var m = new TotalsByFundModel();
            return View(m);
        }

        [HttpPost]
        public ActionResult DonorTotalsByRangeResults(TotalsByFundModel m)
        {
            return View(m);
        }

        [HttpGet]
        public ActionResult TotalsByFund()
        {
            var m = new TotalsByFundModel();
            return View(m);
        }
        [HttpPost]
        public ActionResult TotalsByFundExport(TotalsByFundModel m)
        {
            m.SaveAsExcel();
            return Content("done");
        }
        [HttpPost, Route("~/TotalsByFundCustomReport/{id}")]
        public ActionResult TotalsByFundCustomReport(string id, TotalsByFundModel m)
        {
            var content = DbUtil.Db.ContentOfTypeSql(id);
            if (content == null)
                return Content("no content");
            var cs = Util.ConnectionStringReadOnlyFinance;
            var cn = new SqlConnection(cs);
            cn.Open();
            var p = m.GetDynamicParameters();

            ViewBag.Name = id.SpaceCamelCase();
            var rd = cn.ExecuteReader(content, p, commandTimeout: 1200);
            var excelink = DbUtil.Db.ServerLink($"/TotalsByFundCustomExport/{id}");
            var link = $"<a href='{excelink}' class='CustomExport btn btn-default' target='_blank'><i class='fa fa-file-excel-o'></i> Download as Excel</a>";
            return Content(GridResult.Table(rd, id.SpaceCamelCase(), excellink: link));
        }
        [HttpPost, Route("~/TotalsByFundCustomExport/{id}")]
        public ActionResult TotalsByFundCustomExport(string id, TotalsByFundModel m)
        {
            var content = DbUtil.Db.ContentOfTypeSql(id);
            if (content == null)
                return Content("no content");
            var cs = Util.ConnectionStringReadOnlyFinance;
            var cn = new SqlConnection(cs);
            cn.Open();
            var p = m.GetDynamicParameters();

            var s = id.SpaceCamelCase();
            return cn.ExecuteReader(content, p, commandTimeout: 1200).ToExcel(s + ".xlsx", fromSql: true);
        }

        [HttpPost]
        public ActionResult TotalsByFundResults(TotalsByFundModel m)
        {
            if (m.IncludeBundleType)
                return View("TotalsByFundResults2", m);
            return View(m);
        }

        [HttpGet]
        public ActionResult TotalsByFundRange(bool? pledged)
        {
            var m = new TotalsByFundRangeModel{ Pledged = pledged ?? false };
            return View(m);
        }

        [HttpPost]
        public ActionResult TotalsByFundRangeResults(TotalsByFundRangeModel m)
        {
            return View(m);
        }

        [HttpGet]
        public ActionResult TotalsByFundAgeRange()
        {
            var m = new TotalsByFundAgeRangeModel();
            return View(m);
        }

        [HttpPost]
        public ActionResult TotalsByFundAgeRangeResults(TotalsByFundAgeRangeModel m)
        {
            return View(m);
        }

        [HttpGet]
        public ActionResult Deposits(DateTime dt)
        {
            var m = new DepositsModel(dt);
            return View(m);
        }
        [HttpGet]
        public ActionResult DepositTotalsForDates()
        {
            var m = new DepositTotalsModel();
            return View(m);
        }

        [HttpPost]
        public ActionResult DepositTotalsForDatesResults(DepositTotalsModel m)
        {
            return View(m);
        }

        public ActionResult PledgeReport()
        {
            var fd = DateTime.Parse("1/1/1900");
            var td = DateTime.Parse("1/1/2099");
            var q = from r in DbUtil.Db.PledgeReport(fd, td, 0)
                    orderby r.FundId descending
                    select r;
            return View(q);
        }

        public ActionResult ManagedGiving(string sortBy, string sortDir)
        {
            if (sortBy == "name")
            {
                if (string.IsNullOrEmpty(sortDir) || sortDir == "desc")
                {
                    var q2 = from rg in DbUtil.Db.ViewManagedGivingLists.ToList()
                        orderby rg.Name2 ascending
                        select rg;

                    ViewBag.SortDir = "asc";
                    return View(q2);
                }
                else
                {
                    var q2 = from rg in DbUtil.Db.ViewManagedGivingLists.ToList()
                            orderby rg.Name2 descending
                            select rg;

                    ViewBag.SortDir = "desc";
                    return View(q2);
                }

            }
            var q = from rg in DbUtil.Db.ViewManagedGivingLists.ToList()
                    orderby rg.NextDate
                    select rg;
            return View(q);
        }

        [HttpGet]
        public ActionResult ManageGiving2(int id)
        {
            var m = new ManageGivingModel(id);
            m.testing = true;
            var body = ViewExtensions2.RenderPartialViewToString(this, "ManageGiving2", m);
            return Content(body);
        }

        [HttpPost]
        public ActionResult ToQuickBooks(TotalsByFundModel m)
        {
//            List<int> lFunds = new List<int>();
//            List<QBJournalEntryLine> qbjel = new List<QBJournalEntryLine>();
//
//            var entries = m.TotalsByFund();
//
//            QuickBooksHelper qbh = new QuickBooksHelper();
//
//            foreach (var item in entries)
//            {
//                if (item.QBSynced > 0) continue;
//
//                var accts = (from e in DbUtil.Db.ContributionFunds
//                             where e.FundId == item.FundId
//                             select e).Single();
//
//                if (accts.QBAssetAccount > 0 && accts.QBIncomeAccount > 0)
//                {
//                    QBJournalEntryLine jelCredit = new QBJournalEntryLine();
//
//                    jelCredit.sDescrition = item.FundName;
//                    jelCredit.dAmount = item.Total ?? 0;
//                    jelCredit.sAccountID = accts.QBIncomeAccount.ToString();
//                    jelCredit.bCredit = true;
//
//                    QBJournalEntryLine jelDebit = new QBJournalEntryLine(jelCredit);
//
//                    jelDebit.sAccountID = accts.QBAssetAccount.ToString();
//                    jelDebit.bCredit = false;
//
//                    qbjel.Add(jelCredit);
//                    qbjel.Add(jelDebit);
//
//                    lFunds.Add(item.FundId ?? 0);
//                }
//            }
//
//            int iJournalID = qbh.CommitJournalEntries("Bundle from BVCMS", qbjel);
//
//            if (iJournalID > 0)
//            {
//                string sStart = m.Dt1.Value.ToString("u");
//                string sEnd = m.Dt2.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("u");
//
//                sStart = sStart.Substring(0, sStart.Length - 1);
//                sEnd = sEnd.Substring(0, sEnd.Length - 1);
//
//                string sFundList = string.Join(",", lFunds.ToArray());
//                string sUpdate = "UPDATE dbo.Contribution SET QBSyncID = " + iJournalID + " WHERE FundId IN (" +
//                                 sFundList + ") AND ContributionDate BETWEEN '" + sStart + "' and '" + sEnd + "'";
//
//                DbUtil.Db.ExecuteCommand(sUpdate);
//            }
//
//            return View("TotalsByFund", m);
            return new EmptyResult();
        }

        public ActionResult PledgeFulfillments(int id)
        {
            var q = DbUtil.Db.PledgeFulfillment(id)
                .OrderByDescending(vv => vv.PledgeAmt)
                .ThenByDescending(vv => vv.TotalGiven).ToList();
            var count = q.Count;

            if(count == 0)
                return Message("No Pledges to Report");

            var cols = DbUtil.Db.Mapping.MappingSource.GetModel(typeof(CMSDataContext))
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
