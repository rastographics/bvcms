using System;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models.ExtraValues;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [HttpGet, Route("ExtraValue/Summary")]
        public ActionResult Summary()
        {
            if (!ViewExtensions2.UseNewLook())
                return Redirect("/Reports/ExtraValues");
            var m = ReportsModel.CodeSummary();
            var c = DbUtil.Db.Content("StandardExtraValues2", "<Views />", ContentTypeCode.TypeText);
            ViewBag.EvSpecId = c.Id;
            return View("Reports/Summary", m);
        }

        [HttpGet, Route("ExtraValue/Grid/{id}")]
        public ActionResult Grid(Guid id, string sort)
        {
            var rdr = ReportsModel.GridReader(id, sort);
            ViewBag.queryid = id;
            return View("Reports/Grid", rdr);
        }

        [HttpGet, Route("ExtraValue/Grid2/{id}")]
        public ActionResult Grid2(Guid id, string sort)
        {
            var rdr = ReportsModel.Grid2Reader(id, sort);
            ViewBag.queryid = id;
            return View("Reports/Grid2", rdr);
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

        [HttpPost, Route("ExtraValue/DeleteAll")]
        public ActionResult DeleteAll(string field, string type, string value)
        {
            var ret = ReportsModel.DeleteAll(field, type, value);
            return Content(ret);
        }

        [HttpPost, Route("ExtraValue/RenameAll")]
        public ActionResult RenameAll(string field, string newname )
        {
            ReportsModel.RenameAll(field, newname);
            return Content(newname);
        }
    }
}
