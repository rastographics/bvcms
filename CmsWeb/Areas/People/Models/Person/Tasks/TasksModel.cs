using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public abstract class TasksModel : PagedTableModel<Task, TaskInfo>
    {
        public CmsData.Person person;
        public string AddTask { get; set; }

        protected TasksModel(int id)
            :base("Completed", "desc")
        {
            person = DbUtil.Db.LoadPersonById(id);
        }

        public override IQueryable<Task> ApplySort()
        {
            var q = ModelList();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Created":
                        q = from t in q
                                orderby t.CreatedOn 
                                select t;
                        break;
                    case "Minister":
                        q = from t in q
                                orderby (t.CoOwner ?? t.Owner).Name2
                                select t;
                        break;
                    case "Description":
                        q = from t in q
                                orderby t.Description, t.CreatedOn
                                select t;
                        break;
                    case "About":
                        q = from t in q
                                orderby t.AboutWho.Name2, t.CreatedOn
                                select t;
                        break;
                    case "Due":
                        q = from t in q
                                orderby t.Due ?? t.CompletedOn ?? t.CreatedOn
                                select t;
                        break;
                    case "Completed":
                        q = from t in q
                                orderby (t.CompletedOn != null), t.Due ?? t.CreatedOn
                                select t;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Created":
                        q = from t in q
                                orderby t.CreatedOn descending 
                                select t;
                        break;
                    case "Minister":
                        q = from t in q
                                orderby (t.CoOwner ?? t.Owner).Name2 descending 
                                select t;
                        break;
                    case "Description":
                        q = from t in q
                                orderby t.Description descending, t.CreatedOn descending 
                                select t;
                        break;
                    case "About":
                        q = from t in q
                                orderby t.AboutWho.Name2 descending, t.CreatedOn descending 
                                select t;
                        break;
                    case "Due":
                        q = from t in q
                                orderby t.Due ?? t.CompletedOn ?? t.CreatedOn descending
                                select t;
                        break;
                    case "Completed":
                        q = from t in q
                                orderby (t.CompletedOn != null) descending, t.Due ?? t.CreatedOn descending 
                                select t;
                        break;
                }
            return q;
        }
    }
}