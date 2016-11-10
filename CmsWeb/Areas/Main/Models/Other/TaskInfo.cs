/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CmsData;
using UtilityExtensions;
using CmsWeb.Code;

namespace CmsWeb.Models
{
    public class TaskInfo
    {
        public int Id { get; set; }
        [StringLength(50)]
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
        public string About => who == null ? "" : who.Name;
        public int PrimarySort { get; set; }
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
}
