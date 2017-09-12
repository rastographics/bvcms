using System;
using System.Linq;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class Task
    {
        public string AboutName => AboutWho?.Name ?? "";

        partial void OnCreated()
        {
            CreatedOn = Util.Now;
        }
        public static TaskList GetRequiredTaskList(CMSDataContext Db, string name, int pid)
        {
            var list = Db.TaskLists.SingleOrDefault(tl => tl.CreatedBy == pid && tl.Name == name);
            if (list == null)
            {
                list = new TaskList { CreatedBy = pid, Name = name };
                Db.TaskLists.InsertOnSubmit(list);
                Db.SubmitChanges();
            }
            return list;
        }
        public static void AddNewPerson(CMSDataContext Db, int newpersonid)
        {
            var newPeopleManagerId = Db.NewPeopleManagerId;
            if (Db.Setting("NoNewPersonTasks") || newPeopleManagerId == 0)
                return;
            var task = new Task
            {
                ListId = GetRequiredTaskList(Db, "InBox", newPeopleManagerId).Id,
                OwnerId = newPeopleManagerId,
                Description = "New Person Data Entry",
                WhoId = newpersonid,
                StatusId = TaskStatusCode.Active,
            };
            if (Util.UserPeopleId.HasValue && Util.UserPeopleId.Value != newPeopleManagerId)
            {
                task.CoOwnerId = Util.UserPeopleId.Value;
                task.CoListId = GetRequiredTaskList(Db, "InBox", Util.UserPeopleId.Value).Id;
            }
            Db.Tasks.InsertOnSubmit(task);
            Db.SubmitChanges();
        }
        public static int AddTask(CMSDataContext Db, int pid)
        {
            var p = Db.LoadPersonById(pid);
            var t = new Task
            {
                ListId = GetRequiredTaskList(Db, "InBox", Util.UserPeopleId.Value).Id,
                OwnerId = Util.UserPeopleId.Value,
                Description = "Please Contact",
                ForceCompleteWContact = true,
                StatusId = TaskStatusCode.Active,
            };
            p.TasksAboutPerson.Add(t);
            DbUtil.Db.SubmitChanges();
            return t.Id;
        }
        public static int AddTasks(CMSDataContext Db, Guid qid)
        {
            var q = Db.PeopleQuery(qid);
            int qCount = q.Count();
            if (qCount > 100)
                return qCount;
            foreach (var p in q)
            {
                var t = new Task
                {
                    ListId = GetRequiredTaskList(Db, "InBox", Util.UserPeopleId.Value).Id,
                    OwnerId = Util.UserPeopleId.Value,
                    Description = "Please Contact",
                    ForceCompleteWContact = true,
                    StatusId = TaskStatusCode.Active,
                };
                p.TasksAboutPerson.Add(t);
            }
            DbUtil.Db.SubmitChanges();
            return qCount;
        }
        private static string TaskLink0(int id)
        {
            return $"/Task/{id}";
        }
        public static string TaskLink(CMSDataContext Db, string text, int id)
        {
            return $"<a href='{Db.ServerLink(TaskLink0(id))}'>{text}</a>";
        }
    }
}
