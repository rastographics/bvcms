using System;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models.ExtraValues;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [HttpGet, Route("ExtraValue/Summary/{table}")]
        public ActionResult Summary(string table)
        {
            if (Util2.OrgLeadersOnly)
                return Redirect("/Home");

            var m = ExtraInfo.CodeSummary(table);
            var c = CurrentDatabase.Content("StandardExtraValues2", "<Views />", ContentTypeCode.TypeText);
            ViewBag.EvSpecId = c.Id;
            return View("Reports/Summary", m);
        }

        [HttpPost, Route("ExtraValue/RenameAll/{table}")]
        public ActionResult RenameAll(string table, string field, string newname )
        {
            ExtraInfo.RenameAll(table, field, newname);
            return Content(newname);
        }

        [HttpPost, Route("ExtraValue/DeleteAll/{table}/{type}")]
        public ActionResult DeleteAll(string table, string type, string field, string value)
        {
            var ret = ExtraInfo.DeleteAll(table, type, field, value);
            return Content(ret);
        }



        [HttpGet, Route("ExtraValue/Grid/{id}")]
        public ActionResult Grid(Guid id, string sort)
        {
            var rdr = ReportsModel.GridReader(id, sort);
            ViewBag.queryid = id;
            return View("Reports/Grid", rdr);
        }

        [HttpGet, Route("ExtraValue/QueryCodes")]
        public ActionResult QueryCodes(string field, string value)
        {
            var c = ReportsModel.QueryCodesCondition(field, value);
            return Redirect("/Query/" + c.Id);
        }

        [HttpGet, Route("ExtraValue/QueryData")]
        public ActionResult QueryData(string field, string type)
        {
            var cc = ReportsModel.QueryDataCondition(field, type);
            return Redirect("/Query/" + cc.Id);
        }
        [HttpGet, Route("ExtraValue/FamilyQueryCodes")]
        public ActionResult FamilyQueryCodes(string field, string value)
        {
            var c = ReportsModel.FamilyQueryCodesCondition(field, value);
            return Redirect("/Query/" + c.Id);
        }

        [HttpGet, Route("ExtraValue/FamilyQueryData")]
        public ActionResult FamilyQueryData(string field, string type)
        {
            var cc = ReportsModel.FamilyQueryDataCondition(field, type);
            return Redirect("/Query/" + cc.Id);
        }
    }
}
