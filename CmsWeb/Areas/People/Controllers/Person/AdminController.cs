using CmsData;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost, Authorize(Roles = "Edit")]
        public ActionResult Split(int id)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            DbUtil.LogPersonActivity($"Splitting Family for {p.Name}", id, p.Name);
            p.SplitFamily(CurrentDatabase);

            return Content("/Person2/" + id);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult PromoteToHeadOfHousehold(int peopleId)
        {
            var person = CurrentDatabase.LoadPersonById(peopleId);
            DbUtil.LogPersonActivity($"Replacing {person.Family.HeadOfHousehold.Name} as head of household.", person.PeopleId, person.Name);
            person.PromoteToHeadOfHousehold(CurrentDatabase);

            return Content("/Person2/" + peopleId);
        }

        [HttpPost, Authorize(Roles = "Delete")]
        public ActionResult Delete(int id)
        {
            Util.Auditing = false;
            var i = (from person in CurrentDatabase.People
                     where person.PeopleId == id
                     let devel = person.Users.Any(uu => uu.UserRoles.Any(rr => rr.Role.RoleName == "Developer"))
                     select new { person, devel }).SingleOrDefault();
            if (i == null)
            {
                return Content(ErrorUrl("bad peopleid"));
            }

            if (i.devel && !User.IsInRole("Developer"))
            {
                return Content(ErrorUrl("cannot delete a developer"));
            }

            var p = i.person.Family.People.FirstOrDefault(m => m.PeopleId != id);
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
            CurrentDatabase.PurgePerson(id);
            DbUtil.LogActivity($"Deleted Record {i.person.Name} ({i.person.PeopleId})");
            return Content("/Person2/DeletedPerson");
        }

        [HttpGet]
        public ActionResult DeletedPerson()
        {
            return View("Personal/DeletedPerson");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Move(int id, int to)
        {
            var p = CurrentDatabase.People.Single(pp => pp.PeopleId == id);
            try
            {
                p.MovePersonStuff(CurrentDatabase, to);
                CurrentDatabase.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }

        public ActionResult ShowMeetings(int id, bool all)
        {
            if (all)
            {
                Session["showallmeetings"] = true;
            }
            else
            {
                Session.Remove("showallmeetings");
            }

            return Redirect("/Person2/" + id);
        }
    }
}
