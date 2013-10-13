using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Main.Models.Avery;
using CmsWeb.Areas.Main.Models.Directories;
using CmsWeb.Areas.Main.Models.Report;
using CmsData;
using System.IO;
using CmsWeb.Code;
using CmsWeb.Models;
using CmsWeb.Models.ExtraValues;
using Dapper;
using NPOI.HSSF.UserModel;
using UtilityExtensions;
using System.Text;
using System.Data.SqlClient;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaUrl = "Reports2")]
    public class ReportsController : CmsStaffController
    {
        [GET("Reports2/WeeklyAttendance/{id}")]
        public ActionResult WeeklyAttendance(Guid id)
        {
            return new WeeklyAttendanceResult(id);
        }
        [GET("Reports2/Family/{id}")]
        public ActionResult Family(Guid id)
        {
            return new Main.Models.Report.FamilyResult(id);
        }
        [GET("Reports2/BarCodeLabels/{id}")]
        public ActionResult BarCodeLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new BarCodeLabelsResult(id.Value);
        }
        [GET("Reports2/Contacts/{id:guid}")]
        public ActionResult Contacts(Guid? id, bool? sortAddress, string orgname)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ContactsResult(id.Value, sortAddress, orgname);
        }

        [GET("Reports2/Rollsheet/{id}")]
        public ActionResult Rollsheet(Guid id, string org, string dt, int? meetingid, int? bygroup, string sgprefix, bool? altnames, string highlight, OrgSearchModel m)
        {
            var dt2 = dt.ToDate();
            return new RollsheetResult
            {
                qid = id,
                orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new int[] { 0 },
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                highlightsg = highlight,
                Model = m
            };
        }

        [GET("Reports2/RallyRollsheet/{id}")]
        public ActionResult RallyRollsheet(Guid id, string org, string dt, int? meetingid, int? bygroup, string sgprefix, bool? altnames, string highlight, OrgSearchModel m)
        {
            var dt2 = dt.ToDate();

            return new RallyRollsheetResult
            {
                qid = id,
                orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new int[] { 0 },
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                Model = m
            };
        }
        [GET("Reports2/Roster1/{id}")]
        public ActionResult Roster1(Guid id, int? oid)
        {
            return new RosterResult
            {
                qid = id,
                org = oid,
            };
        }
        [GET("Reports2/Avery/{id}")]
        public ActionResult Avery(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult { id = id };
        }
        [GET("Reports2/NameLabels/{id}")]
        public ActionResult NameLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult { namesonly = true, id = id };
        }
        [GET("Reports2/Avery3/{id}")]
        public ActionResult Avery3(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new Avery3Result { id = id };
        }
        [GET("Reports2/AveryAddress/{id}")]
        public ActionResult AveryAddress(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, int skipNum = 0)
        {
            if (!id.HasValue)
                return Content("no query");
            if (!format.HasValue())
                return Content("no format");
            return new AveryAddressResult
            {
                id = id,
                format = format,
                titles = titles,
                usephone = usephone ?? false,
                skip = skipNum,
                sortzip = sortzip
            };
        }
        [GET("Reports2/RollLabels/{id}")]
        public ActionResult RollLabels(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RollLabelsResult
            {
                qid = id,
                format = format,
                titles = titles ?? false,
                usephone = usephone ?? false,
                sortzip = sortzip
            };
        }
        [GET("Reports2/Prospect/{id}")]
        public ActionResult Prospect(Guid? id, bool? Form, bool? Alpha)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ProspectResult(id, Form ?? false, Alpha ?? false);
        }
        [GET("Reports2/VisitsAbsents2/{id}")]
        public ActionResult VisitsAbsents2(int? id)
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult2(meetingid: id);
        }
        [GET("Reports2/Registration/{id}")]
        public ActionResult Registration(Guid? id, int? oid)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RegistrationResult(id, oid);
        }
        [GET("Reports2/RecentAbsents/{id}")]
        public ActionResult RecentAbsents1(Guid id)
        {
            int? divid = null;
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP", new { orgid = id, divid = divid, days = 36 },
                            commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View("RecentAbsents", q);
        }

        public class ExtraInfo
        {
            public string Field { get; set; }
            public string Value { get; set; }
            public string type { get; set; }
            public int Count { get; set; }
        }
        [GET("Reports2/ExtraValues")]
        public ActionResult ExtraValues()
        {
            var fields = from field in ExtraValueModel.GetStandardExtraFields()
                         where field.table == "People"
                         select field;
            var q = from e in DbUtil.Db.PeopleExtras
                    where e.StrValue != null || e.BitValue != null
                    let TypeValue = e.StrValue != null ? "Code" : "Bit"
                    group e by new { e.Field, val = e.StrValue ?? (e.BitValue == true ? "1" : "0"), TypeValue } into g
                    select new ExtraInfo
                    {
                        Field = g.Key.Field,
                        Value = g.Key.val,
                        type = g.Key.TypeValue,
                        Count = g.Count(),
                    };

            var list = from e in q.ToList()
                       let f = fields.SingleOrDefault(ff => ff.name == e.Field)
                       where f == null || f.UserCanView()
                       orderby e.Field
                       select e;
            return View(list);
        }
        [GET("Reports2/ExtraValueData")]
        public ActionResult ExtraValueData()
        {
            var fields = from field in ExtraValueModel.GetStandardExtraFields()
                         where field.table == "People"
                         select field;
            var q = from e in DbUtil.Db.PeopleExtras
                    where e.StrValue == null && e.BitValue == null
                    let TypeValue = e.DateValue != null ? "Date" : e.Data != null ? "Text" : e.IntValue != null ? "Int" : "?"
                    group e by new { e.Field, TypeValue } into g
                    select new ExtraInfo
                    {
                        Field = g.Key.Field,
                        type = g.Key.TypeValue,
                        Count = g.Count(),
                    };

            var list = from e in q.ToList()
                       let f = fields.SingleOrDefault(ff => ff.name == e.Field)
                       where f == null || f.UserCanView()
                       orderby e.Field
                       select e;
            return View(list);
        }
        [GET("Reports2/ExtraValuesGrid/{id}")]
        [GET("Reports2/ExtraValuesGrid/{id}/{sort}")]
        public ActionResult ExtraValuesGrid(Guid id, string sort)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var fields = from field in ExtraValueModel.GetStandardExtraFields()
                         where field.table == "People"
                         where field.VisibilityRoles != null && (field.VisibilityRoles.Split(',').All(rr => !roles.Contains(rr)))
                         select field.name;
            var nodisplaycols = string.Join("|", fields);

            var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_ExtraValues);
            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", sort ?? "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            var rdr = cmd.ExecuteReader();
            ViewBag.queryid = id;
            return View(rdr);
        }
        [GET("Reports2/ExtraValuesGrid2/{id}")]
        [GET("Reports2/ExtraValuesGrid2/{id}/{sort}")]
        public ActionResult ExtraValuesGrid2(Guid id, string sort)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var fields = from field in ExtraValueModel.GetStandardExtraFields()
                         where field.table == "People"
                         where field.VisibilityRoles != null && (field.VisibilityRoles.Split(',').All(rr => !roles.Contains(rr)))
                         select field.name;
            var nodisplaycols = string.Join("|", fields);

            var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_ExtraValues);
            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", sort ?? "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            var rdr = cmd.ExecuteReader();
            ViewBag.queryid = id;
            return View(rdr);
        }
        [GET("Reports2/FamilyDirectory/{id}")]
        public ActionResult FamilyDirectory(Guid id)
        {
            return new FamilyDir(id);
        }
        [GET("Reports2/FamilyDirectoryCompact/{id}")]
        public ActionResult FamilyDirectoryCompact(Guid id)
        {
            return new CompactDir(id);
        }
        [GET("Reports2/PictureDirectory/{id}")]
        public ActionResult PictureDirectory(Guid id)
        {
            return new PictureDir(id);
        }
        [GET("Reports2/EmployerAddress/{id}")]
        public ActionResult EmployerAddress(Guid id)
        {
            return new EmployerAddress(id, true);
        }
    }
}
