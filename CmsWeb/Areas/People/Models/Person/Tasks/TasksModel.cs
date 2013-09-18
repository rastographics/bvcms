using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public abstract class TasksModel : ITasks
    {
        public CmsData.Person person;
        public PagerModel2 Pager { get; set; }

        protected TasksModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
            Pager = new PagerModel2(Count) {Sort = "Completed", Direction = "desc"};
        }
        int? count;
        public int Count()
        {
            if (!count.HasValue)
                count = FetchTasks().Count();
            return count.Value;
        }
        public abstract IQueryable<Task> FetchTasks();
        public abstract IEnumerable<TaskInfo> Tasks();
        public string AddTask { get; set; }

        public IQueryable<Task> ApplySort(IQueryable<Task> tasks)
        {
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Created":
                        tasks = from t in tasks
                                orderby t.CreatedOn 
                                select t;
                        break;
                    case "Minister":
                        tasks = from t in tasks
                                orderby (t.CoOwner ?? t.Owner).Name2
                                select t;
                        break;
                    case "Description":
                        tasks = from t in tasks
                                orderby t.Description, t.CreatedOn
                                select t;
                        break;
                    case "About":
                        tasks = from t in tasks
                                orderby t.AboutWho.Name2, t.CreatedOn
                                select t;
                        break;
                    case "Due":
                        tasks = from t in tasks
                                orderby t.Due ?? t.CompletedOn ?? t.CreatedOn
                                select t;
                        break;
                    case "Completed":
                        tasks = from t in tasks
                                orderby (t.CompletedOn != null), t.Due ?? t.CreatedOn
                                select t;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Created":
                        tasks = from t in tasks
                                orderby t.CreatedOn descending 
                                select t;
                        break;
                    case "Minister":
                        tasks = from t in tasks
                                orderby (t.CoOwner ?? t.Owner).Name2 descending 
                                select t;
                        break;
                    case "Description":
                        tasks = from t in tasks
                                orderby t.Description descending, t.CreatedOn descending 
                                select t;
                        break;
                    case "About":
                        tasks = from t in tasks
                                orderby t.AboutWho.Name2 descending, t.CreatedOn descending 
                                select t;
                        break;
                    case "Due":
                        tasks = from t in tasks
                                orderby t.Due ?? t.CompletedOn ?? t.CreatedOn descending
                                select t;
                        break;
                    case "Completed":
                        tasks = from t in tasks
                                orderby (t.CompletedOn != null) descending, t.Due ?? t.CreatedOn descending 
                                select t;
                        break;
                }
            return tasks;
        }
    }
}