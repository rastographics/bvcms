using System;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Models;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaUrl = "Export")]
    public class ExportController : CmsStaffController
    {
        [Authorize(Roles = "Finance")]
        [POST("Export/Contributions/{id}")]
        public ActionResult Contributions(string id, ContributionsExcelResult m)
        {
            m.type = id;
            return m;
        }
        [Authorize(Roles = "Finance")]
        [POST("Export/GLExport")]
        public ActionResult GLExport(GLExportResult m)
        {
            return m;
        }

        [GET("Export/Excel/Groups")]
        public ActionResult ExcelGroups(bool? titles)
        {
            return new ExcelResult(ExportInvolvements.OrgMemberListGroups());
        }
    }
}
