using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost, Route("Person2/Split/{id}"), Authorize(Roles = "Edit")]
        public ActionResult Split(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogActivity("Splitting Family for {0}".Fmt(p.Name));
            p.SplitFamily(DbUtil.Db);
            return Content("/Person2/" + id);
        }

        [HttpPost, Route("Person2/Delete/{id}"), Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Util.Auditing = false;
            var person = DbUtil.Db.LoadPersonById(id);
            if (person == null)
                return Content("error, bad peopleid");

            var p = person.Family.People.FirstOrDefault(m => m.PeopleId != id);
            if (p != null)
            {
                Util2.CurrentPeopleId = p.PeopleId;
                Session["ActivePerson"] = p.Name;
            }
            else
            {
                Util2.CurrentPeopleId = 0;
                Session.Remove("ActivePerson");
            }
            DbUtil.Db.PurgePerson(id);
            DbUtil.LogActivity("Deleted Record {0}".Fmt(person.PeopleId));
            return Content("/Person2/DeletedPerson");
        }

        [HttpGet, Route("Person2/DeletedPerson")]
        public ActionResult DeletedPerson()
        {
            return View("Personal/DeletedPerson");
        }



        [Authorize(Roles = "Admin")]
        public ActionResult Move(int id, int to)
        {
            var p = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            try
            {
                p.MovePersonStuff(DbUtil.Db, to);
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }

        public ActionResult ShowMeetings(int id, bool all)
        {
            if (all == true)
                Session["showallmeetings"] = true;
            else
                Session.Remove("showallmeetings");
            return Redirect("/Person2/" + id);
        }
    }
}
