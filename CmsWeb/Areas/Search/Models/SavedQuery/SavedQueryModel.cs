using CmsData;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web.Security;
using UtilityExtensions;
using Query = CmsData.Query;

namespace CmsWeb.Areas.Search.Models
{
    public class SavedQueryModel : PagedTableModel<Query, SavedQueryInfo>
    {
        internal CMSDataContext Db;
        public bool admin { get; set; }
        public bool OnlyMine { get; set; }
        public bool PublicOnly { get; set; }
        public string SearchQuery { get; set; }
        public bool ScratchPadsOnly { get; set; }
        public bool StatusFlagsOnly { get; set; }

        public SavedQueryModel(CMSDataContext db) : base("Last Run", "desc", true)
        {
            Db = db;
            admin = Roles.IsUserInRole("Admin");
        }

        public SavedQueryModel() : base()
        {
        }

        public override IQueryable<Query> DefineModelList()
        {
            admin = Roles.IsUserInRole("Admin");
            var q = from c in Db.Queries
                    where !PublicOnly || c.Ispublic
                    where c.Name.Contains(SearchQuery) || c.Owner == SearchQuery || !SearchQuery.HasValue()
                    where c.Name != "OrgFilter"
                    select c;
            if (ScratchPadsOnly)
            {
                q = from c in q
                    where c.Name == Util.ScratchPad2
                    select c;
            }
            else
            {
                q = from c in q
                    where c.Name != Util.ScratchPad2
                    select c;
            }

            if (StatusFlagsOnly)
            {
                q = from c in q
                    where StatusFlagsOnly == false || SqlMethods.Like(c.Name, "F[0-9][0-9]%")
                    select c;
            }

            Db.SetUserPreference("SavedQueryOnlyMine", OnlyMine);
            if (OnlyMine)
            {
                q = from c in q
                    where c.Owner == Util.UserName
                    select c;
            }
            else if (!admin)
            {
                q = from c in q
                    where c.Owner.ToUpper() == Util.UserName.ToUpper() || c.Ispublic
                    select c;
            }

            return q;
        }

        public override IQueryable<Query> DefineModelSort(IQueryable<Query> q)
        {
            switch (SortExpression)
            {
                case "Public":
                    return from c in q
                           orderby c.Ispublic, c.Owner, c.Name
                           select c;
                case "Description":
                    return from c in q
                           orderby c.Name
                           select c;
                case "Last Run":
                    return from c in q
                           orderby c.LastRun ?? c.Created
                           select c;
                case "Owner":
                    return from c in q
                           orderby c.Owner, c.Name
                           select c;
                case "Count":
                    return from c in q
                           orderby c.RunCount, c.Name
                           select c;
                case "Public desc":
                    return from c in q
                           orderby c.Ispublic descending, c.Owner, c.Name
                           select c;
                case "Description desc":
                    return from c in q
                           orderby c.Name descending
                           select c;
                case "Last Run desc":
                    return from c in q
                           let dt = c.LastRun ?? c.Created
                           orderby dt descending
                           select c;
                case "Owner desc":
                    return from c in q
                           orderby c.Owner descending, c.Name
                           select c;
                case "Count desc":
                    return from c in q
                           orderby c.RunCount descending, c.Name
                           select c;
            }
            return q;
        }

        public override IEnumerable<SavedQueryInfo> DefineViewList(IQueryable<Query> q)
        {
            var user = Util.UserName;
            return from c in q
                   select new SavedQueryInfo(Db)
                   {
                       QueryId = c.QueryId,
                       Name = c.Name,
                       Ispublic = c.Ispublic,
                       LastRun = c.LastRun ?? c.Created,
                       Owner = c.Owner,
                       CanDelete = admin || c.Owner == user,
                       RunCount = c.RunCount,
                   };
        }

    }
}
