using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Classes.GoogleCloudMessaging;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class TaskDetail : TaskInfo
    {
        public string OwnerEmail { get; set; }
        public bool ForceCompleteWContact { get; set; }
        public string CoOwnerEmail { get; set; }
        public string WhoEmail => who?.EmailAddress ?? "";

        public CodeInfo TaskLimitToRole { get; set; }

        public CodeInfo TaskStatus { get; set; }

        public string WhoEmail2 => who != null && who.EmailAddress.HasValue() 
            ? who.EmailAddress : string.Empty;
        public string ContactUrl => "/Contact2/" + CompletedContactId;
        public string WhoAddress => who == null ? "" : who.PrimaryAddress;
        public string WhoAddrCityStateZip => who == null ? "" : who.AddrCityStateZip;
        public string WhoPhone => who == null ? "" : who.HomePhone.FmtFone();
        public DateTime CreatedOn { get; set; }
        public string ChangeWho => AssignChange(WhoId);

        private string AssignChange(int? id) => id == null ? "(assign)" : "(change)";

        public DateTime? Due
        {
            get { return SortDue.HasValue && SortDue != DateTime.MaxValue.Date ? SortDue : null; }
            set { SortDue = value; }
        }
        public string ChangeCoOwner => CoOwnerId == null ? "(delegate)" : "(redelegate)";

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

        public bool CanComplete => IsAnOwner && TaskStatus.IntVal != TaskStatusCode.Complete && !ForceCompleteWContact;

        public bool CanCompleteWithContact => IsAnOwner && TaskStatus.IntVal != TaskStatusCode.Complete && WhoId != null;

        public bool CanAccept => IsCoOwner && (TaskStatus.IntVal == TaskStatusCode.Pending || TaskStatus.IntVal == TaskStatusCode.Declined);

        public string ProspectReportLink()
        {
            if (!WhoId.HasValue)
                return null;
            Util2.CurrentPeopleId = WhoId.Value;
            HttpContext.Current.Session["ActivePerson"] = About;
            var qb = DbUtil.Db.QueryIsCurrentPerson();
            return "/Reports/Prospect/" + qb.QueryId + "?form=true";
        }
        public void UpdateTask()
        {
            var sb = new StringBuilder();
            var task = DbUtil.Db.Tasks.Single(t => t.Id == Id);
            TaskModel.ChangeTask(sb, task, "Description", Description);
            TaskModel.ChangeTask(sb, task, "LimitToRole", TaskLimitToRole.Value);
            TaskModel.ChangeTask(sb, task, "Due", Due);
            TaskModel.ChangeTask(sb, task, "Notes", Notes);
            TaskModel.ChangeTask(sb, task, "StatusId", TaskStatus.IntVal);
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

            if (task.Owner.PeopleId == Util.UserPeopleId)
            {
                if (task.CoOwner != null)
                    GCMHelper.sendNotification(task.CoOwner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated", $"{Util.UserFullName} updated a task delegated to you");
            }
            else
            {
                GCMHelper.sendNotification(task.Owner.PeopleId, GCMHelper.TYPE_TASK, task.Id, "Task Updated", $"{Util.UserFullName} updated a task you own");
            }


            if(Util.UserPeopleId.HasValue)
                GCMHelper.sendRefresh(Util.UserPeopleId.Value, GCMHelper.TYPE_TASK);
        }
    }
}