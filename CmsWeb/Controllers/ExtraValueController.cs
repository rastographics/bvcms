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

        [GET("ExtraValue/Codes")]
        public ActionResult Codes(string name)
        {
            var j = ExtraValueModel.CodesJson(HttpUtility.UrlDecode(name));
            return Content(j);
        }

        [POST("ExtraValue/AddDialog/{location}/{id}")]
        public ActionResult AddDialog(int id, string location)
        {
            return View("DialogList", location);
        }
        [POST("ExtraValue/{table}/{location}/{id}")]
        public ActionResult Index(string table, string location, int id)
        {
            var m = new ExtraValueModel(id, location, table);
            return View(location, m);
        }

        [POST("ExtraValue/Family/{id}")]
        public ActionResult Family(int id)
        {
            return View("Family", id);
        }

        [POST("ExtraValue/Adhoc/{id}")]
        public ActionResult Adhoc(int id)
        {
            return View("Adhoc", id);
        }

        [POST("ExtraValue/Edit/{table}")]
        public ActionResult Edit(string table, string pk, string name, string value)
        {
            var a = pk.SplitStr("-", 2);
            var type = a[0];
            var m = new ExtraValueModel(a[1].ToInt(), table);
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
