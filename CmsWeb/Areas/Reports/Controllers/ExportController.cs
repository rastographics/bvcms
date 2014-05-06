using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using Dapper;
using OfficeOpenXml;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaPrefix = "Export2"), Route("{action}/{id?}")]
    public class ExportController : CmsStaffController
    {
        [HttpGet]
        public ActionResult StatusFlags(Guid id, string flags = "")
        { // ?flags=F35,F03,F01,F04
            return new StatusFlagsExcelResult(id, flags);
        }

        [HttpGet]
        public ActionResult ExtraValues(Guid id)
        {
            return new ExtraValueExcelResult(id);
        }
        [HttpGet]
        public ActionResult WorshipAttendance(Guid id)
        {
            return WorshipAttendanceModel.Attendance(id);
        }

        [Authorize(Roles = "Finance")]
        [HttpPost]
        public ActionResult Contributions(string id, ContributionsExcelResult m)
        {
            m.type = id;
            return m;
        }
        [Authorize(Roles = "Finance")]
        [HttpPost]
        public ActionResult GLExport(GLExportResult m)
        {
            return m;
        }

        [HttpGet, Route("Excel/Groups")]
        public ActionResult ExcelGroups()
        {
            return new ExcelResult(ExportInvolvements.OrgMemberListGroups());
        }


        [HttpPost]
        public ActionResult MeetingsForDateRange(DateTime dt1, DateTime dt2, OrgSearchModel m)
        {
            var orgs = string.Join(",", m.FetchOrgs().Select(oo => oo.OrganizationId));
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("MeetingsForDateRange", new
            {
                orgs,
                startdate = dt1,
                enddate = dt2,
            }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            var entity = (IDictionary<string, object>) q.First();
            var cols = entity.Keys.Cast<string>().ToList();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            int row = 1;
            for (var i = 0; i < cols.Count; i++)
                ws.Cells[row, i+1].Value = cols[i];
            row++;
            foreach (var r in q)
            {
                var rr = (IDictionary<string, object>) r;
                for (var i = 0; i < cols.Count; i++)
                    ws.Cells[row, i+1].Value = rr[cols[i]];
                row++;
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "MeetingsForDateRange.xlsx");
        }

        [HttpPost]
        public ActionResult MissionTripFunding(OrgSearchModel m)
        {
            return MissionTripFundingModel.Result(m);
        }

        [HttpGet]
        [Route("Excel/{id:guid}")]
        [Route("Excel/{format}/{id:guid}")]
        public ActionResult Excel(Guid id, string format, bool? titles, bool? useMailFlags)
        {
            var ctl = new MailingController {UseTitles = titles ?? false, UseMailFlags = useMailFlags ?? false};
            switch (format)
            {
                case "Individual":
                case "GroupAddress":
                    return new ExcelResult(ExportPeople.FetchExcelList(id, maxExcelRows, useMailFlags ?? false));
                case "Library":
                    return new ExcelResult(ExportPeople.FetchExcelLibraryList(id));
                case "AllFamily":
                    return new ExcelResult(ExportPeople.FetchExcelListFamily(id));
                case "Family":
                    return new ExcelResult(ctl.FetchExcelFamily(id, maxExcelRows));
                case "ParentsOf":
                    return new ExcelResult(ctl.FetchExcelParents(id, maxExcelRows));
                case "CouplesEither":
                    return new ExcelResult(ctl.FetchExcelCouplesEither(id, maxExcelRows));
                case "CouplesBoth":
                    return new ExcelResult(ctl.FetchExcelCouplesBoth(id, maxExcelRows));
                case "Involvement":
                    return new ExcelResult(ExportInvolvements.InvolvementList(id));
                case "Children":
                    return new ExcelResult(ExportInvolvements.ChildrenList(id, maxExcelRows));
                case "Church":
                    return new ExcelResult(ExportInvolvements.ChurchList(id, maxExcelRows));
                case "Attend":
                    return new ExcelResult(ExportInvolvements.AttendList(id, maxExcelRows));
                case "Organization":
                    return new ExcelResult(ExportInvolvements.OrgMemberList(id));
                case "Promotion":
                    return new ExcelResult(ExportInvolvements.PromoList(id, maxExcelRows));
                case "IndividualPicture":
                    return ExcelExportModel.Result(id);
//                case "IndividualPicture2":
//                    Response.ContentType = "application/vnd.ms-excel";
//                    Response.AddHeader("content-disposition", "attachment;filename=pictures.xls");
//                    return View("Picture", ExportPeople.FetchExcelListPics(id, maxExcelRows));
                case "FamilyMembers":
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename=familymembers.xls");
                    return View("FamilyMembers", ExportPeople.FetchExcelListFamilyMembers(id));
            }
            return Content("no format");
        }

        [HttpGet]
        public ActionResult Csv(Guid id, string format, bool? sortzip, bool? titles, bool? useMailFlags)
        {
            var ctl = new MailingController {UseTitles = titles ?? false, UseMailFlags = useMailFlags ?? false};

            var sort = "Name";
            if (sortzip ?? false)
                sort = "Zip";

            switch (format)
            {
                case "Individual":
                case "GroupAddress":
                    return new CsvResult(ctl.FetchIndividualList(sort, id));
                case "FamilyMembers":
                    return new CsvResult(ctl.FetchFamilyMembers(sort, id));
                case "Family":
                    return new CsvResult(ctl.FetchFamilyList(sort, id));
                case "ParentsOf":
                    return new CsvResult(ctl.FetchParentsOfList(sort, id));
                case "CouplesEither":
                    return new CsvResult(ctl.FetchCouplesEitherList(sort, id));
                case "CouplesBoth":
                    return new CsvResult(ctl.FetchCouplesBothList(sort, id));
            }
            return Content("no format");

        }

        private static int maxExcelRows
        {
            get { return DbUtil.Db.Setting("MaxExcelRows", "10000").ToInt(); }
        }

    }
}
