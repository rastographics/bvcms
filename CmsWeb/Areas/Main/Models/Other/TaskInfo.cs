/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;
using CmsData.Classes.GoogleCloudMessaging;

namespace CmsWeb.Models
{
    public class TaskInfo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
        public string DispOwner
        {
            get
            {
                var pid = Util.UserPeopleId;
                if (CoOwnerId == pid) // if task has been delegated to me
                    return "You"; // display nothing
                if (OwnerId == pid) // if I am owner
                    if (CoOwnerId.HasValue) // and task has been delegated
                        return CoOwner; // display delegate
                    else // otherwise
                        return ""; // display nothing
                if (CoOwnerId.HasValue) // if this task has been delegated
                    return CoOwner; // display the delegate
                return Owner; // otherwise display the owner
            }
        }
        public string Owner { get; set; }
        public int OwnerId { get; set; }
        public string CoOwner { get; set; }
        public int? CoOwnerId { get; set; }
        internal Person who;

        public string PersonUrl => "/Person2/" + WhoId;

        public int? WhoId
        {
            get
            {
                return who?.PeopleId;
            }
            set
            {
                if (value.HasValue)
                    who = DbUtil.Db.LoadPersonById(value.Value);
            }
        }
        public string Who => who == null ? "" : who.Name;
        public int PrimarySort { get; set; }
        //public int ListId { get; set; }
        public DateTime? SortDue { get; set; }
        public DateTime? DueOrCompleted => SortDueOrCompleted != DateTime.MaxValue.Date ? (DateTime?)SortDueOrCompleted : null;
        public DateTime SortDueOrCompleted { get; set; }
        public string Status { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedOn { get; set; }
        public bool NotiPhone { get; set; }
        public int SortPriority { get; set; }
        public int? Priority
        {
            get { return SortPriority == 4 ? null : (int?)SortPriority; }
            set { SortPriority = value ?? 4; }
        }
        public bool IsAnOwner => IsOwner || IsCoOwner || Util.IsInRole("ManageTasks");
        public bool IsOwner { get; set; }
        public bool IsCoOwner { get; set; }

        public string DeclineReason { get; set; }

        public TaskDetail GetDetail()
        {
            var m = new TaskModel();
            return m.FetchTask(Id);
        }
    }

    public class TaskDetail : TaskInfo
    {
        public string OwnerEmail { get; set; }
        public bool ForceCompleteWContact { get; set; }
        public string CoOwnerEmail { get; set; }
        public string WhoEmail => who?.EmailAddress ?? "";
        public string LimitToRole { get; set; }

        public string WhoEmail2
        {
            get
            {
                if (who != null && who.EmailAddress.HasValue())
                    return who.EmailAddress;
                return string.Empty;
            }
        }
        public string ContactUrl => "/Contact2/" + CompletedContactId;

        public string WhoAddress => who == null ? "" : who.PrimaryAddress;

        public string WhoAddrCityStateZip => who == null ? "" : who.AddrCityStateZip;

        public string WhoPhone => who == null ? "" : who.HomePhone.FmtFone();
        public DateTime CreatedOn { get; set; }
        public string ChangeWho => AssignChange(WhoId);

        private string AssignChange(int? id)
        {
            return id == null ? "(assign)" : "(change)";
        }

        public DateTime? Due
        {
            get { return (SortDue.HasValue && SortDue != DateTime.MaxValue.Date) ? SortDue : null; }
            set { SortDue = value; }
        }
        public string ChangeCoOwner => CoOwnerId == null ? "(delegate)" : "(redelegate)";
        public int StatusId { get; set; }
        public int StatusEnum
        {
            get { return StatusId; }
            set { StatusId = value; }
        }
        public string Location { get; set; }
        public string Project { get; set; }
        public bool ShowCompleted => CompletedOn.HasValue;
        public bool ShowLocation => HttpContext.Current.User.IsInRole("AdvancedTask");

        public bool HasProject => string.IsNullOrEmpty(Project);
        public int? SourceContactId { get; set; }
        public DateTime? SourceContact { get; set; }
        public string SourceContactChange => AssignChange(SourceContactId);
        public int? CompletedContactId { get; set; }
        public DateTime? CompletedContact { get; set; }
        public string Notes { get; set; }
        public string FmtNotes => ViewExtensions2.Markdown(Notes?.Replace("{peopleid}", WhoId.ToString())).ToString();

        public bool HasNotes => string.IsNullOrEmpty(Notes);

        public bool CanComplete => IsAnOwner && this.StatusId != TaskStatusCode.Complete && !ForceCompleteWContact;

        public bool CanCompleteWithContact => IsAnOwner && this.StatusId != TaskStatusCode.Complete && WhoId != null;

        public bool CanAccept => IsCoOwner && (this.StatusId == TaskStatusCode.Pending || this.StatusId == TaskStatusCode.Declined);

        public string ProspectReportLink()
        {
            Util2.CurrentPeopleId = WhoId.Value;
            HttpContext.Current.Session["ActivePerson"] = Who;
            var qb = DbUtil.Db.QueryIsCurrentPerson();
            return "/Reports/Prospect/" + qb.QueryId + "?form=true";
        }
        public IEnumerable<SelectListItem> StatusList()
        {
            return from s in DbUtil.Db.TaskStatuses
                   select new SelectListItem
                   {
                       Text = s.Description,
                       Value = s.Id.ToString()
                   };
        }
        public void UpdateTask()
        {
            var sb = new StringBuilder();
            var task = DbUtil.Db.Tasks.Single(t => t.Id == Id);
            TaskModel.ChangeTask(sb, task, "Description", Description);
            TaskModel.ChangeTask(sb, task, "LimitToRole", LimitToRole);
            TaskModel.ChangeTask(sb, task, "Due", Due);
            TaskModel.ChangeTask(sb, task, "Notes", Notes);
            TaskModel.ChangeTask(sb, task, "StatusId", StatusId);
            task.ForceCompleteWContact = ForceCompleteWContact;
            if (HttpContext.Current.User.IsInRole("AdvancedTask"))
                TaskModel.ChangeTask(sb, task, "Project", Project);

            task.Location = Location;
            if (Priority == 0)
                task.Priority = null;
            else
                task.Priority = Priority;
            DbUtil.Db.SubmitChanges();
            TaskModel.NotifyIfNeeded(sb, task);

            if (task.Owner.PeopleId == Util.UserPeopleId.Value)
            {
                if (task.CoOwner != null)
                    GCMHelper.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated", $"{Util.UserFullName} updated a task delegated to you");
            }
            else
            {
                GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated", $"{Util.UserFullName} updated a task you own");
            }


            GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
        }
    }
}
