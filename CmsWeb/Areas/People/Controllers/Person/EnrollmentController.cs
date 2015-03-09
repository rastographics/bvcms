using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult EnrollGrid(CurrentEnrollments m)
        {
            DbUtil.LogActivity("Viewing Enrollments for: {0}".Fmt(m.Person.Name));
            return View("Enrollment/Current", m);
        }
        [HttpPost]
        public ActionResult PrevEnrollGrid(PreviousEnrollments m)
        {
            DbUtil.LogActivity("Viewing Prev Enrollments for: {0}".Fmt(m.Person.Name));
            return View("Enrollment/Previous", m);
        }
        [HttpPost]
        public ActionResult PendingEnrollGrid(PendingEnrollments m)
        {
            DbUtil.LogActivity("Viewing Pending Enrollments for: {0}".Fmt(m.Person.Name));
            return View("Enrollment/Pending", m);
        }
        [HttpPost]
        public ActionResult Attendance(PersonAttendHistoryModel m)
        {
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            return View("Enrollment/Attendance", m);
        }
        [HttpPost]
        public ActionResult AttendanceFuture(PersonAttendHistoryModel m)
        {
            m.Future = true;
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            return View("Enrollment/Attendance", m);
        }
        [HttpPost]
        public ActionResult Registrations(int PeopleId)
        {
            var m = new RegistrationsModel(PeopleId);
            DbUtil.LogActivity("Viewing Registrations for: {0}".Fmt(m.Person.Name));
            return View("Enrollment/Registrations", m);
        }
        [HttpPost]
        public ActionResult RegistrationsEdit(int PeopleId)
        {
            var m = new RegistrationsModel(PeopleId);
            return View("Enrollment/RegistrationsEdit", m);
        }
        [HttpPost]
        public ActionResult RegistrationsUpdate(RegistrationsModel m)
        {
            m.UpdateModel();
            DbUtil.LogActivity("Updating Registrations for: {0}".Fmt(m.Person.Name));
            return View("Enrollment/Registrations", m);
        }
    }
}
