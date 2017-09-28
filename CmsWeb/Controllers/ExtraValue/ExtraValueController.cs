using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models.ExtraValues;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public partial class ExtraValueController
    {
        [HttpPost, Route("ExtraValue/Display/{table}/{location}/{id}/{id2?}")]
        public ActionResult Display(string table, string location, int id, int? id2)
        {
            var m = table == "OrgMember"
                ? new ExtraValueModel(id, id2 ?? 0, table, location)
                : new ExtraValueModel(id, table, location);
            return View(location, m);
        }

        [HttpPost, Route("ExtraValue/Edit/{table}/{location}/{type}")]
        public ActionResult Edit(string table, string location, string type, string pk, string name, string[] value)
        {
            ExtraValueModel m;
            if(table == "OrgMember")
            {
                var a = pk.Split('.');
                m = new ExtraValueModel(a[0].ToInt(), a[1].ToInt(), table, "Adhoc");
            }
            else
                m = new ExtraValueModel(pk.ToInt(), table, location);
            m.EditExtra(type, HttpUtility.UrlDecode(name), value);
            return new EmptyResult();
        }

        [HttpGet, Route("ExtraValue/Codes/{table}/{location}")]
        public ActionResult Codes(string table, string name, string location)
        {
            var m = new ExtraValueModel(table, location);
            return Content(m.CodesJson(HttpUtility.UrlDecode(name)));
        }

        [HttpGet, Route("ExtraValue/Bits/{table}/{location}/{id:int}")]
        public ActionResult Bits(string table, int id, string name, string location)
        {
            var m = new ExtraValueModel(id, table, location);
            return Content(m.DropdownBitsJson(HttpUtility.UrlDecode(name)));
        }
   }
}
