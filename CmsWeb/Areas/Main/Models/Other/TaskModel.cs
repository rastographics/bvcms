/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using CmsWeb.Code;
using UtilityExtensions;
using CmsData;
using System.ComponentModel;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public class TaskListInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public interface ITaskFormBindable
    {
        string Id { get; set; }
        string CurTab { get; set; }
        string Project { get; set; }
        string Location { get; set; }
        bool? ForceCompleteWContact { get; set; }
        int? StatusId { get; set; }
        bool? OwnerOnly { get; set; }
        string Sort { get; set; }
        int? Page { get; set; }
        int? PageSize { get; set; }
    }
    public class TaskModel : ITaskFormBindable
    {
        private const string STR_InBox = "InBox";

        public int PeopleId { get; set; }
        private string _Id;
        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                CurTab = MyListId();
            }
        }
        public int intId
        {
            get { return _Id.ToInt(); }
        }
        
        public string Project { get; set; }
        public string Location { get; set; }
        public bool? ForceCompleteWContact { get; set; }
        public int? StatusId { get; set; }
        public string CurTab
        {
            get { return DbUtil.Db.UserPreference("CurTaskTab"); }
            set
            {
                if (value.HasValue())
                    DbUtil.Db.SetUserPreference("CurTaskTab", value);
            }
        }
        public bool? OwnerOnly
        {
            get { return DbUtil.Db.UserPreference("tasks-owneronly").ToBool2(); }
            set
            {
                if (value.HasValue)
                    DbUtil.Db.SetUserPreference("tasks-owneronly", value);
            }
        }
        public string Sort { get; set; }

        private int? _Page;
        public int? Page
        {
            get { return _Page ?? 1; }
            set { _Page = value; }
        }
        public int StartRow
        {
            get { return (Page.Value - 1) * PageSize.Value; }
        }
        public int? PageSize
        {
            get { return DbUtil.Db.UserPreference("PageSize", "10").ToInt(); }
            set
            {
                if (value.HasValue)
                    DbUtil.Db.SetUserPreference("PageSize", value);
            }
        }
        private int? count;
        public int Count
        {
            get
            {
                if (!count.HasValue)
                    count = ApplySearch().Count();
                return count.Value;
            }
        }

        private int completedcode = TaskStatusCode.Complete;
        private int somedaycode = TaskStatusCode.Someday;

        public int CurListId
        {
            get
            {
                if (CurTab.HasValue())
                    return CurTab.Substring(1).ToInt();
                return TaskModel.InBoxId(PeopleId);
            }
        }

        public TaskModel()
        {
            if (PeopleId == 0 && Util.UserPeopleId != null)
                PeopleId = Util.UserPeopleId.Value;
        }

        public IEnumerable<TaskListInfo> FetchTaskLists()
        {
            Task.GetRequiredTaskList(STR_InBox, PeopleId);
            Task.GetRequiredTaskList("Personal", PeopleId);
            return from t in DbUtil.Db.TaskLists
                   where t.TaskListOwners.Any(tlo => tlo.PeopleId == PeopleId) || t.CreatedBy == PeopleId
                   orderby t.Name
                   select new TaskListInfo
                   {
                       Id = "t" + t.Id,
                       Name = t.Name,
                   };
        }

        SelectListItem[] actions =
        {
            new SelectListItem { Value = "", Text = "Actions" },
            new SelectListItem { Value = "-", Text = "Tasks..." },
            new SelectListItem { Value = "delegate", Text = ".. Delegate Task" },
            new SelectListItem { Value = "archive", Text = ".. Archive Task" },
            new SelectListItem { Value = "delete", Text = ".. Delete Task" },
            new SelectListItem { Value = "-", Text = "Set Priority..." },
            new SelectListItem { Value = "P1", Text = ".. 1" },
            new SelectListItem { Value = "P2", Text = ".. 2" },
            new SelectListItem { Value = "P3", Text = ".. 3" },
            new SelectListItem { Value = "P0", Text = ".. None" },
            new SelectListItem { Value = "-", Text = "List..." },
            new SelectListItem { Value = "sharelist", Text = ".. Share List" },
            new SelectListItem { Value = "deletelist", Text = ".. Delete List" },
            new SelectListItem { Value = "-", Text = "Move To List..." },
        };
        public IEnumerable<SelectListItem> ActionItems()
        {
            var q = from t in DbUtil.Db.TaskLists
                    where t.TaskListOwners.Any(tlo => tlo.PeopleId == PeopleId) || t.CreatedBy == PeopleId
                    orderby t.Name
                    select new SelectListItem
                    {
                        Text = ".. " + t.Name,
                        Value = "M" + t.Id,
                    };
            return actions.Union(q);
        }
        public IEnumerable<TaskInfo> FetchTasks()
        {
            var q = ApplySearch();
            var iPhone = HttpContext.Current.Request.UserAgent.Contains("iPhone");
            switch (Sort)
            {
                case "123":
                case "123 DESC":
                    q = from t in q
                        orderby (t.StatusId == completedcode ? 3 : (t.StatusId == somedaycode ? 2 : 1)), t.Priority ?? 4, t.Due ?? DateTime.MaxValue.Date, t.Description
                        select t;
                    break;
                case "Task":
                case "Task DESC":
                    q = from t in q
                        orderby t.Description
                        select t;
                    break;
                case "About":
                case "About DESC":
                    q = from t in q
                        orderby t.AboutWho.Name2, t.Description
                        select t;
                    break;
                case "Assigned":
                case "Assigned DESC":
                    q = from t in q
                        orderby t.CoOwner.Name2, t.Owner.Name2
                        select t;
                    break;
                case "Due/Completed":
                case "Due DESC":
                default:
                    q = from t in q
                        orderby (t.StatusId == completedcode ? 3 : (t.StatusId == somedaycode ? 2 : 1)), t.CompletedOn ?? (t.Due ?? DateTime.MaxValue.Date) descending, t.Priority ?? 4, t.Description
                        select t;
                    break;
            }

            var q2 = from t in q
                     //let tListId = t.CoOwnerId == PeopleId ? t.CoListId.Value : t.ListId
                     select new TaskInfo
                     {
                         Id = t.Id,
                         Owner = t.Owner.Name,
                         OwnerId = t.OwnerId,
                         WhoId = t.WhoId,
                         //ListId = tListId,
                         //cListId = listid,
                         Description = t.Description,
                         SortDue = t.Due ?? DateTime.MaxValue.Date,
                         SortDueOrCompleted = t.CompletedOn ?? (t.Due ?? DateTime.MaxValue.Date),
                         CoOwner = t.CoOwner.Name,
                         CoOwnerId = t.CoOwnerId,
                         Status = t.TaskStatus.Description,
                         IsSelected = t.Id == intId,
                         Completed = t.StatusId == completedcode,
                         PrimarySort = t.StatusId == completedcode ? 3 : (t.StatusId == somedaycode ? 2 : 1),
                         SortPriority = t.Priority ?? 4,
                         IsCoOwner = t.CoOwnerId != null && t.CoOwnerId == PeopleId,
                         IsOwner = t.OwnerId == PeopleId,
                         CompletedOn = t.CompletedOn,
                         NotiPhone = !iPhone,
                     };
            return q2.Skip(StartRow).Take(PageSize.Value);
        }
        public TaskDetail FetchTask(int id)
        {
            var completedcode = TaskStatusCode.Complete;
            var somedaycode = TaskStatusCode.Someday;

            var iPhone = HttpContext.Current.Request.UserAgent.Contains("iPhone");
            var q2 = from t in DbUtil.Db.Tasks
                     where t.Id == id
                     //let tListId = t.CoOwnerId == PeopleId ? t.CoListId.Value : t.ListId
                     select new TaskDetail
                     {
                         Id = t.Id,
                         Owner = t.Owner.Name,
                         OwnerId = t.OwnerId,
                         OwnerEmail = t.Owner.EmailAddress,
                         WhoId = t.WhoId,
                         //ListId = tListId,
                         //cListId = CurListId,
                         Description = t.Description,
                         SortDue = t.Due ?? DateTime.MaxValue.Date,
                         SortDueOrCompleted = t.CompletedOn ?? (t.Due ?? DateTime.MaxValue.Date),
                         CoOwner = t.CoOwner.Name,
                         CoOwnerId = t.CoOwnerId,
                         CoOwnerEmail = t.CoOwner.EmailAddress,
                         Status = t.TaskStatus.Description,
                         StatusId = t.StatusId.Value,
                         IsSelected = t.Id == intId,
                         Location = t.Location,
                         Project = t.Project,
                         Completed = t.StatusId == completedcode,
                         PrimarySort = t.StatusId == completedcode ? 3 : (t.StatusId == somedaycode ? 2 : 1),
                         SortPriority = t.Priority ?? 4,
                         IsCoOwner = t.CoOwnerId != null && t.CoOwnerId == PeopleId,
                         IsOwner = t.OwnerId == PeopleId,
                         SourceContactId = t.SourceContactId,
                         SourceContact = t.SourceContact.ContactDate,
                         CompletedContactId = t.CompletedContactId,
                         CompletedContact = t.CompletedContact.ContactDate,
                         Notes = t.Notes,
                         CreatedOn = t.CreatedOn,
                         CompletedOn = t.CompletedOn,
                         NotiPhone = !iPhone,
                         ForceCompleteWContact = t.ForceCompleteWContact ?? false,
                     };
            var tt = q2.SingleOrDefault();
            if (tt == null)
                return null;
            return tt;
        }
        public class ContactTaskInfo
        {
            public int Id { get; set; }
            public int PeopleId { get; set; }
            public string Who { get; set; }
            public string Description { get; set; }
        }
        public IEnumerable<ContactTaskInfo> FetchContactTasks()
        {
            var completedcode = TaskStatusCode.Complete;
            var q = from t in DbUtil.Db.Tasks
                    // not archived
                    where t.Archive == false // not archived
                    // I am involved in
                    where t.OwnerId == PeopleId || t.CoOwnerId == PeopleId
                    // only contact related and not completed
                    where t.WhoId != null && t.StatusId != completedcode
                    // filter out any I own and have delegated
                    where !(t.OwnerId == PeopleId && t.CoOwnerId != null)
                    orderby t.CreatedOn
                    select new ContactTaskInfo
                    {
                        Id = t.Id,
                        PeopleId = t.AboutWho.PeopleId,
                        Who = t.AboutWho.Name,
                        Description = t.Description,
                    };
            return q;
        }

        public static void NotifyIfNeeded(StringBuilder sb, Task task)
        {
            if (sb.Length > 0 && task.CoOwnerId.HasValue)
            {
                var from = Util.UserPeopleId.Value == task.OwnerId ? task.Owner : task.CoOwner;
                var to = from.PeopleId == task.OwnerId ? task.CoOwner : task.Owner;
                var req = HttpContext.Current.Request;
                DbUtil.Db.Email(from.EmailAddress, to, 
                            "Task Updated by " + from.Name,
                            "{0} ({3})<br />\n{1}<br />\n{2}".Fmt(
                            TaskLink(task.Description, task.Id), task.AboutName, sb.ToString(), task.Priority));
            }
        }
        public class IncompleteTask
        {
            public string Desc { get; set; }
            public string About { get; set; }
			public int? AboutId { get; set; }
            public DateTime AssignedDt { get; set; }
            public string link { get; set; }
        }
        public IEnumerable<IncompleteTask> IncompleteTasksList(int pid)
        {
            var q2 = from t in DbUtil.Db.Tasks
                     where t.StatusId != TaskStatusCode.Complete
                     where t.CoOwnerId == pid
					 where t.WhoId != null
                     select new IncompleteTask
                     {
                         Desc = t.Description,
                         About = t.AboutName,
						 AboutId = t.WhoId,
                         AssignedDt = t.CreatedOn,
                         link = TaskLink0(t.Id)
                     };
            return q2;
        }
        public class TasksAbout
        {
            public string Desc { get; set; }
            public string AssignedTo { get; set; }
			public int? AssignedId { get; set; }
            public DateTime AssignedDt { get; set; }
            public string link { get; set; }
        }
        public IEnumerable<TasksAbout> TasksAboutList(int pid)
        {
            var q2 = from t in DbUtil.Db.Tasks
                     where t.StatusId != TaskStatusCode.Complete
                     where t.WhoId == pid
                     select new TasksAbout
                     {
                         Desc = t.Description,
                         AssignedTo = t.CoOwnerId != null? t.CoOwner.Name : t.Owner.Name,
                         AssignedDt = t.CreatedOn,
						 AssignedId = t.CoOwnerId,
                         link = TaskLink0(t.Id)
                     };
            return q2;
        }

        private static string TaskLink0(int id)
        {
            return "/Task/List/{0}#detail".Fmt(id);
        }
        private static string TaskLink(string text, int id)
        {
            return "<a href='{0}{1}'>{2}</a>".Fmt(DbUtil.Db.CmsHost, TaskLink0(id), text);
        }

        public static void ChangeTask(StringBuilder sb, Task task, string field, object value)
        {
            switch (field)
            {
                case "Due":
                    {
                        var dt = (DateTime?)value;
                        if (dt.HasValue)
                        {
                            if ((task.Due.HasValue && task.Due.Value != dt) || !task.Due.HasValue)
                                sb.AppendFormat("Due changed from {0:d} to {1:d}<br />\n", task.Due, dt);
                            task.Due = dt;
                        }
                        else
                        {
                            if (task.Due.HasValue)
                                sb.AppendFormat("Due changed from {0:d} to null<br />\n", task.Due);
                            task.Due = null;
                        }
                    }
                    break;
                case "Notes":
                    if (task.Notes != (string)value)
                        sb.AppendFormat("Notes changed: {{<br />\n{0}<br />}}<br />\n", Util.SafeFormat((string)value));
                    task.Notes = (string)value;
                    break;
                case "StatusId":
                    if (task.StatusId != (int)value)
                    {
                        sb.AppendFormat("Status changed from {0:g} to {1:g}<br />\n",
                            task.StatusId, value);
                        if ((int)value == TaskStatusCode.Complete)
                            task.CompletedOn = Util.Now;
                        else
                            task.CompletedOn = null;
                    }
                    task.StatusId = (int)value;
                    break;
                case "Description":
                    if (task.Description != (string)value)
                        sb.AppendFormat("Description changed from \"{0}\" to \"{1}\"<br />\n", task.Description, value);
                    task.Description = (string)value;
                    break;
                case "Project":
                    if (task.Project != (string)value)
                        sb.AppendFormat("Project changed from \"{0}\" to \"{1}\"<br />\n", task.Project, value);
                    task.Project = (string)value;
                    break;
                default:
                    throw new ArgumentException("Invalid field in ChangeTask", field);
            }
        }
        public string JsonStatusCodes()
        {
            var cv = new CodeValueModel();
            var sb = new StringBuilder("{");
            foreach (var c in cv.TaskStatusCodes())
            {
                if (sb.Length > 1)
                    sb.Append(",");
                sb.AppendFormat("'{0}':'{1}'", c.Value, c.Value);
            }
            sb.Append("}");
            return sb.ToString();
        }

        public void DeleteTask(int TaskId)
        {
            var task = DbUtil.Db.Tasks.SingleOrDefault(t => t.Id == TaskId);
            if (task == null)
                return;
            if (HttpContext.Current.User.IsInRole("Admin"))
            {
                DbUtil.Db.Tasks.DeleteOnSubmit(task);
                DbUtil.Db.SubmitChanges();
            }
            else if (task.OwnerId == PeopleId)
            {
                if (task.CoOwnerId != null)
                    DbUtil.Db.Email(task.Owner.EmailAddress, task.CoOwner,
                        "Task Deleted by " + task.Owner.Name,
                        task.Description + "<br/>\n" + task.AboutName);
                DbUtil.Db.Tasks.DeleteOnSubmit(task);
                DbUtil.Db.SubmitChanges();
            }
            else // I must be cowner, I can't delete
            {
                DbUtil.Db.Email(task.CoOwner.EmailAddress, task.Owner,
                    task.CoOwner.Name + " tried to delete task",
                    TaskLink(task.Description, task.Id) + "<br/>\n" + task.AboutName);
            }
        }
        private IQueryable<Task> ApplySearch()
        {
            int listid = CurListId;
            var q = DbUtil.Db.Tasks.Where(t => t.Archive == false && (t.CoOwnerId == PeopleId ? t.CoListId.Value : t.ListId) == listid);
            if (OwnerOnly == true) // I see only my own tasks or tasks I have been delegated
				q = q.Where(t => t.OwnerId == PeopleId || t.CoOwnerId == PeopleId || t.OrginatorId == PeopleId);
            else // I see my own tasks where I am owner or cowner plus other people's tasks where I share the list the task is in
                q = q.Where(t => t.OwnerId == PeopleId || t.CoOwnerId == PeopleId || t.OrginatorId == PeopleId
                    || t.TaskList.TaskListOwners.Any(tlo => tlo.PeopleId == PeopleId)
                    || t.CoTaskList.TaskListOwners.Any(tlo => tlo.PeopleId == PeopleId)
                    || t.CoTaskList.CreatedBy == PeopleId || t.TaskList.CreatedBy == PeopleId);

            if (StatusId != completedcode)
                q = q.Where(t => t.StatusId != completedcode);

            if (Project.HasValue())
                q = from t in q
                    where t.Project.Contains(Project)
                    select t;
            if (Location.HasValue())
                q = from t in q
                    where t.Location.Contains(Location)
                    select t;
            if (StatusId.HasValue)
                q = from t in q
                    where t.StatusId == StatusId
                    select t;
            return q;
        }
        private SelectListItem[] top = new SelectListItem[] 
        { 
            new SelectListItem { Value = "", Text = "(not specified)" } 
        };
        public IEnumerable<SelectListItem> Locations()
        {
            string[] a = { "@work", "@home", "@car", "@computer" };
            var q = from t in DbUtil.Db.Tasks
                    where t.OwnerId == PeopleId
                    where t.Location != ""
                    orderby t.Location
                    select t.Location;
            return top.Union(
                a.Union(q.Distinct()).Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(i => new SelectListItem { Text = i }));
        }
        public IEnumerable<SelectListItem> TaskStatusCodes()
        {
            var c = new CodeValueModel();
            return top.Union(c.TaskStatusCodes().Select(cv =>
                new SelectListItem { Text = cv.Value, Value = cv.Id.ToString() })); ;
        }
        private IQueryable<string> projects()
        {
            return from t in DbUtil.Db.Tasks
                   where t.Archive == false
                   where t.TaskList.TaskListOwners.Any(tlo => tlo.PeopleId == PeopleId) || t.TaskList.CreatedBy == PeopleId
                   where t.Project != ""
                   orderby t.Project
                   select t.Project;
        }
        public IEnumerable<SelectListItem> Projects()
        {
            var q = projects();
            return top.Union(q.Distinct().Select(p => new SelectListItem { Text = p }));
        }
        public IEnumerable<string> Projects(string startswith)
        {
            var q = projects().Where(p => p.StartsWith(startswith));
            return q.Distinct().ToList();
        }

        public int AddCompletedContact(int id)
        {
            var task = DbUtil.Db.Tasks.SingleOrDefault(t => t.Id == id);
            var c = new Contact { ContactDate = Util.Now.Date };
            c.CreatedDate = c.ContactDate;
            var min = DbUtil.Db.Ministries.SingleOrDefault(m => m.MinistryName == task.Project);
            if (min != null)
                c.MinistryId = min.MinistryId;
            c.contactees.Add(new Contactee { PeopleId = task.WhoId.Value });
            c.contactsMakers.Add(new Contactor { PeopleId = PeopleId });
            c.Comments = task.Notes;
            task.CompletedContact = c;
            task.StatusId = TaskStatusCode.Complete;
            if (task.CoOwnerId == PeopleId)
                DbUtil.Db.Email(task.CoOwner.EmailAddress, task.Owner,
                        "Task Completed with a Contact by " + task.CoOwner.Name,
                        TaskLink(task.Description, task.Id) + "<br />" + task.AboutName);
            else if (task.CoOwnerId != null)
                DbUtil.Db.Email(task.Owner.EmailAddress, task.CoOwner,
                    "Task Completed with a Contact by " + task.Owner.Name,
                    TaskLink(task.Description, task.Id) + "<br />" + task.AboutName);
            task.CompletedOn = c.ContactDate;
            DbUtil.Db.SubmitChanges();
            return c.ContactId;
        }

        public void AcceptTask(int id)
        {
            var task = DbUtil.Db.Tasks.SingleOrDefault(t => t.Id == id);
            task.StatusId = TaskStatusCode.Active;
            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(task.CoOwner.EmailAddress ,task.Owner, 
                "Task Accepted from " + task.CoOwner.Name,
                TaskLink(task.Description, task.Id) + "<br />" + task.AboutName);
        }
        public void AddSourceContact(int id, int contactid)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            task.SourceContact = DbUtil.Db.Contacts.SingleOrDefault(nc => nc.ContactId == contactid);
            DbUtil.Db.SubmitChanges();
        }
        public Task Delegate(int taskid, int toid)
        {
            if (toid == Util.UserPeopleId.Value)
                return null; // cannot delegate to self
            var task = DbUtil.Db.Tasks.SingleOrDefault(t => t.Id == taskid);
            if (task == null)
                return null;
            task.StatusId = TaskStatusCode.Pending;
            task.CoOwnerId = toid;

            // if the owner's list is shared by the coowner
            // then put it in owner's list
            // otherwise put it in the coowner's inbox
            if (task.TaskList.TaskListOwners.Any(tlo => tlo.PeopleId == toid) || task.TaskList.CreatedBy == toid)
                task.CoListId = task.ListId;
            else
                task.CoListId = InBoxId(toid);

            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(task.Owner.EmailAddress, DbUtil.Db.LoadPersonById(toid),
                "TASK: " + task.Description,
                TaskLink(task.Description, taskid) + "<br/>" + task.AboutName);
            return task;
        }
        public void ChangeOwner(int taskid, int toid)
        {
            if (toid == Util.UserPeopleId.Value)
                return; // nothing to do
            var task = DbUtil.Db.Tasks.Single(t => t.Id == taskid);

            // if the owner's list is shared by the coowner
            // then put it in owner's list
            // otherwise put it in the coowner's inbox
            var owner = task.Owner;
            var toowner = DbUtil.Db.LoadPersonById(toid);
            //if (task.TaskList.TaskListOwners.Any(tlo => tlo.PeopleId == toid) || task.TaskList.CreatedBy == toid)
            //    task.CoListId = task.ListId;
            //else
            //    task.ListId = InBoxId(toid);

            //task.CoOwnerId = task.OwnerId;
        	task.OrginatorId = task.OwnerId;
            task.Owner = toowner;

            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(owner.EmailAddress, toowner,
                "Task transferred from " + owner.Name,
                TaskLink(task.Description, taskid) + "<br/>" + task.AboutName);
        }

        public static void SetWhoId(int id, int pid)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            task.WhoId = pid;
            DbUtil.Db.SubmitChanges();
        }

        public void SetDescription(int id, string value)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            var sb = new StringBuilder();
            ChangeTask(sb, task, "Description", value);
            NotifyIfNeeded(sb, task);
            DbUtil.Db.SubmitChanges();
        }

        public void SetLocation(int id, string value)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            task.Location = value;
            DbUtil.Db.SubmitChanges();
        }

        public void SetStatus(int id, string value)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            var cvc = new CodeValueModel();
            var ts = cvc.TaskStatusCodes();
            var statusid = ts.Single(t => t.Value == value).Id;
            var sb = new StringBuilder();
            ChangeTask(sb, task, "StatusId", statusid);
            NotifyIfNeeded(sb, task);
            DbUtil.Db.SubmitChanges();
        }

        public void MoveTasksToList(IEnumerable<int> tids, int listid)
        {
            var mlist = DbUtil.Db.TaskLists.Single(tl => tl.Id == listid);
            var q = from t in DbUtil.Db.Tasks
                    where tids.Contains(t.Id)
                    // if I am the coowner
                    // and if this task is on a shared list
                    // and that list is the same as my owner's list
                    // then don't move it
                    where !(t.CoOwnerId == PeopleId && t.CoTaskList.TaskListOwners.Count() > 0 && t.CoListId == t.ListId)
                    select t;
            foreach (var t in q)
                if (t.OwnerId == PeopleId && (mlist.TaskListOwners.Any(tlo => tlo.PeopleId == t.CoOwnerId) || mlist.CreatedBy == t.CoOwnerId))
                {
                    mlist.CoTasks.Add(t);
                    mlist.Tasks.Add(t);
                }
                else if (t.CoOwnerId == PeopleId)
                    mlist.CoTasks.Add(t);
                else
                    mlist.Tasks.Add(t);
            DbUtil.Db.SubmitChanges();
        }
        public void DeleteList(string tab)
        {
            var Db = DbUtil.Db;
            var id = tab.Substring(1).ToInt();
            if (id <= 0)
                return;
            var list = Db.TaskLists.Single(tl => tl.Id == id);
            if (list.Name == STR_InBox) // can't delete inbox
                return;
            var inbox = Task.GetRequiredTaskList(STR_InBox, PeopleId);
            var q = Db.Tasks.Where(t => t.ListId == id);
            foreach (var t in q)
            {
                if (t.CoOwnerId.HasValue && t.CoListId == id)
                {
                    var cinbox = Task.GetRequiredTaskList(STR_InBox, t.CoOwnerId.Value);
                    cinbox.CoTasks.Add(t);
                }
                inbox.Tasks.Add(t);
            }
            Db.TaskLists.DeleteOnSubmit(list);
            Db.SubmitChanges();
        }

        public void AddList(string name)
        {
            var Db = DbUtil.Db;
            var txt = name.ToLower();
            if (string.Compare(txt, STR_InBox, true) == 0 || string.Compare(txt, "personal", true) == 0)
                return;
            if (Db.TaskLists.Count(t => t.Name == name && t.CreatedBy == PeopleId) > 0)
                return;
            var list = new TaskList { Name = name, CreatedBy = PeopleId };
            Db.TaskLists.InsertOnSubmit(list);
            Db.SubmitChanges();
        }

        public static int InBoxId(int pid)
        {
            return Task.GetRequiredTaskList(STR_InBox, pid).Id;
        }
        public int AddTask(int pid, int listid, string text)
        {
            if (listid <= 0)
                listid = InBoxId(pid);
            var task = new Task
            {
                ListId = listid,
                Description = text,
                OwnerId = pid,
                StatusId = TaskStatusCode.Active,
            };
            DbUtil.Db.Tasks.InsertOnSubmit(task);
            DbUtil.Db.SubmitChanges();
            return task.Id;
        }

        public void SetPriority(int p)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == intId);
            if (p == 0)
                task.Priority = null;
            else
                task.Priority = p;
            DbUtil.Db.SubmitChanges();
        }

        public void SetProject(int id, string value)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            var sb = new StringBuilder();
            ChangeTask(sb, task, "Project", value);
            NotifyIfNeeded(sb, task);
            DbUtil.Db.SubmitChanges();
        }

        public void DeleteTasks(IEnumerable<int> list)
        {
            foreach (var id in list)
                DeleteTask(id);
        }

        public void Priortize(IEnumerable<int> list, string p)
        {
            var q = from t in DbUtil.Db.Tasks
                    where list.Contains(t.Id)
                    select t;
            int? priority = p.Substring(1).ToInt();
            if (priority == 0)
                priority = null;
            foreach (var t in q)
                t.Priority = priority;
            DbUtil.Db.SubmitChanges();
        }

        public void CompleteTask(int id)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            var sb = new StringBuilder();
            var statusid = TaskStatusCode.Complete;
            ChangeTask(sb, task, "StatusId", statusid);
            NotifyIfNeeded(sb, task);
            DbUtil.Db.SubmitChanges();
        }

        public void ArchiveTask(int TaskId)
        {
            var task = DbUtil.Db.Tasks.SingleOrDefault(t => t.Id == TaskId);
            if (task == null)
                return;

            if (task.OwnerId == PeopleId)
            {
                if (task.CoOwnerId != null)
                    DbUtil.Db.Email(task.Owner.EmailAddress, task.CoOwner,
                        "Task Archived by " + task.Owner.Name,
                        task.Description + "<br/>\n" + task.AboutName);
                task.Archive = true;
                DbUtil.Db.SubmitChanges();
            }
            else // I must be cowner, I can't archive
            {
                DbUtil.Db.Email(task.CoOwner.EmailAddress, task.Owner,
                    task.CoOwner.Name + " tried to archive task",
                    TaskLink(task.Description, task.Id) + "<br/>\n" + task.AboutName);
            }
        }

        public void ArchiveTasks(IEnumerable<int> list)
        {
            foreach (var id in list)
                ArchiveTask(id);
        }
        public string MyListId()
        {
            var t = DbUtil.Db.Tasks.SingleOrDefault(k => k.Id == intId);
            if (t == null)
                return "t" + CurListId;
            if (t.CoOwnerId.HasValue && PeopleId == t.CoOwnerId.Value)
                return "t" + (t.CoListId ?? 1);
            else if (PeopleId == t.OwnerId)
                return "t" + t.ListId;
            else
                return "t" + InBoxId(PeopleId);
        }
        public PagerModel pagerModel()
        {
            return new PagerModel
            {
                Page = Page.Value,
                PageSize = PageSize.Value,
                Action = "List",
                Controller = "Task",
                Count = Count,
				ToggleTarget = false
            };
        }
    }
}
