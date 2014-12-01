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
        public ActionResult EnrollGrid(int id, PagerModel2 pager)
        {
            var m = new CurrentEnrollments(id, pager);
            DbUtil.LogActivity("Viewing Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Current", m);
        }
        [HttpPost]
        public ActionResult PrevEnrollGrid(int id, PagerModel2 pager)
        {
            var m = new PreviousEnrollments(id, pager);
            DbUtil.LogActivity("Viewing Prev Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Previous", m);
        }
        [HttpPost, Route("PendingEnrollGrid/{id}")]
        public ActionResult PendingEnrollGrid(int id)
        {
            var m = new PendingEnrollments(id);
            DbUtil.LogActivity("Viewing Pending Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Pending", m);
        }
        [HttpPost]
        public ActionResult Attendance(int id, PagerModel2 pager)
        {
            var m = new PersonAttendHistoryModel(id, pager, future: false);
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            UpdateModel(m.Pager);
            return View("Enrollment/Attendance", m);
        }
        [HttpPost]
        public ActionResult AttendanceFuture(int id, PagerModel2 pager)
        {
            var m = new PersonAttendHistoryModel(id, pager, future: true);
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            UpdateModel(m.Pager);
            return View("Enrollment/Attendance", m);
        }
        [HttpPost]
        public ActionResult Registrations(int id)
        {
            var m = new RegistrationsModel(id);
            DbUtil.LogActivity("Viewing Registrations for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Registrations", m);
        }
        [HttpPost]
        public ActionResult RegistrationsEdit(int id)
        {
            var m = new RegistrationsModel(id);
            return View("Enrollment/RegistrationsEdit", m);
        }
        [HttpPost]
        public ActionResult RegistrationsUpdate(RegistrationsModel m)
        {
            m.UpdateModel();
            DbUtil.LogActivity("Updating Registrations for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Registrations", m);
        }
    }
}
