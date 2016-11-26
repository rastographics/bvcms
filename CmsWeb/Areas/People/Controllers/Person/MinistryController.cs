using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
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
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogPersonActivity($"Adding contact from: {p.Name}", id, p.Name);
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date
            };

            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();

            var cp = new Contactor
            {
                PeopleId = p.PeopleId,
                ContactId = c.ContactId
            };

            DbUtil.Db.Contactors.InsertOnSubmit(cp);
            DbUtil.Db.SubmitChanges();

            var defaultRole = DbUtil.Db.Setting("Contacts-DefaultRole", null);
            if (!string.IsNullOrEmpty(defaultRole) && DbUtil.Db.Roles.Any(x => x.RoleName == defaultRole))
                TempData["SetRole"] = defaultRole;

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
            var p = DbUtil.Db.LoadPersonById(id);
            DbUtil.LogPersonActivity($"Adding contact to: {p.Name}", id, p.Name);
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date
            };

            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();

            c.contactees.Add(new Contactee {PeopleId = p.PeopleId});
            c.contactsMakers.Add(new Contactor {PeopleId = Util.UserPeopleId.Value});
            DbUtil.Db.SubmitChanges();

            var defaultRole = DbUtil.Db.Setting("Contacts-DefaultRole", null);
            if (!string.IsNullOrEmpty(defaultRole) && DbUtil.Db.Roles.Any(x => x.RoleName == defaultRole))
                TempData["SetRole"] = defaultRole;

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
            var p = DbUtil.Db.LoadPersonById(id);
            if (p == null || !Util.UserPeopleId.HasValue)
                return Content("no id");
            var t = p.AddTaskAbout(DbUtil.Db, Util.UserPeopleId.Value, "Please Contact");
            DbUtil.Db.SubmitChanges();
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
