using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Areas.Reports.ViewModels;
using Dapper;
using UtilityExtensions;
using UtilityExtensions.Extensions;

namespace CmsWeb.Areas.Reports.Controllers
{
    public partial class ReportsController 
    {
        [HttpGet]
        [Route("Custom/{report}/{id?}")]
        public ActionResult CustomReport(Guid id, string report)
        {
            var m = new CustomReportsModel(DbUtil.Db, report, id);
            return View(m);
        }
        [HttpGet]
        [Route("CustomExcel/{report}/{id?}")]
        public ActionResult CustomReportExcel(Guid id, string report)
        {
            var m = new CustomReportsModel(DbUtil.Db, report, id);
            return m.Result();
        }

        [HttpGet]
        [Route("CustomSql/{report}")]
        public ActionResult CustomReportSql(string report)
        {
            try
            {
                var m = new CustomReportsModel(DbUtil.Db, report);
                return Content($"<pre style='font-family:monospace'>{m.Sql()}\n</pre>");
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
        }
        [HttpGet]
        [Route("SqlReport/{report}/{id?}")]
        public ActionResult SqlReport(Guid id, string report)
        {
            var content = DbUtil.Db.ContentOfTypeSql(report);
            if (content == null)
                return Message("no content");
            if (!CanRunScript(content.Body))
                return Message("Not Authorized to run this script");

            var hasqtag = content.Body.Contains("@qtagid");
            var hascurrentorg = content.Body.Contains("@CurrentOrgId");
            if (!hasqtag && !hascurrentorg)
                return Message("missing @qtagid or @CurrentOrgId");
            ViewBag.Name = report.SpaceCamelCase();

            var p = new DynamicParameters();
            if (hasqtag)
            {
                var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                p.Add("@qtagid", tag.Id);
            }
            if (hascurrentorg)
            {
                var oid = DbUtil.Db.CurrentOrgId0;
                p.Add("@CurrentOrgId", oid);
                if(oid > 0)
                    ViewBag.Name2 = DbUtil.Db.LoadOrganizationById(oid).FullName2;
            }

            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                ViewBag.name = report;
                using (var rd = cn.ExecuteReader(content.Body, p))
                    ViewBag.report = GridResult.Table(rd, ViewBag.Name2);
                return View();
            }
        }
        [HttpGet]
        [Route("PyScript/{report}/{id?}")]
        public ActionResult PyScript(Guid id, string report)
        {
            var content = DbUtil.Db.ContentOfTypePythonScript(report);
            if (content == null)
                return Content("no script named " + report);
            if (!CanRunScript(content))
                return Message("Not Authorized to run this script");
            if (!content.Contains("BlueToolbarReport") && !content.Contains("@BlueToolbarTagId"))
                return Content("Missing Call to Query Function 'BlueToolbarReport'");
            if (id == Guid.Empty)
                return Content("Must be run from the BlueToolbar");

            var pe = new PythonModel(Util.Host);

            pe.DictionaryAdd("BlueToolbarGuid", id.ToCode());
            foreach (var key in Request.QueryString.AllKeys)
                pe.DictionaryAdd(key, Request.QueryString[key]);

            pe.RunScript(content);

            return View(pe);

        }

        [Authorize(Roles = "Admin, Design")]
        public ActionResult AddReport(string report, string url, string type)
        {
            var m = new CustomReportsModel(DbUtil.Db);
            try
            {

            }
            catch (Exception ex)
            {
                return Message(ex);
            }
            var dest = m.AddReport(report, url, type);
            return Message($"Report Added to {dest}");
        }
        private bool CanRunScript(string script)
        {
            if (!script.StartsWith("#Roles=") && !script.StartsWith("--Roles"))
                return true;
            var re = new Regex("(--|#)Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim());
            if (!roles.Any(rr => User.IsInRole(rr)))
                return false;
            return true;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Design")]
        [Route("EditCustomReport/{queryId?}")]
        [Route("EditCustomReport/{report}/{queryId?}")]
        public ActionResult EditCustomReport(int? orgId, string report, Guid queryId)
        {
            CustomReportViewModel originalReportViewModel = null;
            if (TempData[TempDataModelStateKey] != null)
            {
                ModelState.Merge((ModelStateDictionary)TempData[TempDataModelStateKey]);
                originalReportViewModel = TempData[TempDataCustomReportKey] as CustomReportViewModel;
            }

            var m = new CustomReportsModel(DbUtil.Db, orgId);

            var orgName = orgId.HasValue
                ? DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == orgId.Value)?.OrganizationName
                : null;

            CustomReportViewModel model;
            if (string.IsNullOrEmpty(report))
            {
                model = new CustomReportViewModel(orgId, queryId, orgName, GetAllStandardColumns(m));
                return View(model);
            }

            model = new CustomReportViewModel(orgId, queryId, orgName, GetAllStandardColumns(m), report);

            var reportXml = m.GetReportByName(report);
            if (reportXml == null)
                throw new Exception("Report not found.");

            var columns = MapXmlToCustomReportColumn(reportXml);

            var showOnOrgIdValue = reportXml.AttributeOrNull("showOnOrgId");
            int showOnOrgId;
            if (!string.IsNullOrEmpty(showOnOrgIdValue) && int.TryParse(showOnOrgIdValue, out showOnOrgId))
                model.RestrictToThisOrg = showOnOrgId == orgId;

            model.SetSelectedColumns(columns);
            model.Columns = model.Columns.OrderBy(cc => cc.Order).ToList();

            if (originalReportViewModel != null)
                model.ReportName = originalReportViewModel.ReportName;

            var alreadySaved = TempData[TempDataSuccessfulSaved] as bool?;
            model.CustomReportSuccessfullySaved = alreadySaved.GetValueOrDefault();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Design")]
        public ActionResult EditCustomReport(CustomReportViewModel viewModel)
        {
            if (!viewModel.Columns.Any(c => c.IsSelected))
                ModelState.AddModelError("Columns", "At least one column must be selected.");

            if (ModelState.IsValid)
            {
                viewModel.ReportName = SecurityElement.Escape(viewModel.ReportName.Trim());

                var m = new CustomReportsModel(DbUtil.Db, viewModel.OrgId);
                var result = m.SaveReport(viewModel.OriginalReportName, viewModel.ReportName,
                    viewModel.Columns.Where(c => c.IsSelected), viewModel.RestrictToThisOrg);

                switch (result)
                {
                    case CustomReportsModel.SaveReportStatus.ReportAlreadyExists:
                        ModelState.AddModelError("ReportName", "A report by this name already exists.");
                        break;
                    default:
                        TempData[TempDataSuccessfulSaved] = true;
                        break;
                }
            }

            if (!ModelState.IsValid)
            {
                TempData[TempDataModelStateKey] = ModelState;
                TempData[TempDataCustomReportKey] = viewModel;
                return Redirect(CustomReportsModel.GetEditUrl(viewModel.OriginalReportName, viewModel.QueryId, viewModel.OrgId));
            }
            return Redirect(CustomReportsModel.GetEditUrl(viewModel.ReportName, viewModel.QueryId, viewModel.OrgId));
        }

        [Authorize(Roles = "Admin, Design")]
        [HttpPost, Route("DeleteCustomReport/{report}")]
        public JsonResult DeleteCustomReport(string report)
        {
            if (string.IsNullOrEmpty(report))
                return new JsonResult { Data = "Report name is required." };

            var m = new CustomReportsModel(DbUtil.Db);
            m.DeleteReport(report);

            return new JsonResult { Data = "success" };
        }

        private static List<CustomReportColumn> GetAllStandardColumns(CustomReportsModel model)
        {
            var reportXml = model.StandardColumns();
            return MapXmlToCustomReportColumn(reportXml);
        }

        private static List<CustomReportColumn> MapXmlToCustomReportColumn(XContainer reportXml)
        {
            var q = from column in reportXml.Descendants("Column")
                   select new CustomReportColumn
                   {
                       Name = column.AttributeOrNull("name"),
                       Description = column.AttributeOrNull("description"),
                       Flag = column.AttributeOrNull("flag"),
                       OrgId = column.AttributeOrNull("orgid"),
                       SmallGroup = column.AttributeOrNull("smallgroup"),
                       Field = column.AttributeOrNull("field"),
                       IsDisabled = column.AttributeOrNull("disabled").ToBool(),
                   };
            return q.ToList();
        }

        private const string TempDataModelStateKey = "ModelState";
        private const string TempDataCustomReportKey = "InvalidCustomReportViewModel";
        private const string TempDataSuccessfulSaved = "CustomReportSuccessfullySaved";
    }
}
