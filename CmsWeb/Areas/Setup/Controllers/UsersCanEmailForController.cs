using CmsData;
using CmsWeb.Lifecycle;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin,manageemails")]
    [RouteArea("Setup")]
    public class UsersCanEmailForController : CmsStaffController
    {
        public UsersCanEmailForController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/UsersCanEmailFor")]
        public ActionResult Index()
        {
            var q = from cf in CurrentDatabase.PeopleCanEmailFors
                    orderby cf.PersonCanEmail.Name2, cf.OnBehalfOfPerson.Name2
                    select cf;
            return View(q);
        }
        [Route("~/PersonCanEmailForList/{id:int}")]
        public ActionResult PersonCanEmailForList(int id)
        {
            Response.NoCache();
            var t = CurrentDatabase.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            CurrentDatabase.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            CurrentDatabase.SubmitChanges();
            if (id > 0)
            {
                var q = (from cf in CurrentDatabase.PeopleCanEmailFors
                         where cf.CanEmail == id
                         select cf.OnBehalfOf).ToList();
                if (!q.Contains(id))
                {
                    t.PersonTags.Add(new TagPerson { PeopleId = id });
                }

                foreach (var pid in q)
                {
                    t.PersonTags.Add(new TagPerson { PeopleId = pid });
                }

                CurrentDatabase.SubmitChanges();
                return Redirect("/SearchUsers?ordered=true&topid=" + id);
            }
            return Redirect("/SearchUsers?singlemode=true");
        }

        [HttpPost]
        [Route("~/UpdatePersonCanEmailForList/{id:int}")]
        public ActionResult UpdatePersonCanEmailForList(int id, int? topid0)
        {
            var t = CurrentDatabase.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var selected_pids = (from p in t.People(CurrentDatabase)
                                 where p.PeopleId != id
                                 select p.PeopleId).ToArray();
            CurrentDatabase.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            CurrentDatabase.Tags.DeleteOnSubmit(t);
            CurrentDatabase.SubmitChanges();
            if (topid0 == id)
            {
                var cn = new SqlConnection(Util.ConnectionString);
                cn.Open();
                cn.Execute("delete PeopleCanEmailFor where CanEmail = @id", new { id });
            }
            foreach (var pid in selected_pids)
            {
                CurrentDatabase.PeopleCanEmailFors.InsertOnSubmit(new PeopleCanEmailFor { CanEmail = id, OnBehalfOf = pid });
            }

            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }
    }
}
