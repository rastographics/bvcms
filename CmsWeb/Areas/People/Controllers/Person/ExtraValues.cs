using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Person;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ContentResult DeleteExtra(int id, string field)
        {
            var e = DbUtil.Db.PeopleExtras.First(ee => ee.PeopleId == id && ee.Field == field);
            DbUtil.Db.PeopleExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            return Content("done");
        }

        [HttpPost]
        public ContentResult EditExtra(string id, string value)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var p = DbUtil.Db.LoadPersonById(b[1].ToInt());
            switch (a[0])
            {
                case "s":
                    p.AddEditExtraValue(b[0], value);
                    break;
                case "t":
                    p.AddEditExtraData(b[0], value);
                    break;
                case "d":
                    {
                        DateTime dt;
                        if (DateTime.TryParse(value, out dt))
                        {
                            p.AddEditExtraDate(b[0], dt);
                            value = dt.ToShortDateString();
                        }
                        else
                        {
                            p.RemoveExtraValue(DbUtil.Db, b[0]);
                            value = "";
                        }
                    }
                    break;
                case "i":
                    p.AddEditExtraInt(b[0], value.ToInt());
                    break;
                case "b":
                    if (value == "True")
                        p.AddEditExtraBool(b[0], true);
                    else
                        p.RemoveExtraValue(DbUtil.Db, b[0]);
                    break;
                case "m":
                    {
                        if (value == null)
                            value = Request.Form["value[]"];
                        var cc = Code.StandardExtraValues.ExtraValueBits(b[0], b[1].ToInt());
                        var aa = value.Split(',');
                        foreach (var c in cc)
                        {
                            if (aa.Contains(c.Key)) // checked now
                                if (!c.Value) // was not checked before
                                    p.AddEditExtraBool(c.Key, true);
                            if (!aa.Contains(c.Key)) // not checked now
                                if (c.Value) // was checked before
                                    p.RemoveExtraValue(DbUtil.Db, c.Key);
                        }
                        DbUtil.Db.SubmitChanges();
                        break;
                    }
            }
            DbUtil.Db.SubmitChanges();
            if (value == "null")
                return Content(null);
            return Content(value);
        }

        [HttpPost]
        public JsonResult ExtraValues(string id)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var c = Code.StandardExtraValues.Codes(b[0]);
            var j = Json(c);
            return j;
        }
        [HttpPost]
        public JsonResult ExtraValues2(string id)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var c = Code.StandardExtraValues.ExtraValueBits(b[0], b[1].ToInt());
            var j = Json(c);
            return j;
        }
        [HttpPost]
        public ActionResult NewExtraValue(int id, string field, string type, string value)
        {
            field = field.Replace('/', '-');
            var v = new PeopleExtra { PeopleId = id, Field = field };
            DbUtil.Db.PeopleExtras.InsertOnSubmit(v);
            switch (type)
            {
                case "string":
                    v.StrValue = value;
                    break;
                case "text":
                    v.Data = value;
                    break;
                case "date":
                    var dt = DateTime.MinValue;
                    DateTime.TryParse(value, out dt);
                    v.DateValue = dt;
                    break;
                case "int":
                    v.IntValue = value.ToInt();
                    break;
            }
            try
            {
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.Message);
            }
            return Content("ok");
        }
    }
}
