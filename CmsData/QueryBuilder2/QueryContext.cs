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
            Condition c;
            var q = (from cc in Queries
                     where cc.Owner == Util.UserName
                     where !cc.Ispublic
                     where cc.Name == Util.ScratchPad2
                     orderby cc.LastRun // get the oldest one
                     select cc).FirstOrDefault();
            if (q == null)
            {
                c = Condition.CreateNewGroupClause();
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
                SubmitChanges();
            }
            else
                c = q.ToClause();

            c.Id = q.QueryId; // force these to match
            c.justloadedquery = q;
            return c;
        }
        public Query QueryIsCurrentPerson()
        {
            const string STR_IsCurrentPerson = "IsCurrentPerson2";
            var qb = Queries.FirstOrDefault(c => c.Owner == STR_System
                && c.Name == STR_IsCurrentPerson);
            if (qb == null)
            {
                var c = Condition.CreateNewGroupClause();
                c.AddNewClause(QueryType.IsCurrentPerson, CompareType.Equal, "1,T");
                qb = new Query
                {
                    QueryId = c.Id,
                    Owner = STR_System,
                    Created = DateTime.Now,
                    LastRun = DateTime.Now,
                    Name = STR_IsCurrentPerson,
                    Text = c.ToXml()
                };
                Queries.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
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
            c.Description = q.Name;
            return c;
        }
        public Condition LoadCopyOfExistingQuery(Guid existingId)
        {
            var i = (from existing in Queries
                     where existing.QueryId == existingId
                     select existing).Single();

            // This is my non public query
            if (string.Compare(i.Owner, Util.UserName, StringComparison.OrdinalIgnoreCase) == 0)
                return i.ToClause();

            i.RunCount = i.RunCount + 1;
            var q = ScratchPadCondition().justloadedquery;
            q.Text = i.Text;
            var c = Condition.Import(i.Text, i.Name, newGuids: true, topguid: q.QueryId);
            c.Description = Util.ScratchPad2;
            c.PreviousName = i.Name;
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
