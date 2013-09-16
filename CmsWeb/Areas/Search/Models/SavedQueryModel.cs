using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Security;
using CmsData;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.SS.Formula.Functions;
using UtilityExtensions;
using Query = CmsData.Query;

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
                       where !PublicOnly || c.Ispublic == true
                       where c.Name.Contains(SearchQuery) || c.Owner == SearchQuery || !SearchQuery.HasValue()
                       select c;
            if (ScratchPadsOnly)
                _queries = from c in _queries
                           where c.Name == Util.ScratchPad2
                           select c;
            else
                _queries = from c in _queries
                           where c.Name != Util.ScratchPad2
                           select c;
            if (OnlyMine)
                _queries = from c in _queries
                           where c.Owner == Util.UserName
                           select c;
            else if (!isdev)
                _queries = from c in _queries
                           where c.Owner == Util.UserName || c.Ispublic
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
                         Name = c.Name,
                         Ispublic = c.Ispublic,
                         LastRun = c.LastRun ?? c.Created,
                         Owner = c.Owner,
                         CanDelete = admin || c.Owner == user,
                         RunCount = c.RunCount,
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
                        case "Last Run":
                            q = from c in q
                                orderby c.LastRun ?? c.Created
                                select c;
                            break;
                        case "Owner":
                            q = from c in q
                                orderby c.Owner, c.Name
                                select c;
                            break;
                        case "Count":
                            q = from c in q
                                orderby c.RunCount, c.Name
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
                        case "Last Run":
                            q = from c in q
                                let dt = c.LastRun ?? c.Created
                                orderby dt descending
                                select c;
                            break;
                        case "Owner":
                            q = from c in q
                                orderby c.Owner descending, c.Name
                                select c;
                            break;
                        case "Count":
                            q = from c in q
                                orderby c.RunCount descending, c.Name
                                select c;
                            break;
                    }
                    break;
            }
            return q;
        }
    }
}