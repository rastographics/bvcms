using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Security;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class SavedQueryModel
    {
        public PagerModel2 Pager { get; set; }
        public bool isdev { get; set; }
        public bool OnlyMine { get; set; }
        public bool PublicOnly { get; set; }
        public string SearchQuery { get; set; }
        public bool ScratchPadsOnly { get; set; }

        public SavedQueryModel()
        {
            Pager = new PagerModel2(Count);
        }
        private int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = fetchqueries().Count();
            return _count.Value;
        }
        private IQueryable<Query> _queries;
        private IQueryable<Query> fetchqueries()
        {
            if (_queries != null)
                return _queries;
            isdev = Roles.IsUserInRole("Developer");
            _queries = from c in DbUtil.Db.Queries
                       where c.Owner == Util.UserName || ((c.Ispublic.Value || isdev) && !OnlyMine)
                       where (!ScratchPadsOnly && !c.Name.Contains("scratchpad"))
                            || (ScratchPadsOnly && c.Name.Contains("scratchpad"))
                       where c.Name.Contains(SearchQuery) || c.Owner == SearchQuery || !SearchQuery.HasValue()
                       select c;
            return _queries;
        }
        public IEnumerable<SavedQueryInfo> FetchQueries()
        {
            var q = fetchqueries();
            var q2 = ApplySort(q).Skip(Pager.StartRow).Take(Pager.PageSize);
            var admin = HttpContext.Current.User.IsInRole("Admin");
            var user = Util.UserName;
            var q3 = from c in q2
                     select new SavedQueryInfo
                     {
                         QueryId = c.QueryId,
                         Description = c.Name,
                         IsPublic = c.Ispublic == true,
                         LastUpdated = c.Modified ?? c.Created,
                         SavedBy = c.Owner,
                         CanDelete = admin || c.Owner == user
                     };
            return q3;
        }
        private IEnumerable<Query> ApplySort(IQueryable<Query> q)
        {
            switch (Pager.Direction)
            {
                case "asc":
                    switch (Pager.Sort)
                    {
                        case "Public":
                            q = from c in q
                                orderby c.Ispublic, c.Owner, c.Name
                                select c;
                            break;
                        case "Description":
                            q = from c in q
                                orderby c.Name
                                select c;
                            break;
                        case "Last Updated":
                            q = from c in q
                                orderby c.Modified ?? c.Created
                                select c;
                            break;
                        case "Owner":
                            q = from c in q
                                orderby c.Owner, c.Name
                                select c;
                            break;
                    }
                    break;
                case "desc":
                    switch (Pager.Sort)
                    {
                        case "Public":
                            q = from c in q
                                orderby c.Ispublic descending, c.Owner, c.Name
                                select c;
                            break;
                        case "Description":
                            q = from c in q
                                orderby c.Name descending
                                select c;
                            break;
                        case "Last Updated":
                            q = from c in q
                                let dt = c.Modified ?? c.Created
                                orderby dt descending
                                select c;
                            break;
                        case "Owner":
                            q = from c in q
                                orderby c.Owner descending, c.Name
                                select c;
                            break;
                    }
                    break;
            }
            return q;
        }
    }
}