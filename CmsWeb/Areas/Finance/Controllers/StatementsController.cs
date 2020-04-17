using CmsData;
using CmsData.API;
using CmsWeb.Areas.Finance.Models.Report;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance")]
    [RouteArea("Finance", AreaPrefix = "Statements"), Route("{action}")]
    public class StatementsController : CmsController
    {
        public StatementsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Statements")]
        public ActionResult Index()
        {
            var userId = CurrentDatabase.UserId;
            var r = CurrentDatabase.ContributionsRuns.OrderByDescending(mm => mm.Id).FirstOrDefault();
            if (r != null && r.Running == 1 && r.UserId == userId && DateTime.Now.Subtract(r.Started ?? DateTime.MinValue).TotalMinutes < 1)
            {
                return Redirect("/Statements/Progress");
            }

            ViewBag.Previous = r != null;
            return View();
        }

        [HttpPost, Route("Start")]
        public ActionResult ContributionStatements(DateTime? fromDate, DateTime? endDate, string startswith, string sort, int? tagid, bool excludeelectronic, string customstatement = null, bool exportcontributors = false)
        {
            if (!fromDate.HasValue || !endDate.HasValue)
            {
                return Content("<h3>Must have a Startdate and Enddate</h3>");
            }

            if (fromDate.Value > endDate.Value)
            { 
                return Content("<h3>The Startdate must be earlier than the Enddate</h3>");
            }

            var spec = ContributionStatementsExtract.GetStatementSpecification(CurrentDatabase, customstatement);

            if (!startswith.HasValue())
            {
                startswith = null;
            }
            var noaddressok = !CurrentDatabase.Setting("RequireAddressOnStatement", true);
            const bool useMinAmt = true;
            if (tagid == 0)
            {
                tagid = null;
            }
            var qc = APIContribution.Contributors(CurrentDatabase, fromDate.Value, endDate.Value, 0, 0, 0, spec.Funds, noaddressok, useMinAmt, startswith, sort, tagid: tagid, excludeelectronic: excludeelectronic);
            var contributors = qc.ToList();
            if (exportcontributors)
            {
                return ExcelExportModel.ToDataTable(contributors).ToExcel("Contributors.xlsx");
            }
            var statementsRun = new ContributionsRun
            {
                Started = DateTime.Now,
                Count = contributors.Count,
                Processed = 0,
                UUId = Guid.NewGuid(),
                UserId = CurrentDatabase.UserId,
            };
            CurrentDatabase.ContributionsRuns.InsertOnSubmit(statementsRun);
            CurrentDatabase.SubmitChanges();
            var cul = CurrentDatabase.Setting("Culture", "en-US");
            var host = CurrentDatabase.Host;
            var id = $"{statementsRun.UUId:n}";

            var output = Output(host, id);
            if (tagid == 0)
            {
                tagid = null;
            }

            var showCheckNo = CurrentDatabase.Setting("RequireCheckNoOnStatement");
            var showNotes = CurrentDatabase.Setting("RequireNotesOnStatement");
            var statements = new ContributionStatements
            {
                UUId = Guid.Parse(id),
                FromDate = fromDate.Value,
                ToDate = endDate.Value,
                typ = 3,
                //TODO: once we switch to entirely html-based statement templates we won't need to check for these options
                NumberOfColumns = showCheckNo || showNotes ? 1 : 2,
                ShowCheckNo = showCheckNo,
                ShowNotes = showNotes,
            };
            if (CurrentDatabase.Setting("UseNewStatementsFormat"))
            { 
                // Must do this before entering the background worker because it relies on the Application context
                statements.GetConverter();
            }

            var elmah = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current);
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
                try
                {
                    var m = new ContributionStatementsExtract(host, fromDate.Value, endDate.Value, output, startswith, sort, tagid, excludeelectronic)
                    {
                        id = id
                    };
                    m.DoWork(statements, spec, contributors);
                }
                catch (Exception e)
                {
                    elmah.Log(new Elmah.Error(e));
                    var db = CMSDataContext.Create(host);
                    var run = db.ContributionsRuns.Single(c => c.UUId == Guid.Parse(id));
                    run.Error = e.Message;
                    run.Completed = DateTime.Now;
                    db.SubmitChanges();
                }
            });
            return Redirect($"/Statements/Progress/{id}");
        }

        public ActionResult SomeTaskCompleted(string result)
        {
            return Content(result);
        }

        private static string Output(string host, string id)
        {
            var path = Path.Combine(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["SharedFolder"]), "Statements");
            Directory.CreateDirectory(path);
            var output = Path.Combine(path, $"contributions_{host}_{id}.pdf");
            return output;
        }

        [HttpGet, Route("~/Statements/Progress/{id?}")]
        public ActionResult Progress(string id)
        {
            Guid? uuid = id.HasValue() ? Guid.Parse(id) : (Guid?)null;
            var r = CurrentDatabase.ContributionsRuns
                .Where(mm => uuid == null || mm.UUId == uuid)
                .Where(mm => mm.UserId == CurrentDatabase.CurrentUser.UserId)
                .OrderByDescending(mm => mm.Started).First();
            var html = new StringBuilder();
            if (r.CurrSet > 0)
            {
                html.Append($"<a href=\"/Statements/Download/{r.UUId:n}\">PDF with all households</a><br>");
            }

            if (r.Sets.HasValue())
            {
                var sets = r.Sets.Split(',').Select(ss => ss.ToInt()).ToList();
                foreach (var set in sets)
                {
                    html.Append($"<a href=\"/Statements/Download/{r.UUId:n}/{set}\">Download PDF ({set} page format)</a><br>");
                }
            }
            ViewBag.download = html.ToString();
            return View(r);
        }

        [HttpGet, Route("~/Statements/Download/{id}/{idx:int?}")]
        public ActionResult Download(string id, int? idx)
        {
            var output = Output(CurrentDatabase.Host, id);
            var fn = output;
            if (idx.HasValue)
            {
                fn = ContributionStatementsExtract.Output(output, idx.Value);
            }

            if (!System.IO.File.Exists(fn))
            {
                return Content("no pending download");
            }

            return new ContributionStatementsResult(fn);
        }
    }
}
