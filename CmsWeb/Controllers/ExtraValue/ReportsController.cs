using System;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsWeb.Models.ExtraValues;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [GET("ExtraValue/Summary")]
        public ActionResult Summary()
        {
            var m = ReportsModel.CodeSummary();
            return View("Reports/Summary", m);
        }

        [GET("ExtraValue/Grid/{id}")]
        public ActionResult Grid(Guid id, string sort)
        {
            var rdr = ReportsModel.GridReader(id, sort);
            ViewBag.queryid = id;
            return View("Reports/Grid", rdr);
        }

        [GET("ExtraValue/Grid2/{id}")]
        public ActionResult Grid2(Guid id, string sort)
        {
            var rdr = ReportsModel.Grid2Reader(id, sort);
            ViewBag.queryid = id;
            return View("Reports/Grid2", rdr);
        }

        [GET("ExtraValue/QueryCodes")]
        public ActionResult QueryCodes(string field, string value)
        {
            var c = ReportsModel.QueryCodesCondition(field, value);
            return Redirect("/Query/" + c.Id);
        }

        [GET("ExtraValue/QueryData")]
        public ActionResult QueryData(string field, string type)
        {
            var cc = ReportsModel.QueryDataCondition(field, type);
            return Redirect("/Query/" + cc.Id);
        }

        [POST("ExtraValue/DeleteAll")]
        public ActionResult DeleteAll(string field, string type, string value)
        {
            var ret = ReportsModel.DeleteAll(field, type, value);
            return Content(ret);
        }

        [POST("ExtraValue/RenameAll")]
        public ActionResult RenameAll(string field, string newname )
        {
            ReportsModel.RenameAll(field, newname);
            return Content(newname);
        }
    }
}
