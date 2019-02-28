using CmsData;
using CmsData.Classes.GoogleCloudMessaging;
using CmsData.Codes;
using CmsWeb.Code;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Task
{
    public class TaskModel
    {
        private readonly string _host;
        private readonly CMSDataContext _cmsDataContext;

        public TaskModel()
        {
            _host = Util.Host;
            _cmsDataContext = DbUtil.Create(_host);

        }
        public TaskModel(string host, CMSDataContext db)
        {
            _host = host;
            _cmsDataContext = db;
        }
         

        internal Person Who;
        private DateTime? SortDue { get; set; }
        
        public string About
        {
            get { return Who?.Name ?? ""; }
        }

        public bool CanAccept
        {
            get
            {
                return IsCoOwner && (TaskStatus.IntVal == TaskStatusCode.Pending || TaskStatus.IntVal == TaskStatusCode.Declined);
            }
        }

        public bool CanComplete
        {
            get
            {
                return IsAnOwner && TaskStatus.IntVal != TaskStatusCode.Complete && !ForceCompleteWithContact;
            }
        }

        public bool CanCompleteWithContact
        {
            get { return IsAnOwner && TaskStatus.IntVal != TaskStatusCode.Complete && WhoId != null; }
        }

        public string ContactUrl
        {
            get { return $"/Contact2/{CompletedContactId}"; }
        }

        public string FmtNotes
        {
            get { return ViewExtensions2.Markdown(Notes?.Replace("{peopleid}", WhoId.ToString())).ToString(); }
        }

        public bool HasNotes
        {
            get { return Notes.HasValue(); }
        }

        public bool IsAnOwner
        {
            get { return IsOwner || IsCoOwner || Util.IsInRole("ManageTasks"); }
        }

        public bool ShowCompleted
        {
            get { return CompletedOn.HasValue; }
        }

        public string WhoAddrCityStateZip
        {
            get { return Who?.AddrCityStateZip ?? ""; }
        }

        public string WhoAddress
        {
            get { return Who?.PrimaryAddress ?? ""; }
        }

        public string WhoEmail
        {
            get { return Who?.EmailAddress ?? ""; }
        }

        public string WhoPhone
        {
            get { return Who?.HomePhone?.FmtFone() ?? ""; }
        }

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
            get { return SortDue.HasValue && SortDue != DateTime.MaxValue.Date ? SortDue : null; }
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
            get { return SortPriority == 4 ? null : (int?)SortPriority; }
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
                {
                    Who = _cmsDataContext.LoadPersonById(value.Value);
                }
            }
        }

        public static void AcceptTask(int id, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);

            var task = db.Tasks.Single(t => t.Id == id);
            task.StatusId = TaskStatusCode.Active;

            db.SubmitChanges();
            db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task accepted by {Util.UserFullName}", CreateEmailBody(task, host, db));

            gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Accepted", $"{Util.UserFullName} accepted a task");

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
            }
        }

        public static bool AcceptTask(User user, int id, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.SingleOrDefault(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            task.StatusId = TaskStatusCode.Active;

            db.SubmitChanges();
            db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task accepted by {user.Person.Name}", CreateEmailBody(task, host, db));

            gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Accepted", $"{user.Person.Name} accepted a task");

            if (user.PeopleId.HasValue)
            {
                gcm.sendRefresh(user.PeopleId.Value, GCMHelper.TYPE_TASK);
            }

            return true;
        }

        public static int AddTask(int pid, string text, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = new CmsData.Task
            {
                Description = text,
                OwnerId = pid,
                StatusId = TaskStatusCode.Active
            };

            db.Tasks.InsertOnSubmit(task);
            db.SubmitChanges();

            gcm.sendRefresh(pid, GCMHelper.ACTION_REFRESH);

            return task.Id;
        }

        public static int AddCompletedContact(int taskId, User user, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.Single(t => t.Id == taskId);

            var c = new Contact { CreatedDate = DateTime.Now, ContactDate = DateTime.Now };

            var min = db.Ministries.SingleOrDefault(m => m.MinistryName == task.Project);

            if (min != null)
            {
                c.MinistryId = min.MinistryId;
            }

            if (task.WhoId.HasValue)
            {
                c.contactees.Add(new Contactee { PeopleId = task.WhoId.Value });
            }

            if (user.PeopleId.HasValue)
            {
                c.contactsMakers.Add(new Contactor { PeopleId = user.PeopleId.Value });
            }

            c.Comments = task.Notes;
            task.CompletedContact = c;
            task.StatusId = TaskStatusCode.Complete;

            if (task.CoOwnerId == user.PeopleId)
            {
                db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task completed with a Contact by {user.Name}", CreateEmailBody(task, host, db));
            }
            else if (task.CoOwnerId != null)
            {
                db.Email(task.Owner.EmailAddress, task.CoOwner, $"Task completed with a Contact by {user.Name}", CreateEmailBody(task, host, db));
            }

            task.CompletedOn = c.ContactDate;

            db.SubmitChanges();

            if (task.Owner.PeopleId == user.PeopleId)
            {
                if (task.CoOwner != null)
                {
                    gcm.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{user.Name} completed a task they delegated to you");
                }
            }
            else
            {
                gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{user.Name} completed a task you delegated them");
            }

            if (user.PeopleId.HasValue)
            {
                gcm.sendRefresh(user.PeopleId.Value, GCMHelper.TYPE_TASK);
            }

            return c.ContactId;
        }

        public static int AddCompletedContact(int id, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.Single(t => t.Id == id);
            var c = new Contact { ContactDate = Util.Now.Date };
            c.CreatedDate = c.ContactDate;

            var min = db.Ministries.SingleOrDefault(m => m.MinistryName == task.Project);
            if (min != null)
            {
                c.MinistryId = min.MinistryId;
            }

            if (task.WhoId.HasValue)
            {
                c.contactees.Add(new Contactee { PeopleId = task.WhoId.Value });
            }

            if (Util.UserPeopleId.HasValue)
            {
                c.contactsMakers.Add(new Contactor { PeopleId = Util.UserPeopleId.Value });
            }

            c.Comments = task.Notes;
            task.CompletedContact = c;
            task.StatusId = TaskStatusCode.Complete;

            if (task.CoOwnerId == Util.UserPeopleId)
            {
                db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task completed with a Contact by {Util.UserFullName}", CreateEmailBody(task, host, db));
            }
            else if (task.CoOwnerId != null)
            {
                db.Email(task.Owner.EmailAddress, task.CoOwner, $"Task completed with a Contact by {Util.UserFullName}", CreateEmailBody(task, host, db));
            }

            task.CompletedOn = c.ContactDate;

            db.SubmitChanges();

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                {
                    gcm.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{Util.UserFullName} completed a task they delegated to you");
                }
            }
            else
            {
                gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{Util.UserFullName} completed a task you delegated them");
            }

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
            }

            return c.ContactId;
        }

        public static void ChangeOwner(int taskId, int toId, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            if (toId == Util.UserPeopleId)
            {
                return; // nothing to do
            }

            var task = db.Tasks.Single(t => t.Id == taskId);

            var owner = task.Owner;
            var toOwner = db.LoadPersonById(toId);

            //task.CoOwnerId = task.OwnerId;
            task.OrginatorId = task.OwnerId;
            task.Owner = toOwner;

            db.SubmitChanges();
            db.Email(owner.EmailAddress, toOwner, $"Task transferred to you from {owner.Name}", CreateEmailBody(task, host, db));

            gcm.sendNotification(toId, GCMHelper.TYPE_TASK, task.Id, "Task Transferred", $"{Util.UserFullName} has transferred a task to you");

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.ACTION_REFRESH);
            }

            if (task.CoOwner != null)
            {
                gcm.sendRefresh(task.CoOwner.PeopleId, GCMHelper.ACTION_REFRESH);
            }
        }

        public static void ChangeTask(StringBuilder sb, CmsData.Task task, string field, object value, string host, CMSDataContext db)
        {
            switch (field)
            {
                case "Due":
                    {
                        var dt = (DateTime?)value;
                        if (dt.HasValue)
                        {
                            if (task.Due.HasValue && task.Due.Value != dt || !task.Due.HasValue)
                            {
                                sb.AppendFormat("Due changed from {0:d} to {1:d}<br />\n", task.Due, dt);
                            }

                            task.Due = dt;
                        }
                        else
                        {
                            if (task.Due.HasValue)
                            {
                                sb.AppendFormat("Due changed from {0:d} to null<br />\n", task.Due);
                            }

                            task.Due = null;
                        }
                    }
                    break;
                case "Notes":
                    if (task.Notes != (string)value)
                    {
                        sb.AppendFormat("Notes changed: {{<br />\n{0}<br />}}<br />\n",
                            Util.SafeFormat((string)value));
                    }

                    task.Notes = (string)value;
                    break;
                case "StatusId":
                    if (task.StatusId != (int)value)
                    {
                        var dict = db.TaskStatuses.AsEnumerable().ToDictionary(ts => ts.Id, ts => ts.Description);
                        sb.AppendFormat("Task Status changed from {0} to {1}<br />\n", dict[task.StatusId ?? 10], dict[(int)value]);
                        if ((int)value == TaskStatusCode.Complete)
                        {
                            task.CompletedOn = Util.Now;
                        }
                        else
                        {
                            task.CompletedOn = null;
                        }
                    }

                    task.StatusId = (int)value;
                    break;
                case "Description":
                    if (task.Description != (string)value)
                    {
                        sb.AppendFormat("Description changed from \"{0}\" to \"{1}\"<br />\n", task.Description, value);
                    }

                    task.Description = (string)value;
                    break;
                case "LimitToRole":
                    if (task.LimitToRole != (string)value)
                    {
                        sb.AppendFormat("LimitToRole changed from \"{0}\" to \"{1}\"<br />\n", task.LimitToRole, value);
                    }

                    task.LimitToRole = (string)value;
                    break;
                case "Project":
                    if (task.Project != (string)value)
                    {
                        sb.AppendFormat("Project changed from \"{0}\" to \"{1}\"<br />\n", task.Project, value);
                    }

                    task.Project = (string)value;
                    break;
                default:
                    throw new ArgumentException("Invalid field in ChangeTask", field);
            }
        }

        public static void CompleteTask(int id, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.Single(t => t.Id == id);
            var sb = new StringBuilder();
            var statusId = TaskStatusCode.Complete;
            ChangeTask(sb, task, "StatusId", statusId, host, db);
            NotifyIfNeeded(sb, task, host, db);
            db.SubmitChanges();

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                {
                    gcm.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{Util.UserFullName} completed a task they delegated to you");
                }
            }
            else
            {
                gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{Util.UserFullName} completed a task you delegated them");
            }

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
            }
        }

        public static bool CompleteTask(User user, int id, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.SingleOrDefault(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            var sb = new StringBuilder();

            ChangeTask(sb, task, "StatusId", TaskStatusCode.Complete, host, db);

            NotifyIfNeeded(user, task, sb, host, db);

            db.SubmitChanges();

            if (task.Owner.PeopleId == user.PeopleId)
            {
                if (task.CoOwner != null)
                {
                    gcm.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{user.Person.Name} completed a task they delegated to you");
                }
            }
            else
            {
                gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Complete", $"{user.Person.Name} completed a task you delegated them");
            }

            if (user.PeopleId.HasValue)
            {
                gcm.sendRefresh(user.PeopleId.Value, GCMHelper.TYPE_TASK);
            }

            return true;
        }

        public static void DeclineTask(int id, string reason, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.Single(t => t.Id == id);
            task.StatusId = TaskStatusCode.Declined;
            task.DeclineReason = reason;

            db.SubmitChanges();
            db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task declined by {Util.UserFullName}", CreateEmailBody(task, host, db));

            gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Declined", $"{Util.UserFullName} declined a task");

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
            }
        }

        public static bool DeclineTask(User user, int id, string reason, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.SingleOrDefault(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            task.StatusId = TaskStatusCode.Declined;
            task.DeclineReason = reason;

            db.SubmitChanges();
            db.Email(task.CoOwner.EmailAddress, task.Owner, $"Task declined by {user.Person.Name}", CreateEmailBody(task, host, db));

            gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Declined", $"{user.Person.Name} declined a task");

            if (user.PeopleId.HasValue)
            {
                gcm.sendRefresh(user.PeopleId.Value, GCMHelper.TYPE_TASK);
            }

            return true;
        }

        public static void Delegate(int taskId, int toId, string host, CMSDataContext db, bool notify = true, bool forceCompleteWithContact = false)
        {
            if (toId == Util.UserPeopleId)
            {
                return; // cannot delegate to self
            }

            var gcm = new GCMHelper(host, db);

            var task = db.Tasks.SingleOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                return;
            }

            var previousDelegatee = task.CoOwnerId ?? 0;

            task.StatusId = TaskStatusCode.Pending;
            task.CoOwnerId = toId;

            if (forceCompleteWithContact)
            {
                task.ForceCompleteWContact = true;
            }

            var toPerson = db.LoadPersonById(toId);

            db.SubmitChanges();
            db.Email(task.Owner.EmailAddress, toPerson, $"Task delegated to you by {Util.UserFullName}", CreateEmailBody(task, host, db));

            if (!notify)
            {
                return;
            }

            if (previousDelegatee == 0) // No previous delegatee
            {
                gcm.sendNotification(toPerson.PeopleId, GCMHelper.TYPE_TASK, taskId, "Task Delegated", $"{Util.UserFullName} delegated you a task");
                gcm.sendRefresh(task.Owner.PeopleId, GCMHelper.TYPE_TASK);
            }
            else // Had a previous delegatee
            {
                if (previousDelegatee == Util.UserPeopleId) // Delegatee redelegating
                {
                    gcm.sendRefresh(previousDelegatee, GCMHelper.TYPE_TASK);
                    gcm.sendNotification(toPerson.PeopleId, GCMHelper.TYPE_TASK, taskId, "Task Delegated", $"{Util.UserFullName} has delegated a task to you");
                    gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, taskId, "Task Redelegated", $"{Util.UserFullName} has redelegated a task you delegated to them");
                }
                else // Owner, with previous delegatee
                {
                    gcm.sendRefresh(task.Owner.PeopleId, GCMHelper.TYPE_TASK);
                    gcm.sendNotification(toPerson.PeopleId, GCMHelper.TYPE_TASK, taskId, "Task Delegated", $"{Util.UserFullName} delegated you a task");
                    gcm.sendNotification(previousDelegatee, GCMHelper.TYPE_TASK, 0, "Task Redelegated", $"{Util.UserFullName} has redelegated a task to someone else");
                }
            }
        }

        

        public static TaskModel FetchModel(int id, string host, CMSDataContext db)
        {
            var q = from t in db.Tasks
                    where t.Id == id
                    select new TaskModel(host, db)
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
                        IsCoOwner = t.CoOwnerId != null && t.CoOwnerId == Util.UserPeopleId,
                        IsOwner = t.OwnerId == Util.UserPeopleId,
                        CompletedContact = t.CompletedContact.ContactDate,
                        ForceCompleteWithContact = t.ForceCompleteWContact ?? false
                    };
            return q.Single();
        }

        public static void NotifyIfNeeded(StringBuilder sb, CmsData.Task task, string host, CMSDataContext db)
        {
            if (sb.Length <= 0 || !task.CoOwnerId.HasValue)
            {
                return;
            }

            var from = Util.UserPeopleId.Value == task.OwnerId ? task.Owner : task.CoOwner;
            var to = from.PeopleId == task.OwnerId ? task.CoOwner : task.Owner;

            db.Email(from.EmailAddress, to, $"Task updated by {Util.UserFullName}", CreateEmailBody(task, host, db));
        }

        public static void NotifyIfNeeded(User user, CmsData.Task task, StringBuilder sb, string host, CMSDataContext db)
        {
            if (sb.Length <= 0 || !task.CoOwnerId.HasValue)
            {
                return;
            }

            var from = user.PeopleId == task.OwnerId ? task.Owner : task.CoOwner;
            var to = from.PeopleId == task.OwnerId ? task.CoOwner : task.Owner;

            db.Email(from.EmailAddress, to, $"Task updated by {user.Person.Name}", CreateEmailBody(task, host, db));
        }

        public string ProspectReportLink()
        {
            if (!WhoId.HasValue)
            {
                return null;
            }

            Util2.CurrentPeopleId = WhoId.Value;
            HttpContextFactory.Current.Session["ActivePerson"] = About;
            var qb = _cmsDataContext.QueryIsCurrentPerson();
            return $"/Reports/Prospect/{qb.QueryId}?form=true";
        }

        public static void SetWhoId(int id, int pid, string host, CMSDataContext db)
        {
            var gcm = new GCMHelper(host, db);
            var task = db.Tasks.Single(t => t.Id == id);
            task.WhoId = pid;
            db.SubmitChanges();

            if (task.CoOwner != null)
            {
                gcm.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Tasks About Changed", $"{Util.UserFullName} has change the about person on a task delegated to you");
            }

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.ACTION_REFRESH);
            }
        }

        public void UpdateTask()
        {
            var gcm = new GCMHelper(_host, _cmsDataContext);
            var sb = new StringBuilder();
            var task = _cmsDataContext.Tasks.Single(t => t.Id == Id);
            ChangeTask(sb, task, "Description", Description, _host, _cmsDataContext);
            ChangeTask(sb, task, "LimitToRole", TaskLimitToRole.Value == "0" ? null : TaskLimitToRole.Value, _host, _cmsDataContext);
            ChangeTask(sb, task, "Due", Due, _host, _cmsDataContext);
            ChangeTask(sb, task, "Notes", Notes, _host, _cmsDataContext);
            ChangeTask(sb, task, "StatusId", TaskStatus.IntVal, _host, _cmsDataContext);
            task.ForceCompleteWContact = ForceCompleteWithContact;

            if (HttpContextFactory.Current.User.IsInRole("AdvancedTask"))
            {
                ChangeTask(sb, task, "Project", Project, _host, _cmsDataContext);
            }

            task.Location = Location;

            task.Priority = Priority == 0 ? null : Priority;

            _cmsDataContext.SubmitChanges();
            NotifyIfNeeded(sb, task, _host, _cmsDataContext);

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                {
                    gcm.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated", $"{Util.UserFullName} updated a task delegated to you");
                }
            }
            else
            {
                gcm.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated", $"{Util.UserFullName} updated a task you own");
            }

            if (Util.UserPeopleId.HasValue)
            {
                gcm.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
            }
        }

        public static string CreateEmailBody(CmsData.Task task, string host, CMSDataContext db)
        {
            var body = new StringBuilder();

            body.Append($"Task: {TaskLink(task.Description, task.Id, host, db)}<br/>\n");
            body.Append($"Created: {task.CreatedOn.FormatDateTm()}<br/>\n");

            if (task.Due != null)
            {
                body.Append($"Due: {task.Due.FormatDate()}<br/>\n");
            }

            body.Append(task.StatusId == TaskStatusCode.Declined
                ? $"Status: {task.TaskStatus.Description} - {task.DeclineReason}<br/>\n"
                : $"Status: {task.TaskStatus.Description}<br/>\n");

            body.Append($"About: {PeopleLink(task.AboutWho.Name, task.AboutWho.PeopleId, host, db)}<br/>\n");
            body.Append($"Owner: {PeopleLink(task.Owner.Name, task.Owner.PeopleId, host, db)}<br/>\n");

            if (task.CoOwnerId != null)
            {
                body.Append($"Delegated To: {PeopleLink(task.CoOwner.Name, task.CoOwner.PeopleId, host, db)}<br/>\n");
            }

            body.Append($"Notes:<br/>\n{PythonModel.Markdown(task.Notes)}");

            return body.ToString();
        }

        private static string PeopleLink(string text, int? id, string host, CMSDataContext db)
        {
            return $"<a href='{db.ServerLink("/Person2/" + id)}'>{text}</a>";
        }

        private static string TaskLink(string text, int id, string host, CMSDataContext db)
        {
            return $"<a href='{db.ServerLink($"/Task/{id}")}'>{text}</a>";
        }
    }
}
