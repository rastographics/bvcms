using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class TasksAssignedModel : TasksModel
    {
        public TasksAssignedModel(int id, PagerModel2 pager) : base(id, pager) { }

        override public IQueryable<Task> DefineModelList()
        {
            return from t in DbUtil.Db.Tasks
                   where t.WhoId != null
                   where (t.CoOwnerId ?? t.OwnerId) == person.PeopleId
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
                       About = t.AboutWho.Name,
                       AssignedTo = person.PreferredName,
                       AboutId = t.WhoId,
                       AssignedToId = (t.CoOwnerId ?? t.OwnerId),
                       link = "/Task/List/" + t.Id + "#detail",
                       Desc = t.Description,
                       completed = t.CompletedOn
                   };
        }
    }
}