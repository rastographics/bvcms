using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Finance.Models.Report;
using CmsData;
using CmsData.Classes.QuickBooks;
using System.IO;
using UtilityExtensions;
using CmsWeb.Models;
using System.Text;
using System.Web.UI;
using System.Data.SqlClient;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance")]
    public class FinanceReportsController : CmsStaffController
    {
        public ActionResult ContributionYears(int id)
        {
            var m = new ContributionModel(id);
            return View(m);
        }
        public ActionResult ContributionStatement(int id, DateTime fromDate, DateTime toDate, int typ)
        {
            DbUtil.LogActivity("Contribution Statement for ({0})".Fmt(id));
            return new ContributionStatementResult
                       {
                           PeopleId = id, 
                           FromDate = fromDate,
                           ToDate = toDate,
                           typ = typ
                       };
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
        public ActionResult TotalsByFundResults(TotalsByFundModel m)
        {
            if(m.IncludeBundleType)
                return View("TotalsByFundResults2", m);
            return View(m);
        }
		[HttpGet]
        public ActionResult BundleTotals()
		{
			var m = new TotalsByFundModel();
            return View(m);
        }
        public ActionResult PledgeReport()
        {
        	var fd = DateTime.Parse("1/1/1900");
        	var td = DateTime.Parse("1/1/2099");
        	var q = from r in DbUtil.Db.PledgeReport(fd, td, 0)
        	        select r;
		    return View(q);
        }
        public ActionResult ManagedGiving()
        {
			var q = from rg in DbUtil.Db.ManagedGivings.ToList()
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
            List<int> lFunds = new List<int>();
            List<QBJournalEntryLine> qbjel = new List<QBJournalEntryLine>();

			var entries = m.TotalsByFund();

            QuickBooksHelper qbh = new QuickBooksHelper();

			foreach (var item in entries)
			{
                if (item.QBSynced > 0) continue;

				var accts = (from e in DbUtil.Db.ContributionFunds
							where e.FundId == item.FundId
							select e).Single();

                if (accts.QBAssetAccount > 0 && accts.QBIncomeAccount > 0)
                {
                    QBJournalEntryLine jelCredit = new QBJournalEntryLine();

                    jelCredit.sDescrition = item.FundName;
                    jelCredit.dAmount = item.Total ?? 0;
                    jelCredit.sAccountID = accts.QBIncomeAccount.ToString();
                    jelCredit.bCredit = true;

                    QBJournalEntryLine jelDebit = new QBJournalEntryLine(jelCredit);

                    jelDebit.sAccountID = accts.QBAssetAccount.ToString();
                    jelDebit.bCredit = false;

                    qbjel.Add(jelCredit);
                    qbjel.Add(jelDebit);

                    lFunds.Add(item.FundId ?? 0);
                }
			}

            int iJournalID = qbh.CommitJournalEntries( "Bundle from BVCMS", qbjel );

            if (iJournalID > 0)
            {
                string sStart = m.Dt1.Value.ToString("u");
                string sEnd = m.Dt2.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("u");

                sStart = sStart.Substring(0, sStart.Length - 1);
                sEnd = sEnd.Substring(0, sEnd.Length - 1);

                string sFundList = string.Join( ",", lFunds.ToArray() );
                string sUpdate = "UPDATE dbo.Contribution SET QBSyncID = " + iJournalID + " WHERE FundId IN (" + sFundList + ") AND ContributionDate BETWEEN '" + sStart + "' and '" + sEnd + "'";

                DbUtil.Db.ExecuteCommand( sUpdate );
            }

            return View("TotalsByFund", m);
		}
		public ActionResult PledgeFulfillments(int id)
		{
			var list = DbUtil.Db.PledgeFulfillment(id).OrderBy(vv => vv.Last).ThenBy(vv => vv.First);
			return new DataGridResult(list);
		}
    }
}
