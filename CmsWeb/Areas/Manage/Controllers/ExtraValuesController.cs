using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix= "Manage/ExtraValues"), Route("{action}/{id?}")]
    public class ExtraValuesController : CmsStaffController
    {
        [HttpPost, Route("Add2/{id:guid}")]
        public ActionResult Add2(Guid id, string field, string value)
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
        [HttpPost, Route("Delete2/{id:guid}")]
        public ActionResult Delete2(Guid id, string field, string value)
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
        [Authorize(Roles = "Admin")]
        [HttpPost, Route("DeleteAll")]
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
        [HttpGet, Route("QueryCodes")]
        public ActionResult QueryCodes(string field, string value)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();
            cc.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, $"{field}:{value}");
            cc.Save(DbUtil.Db);
            return Redirect("/Query/" + cc.Id);
        }
        [HttpGet, Route("QueryDataFields")]
        public ActionResult QueryDataFields(string field, string type)
        {
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset();
            Condition c2;
            switch (type.ToLower())
            {
                case "text":
                    c2 = cc.AddNewClause(QueryType.PeopleExtraData, CompareType.NotEqual, "");
                    c2.Quarters = field;
                    break;
                case "date":
                    c2 = cc.AddNewClause(QueryType.PeopleExtraDate, CompareType.NotEqual, null);
                    c2.Quarters = field;
                    break;
                case "int":
                    c2 = cc.AddNewClause(QueryType.PeopleExtraInt, CompareType.NotEqual, "");
                    c2.Quarters = field;
                    break;
                case "?":
                    cc.AddNewClause(QueryType.HasPeopleExtraField, CompareType.Equal, field);
                    break;
            }
            cc.Save(DbUtil.Db);
            return Redirect("/Query/" + cc.Id);
        }
    }
}
