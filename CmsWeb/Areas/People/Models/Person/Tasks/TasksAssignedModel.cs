using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models.Person
{
    public class TasksAssignedModel : TasksModel
    {
        public TasksAssignedModel(int id) : base(id) { }
        private IQueryable<Task> tasks;
        override public IQueryable<Task> FetchTasks()
        {
            if (tasks == null)
                tasks = from t in DbUtil.Db.Tasks
                        where t.WhoId != null
                        where (t.CoOwnerId ?? t.OwnerId) == person.PeopleId
                        select t;
            return tasks;
        }
        override public IEnumerable<TaskInfo> Tasks()
        {
            var q = FetchTasks();
            q = ApplySort(q);
            var q2 = from t in q
                     select new TaskInfo
                     {
                         TaskId = t.Id,
                         CreatedDate = t.CreatedOn,
                         DueDate = t.Due,
                         About = t.AboutWho.Name,
                         AssignedTo = person.PreferredName,
                         AboutId = t.WhoId,
                         AssignedToId = (t.CoOwnerId ?? t.OwnerId),
                         link = "/Task/List/" + t.Id + "#detail",
                         Desc = t.Description,
                         completed = t.CompletedOn
                     };
            return q2.Skip(Pager.StartRow).Take(Pager.PageSize);
        }
    }
}