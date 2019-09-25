using CmsWeb.Areas.People.Models.Task;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using MoreLinq;
using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
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

        [HttpPost, Route("~/Task/Edit")]
        public ContentResult Edit(int pk, string name, string value)
        {
            CurrentDatabase.SetTaskDetails(pk, name, value);
            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity($"Edit Task {pk} to {value}", userId: Util.UserId);

            return new ContentResult {Content = value};
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

        [HttpGet, Route("~/Task/GetStatuses")]
        public JsonResult GetStatuses()
        {
            var statuses = CurrentDatabase.TaskStatuses.Select(x => new { value = x.Id, text = x.Description });

            return Json(statuses, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("~/Task/GetRoles")]
        public JsonResult GetRoles()
        {
            // TODO: Use real data, do not hardcode
            return Json(new []
            {
                new {value = "0", text = "(not specified)"},
                new {value = "Access", text = "Access"},
                new {value = "Admin", text = "Admin"},
                new {value = "CG-LC", text = "CG-LC"},
                new {value = "Finance", text = "Finance"},
                new {value = "Membership", text = "Membership"},
                new {value = "Pastor", text = "Pastor"},
                new {value = "RiseCampaign", text = "RiseCampaign"}
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
