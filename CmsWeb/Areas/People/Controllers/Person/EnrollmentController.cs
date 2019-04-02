using System.Web.Mvc;
using System.Linq;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult EnrollGrid(CurrentEnrollments m)
        {
            if (m.Person == null)
                return Content("Cannot find person");
            if (!m.Sort.HasValue())
                m.Sort = "default";
            DbUtil.LogPersonActivity($"Viewing Enrollments for: {m.Person.Name}", m.Person.PeopleId, m.Person.Name);
            return View("Enrollment/Current", m);
        }

        [HttpPost]
        public ActionResult PrevEnrollGrid(PreviousEnrollments m)
        {
            if (!m.Sort.HasValue())
                m.Sort = "default";
            DbUtil.LogPersonActivity($"Viewing Prev Enrollments for: {m.Person.Name}", m.Person.PeopleId, m.Person.Name);
            return View("Enrollment/Previous", m);
        }

        [HttpPost]
        public ActionResult PendingEnrollGrid(PendingEnrollments m)
        {
            DbUtil.LogPersonActivity($"Viewing Pending Enrollments for: {m.Person.Name}", m.Person.PeopleId, m.Person.Name);
            return View("Enrollment/Pending", m);
        }

        [HttpPost]
        public ActionResult Attendance(PersonAttendHistoryModel m)
        {
            var name = Session["ActivePerson"] as string;
            DbUtil.LogPersonActivity($"Viewing Attendance History for: {name}", m.PeopleId, name);
            return View("Enrollment/Attendance", m);
        }

        [HttpPost]
        public ActionResult AttendanceFuture(PersonAttendHistoryModel m)
        {
            m.Future = true;
            var name = Session["ActivePerson"] as string;
            DbUtil.LogPersonActivity($"Viewing Attendance History for: {name}", m.PeopleId, name);
            return View("Enrollment/Attendance", m);
        }

        [HttpPost]
        public ActionResult Registrations(int PeopleId)
        {
            var m = new RegistrationsModel(PeopleId);
            DbUtil.LogPersonActivity($"Viewing Registrations for: {m.Person.Name}", m.Person.PeopleId, m.Person.Name);
            m.ShowComments = ShowComments();
            return View("Enrollment/Registrations", m);
        }

        [HttpPost]
        public ActionResult RegistrationsEdit(int PeopleId)
        {
            var m = new RegistrationsModel(PeopleId);
            m.ShowComments = ShowComments();
            return View("Enrollment/RegistrationsEdit", m);
        }

        [HttpPost]
        public ActionResult RegistrationsUpdate(RegistrationsModel m)
        {
            bool ExcludeComments = !ShowComments();
            m.UpdateModel(ExcludeComments);
            DbUtil.LogPersonActivity($"Updating Registrations for: {m.Person.Name}", m.Person.PeopleId, m.Person.Name);
            m.ShowComments = ShowComments();
            return View("Enrollment/Registrations", m);
        }

        private bool ShowComments()
        {
            string limitToRole = CurrentDatabase.Setting("LimitRegistrationHistoryToRole", "false");
            if (limitToRole == "false")
            {
                return true;
            }
            else
            {
                return CurrentDatabase.CurrentRoles().Contains(limitToRole);
            }
        }
    }
}
