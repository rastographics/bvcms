using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class TasksAboutModel : TasksModel
    {
        public TasksAboutModel(int id)
            : base(id)
        {
            AddTask = "/Person2/AddTaskAbout/" + id;
        }
        public override IQueryable<Task> DefineModelList()
        {
            return from t in DbUtil.Db.Tasks
                   where t.WhoId == person.PeopleId
                   select t;
        }

        public override IEnumerable<TaskInfo> DefineViewList(IQueryable<Task> q)
        {
            return from t in q
                   select new TaskInfo
                   {
                       TaskId = t.Id,
                       CreatedDate = t.CreatedOn,
                       DueDate = t.Due,
                       About = t.AboutWho.PreferredName,
                       AssignedTo = (t.CoOwner ?? t.Owner).Name,
                       AboutId = t.WhoId,
                       AssignedToId = (t.CoOwnerId ?? t.OwnerId),
                       link = "/Task/List/" + t.Id + "#detail",
                       Desc = t.Description,
                       completed = t.CompletedOn
                   };
        }
    }
}