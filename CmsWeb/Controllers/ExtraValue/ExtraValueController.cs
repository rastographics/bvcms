using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models.ExtraValues;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController 
    {
        [HttpPost, Route("ExtraValue/Display/{table}/{location}/{id}")]
        public ActionResult Display(string table, string location, int id)
        {
            var m = new ExtraValueModel(id, table, location);
            return View(location, m);
        }

        [HttpPost, Route("ExtraValue/Edit/{table}/{type}")]
        public ActionResult Edit(string table, string type, string pk, string name, string value)
        {
            var m = new ExtraValueModel(pk.ToInt(), table);
            m.EditExtra(type, HttpUtility.UrlDecode(name), value);

            return new EmptyResult();
        }

        [HttpGet, Route("ExtraValue/Codes/{table}")]
        public ActionResult Codes(string table, string name)
        {
            var m = new ExtraValueModel(table);
            return Content(m.CodesJson(HttpUtility.UrlDecode(name)));
        }

        [HttpGet, Route("ExtraValue/Bits/{table}/{id:int}")]
        public ActionResult Bits(string table, int id, string name)
        {
            var m = new ExtraValueModel(id, table);
            return Content(m.DropdownBitsJson(HttpUtility.UrlDecode(name)));
        }
   }
}
