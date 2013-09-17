using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityExtensions;
using Dapper;

namespace CmsData
{
    public partial class CMSDataContext
    {
        public Query LoadQueryById2(Guid? id)
        {
            var q = Queries.SingleOrDefault(cc => cc.QueryId == id);
            return q;
        }
        public Condition ScratchPadCondition()
        {
            var c = Condition.CreateNewGroupClause();
            c.AddNewClause();
            var q = new Query
            {
                QueryId = c.Id,
                Owner = Util.UserName,
                Created = DateTime.Now,
                LastRun = DateTime.Now,
                Name = Util.ScratchPad2,
                Text = c.ToXml()
            };
            Queries.InsertOnSubmit(q);
            SubmitChanges();
            return q.ToClause();
        }
        public List<Query> FetchLastFiveQueries()
        {
            var q = from cc in Queries
                    where cc.Owner == Util.UserName
                    where !cc.Ispublic
                    where cc.Name != Util.ScratchPad2
                    orderby cc.LastRun descending
                    select cc;
            var list = q.Take(5).ToList();
            if (!list.Any())
            {
                var c = Condition.CreateNewGroupClause();
                c.AddNewClause();
                var query = new Query
                {
                    QueryId = c.Id,
                    Owner = Util.UserName,
                    Created = DateTime.Now,
                    LastRun = DateTime.Now,
                    Text = c.ToXml()
                };
                Queries.InsertOnSubmit(query);
                SubmitChanges();
                list.Add(query);
            }
            return list;
        }
        public Condition FetchLastQuery()
        {
            var q = (from cc in Queries
                     where cc.Owner == Util.UserName
                     where !cc.Ispublic
                     orderby cc.LastRun descending
                     select cc).FirstOrDefault();
            Condition c;
            if (q == null)
            {
                c = Condition.CreateNewGroupClause();
                c.AddNewClause();
                q = new Query
                {
                    QueryId = c.Id,
                    Owner = Util.UserName,
                    Created = DateTime.Now,
                    LastRun = DateTime.Now,
                    Name = Util.ScratchPad2,
                    Text = c.ToXml()
                };
                Queries.InsertOnSubmit(q);
            }
            else
                c = q.ToClause();
            c.Id = q.QueryId; // force these to match
            c.justloadedquery = q;
            return c;
        }
        public Condition LoadCopyOfExistingQuery(Guid existingId)
        {
            var i = (from existing in Queries
                     let copy = (from v in Queries
                                 where v.Owner == Util.UserName && v.CopiedFrom == existingId
                                 orderby v.LastRun descending
                                 select v).FirstOrDefault()
                     where existing.QueryId == existingId
                     select new { copy, existing }).Single();

            if (string.Compare(i.existing.Owner, Util.UserName, StringComparison.OrdinalIgnoreCase) == 0 && !i.existing.Ispublic)
                return i.existing.ToClause();

            // record on existing that it was run
            i.existing.RunCount = i.existing.RunCount + 1;

            // return the copy if it is available
            if (i.copy != null)
                return i.copy.ToClause();

            // at this point, I am either not the owner or this is a public query

            var c = i.existing.ToClause().Clone(useGuid: Guid.NewGuid());
            c.CopiedFrom = existingId;
            c.Description = "Copy of " + i.existing.Name;
            c.Save(this);
            return c;
        }
        public void CheckLoadQueries(bool reload = false)
        {
            if (Queries.Any() && !reload)
                return;
            List<int> list;
            {
                ExecuteCommand("Truncate table dbo.Query");
                var q = ExecuteQuery<int>("select QueryId from QueryBuilderClauses where GroupId is null and Description is not null and savedby is not null");
                list = q.ToList();
            }
            foreach (var qid in list)
            {
                var c = LoadQueryById(qid);
                var xml = c.ToXml("conv", qid);
                string name = null;
                if (c.Description != Util.ScratchPad)
                    name = c.Description.Truncate(50);
                var nc = Condition.Import(xml, name);
                xml = nc.ToXml();
                if (!nc.Conditions.Any())
                    continue;
                var q = new Query()
                {
                    QueryId = nc.Id,
                    Name = name ?? Util.ScratchPad2,
                    Text = xml,
                    Owner = c.SavedBy,
                    Created = c.CreatedOn,
                    LastRun = c.CreatedOn,
                    Ispublic = c.IsPublic,
                    RunCount = 0
                };
                Queries.InsertOnSubmit(q);
                SubmitChanges();
            }
        }
    }
}
