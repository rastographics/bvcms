using CmsWeb.Areas.People.Models.Task;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using MoreLinq;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Task"), Route("{action}/{id:int}")]
    public class TaskController : CmsStaffController
    {
        public TaskController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/Task/{id}")]
        public ActionResult Index(int id)
        {
            var t = TaskModel.FetchModel(id, CurrentDatabase.Host, CurrentDatabase);
            return View(t);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var t = TaskModel.FetchModel(id, CurrentDatabase.Host, CurrentDatabase);
            return View(t);
        }

        [HttpPost, Route("~/Task/Update")]
        public ActionResult Update(TaskModel t)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", t);
            }

            t.UpdateTask();
            return RedirectToAction("Index", new { id = t.Id });
        }

        [HttpPost]
        public ActionResult Accept(int id)
        {
            TaskModel.AcceptTask(id, CurrentDatabase.Host, CurrentDatabase);
            return Content("Done");
        }

        [HttpPost]
        public ActionResult AddTask(string description)
        {
            var tid = TaskModel.AddTask(Util.UserPeopleId ?? 0, description, CurrentDatabase.Host, CurrentDatabase);
            return Content(tid.ToString());
        }

        [HttpPost]
        public ActionResult ChangeAbout(int id, int peopleid)
        {
            TaskModel.SetWhoId(id, peopleid, CurrentDatabase.Host, CurrentDatabase);
            return View("Index", TaskModel.FetchModel(id, CurrentDatabase.Host, CurrentDatabase));
        }

        [HttpPost]
        public ActionResult ChangeOwner(int id, int peopleid)
        {
            TaskModel.ChangeOwner(id, peopleid, CurrentDatabase.Host, CurrentDatabase);
            return View("Index", TaskModel.FetchModel(id, CurrentDatabase.Host, CurrentDatabase));
        }

        [HttpPost]
        public JsonResult CompleteWithContact(int id)
        {
            var contactId = TaskModel.AddCompletedContact(id, CurrentDatabase.Host, CurrentDatabase);
            return Json(new { ContactId = contactId });
        }

        [HttpPost]
        public ActionResult Decline(int id, string reason)
        {
            TaskModel.DeclineTask(id, reason, CurrentDatabase.Host, CurrentDatabase);
            return RedirectToAction("Index", new { id });
        }

        [HttpPost]
        public ActionResult Delegate(int id, int peopleid)
        {
            TaskModel.Delegate(id, peopleid, CurrentDatabase.Host, CurrentDatabase);
            return View("Index", TaskModel.FetchModel(id, CurrentDatabase.Host, CurrentDatabase));
        }

        [Route("~/Task/NotesExcel2/{id}")]
        public ActionResult NotesExcel2(Guid? id)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            var q = CurrentDatabase.PeopleQuery(id.Value);
            var q2 = from p in q
                     let t = p.TasksAboutPerson.OrderByDescending(t => t.CreatedOn).FirstOrDefault(t => t.Notes != null)
                     where t != null
                     select new
                     {
                         p.Name,
                         t.Notes,
                         t.CreatedOn
                     };
            return q2.ToDataTable().ToExcel("TaskNotes.xlsx");
        }

        [HttpPost]
        public ActionResult SetComplete(int id)
        {
            TaskModel.CompleteTask(id, CurrentDatabase.Host, CurrentDatabase);
            return Content("Done");
        }

    }
}
