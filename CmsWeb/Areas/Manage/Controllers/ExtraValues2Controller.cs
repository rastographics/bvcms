using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaUrl = "Manage/ExtraValues2")]
    public class ExtraValues2Controller : CmsStaffController
    {
        [POST("Manage/ExtraValues2/Add/{id}")]
        public ActionResult Add(Guid id, string field, string value)
        {
            var list = DbUtil.Db.PeopleQuery(id).Select(pp => pp.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraValue(DbUtil.Db, pid, field, value);
                DbUtil.Db.SubmitChanges();
                DbUtil.DbDispose();
            }
            return Content("done");
        }
        [POST("Manage/ExtraValues2/Delete/{id}")]
        public ActionResult Delete(Guid id, string field, string value)
        {
            var list = DbUtil.Db.PeopleQuery(id).Select(pp => pp.PeopleId).ToList();
            foreach (var pid in list)
            {
                var ev = Person.GetExtraValue(DbUtil.Db, pid, field, value);
                if (ev == null)
                    continue;
                DbUtil.Db.PeopleExtras.DeleteOnSubmit(ev);
                DbUtil.Db.SubmitChanges();
                DbUtil.DbDispose();
            }
            return Content("done");
        }
        [GET("QueryCodes")]
        public ActionResult QueryCodes(string field, string value)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset(DbUtil.Db);
            c.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, "{0}:{1}".Fmt(field, value));
            c.Save(DbUtil.Db);
            return Redirect("/Query/" + c.Id);
        }
        [POST("DeleteAll")]
        public ActionResult DeleteAll(string field, string type, string value)
        {
			var ev = DbUtil.Db.PeopleExtras.FirstOrDefault(ee => ee.Field == field);
		    if (ev == null)
		        return Content("error: no field");
		    switch (type.ToLower())
		    {
                case "code":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and StrValue = {1}", field, value);
		            break;
                case "bit":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and BitValue = {1}", field, value);
		            break;
                case "int":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and IntValue is not null", field);
		            break;
                case "date":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and DateValue is not null", field);
		            break;
                case "text":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and Data is not null", field);
		            break;
                case "?":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and data is null and datevalue is null and intvalue is null", field);
		            break;
		    }
		    return Content("done");
        }
    }
}
