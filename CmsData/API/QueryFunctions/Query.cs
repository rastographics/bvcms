using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using UtilityExtensions;

namespace CmsData
{
    public partial class QueryFunctions
    {
        public int ElapsedTime { get; private set; }

        public IEnumerable<Person> BlueToolbarReport()
        {
            if (!dictionary.ContainsKey("BlueToolbarGuid"))
                return new List<Person>();
            var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
            if (!guid.HasValue)
                return new List<Person>();
            return db.PeopleQuery(guid.Value).Take(1000);
        }
        public IEnumerable<Person> BlueToolbarReport(string sort)
        {
            if (!dictionary.ContainsKey("BlueToolbarGuid"))
                return new List<Person>();
            var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
            if (!guid.HasValue)
                return new List<Person>();
            var q = SortList(sort, db.PeopleQuery(guid.Value));
            return q.Take(1000);
        }

        public int QueryCount(string query)
        {
            var qb = db.PeopleQuery2(query);
            if (qb == null)
                return 0;
            var start = DateTime.Now;
            var count = qb.Count();
            ElapsedTime = Math.Round(DateTime.Now.Subtract(start).TotalSeconds).ToInt();
            return count;
        }

        public IEnumerable<Person> QueryList(object query, string sort="name")
        {
            return SortList(sort, db.PeopleQuery2(query)).Take(1000);
        }

        private static IQueryable<Person> SortList(string sort, IQueryable<Person> q)
        {
            switch (sort.ToLower())
            {
                case "age":
                    q = from u in q
                        orderby u.Age, u.Name2
                        select u;
                    break;
                case "age desc":
                    q = from u in q
                        orderby u.Age descending, u.Name2
                        select u;
                    break;
                case "birthday":
                    q = from u in q
                        orderby u.BirthMonth, u.BirthDay, u.Name2
                        select u;
                    break;
                case "birthday desc":
                    q = from u in q
                        orderby u.BirthMonth descending, u.BirthDay descending, u.Name2
                        select u;
                    break;
                case "name":
                    q = from u in q
                        orderby u.Name2
                        select u;
                    break;
                case "name desc":
                    q = from u in q
                        orderby u.Name2 descending
                        select u;
                    break;
            }
            return q;
        }

        public IEnumerable<dynamic> QuerySql(string sql)
        {
            return QuerySql(sql, null);
        }

        public IEnumerable<dynamic> QuerySql(string sql, object p1)
        {
            return QuerySql(sql, p1, null);
        }

        public IEnumerable<dynamic> QuerySql(string sql, object p1, Dictionary<string, string> d)
        {
            var cn = GetReadonlyConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@p1", p1 ?? "");
            if (d != null)
                foreach (var kv in d)
                    parameters.Add("@" + kv.Key, kv.Value);

            if (sql.Contains("@BlueToolbarTagId"))
                if (dictionary.ContainsKey("BlueToolbarGuid"))
                {
                    var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
                    if (!guid.HasValue)
                        throw new Exception("missing BlueToolbar Information");
                    var j = db.PeopleQuery(guid.Value).Select(vv => vv.PeopleId).Take(1000);
                    var tag = db.PopulateTemporaryTag(j);
                    parameters.Add("@BlueToolbarTagId", tag.Id);
                }

            return cn.Query(sql, parameters, commandTimeout: 300);
        }

        public string SqlNameCountArray(string title, string sql)
        {
            var cn = GetReadonlyConnection();
            string declareqtagid = null;
            if (sql.Contains("@qtagid"))
            {
                var id = db.FetchLastQuery().Id;
                var tag = db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                declareqtagid = $"DECLARE @qtagid INT = {tag.Id}\n";
            }
            sql = $"{declareqtagid}{sql}";
            var q = cn.Query(sql);
            var list = q.Select(rr => new NameValuePair { Name = rr.Name, Value = rr.Cnt }).ToList();
            if (list.Count == 0)
                return @"[ ['No Data', 'Count'], ['Dummy Value 1', 1], ['Dummy Value 2', 2], ['Dummy Value 3', 3], ]";
            return $@"[
  ['{title}', 'Count'],
{string.Join(",\n", list)}
]";
        }

        public int StatusCount(string flags)
        {
            if (flags == "F00")
                return db.People.Count();
            var statusflags = flags.Split(',').Where(ss => ss != "F00").ToArray();
            var q = from p in db.People
                    let ac = p.Tags.Count(tt => statusflags.Contains(tt.Tag.Name))
                    where ac == statusflags.Length
                    select p;
            return q.Count();
        }

        public int TagCount(int tagid)
        {
            var n = db.TagPeople.Count(v => v.Id == tagid);
            return n;
        }

        public int TagQueryList(object query)
        {
            var q = db.PeopleQuery2(query).Select(vv => vv.PeopleId);
            var tag = db.PopulateTemporaryTag(q);
            return tag.Id;
        }

        public class NameValuePair
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return $"  ['{Name}', {Value}]";
            }
        }
    }
}