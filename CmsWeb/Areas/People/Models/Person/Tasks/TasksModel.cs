using CmsData;
using CmsWeb.Models;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public abstract class TasksModel : PagedTableModel<CmsData.Task, TaskInfo>
    {
        public Person Person { get; set; }
        public int? PeopleId
        {
            get { return Person?.PeopleId; }
            set { Person = DbUtil.Db.LoadPersonById(value ?? 0); }
        }

        public abstract string AddTask { get; }

        protected TasksModel()
            : base("Completed", "desc", true)
        { }

        public IQueryable<CmsData.Task> FilteredModelList()
        {
            var u = DbUtil.Db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var managePrivateContacts = HttpContextFactory.Current.User.IsInRole("ManagePrivateContacts");
            return from t in DbUtil.Db.Tasks
                   where (t.LimitToRole ?? "") == "" || roles.Contains(t.LimitToRole) || managePrivateContacts
                   select t;
        }

        public override IQueryable<CmsData.Task> DefineModelSort(IQueryable<CmsData.Task> q)
        {
            switch (SortExpression)
            {
                case "Created":
                    return from t in q
                           orderby t.CreatedOn
                           select t;
                case "Minister":
                    return from t in q
                           orderby (t.CoOwner ?? t.Owner).Name2
                           select t;
                case "Description":
                    return from t in q
                           orderby t.Description, t.CreatedOn
                           select t;
                case "About":
                    return from t in q
                           orderby t.AboutWho.Name2, t.CreatedOn
                           select t;
                case "Due":
                    return from t in q
                           orderby t.Due ?? t.CompletedOn ?? t.CreatedOn
                           select t;
                case "Completed":
                    return from t in q
                           orderby (t.CompletedOn != null) descending, t.Due ?? t.CreatedOn
                           select t;
                case "Created desc":
                    return from t in q
                           orderby t.CreatedOn descending
                           select t;
                case "Minister desc":
                    return from t in q
                           orderby (t.CoOwner ?? t.Owner).Name2 descending
                           select t;
                case "Description desc":
                    return from t in q
                           orderby t.Description descending, t.CreatedOn descending
                           select t;
                case "About desc":
                    return from t in q
                           orderby t.AboutWho.Name2 descending, t.CreatedOn descending
                           select t;
                case "Due desc":
                    return from t in q
                           orderby t.Due ?? t.CompletedOn ?? t.CreatedOn descending
                           select t;
                case "Completed desc":
                default:
                    return from t in q
                           orderby (t.CompletedOn != null), t.Due ?? t.CreatedOn descending
                           select t;
            }
        }
    }
}
