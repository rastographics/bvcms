using CmsWeb.Areas.Reports.Models;
using CmsWeb.Areas.Reports.ViewModels;
using System;
using System.Linq;
using System.Security;
using System.Web.Mvc;

namespace CmsWeb.Areas.Reports.Controllers
{
    public partial class ReportsController
    {
        [HttpGet]
        [Route("Custom/{report}/{id?}")]
        public ActionResult CustomReport(Guid id, string report)
        {
            var m = new CustomReportsModel(CurrentDatabase, report, id);
            return View(m);
        }

        [HttpGet]
        [Route("CustomExcel/{report}/{id?}")]
        public ActionResult CustomReportExcel(Guid id, string report)
        {
            var m = new CustomReportsModel(CurrentDatabase, report, id);
            return m.Result();
        }

        [HttpGet]
        [Route("CustomSql/{report}")]
        public ActionResult CustomReportSql(string report)
        {
            try
            {
                var m = new CustomReportsModel(CurrentDatabase, report);
                return Content($"<pre style='font-family:monospace'>{m.Sql()}\n</pre>");
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
        }

        [HttpGet]
        [Route("SqlReportExcel/{report}/{id?}")]
        public ActionResult SqlReportExcel(string report, Guid id)
        {
            try
            {
                var m = new SpecialReportViewModel(report, id);
                return m.RunSqlExcel();
            }
            catch (Exception ex)
            {
                return Message(ex);
            }
        }
        [HttpGet]
        [Route("SqlReport/{report}/{id?}")]
        public ActionResult SqlReport(string report, Guid id)
        {
            try
            {
                var m = new SpecialReportViewModel(report, id);
                m.RunSqlReport();
                return View(m);
            }
            catch (Exception ex)
            {
                return Message(ex);
            }
        }

        [HttpGet]
        [Route("PyScript/{report}/{id?}")]
        public ActionResult PyScript(string report, Guid id)
        {
            try
            {
                var m = new SpecialReportViewModel(report, id);
                m.RunPyScript();
                return View(m);
            }
            catch (Exception ex)
            {
                return Message(ex);
            }
        }

        [Authorize(Roles = "Admin, Design")]
        public ActionResult AddReport(string report, string url, string type)
        {
            var m = new CustomReportsModel(CurrentDatabase);
            try
            {
                var dest = m.AddReport(report, url, type);
                return PageMessage($"Report Added to {dest}", "Report Added", "success");
            }
            catch (Exception ex)
            {
                return PageMessage(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Design")]
        [Route("EditCustomReport/{queryId?}")]
        [Route("EditCustomReport/{report}/{queryId?}")]
        public ActionResult EditCustomReport(int? orgId, string report, Guid queryId)
        {
            var modelstate = TempDataModelState;
            if (modelstate != null)
            {
                ModelState.Merge(modelstate);
            }

            var m = new CustomReportsModel(CurrentDatabase, report, queryId, orgId);
            var vm = m.EditCustomReport(TempDataCustomReport, TempDataSaved);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Design")]
        public ActionResult EditCustomReport(CustomReportViewModel vm)
        {
            if (!vm.Columns.Any(c => c.IsSelected))
            {
                ModelState.AddModelError("Columns", "At least one column must be selected.");
            }

            if (ModelState.IsValid)
            {
                vm.ReportName = SecurityElement.Escape(vm.ReportName.Trim());
                try
                {
                    var m = new CustomReportsModel(CurrentDatabase, vm.OrgId);
                    m.SaveReport(vm.OriginalReportName, vm.ReportName,
                        vm.Columns.Where(c => c.IsSelected), vm.RestrictToThisOrg);
                    TempDataSaved = true;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ReportName", "A report by this name already exists.");
                }
            }
            if (ModelState.IsValid)
            {
                return Redirect(CustomReportsModel.GetEditUrl(vm.ReportName, vm.QueryId, vm.OrgId));
            }

            TempDataModelState = ModelState;
            TempDataCustomReport = vm;
            return Redirect(CustomReportsModel.GetEditUrl(vm.OriginalReportName, vm.QueryId, vm.OrgId));
        }

        [Authorize(Roles = "Admin, Design")]
        [HttpPost, Route("DeleteCustomReport/{report}")]
        public JsonResult DeleteCustomReport(string report)
        {
            if (string.IsNullOrEmpty(report))
            {
                return new JsonResult { Data = "Report name is required." };
            }

            var m = new CustomReportsModel(CurrentDatabase);
            m.DeleteReport(report);

            return new JsonResult { Data = "success" };
        }

        private CustomReportViewModel TempDataCustomReport
        {
            get { return TempData["tdreport"] as CustomReportViewModel; }
            set { TempData["tdreport"] = value; }
        }

        private ModelStateDictionary TempDataModelState
        {
            get { return TempData["tdstate"] as ModelStateDictionary; }
            set { TempData["tdstate"] = value; }
        }

        private bool? TempDataSaved
        {
            get { return TempData["tdsaved"] as bool?; }
            set { TempData["tdSaved"] = value; }
        }
    }
}
