using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

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
                     select cc).ToList();
            Query query = null;
            if (q.Count < 5)
            {
                c = Condition.CreateNewGroupClause();
                query = new Query
                {
                    QueryId = c.Id,
                    Owner = Util.UserName,
                    Created = DateTime.Now,
                    LastRun = DateTime.Now,
                    Name = Util.ScratchPad2,
                    Text = c.ToXml()
                };
                Queries.InsertOnSubmit(query);
                SubmitChanges();
            }
            else
            {
                query = q.First();
                c = query.ToClause();
            }
            c.Id = query.QueryId; // force these to match
            c.JustLoadedQuery = query;
            return c;
        }
        public Condition StandardQuery(string name)
        {
            var q = Queries.FirstOrDefault(c => c.Owner == STR_System && c.Name == name);
            if(q == null)
                return null;
            return q.ToClause();
        }
        public Query StandardQuery(string name, QueryType typ)
        {
            var qb = Queries.FirstOrDefault(c => c.Owner == STR_System && c.Name == name);
            if (qb == null)
            {
                var c = Condition.CreateNewGroupClause();
                c.AddNewClause(typ, CompareType.Equal, "1,T");
                qb = new Query
                {
                    QueryId = c.Id,
                    Owner = STR_System,
                    Created = DateTime.Now,
                    LastRun = DateTime.Now,
                    Name = name,
                    Text = c.ToXml()
                };
                Queries.InsertOnSubmit(qb);
                SubmitChanges();
            }
            return qb;
        }
        public Query MatchNothing()
        {
            return StandardQuery("MatchNothing", QueryType.MatchNothing);
        }
        public Query QueryIsCurrentUser()
        {
            return StandardQuery("IsCurrentUser", QueryType.IsCurrentUser);
        }
        public Query QueryIsCurrentPerson()
        {
            return StandardQuery("IsCurrentPerson", QueryType.IsCurrentPerson);
        }
        public Query QueryHasCurrentTag()
        {
            return StandardQuery("HasCurrentTag", QueryType.HasCurrentTag);
        }
        public Query QueryInCurrentOrg()
        {
            return StandardQuery("InCurrentOrg", QueryType.InCurrentOrg);
        }
        public Query QueryLeadersUnderCurrentOrg()
        {
            return StandardQuery("LeadersUnderCurrentOrg", QueryType.LeadersUnderCurrentOrg);
        }
        public Query QueryMembersUnderCurrentOrg()
        {
            return StandardQuery("MembersUnderCurrentOrg", QueryType.MembersUnderCurrentOrg);
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
                     //where !cc.Ispublic
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
            c.JustLoadedQuery = q;
            c.Description = q.Name;
            return c;
        }
        public Condition LoadExistingQuery(Guid existingId)
        {
            var i = (from existing in Queries
                     where existing.QueryId == existingId
                     select existing).First();
            var c = Condition.Import(i.Text, i.Name, newGuids: false, topguid: i.QueryId);
            return c;
        }
        public Condition LoadCopyOfExistingQuery(Guid existingId)
        {
            var i = (from existing in Queries
                     where existing.QueryId == existingId
                     select existing).First();
            i.RunCount = i.RunCount + 1;
            i.LastRun = DateTime.Now;
            if (i.Name == Util.ScratchPad2)
                return i.ToClause();
            var q = ScratchPadCondition().JustLoadedQuery;
            q.Text = i.Text;
            var c = Condition.Import(i.Text, i.Name, newGuids: true, topguid: q.QueryId);
            c.Description = Util.ScratchPad2;
            c.PreviousName = i.Name;
            c.Save(this);
            return c;
        }
        public Guid QueryIdByName(string name)
        {
            var i = (from existing in Queries
                     where existing.Name == name
                     select existing).FirstOrDefault();
            if (i != null)
                return i.QueryId;

            var qb = ScratchPadCondition();
            qb.Reset();
            var nc = qb.AddNewClause();
            qb.Description = Util.ScratchPad2;
            qb.Save(DbUtil.Db);
            return qb.Id;
        }
        public Query QueryInactiveCurrentOrg()
        {
            return StandardQuery("InactiveCurrentOrg", QueryType.InactiveCurrentOrg);
        }
        public Query QueryProspectCurrentOrg()
        {
            return StandardQuery("ProspectCurrentOrg", QueryType.ProspectCurrentOrg);
        }
        public Query QueryPendingCurrentOrg()
        {
            return StandardQuery("PendingCurrentOrg", QueryType.PendingCurrentOrg);
        }
        public Query QueryPreviousCurrentOrg()
        {
            return StandardQuery("PreviousCurrentOrg", QueryType.PreviousCurrentOrg);
        }
        public Query QueryVisitedCurrentOrg()
        {
            return StandardQuery("VisitedCurrentOrg", QueryType.VisitedCurrentOrg);
        }
    }
}
