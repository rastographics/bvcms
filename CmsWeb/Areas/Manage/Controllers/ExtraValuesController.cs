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
    [RouteArea("Manage", AreaUrl = "Manage/ExtraValues")]
    public class ExtraValuesController : CmsStaffController
    {
        [POST("Add/{id}")]
        public ActionResult Add(int id, string field, string value)
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
        [POST("Delete/{id}")]
        public ActionResult Delete(int id, string field, string value)
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
		[Authorize(Roles="Admin")]
        [POST("DeleteAll")]
        public ActionResult DeleteAll(string field, string type, string value)
        {
			var ev = DbUtil.Db.PeopleExtras.Where(ee => ee.Field == field).FirstOrDefault();
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
        [GET("QueryCodes")]
        public ActionResult QueryCodes(string field, string value)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);
            qb.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, "{0}:{1}".Fmt(field, value));
            DbUtil.Db.SubmitChanges();
            return Redirect("/QueryBuilder/Main/" + qb.QueryId);
        }
        [GET("QueryDataFields")]
        public ActionResult QueryDataFields(string field, string type)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            QueryBuilderClause c = null;
            qb.CleanSlate(DbUtil.Db);
            switch (type.ToLower())
            {
                case "text":
                    c = qb.AddNewClause(QueryType.PeopleExtraData, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "date":
                    c = qb.AddNewClause(QueryType.PeopleExtraDate, CompareType.NotEqual, null);
                    c.Quarters = field;
                    break;
                case "int":
                    c = qb.AddNewClause(QueryType.PeopleExtraInt, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "?":
                    qb.AddNewClause(QueryType.HasPeopleExtraField, CompareType.Equal, field);
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return Redirect("/QueryBuilder/Main/" + qb.QueryId);
        }
    }
}
