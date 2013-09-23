using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/EnrollGrid/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult EnrollGrid(int id, int? page, int? size, string sort, string dir)
        {
            var m = new CurrentEnrollments(id);
            m.Pager.Set("/Person2/EnrollGrid/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Current", m);
        }
        [POST("Person2/PrevEnrollGrid/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult PrevEnrollGrid(int id, int? page, int? size, string sort, string dir)
        {
            var m = new PreviousEnrollments(id);
            m.Pager.Set("/Person2/PrevEnrollGrid/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Prev Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Previous", m);
        }
        [POST("Person2/PendingEnrollGrid/{id}")]
        public ActionResult PendingEnrollGrid(int id)
        {
            var m = new PendingEnrollments(id);
            DbUtil.LogActivity("Viewing Pending Enrollments for: {0}".Fmt(m.person.Name));
            return View("Enrollment/Pending", m);
        }
        [POST("Person2/Attendance/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Attendance(int id, int? page, int? size, string sort, string dir)
        {
            var m = new PersonAttendHistoryModel(id, future: false);
            m.Pager.Set("/Person2/Attendance/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            UpdateModel(m.Pager);
            return View("Enrollment/Attendance", m);
        }
        [POST("Person2/AttendanceFuture/{id}/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult AttendanceFuture(int id, int? page, int? size, string sort, string dir)
        {
            var m = new PersonAttendHistoryModel(id, future: true);
            m.Pager.Set("/Person2/AttendanceFuture/" + id, page, size, sort, dir);
            DbUtil.LogActivity("Viewing Attendance History for: {0}".Fmt(Session["ActivePerson"]));
            UpdateModel(m.Pager);
            return View("Enrollment/Attendance", m);
        }
    }
}
