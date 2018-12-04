using CmsData;
using CmsWeb.Areas.People.Models;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult ContactsMade(ContactsMadeModel m)
        {
            return View("Ministry/Contacts", m);
        }

        [HttpPost]
        public ActionResult AddContactMade(int id)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            DbUtil.LogPersonActivity($"Adding contact from: {p.Name}", id, p.Name);
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date
            };

            CurrentDatabase.Contacts.InsertOnSubmit(c);
            CurrentDatabase.SubmitChanges();

            var cp = new Contactor
            {
                PeopleId = p.PeopleId,
                ContactId = c.ContactId
            };

            CurrentDatabase.Contactors.InsertOnSubmit(cp);
            CurrentDatabase.SubmitChanges();

            var defaultRole = CurrentDatabase.Setting("Contacts-DefaultRole", null);
            if (!string.IsNullOrEmpty(defaultRole) && CurrentDatabase.Roles.Any(x => x.RoleName == defaultRole))
            {
                TempData["SetRole"] = defaultRole;
            }

            TempData["ContactEdit"] = true;
            return Content("/Contact2/" + c.ContactId);
        }

        [HttpPost]
        public ActionResult ContactsReceived(ContactsReceivedModel m)
        {
            return View("Ministry/Contacts", m);
        }

        [HttpPost]
        public ActionResult AddContactReceived(int id)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            DbUtil.LogPersonActivity($"Adding contact to: {p.Name}", id, p.Name);
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date
            };

            CurrentDatabase.Contacts.InsertOnSubmit(c);
            CurrentDatabase.SubmitChanges();

            c.contactees.Add(new Contactee { PeopleId = p.PeopleId });
            c.contactsMakers.Add(new Contactor { PeopleId = Util.UserPeopleId.Value });
            CurrentDatabase.SubmitChanges();

            var defaultRole = CurrentDatabase.Setting("Contacts-DefaultRole", null);
            if (!string.IsNullOrEmpty(defaultRole) && CurrentDatabase.Roles.Any(x => x.RoleName == defaultRole))
            {
                TempData["SetRole"] = defaultRole;
            }

            TempData["ContactEdit"] = true;
            return Content($"/Contact2/{c.ContactId}");
        }

        [HttpPost]
        public ActionResult TasksAbout(TasksAboutModel m)
        {
            return View("Ministry/Tasks", m);
        }

        [HttpPost]
        public ActionResult AddTaskAbout(int id)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            if (p == null || !Util.UserPeopleId.HasValue)
            {
                return Content("no id");
            }

            var t = p.AddTaskAbout(CurrentDatabase, Util.UserPeopleId.Value, "Please Contact");
            CurrentDatabase.SubmitChanges();
            return Content($"/Task/{t.Id}");
        }

        [HttpPost]
        public ActionResult TasksAssigned(TasksAssignedModel m)
        {
            return View("Ministry/Tasks", m);
        }

        [HttpPost]
        public ActionResult VolunteerApprovals(VolunteerModel m)
        {
            m.Populate(m.PeopleId);
            return View("Ministry/Volunteer", m);
        }
    }
}
