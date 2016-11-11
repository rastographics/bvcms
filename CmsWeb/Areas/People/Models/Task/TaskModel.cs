using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using CmsData;
using CmsData.Classes.GoogleCloudMessaging;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Task
{
    public class TaskModel
    {
        internal Person Who;
        private DateTime? SortDue { get; set; }

        public string About => Who?.Name ?? "";

        public bool CanAccept => IsCoOwner &&
                                 ((TaskStatus.IntVal == TaskStatusCode.Pending) ||
                                  (TaskStatus.IntVal == TaskStatusCode.Declined));

        public bool CanComplete => IsAnOwner &&
                                   TaskStatus.IntVal != TaskStatusCode.Complete &&
                                   !ForceCompleteWithContact;

        public bool CanCompleteWithContact
            => IsAnOwner && (TaskStatus.IntVal != TaskStatusCode.Complete) && (WhoId != null);

        public string ContactUrl => $"/Contact2/{CompletedContactId}";

        public string FmtNotes
            => ViewExtensions2.Markdown(Notes?.Replace("{peopleid}", WhoId.ToString())).ToString();

        public bool HasNotes => Notes.HasValue();
        public bool IsAnOwner => IsOwner || IsCoOwner || Util.IsInRole("ManageTasks");
        public bool ShowCompleted => CompletedOn.HasValue;
        public string WhoAddrCityStateZip => Who?.AddrCityStateZip ?? "";
        public string WhoAddress => Who?.PrimaryAddress ?? "";
        public string WhoEmail => Who?.EmailAddress ?? "";
        public string WhoPhone => Who?.HomePhone?.FmtFone() ?? "";

        public bool Completed { get; set; }
        public DateTime? CompletedContact { get; set; }
        public int? CompletedContactId { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string CoOwner { get; set; }
        public string CoOwnerEmail { get; set; }
        public int? CoOwnerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DeclineReason { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public DateTime? Due
        {
            get { return SortDue.HasValue && (SortDue != DateTime.MaxValue.Date) ? SortDue : null; }
            set { SortDue = value; }
        }

        public bool ForceCompleteWithContact { get; set; }
        public int Id { get; set; }
        public bool IsCoOwner { get; set; }
        public bool IsOwner { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string Owner { get; set; }
        public string OwnerEmail { get; set; }
        public int OwnerId { get; set; }

        public int? Priority
        {
            get { return SortPriority == 4 ? null : (int?) SortPriority; }
            set { SortPriority = value ?? 4; }
        }

        public string Project { get; set; }
        public int SortPriority { get; set; }
        public string Status { get; set; }
        public CodeInfo TaskLimitToRole { get; set; }
        public CodeInfo TaskStatus { get; set; }

        public int? WhoId
        {
            get { return Who?.PeopleId; }
            set
            {
                if (value.HasValue)
                    Who = DbUtil.Db.LoadPersonById(value.Value);
            }
        }

        public static void AcceptTask(int id)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            task.StatusId = TaskStatusCode.Active;
            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task accepted by {Util.UserFullName}",
                CreateEmailBody(task));

            GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Accepted",
                $"{Util.UserFullName} accepted a task");
            if (Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
        }

        public static int AddTask(int pid, string text)
        {
            var task = new CmsData.Task
            {
                Description = text,
                OwnerId = pid,
                StatusId = TaskStatusCode.Active
            };
            DbUtil.Db.Tasks.InsertOnSubmit(task);
            DbUtil.Db.SubmitChanges();

            GCMHelper.sendRefresh(pid, GCMHelper.ACTION_REFRESH);

            return task.Id;
        }

        public static int AddCompletedContact(int id)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            var c = new Contact {ContactDate = Util.Now.Date};
            c.CreatedDate = c.ContactDate;

            var min = DbUtil.Db.Ministries.SingleOrDefault(m => m.MinistryName == task.Project);
            if (min != null)
                c.MinistryId = min.MinistryId;
            if (task.WhoId.HasValue)
                c.contactees.Add(new Contactee {PeopleId = task.WhoId.Value});
            if (Util.UserPeopleId.HasValue)
                c.contactsMakers.Add(new Contactor {PeopleId = Util.UserPeopleId.Value});
            c.Comments = task.Notes;
            task.CompletedContact = c;
            task.StatusId = TaskStatusCode.Complete;

            if (task.CoOwnerId == Util.UserPeopleId)
                DbUtil.Db.Email(task.CoOwner.EmailAddress, task.Owner,
                    $"Task completed with a Contact by {Util.UserFullName}", CreateEmailBody(task));
            else if (task.CoOwnerId != null)
                DbUtil.Db.Email(task.Owner.EmailAddress, task.CoOwner,
                    $"Task completed with a Contact by {Util.UserFullName}", CreateEmailBody(task));

            task.CompletedOn = c.ContactDate;

            DbUtil.Db.SubmitChanges();

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                    GCMHelper.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete",
                        $"{Util.UserFullName} completed a task they delegated to you");
            }
            else
            {
                GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete",
                    $"{Util.UserFullName} completed a task you delegated them");
            }

            if (Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);

            return c.ContactId;
        }

        public static void ChangeOwner(int taskid, int toid)
        {
            if (toid == Util.UserPeopleId)
                return; // nothing to do
            var task = DbUtil.Db.Tasks.Single(t => t.Id == taskid);

            var owner = task.Owner;
            var toowner = DbUtil.Db.LoadPersonById(toid);

            //task.CoOwnerId = task.OwnerId;
            task.OrginatorId = task.OwnerId;
            task.Owner = toowner;

            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(owner.EmailAddress, toowner, $"Task transferred to you from {owner.Name}",
                CreateEmailBody(task));

            GCMHelper.sendNotification(toid, GCMHelper.TYPE_TASK, task.Id, "Task Transferred",
                $"{Util.UserFullName} has transferred a task to you");
            if(Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.ACTION_REFRESH);

            if (task.CoOwner != null)
                GCMHelper.sendRefresh(task.CoOwner.PeopleId, GCMHelper.ACTION_REFRESH);
        }

        public static void ChangeTask(StringBuilder sb, CmsData.Task task, string field, object value)
        {
            switch (field)
            {
                case "Due":
                {
                    var dt = (DateTime?) value;
                    if (dt.HasValue)
                    {
                        if ((task.Due.HasValue && (task.Due.Value != dt)) || !task.Due.HasValue)
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
                    if (task.Notes != (string) value)
                        sb.AppendFormat("Notes changed: {{<br />\n{0}<br />}}<br />\n", Util.SafeFormat((string) value));
                    task.Notes = (string) value;
                    break;
                case "StatusId":
                    if (task.StatusId != (int) value)
                    {
                        var dict = DbUtil.Db.TaskStatuses.AsEnumerable().ToDictionary(ts => ts.Id, ts => ts.Description);
                        sb.AppendFormat("Task Status changed from {0} to {1}<br />\n",
                            dict[task.StatusId ?? 10], dict[(int) value]);
                        if ((int) value == TaskStatusCode.Complete)
                            task.CompletedOn = Util.Now;
                        else
                            task.CompletedOn = null;
                    }
                    task.StatusId = (int) value;
                    break;
                case "Description":
                    if (task.Description != (string) value)
                        sb.AppendFormat("Description changed from \"{0}\" to \"{1}\"<br />\n", task.Description, value);
                    task.Description = (string) value;
                    break;
                case "LimitToRole":
                    if (task.LimitToRole != (string) value)
                        sb.AppendFormat("LimitToRole changed from \"{0}\" to \"{1}\"<br />\n", task.LimitToRole, value);
                    task.LimitToRole = (string) value;
                    break;
                case "Project":
                    if (task.Project != (string) value)
                        sb.AppendFormat("Project changed from \"{0}\" to \"{1}\"<br />\n", task.Project, value);
                    task.Project = (string) value;
                    break;
                default:
                    throw new ArgumentException("Invalid field in ChangeTask", field);
            }
        }

        public static void CompleteTask(int id)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            var sb = new StringBuilder();
            var statusid = TaskStatusCode.Complete;
            ChangeTask(sb, task, "StatusId", statusid);
            NotifyIfNeeded(sb, task);
            DbUtil.Db.SubmitChanges();

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                    GCMHelper.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete",
                        $"{Util.UserFullName} completed a task they delegated to you");
            }
            else
            {
                GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete",
                    $"{Util.UserFullName} completed a task you delegated them");
            }

            if (Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
        }

        public static void DeclineTask(int id, string reason)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            task.StatusId = TaskStatusCode.Declined;
            task.DeclineReason = reason;

            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task declined by {Util.UserFullName}",
                CreateEmailBody(task));

            GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Declined",
                $"{Util.UserFullName} declined a task");
            if(Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
        }

        public static void Delegate(int taskid, int toid, bool notify = true, bool forceCompleteWithContact = false)
        {
            if (toid == Util.UserPeopleId)
                return; // cannot delegate to self

            var task = DbUtil.Db.Tasks.SingleOrDefault(t => t.Id == taskid);

            if (task == null)
                return;

            var previousDelegatee = task.CoOwnerId ?? 0;

            task.StatusId = TaskStatusCode.Pending;
            task.CoOwnerId = toid;

            if (forceCompleteWithContact)
                task.ForceCompleteWContact = true;

            var toPerson = DbUtil.Db.LoadPersonById(toid);

            DbUtil.Db.SubmitChanges();
            DbUtil.Db.Email(task.Owner.EmailAddress, toPerson, $"Task delegated to you by {Util.UserFullName}",
                CreateEmailBody(task));

            if (notify)
            {
                if (previousDelegatee == 0) // No previous delegatee
                {
                    GCMHelper.sendNotification(toPerson.PeopleId, GCMHelper.TYPE_TASK, taskid, "Task Delegated",
                        $"{Util.UserFullName} delegated you a task");
                    GCMHelper.sendRefresh(task.Owner.PeopleId, GCMHelper.TYPE_TASK);
                }
                else // Had a previous delegatee
                {
                    if (previousDelegatee == Util.UserPeopleId) // Delegatee redelegating
                    {
                        GCMHelper.sendRefresh(previousDelegatee, GCMHelper.TYPE_TASK);
                        GCMHelper.sendNotification(toPerson.PeopleId, GCMHelper.TYPE_TASK, taskid, "Task Delegated",
                            $"{Util.UserFullName} has delegated a task to you");
                        GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, taskid, "Task Redelegated",
                            $"{Util.UserFullName} has redelegated a task you delegated to them");
                    }
                    else // Owner, with previous delegatee
                    {
                        GCMHelper.sendRefresh(task.Owner.PeopleId, GCMHelper.TYPE_TASK);
                        GCMHelper.sendNotification(toPerson.PeopleId, GCMHelper.TYPE_TASK, taskid, "Task Delegated",
                            $"{Util.UserFullName} delegated you a task");
                        GCMHelper.sendNotification(previousDelegatee, GCMHelper.TYPE_TASK, 0, "Task Redelegated",
                            $"{Util.UserFullName} has redelegated a task to someone else");
                    }
                }
            }
        }

        public void DelegateAll(int[] tasks, int peopleId)
        {
            if (!Util.UserPeopleId.HasValue)
                return;
            var owners = (from o in DbUtil.Db.Tasks
                where tasks.Contains(o.Id)
                select o.OwnerId).Distinct().ToList();

            var delegates = (from o in DbUtil.Db.Tasks
                where tasks.Contains(o.Id)
                where o.CoOwnerId != null
                select o.CoOwnerId ?? 0).Distinct().ToList();

            foreach (var tid in tasks)
                Delegate(tid, peopleId, false, true);

            owners.Remove(Util.UserPeopleId.Value);
            owners.Remove(peopleId);
            delegates.Remove(Util.UserPeopleId.Value);
            delegates.Remove(peopleId);

            string taskString = tasks.Count() > 1 ? "tasks" : "a task";

            GCMHelper.sendNotification(owners, GCMHelper.TYPE_TASK, 0, "Tasks Redelegated",
                $"{Util.UserFullName} has redelegated {taskString} you own");
            GCMHelper.sendNotification(delegates, GCMHelper.TYPE_TASK, 0, "Tasks Redelegated",
                $"{Util.UserFullName} has redelegated {taskString} to someone else");
            GCMHelper.sendNotification(peopleId, GCMHelper.TYPE_TASK, 0, "Task Delegated",
                $"{Util.UserFullName} delegated you {taskString}");
            GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.ACTION_REFRESH);

            DbUtil.Db.SubmitChanges();
        }

        public static TaskModel FetchModel(int id)
        {
            var q = from t in DbUtil.Db.Tasks
                where t.Id == id
                select new TaskModel
                {
                    Id = id,
                    OwnerId = t.OwnerId,
                    WhoId = t.WhoId,
                    Description = t.Description,
                    CoOwnerId = t.CoOwnerId,
                    Location = t.Location,
                    Project = t.Project,
                    CompletedContactId = t.CompletedContactId,
                    Notes = t.Notes,
                    CreatedOn = t.CreatedOn,
                    CompletedOn = t.CompletedOn,
                    DeclineReason = t.DeclineReason,
                    Owner = t.Owner.Name,
                    OwnerEmail = t.Owner.EmailAddress,
                    SortDue = t.Due ?? DateTime.MaxValue.Date,
                    CoOwner = t.CoOwner.Name,
                    CoOwnerEmail = t.CoOwner.EmailAddress,
                    TaskLimitToRole = new CodeInfo(t.LimitToRole, "TaskLimitToRole"),
                    Status = t.TaskStatus.Description,
                    TaskStatus = new CodeInfo(t.StatusId, "TaskStatus"),
                    Completed = t.StatusId == TaskStatusCode.Complete,
                    SortPriority = t.Priority ?? 4,
                    IsCoOwner = (t.CoOwnerId != null) && (t.CoOwnerId == Util.UserPeopleId),
                    IsOwner = t.OwnerId == Util.UserPeopleId,
                    CompletedContact = t.CompletedContact.ContactDate,
                    ForceCompleteWithContact = t.ForceCompleteWContact ?? false
                };
            return q.Single();
        }

        public static void NotifyIfNeeded(StringBuilder sb, CmsData.Task task)
        {
            if ((sb.Length <= 0) || !task.CoOwnerId.HasValue) return;

            var from = Util.UserPeopleId.Value == task.OwnerId ? task.Owner : task.CoOwner;
            var to = from.PeopleId == task.OwnerId ? task.CoOwner : task.Owner;
            var req = HttpContext.Current.Request;

            DbUtil.Db.Email(from.EmailAddress, to, $"Task updated by {Util.UserFullName}", CreateEmailBody(task));
        }

        public string ProspectReportLink()
        {
            if (!WhoId.HasValue)
                return null;
            Util2.CurrentPeopleId = WhoId.Value;
            HttpContext.Current.Session["ActivePerson"] = About;
            var qb = DbUtil.Db.QueryIsCurrentPerson();
            return $"/Reports/Prospect/{qb.QueryId}?form=true";
        }

        public static void SetWhoId(int id, int pid)
        {
            var task = DbUtil.Db.Tasks.Single(t => t.Id == id);
            task.WhoId = pid;
            DbUtil.Db.SubmitChanges();

            if (task.CoOwner != null)
                GCMHelper.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Tasks About Changed",
                    $"{Util.UserFullName} has change the about person on a task delegated to you");
            if(Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.ACTION_REFRESH);
        }

        public void UpdateTask()
        {
            var sb = new StringBuilder();
            var task = DbUtil.Db.Tasks.Single(t => t.Id == Id);
            ChangeTask(sb, task, "Description", Description);
            ChangeTask(sb, task, "LimitToRole", TaskLimitToRole.Value);
            ChangeTask(sb, task, "Due", Due);
            ChangeTask(sb, task, "Notes", Notes);
            ChangeTask(sb, task, "StatusId", TaskStatus.IntVal);
            task.ForceCompleteWContact = ForceCompleteWithContact;
            if (HttpContext.Current.User.IsInRole("AdvancedTask"))
                ChangeTask(sb, task, "Project", Project);

            task.Location = Location;
            if (Priority == 0)
                task.Priority = null;
            else
                task.Priority = Priority;
            DbUtil.Db.SubmitChanges();
            NotifyIfNeeded(sb, task);

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                    GCMHelper.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated",
                        $"{Util.UserFullName} updated a task delegated to you");
            }
            else
            {
                GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated",
                    $"{Util.UserFullName} updated a task you own");
            }

            if (Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
        }

        private static string CreateEmailBody(CmsData.Task task)
        {
            var body = new StringBuilder();

            body.Append($"Task: {TaskLink(task.Description, task.Id)}<br/>\n");
            body.Append($"Created: {task.CreatedOn.FormatDateTm()}<br/>\n");

            if (task.Due != null)
                body.Append($"Due: {task.Due.FormatDate()}<br/>\n");

            if (task.StatusId == TaskStatusCode.Declined)
                body.Append($"Status: {task.TaskStatus.Description} - {task.DeclineReason}<br/>\n");
            else
                body.Append($"Status: {task.TaskStatus.Description}<br/>\n");

            body.Append($"About: {PeopleLink(task.AboutWho.Name, task.AboutWho.PeopleId)}<br/>\n");
            body.Append($"Owner: {PeopleLink(task.Owner.Name, task.Owner.PeopleId)}<br/>\n");

            if (task.CoOwnerId != null)
                body.Append($"Delegated To: {PeopleLink(task.CoOwner.Name, task.CoOwner.PeopleId)}<br/>\n");


            body.Append($"Notes:<br/>\n{PythonModel.Markdown(task.Notes)}");

            return body.ToString();
        }

        private static string PeopleLink(string text, int? id)
            => $"<a href='{DbUtil.Db.ServerLink("/Person2/" + id)}'>{text}</a>";

        private static string TaskLink(string text, int id)
            => $"<a href='{DbUtil.Db.ServerLink($"/Task/{id}")}'>{text}</a>";
    }
}