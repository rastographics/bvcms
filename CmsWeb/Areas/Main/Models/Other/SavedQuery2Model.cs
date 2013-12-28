using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web.Security;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class SavedQuery2Model
    {
        public PagerModel2 Pager { get; set; }
        public bool isdev { get; set; }
        public bool onlyMine { get; set; }
        public bool statusFlags { get; set; }
        public string search { get; set; }
        public bool showscratchpads { get; set; }

        public SavedQuery2Model()
        {
            onlyMine = DbUtil.Db.UserPreference("savedSearchOnlyMine", "true").ToBool();
            Pager = new PagerModel2(Count);
            Pager.Direction = "desc";
            Pager.Sort = "LastRun";
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
                       where c.Owner == Util.UserName || ((c.Ispublic || isdev) && !onlyMine)
                       where statusFlags == false || SqlMethods.Like(c.Name, "F[0-9][0-9]%")
                       where c.Owner != null || isdev
                       where !c.Name.Contains("scratchpad") || showscratchpads
                       where c.Name.Contains(search) || c.Owner == search || !search.HasValue()
                       select c;
            return _queries;
        }
        public IEnumerable<SavedQueryInfo2> FetchQueries()
        {
            var q = fetchqueries();
            var q2 = ApplySort(q).Skip(Pager.StartRow).Take(Pager.PageSize);
            var q3 = from c in q2
                     select new SavedQueryInfo2
                     {
                         QueryId = c.QueryId,
                         Description = c.Name,
                         IsPublic = c.Ispublic,
                         LastUpdated = c.Created,
                         LastRun = c.LastRun ?? c.Created,
                         User = c.Owner
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
                        case "LastUpdated":
                            q = from c in q
                                orderby c.Created
                                select c;
                            break;
                        case "Owner":
                            q = from c in q
                                orderby c.Owner, c.Name
                                select c;
                            break;
                        case "LastRun":
                            q = from c in q
                                let dt = c.LastRun ?? c.Created
                                orderby dt 
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
                        case "LastUpdated":
                            q = from c in q
                                orderby c.Created descending
                                select c;
                            break;
                        case "Owner":
                            q = from c in q
                                orderby c.Owner descending, c.Name
                                select c;
                            break;
                        case "LastRun":
                            q = from c in q
                                let dt = c.LastRun ?? c.Created
                                orderby dt descending
                                select c;
                            break;
                    }
                    break;
            }
            return q;
        }
    }
    public class SavedQueryInfo2
    {
        public Guid QueryId { get; set; }
        public bool IsPublic { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? LastRun { get; set; }
    }
}