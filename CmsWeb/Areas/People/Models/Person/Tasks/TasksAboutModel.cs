using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class TasksAboutModel : TasksModel
    {
        public TasksAboutModel() { }
        public override IQueryable<CmsData.Task> DefineModelList()
        {
            return from t in FilteredModelList()
                   where t.WhoId == Person.PeopleId
                   select t;
        }
        public override string AddTask { get { return "/Person2/AddTaskAbout/" + PeopleId; } }

        public override IEnumerable<TaskInfo> DefineViewList(IQueryable<CmsData.Task> q)
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
                       link = "/Task/" + t.Id,
                       Desc = t.Description,
                       completed = t.CompletedOn
                   };
        }
    }
}
