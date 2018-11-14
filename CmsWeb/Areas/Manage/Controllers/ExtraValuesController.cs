using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Manage/ExtraValues"), Route("{action}/{id?}")]
    public class ExtraValuesController : CmsStaffController
    {
        public ExtraValuesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("Add2/{id:guid}")]
        public ActionResult Add2(Guid id, string field, string value)
        {
            var list = CurrentDatabase.PeopleQuery(id).Select(pp => pp.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraValue(CurrentDatabase, pid, field, value);
                CurrentDatabase.SubmitChanges();
                //DbDispose();
            }
            return Content("done");
        }
        [HttpPost, Route("Delete2/{id:guid}")]
        public ActionResult Delete2(Guid id, string field, string value)
        {
            var list = CurrentDatabase.PeopleQuery(id).Select(pp => pp.PeopleId).ToList();
            foreach (var pid in list)
            {
                var ev = Person.GetExtraValue(CurrentDatabase, pid, field, value);
                if (ev == null)
                {
                    continue;
                }

                CurrentDatabase.PeopleExtras.DeleteOnSubmit(ev);
                CurrentDatabase.SubmitChanges();
                //DbDispose();
            }
            return Content("done");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, Route("DeleteAll")]
        public ActionResult DeleteAll(string field, string type, string value)
        {
            var ev = CurrentDatabase.PeopleExtras.Where(ee => ee.Field == field).FirstOrDefault();
            if (ev == null)
            {
                return Content("error: no field");
            }

            switch (type.ToLower())
            {
                case "code":
                    CurrentDatabase.ExecuteCommand("delete PeopleExtra where field = {0} and StrValue = {1}", field, value);
                    break;
                case "bit":
                    CurrentDatabase.ExecuteCommand("delete PeopleExtra where field = {0} and BitValue = {1}", field, value);
                    break;
                case "int":
                    CurrentDatabase.ExecuteCommand("delete PeopleExtra where field = {0} and IntValue is not null", field);
                    break;
                case "date":
                    CurrentDatabase.ExecuteCommand("delete PeopleExtra where field = {0} and DateValue is not null", field);
                    break;
                case "text":
                    CurrentDatabase.ExecuteCommand("delete PeopleExtra where field = {0} and Data is not null", field);
                    break;
                case "?":
                    CurrentDatabase.ExecuteCommand("delete PeopleExtra where field = {0} and data is null and datevalue is null and intvalue is null", field);
                    break;
            }
            return Content("done");
        }
        [HttpGet, Route("QueryCodes")]
        public ActionResult QueryCodes(string field, string value)
        {
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            cc.AddNewClause(QueryType.PeopleExtra, CompareType.Equal, $"{field}:{value}");
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }
        [HttpGet, Route("QueryDataFields")]
        public ActionResult QueryDataFields(string field, string type)
        {
            var cc = CurrentDatabase.ScratchPadCondition();
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
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }
    }
}
