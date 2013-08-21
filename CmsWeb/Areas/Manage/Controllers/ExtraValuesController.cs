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
                AddEditExtraValue(pid, field, value);
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
                var ev = GetExtraValue(pid, field, value);
                if (ev == null)
                    continue;
                DbUtil.Db.PeopleExtras.DeleteOnSubmit(ev);
                DbUtil.Db.SubmitChanges();
                DbUtil.DbDispose();
            }
            return Content("done");
        }
		[Authorize(Roles="Admin")]
        [POST("DeleteAll/{field}/{type}/{val}")]
        public ActionResult DeleteAll(string field, string type, string val)
        {
			var ev = DbUtil.Db.PeopleExtras.Where(ee => ee.Field == field).FirstOrDefault();
		    if (ev == null)
		        return Content("error: no field");
		    switch (type)
		    {
                case "Code":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and StrValue = {1}", field, val);
		            break;
                case "Bit":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and BitValue = {1}", field, val);
		            break;
                case "Int":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and IntValue is not null", field);
		            break;
                case "Date":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and DateValue is not null", field);
		            break;
                case "Text":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and Data is not null", field);
		            break;
                case "?":
	                DbUtil.Db.ExecuteCommand("delete PeopleExtra where field = {0} and data is null and datevalue is null and intvalue is null", field);
		            break;
		    }
		    return Content("done");
        }
        [GET("Query/{field}/{val}")]
        public ActionResult Query(string field, string val)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);
            qb.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, "{0}:{1}".Fmt(field, val));
            DbUtil.Db.SubmitChanges();
            return Redirect("/QueryBuilder/Main/" + qb.QueryId);
        }
        [GET("QueryDataFields/{field}/{type}")]
        public ActionResult QueryDataFields(string field, string type)
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            QueryBuilderClause c = null;
            qb.CleanSlate(DbUtil.Db);
            switch (type)
            {
                case "Text":
                    c = qb.AddNewClause(QueryType.PeopleExtraData, CompareType.NotEqual, "");
                    c.Quarters = field;
                    break;
                case "Date":
                    c = qb.AddNewClause(QueryType.PeopleExtraDate, CompareType.NotEqual, null);
                    c.Quarters = field;
                    break;
                case "Int":
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
        private static PeopleExtra GetExtraValue(int id, string field)
        {
		    field = field.Replace('/', '-');
            var q = from v in DbUtil.Db.PeopleExtras
                    where v.Field == field
                    where v.PeopleId == id
                    select v;
            var ev = q.SingleOrDefault();
            if (ev == null)
            {
                ev = new PeopleExtra
                {
                    PeopleId = id,
                    Field = field,
                    TransactionTime = DateTime.Now
                };
                DbUtil.Db.PeopleExtras.InsertOnSubmit(ev);
            }
            return ev;
        }
        private static PeopleExtra GetExtraValue(int id, string field, string value)
        {
            var novalue = !value.HasValue();
            var q = from v in DbUtil.Db.PeopleExtras
                    where v.PeopleId == id
                    where v.Field == field
                    where novalue || v.StrValue == value 
                    select v;
            var ev = q.SingleOrDefault();
            return ev;
        }
        private static void AddEditExtraValue(int id, string field, string value)
        {
            if (!value.HasValue())
                return;
            var ev = GetExtraValue(id, field);
            ev.StrValue = value;
        }
    }
}
