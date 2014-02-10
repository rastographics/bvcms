using System;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaUrl = "Export2")]
    public class ExportController : CmsStaffController
    {
        [GET("Export2/StatusFlags/{id:guid}")]
        public ActionResult StatusFlags(Guid id)
        {
            return new StatusFlagsExcelResult(id);
        }

        [GET("Export2/ExtraValues/{id:guid}")]
        public ActionResult ExtraValues(Guid id)
        {
            return new ExtraValueExcelResult(id);
        }
        [GET("Export2/WorshipAttendance/{id:guid}")]
        public ActionResult WorshipAttendance(Guid id)
        {
            return WorshipAttendanceModel.Attendance(id);
        }

        [Authorize(Roles = "Finance")]
        [POST("Export2/Contributions/{id}")]
        public ActionResult Contributions(string id, ContributionsExcelResult m)
        {
            m.type = id;
            return m;
        }
        [Authorize(Roles = "Finance")]
        [POST("Export2/GLExport")]
        public ActionResult GLExport(GLExportResult m)
        {
            return m;
        }

        [GET("Export2/Excel/Groups")]
        public ActionResult ExcelGroups()
        {
            return new ExcelResult(ExportInvolvements.OrgMemberListGroups());
        }

        [GET("Export2/Excel/{id:guid}")]
        [GET("Export2/Excel/{format}/{id:guid}")]
        public ActionResult Excel(Guid id, string format, bool? titles)
        {
            var ctl = new MailingController {UseTitles = titles ?? false};
            switch (format)
            {
                case "Individual":
                case "GroupAddress":
                    return new ExcelResult(ExportPeople.FetchExcelList(id, maxExcelRows));
                case "Library":
                    return new ExcelResult(ExportPeople.FetchExcelLibraryList(id));
                case "Family":
                    return new ExcelResult(ctl.FetchExcelFamily(id, maxExcelRows));
                case "AllFamily":
                    return new ExcelResult(ExportPeople.FetchExcelListFamily(id));
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
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename=pictures.xls");
                    return View("Picture", ExportPeople.FetchExcelListPics(id, maxExcelRows));
                case "FamilyMembers":
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename=familymembers.xls");
                    return View("FamilyMembers", ExportPeople.FetchExcelListFamilyMembers(id));
            }
            return Content("no format");
        }

        [GET("Export2/Csv/{id:guid}")]
        public ActionResult Csv(Guid id, string format, bool? sortzip, bool? titles)
        {
            var ctl = new MailingController {UseTitles = titles ?? false};

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
