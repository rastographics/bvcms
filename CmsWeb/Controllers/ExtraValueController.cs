using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Models.ExtraValues;
using UtilityExtensions;

namespace CmsWeb.Controllers
{

    public class ExtraValueController : CmsStaffController
    {
        [POST("ExtraValue/Delete/{id:int}")]
        public ActionResult Delete(int id, string name)
        {
            var e = DbUtil.Db.PeopleExtras.First(ee => ee.PeopleId == id && ee.Field == HttpUtility.UrlDecode(name));
            DbUtil.Db.PeopleExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            return View("AdHoc", id);
        }

        [GET("ExtraValue/Codes/{table}")]
        public ActionResult Codes(string table, string name)
        {
            var m = new ExtraValueModel(0, table);
            var j = m.CodesJson(HttpUtility.UrlDecode(name));
            return Content(j);
        }

        [POST("ExtraValue/AddDialog/{table}/{location}/{id:int}")]
        public ActionResult AddDialog(string location, string table, int id)
        {
            var m = new ExtraValueModel(id, location, table);
            return View("DialogList", m);
        }
        [POST("ExtraValue/Index/{table}/{location}/{id}")]
        public ActionResult Index(string table, string location, int id)
        {
            var m = new ExtraValueModel(id, location, table);
            return View(location, m);
        }

        [POST("ExtraValue/Edit/{table}/{type}")]
        public ActionResult Edit(string table, string type, string pk, string name, string value)
        {
            var m = new ExtraValueModel(pk.ToInt(), table);
            m.EditExtra(type, HttpUtility.UrlDecode(name), value);
            return new EmptyResult();
        }

        [GET("ExtraValue/Bits/{table}/{id:int}")]
        public ActionResult Bits(string table, int id, string name)
        {
            var m = new ExtraValueModel(id, table);
            var j = m.DropdownBitsJson(HttpUtility.UrlDecode(name));
            return Content(j);
        }

        [POST("ExtraValue/New/{id:int}")]
        public ActionResult New(int id, string field, string type, string value)
        {
            ExtraValueModel.NewExtra(id, field, type, value);
            return new EmptyResult();
        }
    }
}
