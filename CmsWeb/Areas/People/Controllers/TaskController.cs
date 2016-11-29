using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Task;
using CmsWeb.Models;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Task"), Route("{action}/{id:int}")]
    public class TaskController : CmsStaffController
    {
        [HttpGet, Route("~/Task/{id}")]
        public ActionResult Index(int id)
        {
            var t = TaskModel.FetchModel(id);
            return View(t);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var t = TaskModel.FetchModel(id);
            return View(t);
        }

        [HttpPost, Route("~/Task/Update")]
        public ActionResult Update(TaskModel t)
        {
            if (!ModelState.IsValid)
                return View("Edit", t);
            t.UpdateTask();
            return RedirectToAction("Index", new {id = t.Id});
        }

        [HttpPost]
        public ActionResult Accept(int id)
        {
            TaskModel.AcceptTask(id);
            return Content("Done");
        }

        [HttpPost]
        public ActionResult AddTask(string description)
        {
            var tid = TaskModel.AddTask(Util.UserPeopleId ?? 0, description);
            return Content(tid.ToString());
        }

        [HttpPost]
        public ActionResult ChangeAbout(int id, int peopleid)
        {
            TaskModel.SetWhoId(id, peopleid);
            return View("Index", TaskModel.FetchModel(id));
        }

        [HttpPost]
        public ActionResult ChangeOwner(int id, int peopleid)
        {
            TaskModel.ChangeOwner(id, peopleid);
            return View("Index", TaskModel.FetchModel(id));
        }

        [HttpPost]
        public JsonResult CompleteWithContact(int id)
        {
            var contactid = TaskModel.AddCompletedContact(id);
            return Json(new {ContactId = contactid});
        }

        [HttpPost]
        public ActionResult Decline(int id, string reason)
        {
            TaskModel.DeclineTask(id, reason);
            return RedirectToAction("Index", new {id});
        }

        [HttpPost]
        public ActionResult Delegate(int id, int peopleid)
        {
            TaskModel.Delegate(id, peopleid);
            return View("Index", TaskModel.FetchModel(id));
        }

        [Route("~/Task/NotesExcel2/{id}")]
        public ActionResult NotesExcel2(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            var q = DbUtil.Db.PeopleQuery(id.Value);
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
            TaskModel.CompleteTask(id);
            return Content("Done");
        }

    }
}