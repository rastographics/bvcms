/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MoreLinq;
using UtilityExtensions;
using System.Web.Routing;
using CmsWeb;
using CmsWeb.Models;
using CmsData;

namespace CmsWeb.Areas.Main.Controllers
{
    [ValidateInput(false)]
    [RouteArea("Main", AreaPrefix = "Task"), Route("{action=List}/{id?}")]
    public class TaskController : CmsStaffController
    {
        public TaskController()
        {
            ViewData["Title"] = "Tasks";
        }

        public ActionResult List(string id)
        {
            var tasks = new TaskModel();
            UpdateModel(tasks);
            DbUtil.LogActivity("Tasks");
            if (id == "0")
                return PartialView("Rows", tasks);
            return View(tasks);
        }

        [HttpPost]
        public ActionResult SetComplete(int id)
        {
            var tasks = new TaskModel { Id = id.ToString() };
            tasks.CompleteTask(id);
            return Content("Done");
        }

        [HttpPost]
        public ActionResult Accept(int id)
        {
            var tasks = new TaskModel { Id = id.ToString() };
            tasks.AcceptTask(id);
            return Content("Done");
        }

        public ActionResult Detail(int id)
        {
            var tasks = new TaskModel();
            return View("Detail", tasks.FetchTask(id));
        }

        public ActionResult Columns(int id)
        {
            var tasks = new TaskModel();
            return PartialView(tasks.FetchTask(id));
        }

        public ActionResult Row(int id)
        {
            var tasks = new TaskModel();
            return PartialView(tasks.FetchTask(id));
        }

        [HttpPost]
        public ActionResult AddTask(string CurTab, string TaskDesc)
        {
            var model = new TaskModel { CurTab = CurTab };
            var listid = model.CurListId;
            if (listid == 0)
            {
                listid = TaskModel.InBoxId(model.PeopleId);
                var c = new HttpCookie("tasktab", model.CurTab);
                c.Expires = Util.Now.AddDays(360);
                Response.Cookies.Add(c);
            }
            var tid = model.AddTask(model.PeopleId, listid, TaskDesc);
            return PartialView("Row", model.FetchTask(tid));
        }

        [HttpPost]
        public ActionResult AddList(string ListName)
        {
            var model = new TaskModel();
            model.AddList(ListName);
            return View("TabsOptions", model);
        }

        [HttpPost]
        public ActionResult AddSourceContact(int id, int contactid)
        {
            var tasks = new TaskModel();
            tasks.AddSourceContact(id, contactid);
            return PartialView("Detail", tasks.FetchTask(id));
        }

        [HttpPost]
        public JsonResult CompleteWithContact(int id)
        {
            var tasks = new TaskModel();
            var contactid = tasks.AddCompletedContact(id);
            return Json(new { ContactId = contactid });
        }

        [HttpPost]
        public ActionResult ChangeOwner(int id, int peopleid)
        {
            var tasks = new TaskModel();
            tasks.ChangeOwner(id, peopleid);
            return PartialView("Detail", tasks.FetchTask(id));
        }

        [HttpPost]
        public ActionResult Delegate(int id, int peopleid)
        {
            var tasks = new TaskModel();
            tasks.Delegate(id, peopleid);
            return PartialView("Detail", tasks.FetchTask(id));
        }

        [HttpPost]
        public ActionResult DelegateAll(int id, string items)
        {
            var tasks = new TaskModel();
            var a = items.SplitStr(",").Select(i => i.ToInt());
            foreach (var tid in a)
            {
                var t = tasks.Delegate(tid, id);
                if (t != null)
                    t.ForceCompleteWContact = true;
            }
            DbUtil.Db.SubmitChanges();
            return PartialView("Rows", tasks);
        }

        [HttpPost]
        public ActionResult ChangeAbout(int id, int peopleid)
        {
            TaskModel.SetWhoId(id, peopleid);
            var tasks = new TaskModel();
            return PartialView("Detail", tasks.FetchTask(id));
        }

        public ActionResult Edit(int id)
        {
            var m = new TaskModel();
            return View(m.FetchTask(id));
        }

        [HttpPost]
        public ActionResult Update(int id)
        {
            var m = new TaskModel();
            var t = m.FetchTask(id);
            UpdateModel(t);
            t.UpdateTask();
            return RedirectToAction("Detail", new {id = id});
        }

        [HttpPost]
        public ActionResult Action(int? id, string option, string items, string curtab)
        {
            var tasks = new TaskModel();
            tasks.CurTab = curtab;
            var a = items.SplitStr(",").Select(i => i.ToInt());

            if (option.StartsWith("M"))
            {
                var ToTab = option.Substring(1).ToInt();
                if (curtab == "t" + ToTab)
                    return new EmptyResult();
                tasks.MoveTasksToList(a, ToTab);
            }
            else if (option == "deletelist")
            {
                tasks.DeleteList(curtab);
                return PartialView("TabsOptionsRows", tasks);
            }
            else if (option == "delete")
                tasks.DeleteTasks(a);
            else if (option.StartsWith("P"))
                tasks.Priortize(a, option);
            else if (option == "archive")
                tasks.ArchiveTasks(a);

            return PartialView("Rows", tasks);
        }

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

    }
}
