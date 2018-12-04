using CmsWeb.Areas.Reports.Models;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using MoreLinq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaPrefix = "Export2"), Route("{action}/{id?}")]
    public class ExportController : CmsStaffController
    {
        public ExportController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult StatusFlags(Guid id, string flags = "")
        { // ?flags=F35,F03,F01,F04
            return StatusFlagsExportModel.StatusFlagsList(id, flags);
        }

        [HttpGet]
        public ActionResult ExtraValues(Guid id)
        {
            return ExtraValueExcelResult.ExtraValueExcel(id);
        }
        [HttpGet]
        public ActionResult WorshipAttendance(Guid id)
        {
            return WorshipAttendanceModel.Attendance(id);
        }
        [HttpGet]
        public ActionResult MembershipInfo(Guid id)
        {
            return MembershipExportModel.MembershipInfoList(id);
        }

        [Authorize(Roles = "Finance,FinanceViewOnly")]
        [HttpPost]
        public ActionResult Contributions(string id, ContributionsExcelResult m)
        {
            return m.ToExcel(id);
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
            var entity = (IDictionary<string, object>)q.First();
            var cols = entity.Keys.Cast<string>().ToList();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            int row = 1;
            for (var i = 0; i < cols.Count; i++)
            {
                ws.Cells[row, i + 1].Value = cols[i];
            }

            row++;
            foreach (var r in q)
            {
                var rr = (IDictionary<string, object>)r;
                for (var i = 0; i < cols.Count; i++)
                {
                    ws.Cells[row, i + 1].Value = rr[cols[i]];
                }

                row++;
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "MeetingsForDateRange.xlsx");
        }
        [HttpPost]
        public ActionResult MeetingsAttendance(DateTime? dt1, DateTime? dt2, OrgSearchModel m)
        {
            var dt = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!dt1.HasValue)
            {
                dt1 = new DateTime(dt.Year, 1, 1);
            }

            if (!dt2.HasValue)
            {
                dt2 = dt;
            }

            var m2 = new AttendanceDetailModel(dt1.Value, dt2, m);
            return m2.FetchMeetings().ToDataTable().ToExcel("MeetingsExport.xlsx");
        }
        [HttpPost]
        public ActionResult OrgDayStats(DateTime? dt, OrgSearchModel m)
        {
            if (!dt.HasValue)
            {
                dt = ChurchAttendanceModel.MostRecentAttendedSunday();
            }

            var orgs = string.Join(",", m.FetchOrgs().Select(oo => oo.OrganizationId));
            var q = CurrentDatabase.OrgDayStats(orgs, dt);
            return q.ToDataTable().ToExcel("OrgDatStats.xlsx");
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
            var ctl = new MailingController { UseTitles = titles ?? false, UseMailFlags = useMailFlags ?? false };
            switch (format)
            {
                case "Individual":
                case "GroupAddress":
                    return ExportPeople.FetchExcelList(id, maxExcelRows, useMailFlags ?? false).ToExcel();
                case "Library":
                    return ExportPeople.FetchExcelLibraryList(id);
                case "AllFamily":
                    return ExportPeople.FetchExcelListFamily(id);
                case "Family":
                    return ctl.FetchExcelFamily(id, maxExcelRows);
                case "ParentsOf":
                    return ctl.FetchExcelParents(id, maxExcelRows);
                case "CouplesEither":
                    return ctl.FetchExcelCouplesEither(id, maxExcelRows);
                case "CouplesBoth":
                    return ctl.FetchExcelCouplesBoth(id, maxExcelRows);
                case "Involvement":
                    return ExportInvolvements.InvolvementList(id);
                case "Involvement2":
                    return ExportInvolvements.InvolvementList(id);
                case "Children":
                    return ExportInvolvements.ChildrenList(id, maxExcelRows);
                case "Church":
                    return ExportInvolvements.ChurchList(id, maxExcelRows);
                case "Attend":
                    return ExportInvolvements.AttendList(id, maxExcelRows);
                case "Promotion":
                    return ExportInvolvements.PromoList(id, maxExcelRows);
                case "IndividualPicture":
                    return ExcelExportModel.Result(id);
                case "FamilyMembers":
                    return ExportPeople.FetchExcelListFamilyMembers(id);
                case "OrgMembers":
                    return OrgsMembersExcelModel.Export(id);
                case "Groups":
                    return ExportInvolvements.OrgMemberListGroups(id);
            }
            return Content("no format");
        }

        [HttpGet]
        public ActionResult Csv(Guid id, string format, bool? sortzip, bool? titles, bool? useMailFlags)
        {
            var ctl = new MailingController { UseTitles = titles ?? false, UseMailFlags = useMailFlags ?? false };

            var sort = "Name";
            if (sortzip ?? false)
            {
                sort = "Zip";
            }

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
                    return new CsvResult(ctl.FetchCouplesEitherList(sort, id), couples: true);
                case "CouplesBoth":
                    return new CsvResult(ctl.FetchCouplesBothList(sort, id), couples: true);
            }
            return Content("no format");

        }

        private int maxExcelRows
        {
            get { return CurrentDatabase.Setting("MaxExcelRows", "10000").ToInt(); }
        }

        [HttpGet]
        public ActionResult DocXMerge(Guid id, string filename, string template)
        {
            return new DocXReplacements(id, filename, template);
        }
        [HttpGet]
        public ActionResult DocXMerge1(int id, string filename, string template)
        {
            return new DocXReplacements(id, filename, template);
        }
    }
}
