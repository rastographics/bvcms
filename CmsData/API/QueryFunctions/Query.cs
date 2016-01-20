using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using Dapper;
using UtilityExtensions;

namespace CmsData
{
    public partial class QueryFunctions
    {
        public IEnumerable<Person> BlueToolbarReport()
        {
            if (!dictionary.ContainsKey("BlueToolbarGuid"))
                return new List<Person>();
            var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
            if (!guid.HasValue)
                return new List<Person>();
            return db.PeopleQuery(guid.Value).Take(1000);
        }

        public int DecisionCountDateRange(string decisiontype, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);
            var a = decisiontype.Split(',');
            var q = from p in db.People
                    where p.DecisionDate >= start
                    where p.DecisionDate < enddt
                    where a.Contains(p.DecisionType.Description)
                    select p;
            return q.Count();
        }

        public int ElapsedTime { get; private set; }

        public int QueryCount(string s)
        {
            var qb = db.PeopleQuery2(s);
            if (qb == null)
                return 0;
            var start = DateTime.Now;
            var count = qb.Count();
            ElapsedTime = Math.Round(DateTime.Now.Subtract(start).TotalSeconds).ToInt();
            return count;
        }


        /* QueryList is designed to run a pre-saved query referenced by name which is passed in as a string in the function call.
        * The resulting collection of people records (limited to 1000) is returned as an IEnumerable so that all attributes of the
        * Person record are accessible
        */

        public IEnumerable<Person> QueryList(object savedQuery)
        {
            return db.PeopleQuery2(savedQuery).Take(1000);
        }

        public IEnumerable<Person> QueryList2(object savedQuery, string orderbyparam, bool ascending)
        {
            var q = db.PeopleQuery2(savedQuery);

            switch (orderbyparam.ToLower())
            {
                case "age":
                    if (ascending)
                        q = from u in q
                            orderby u.Age, u.LastName, u.FirstName
                            select u;
                    else
                        q = from u in q
                            orderby u.Age descending, u.LastName, u.FirstName
                            select u;
                    break;
                case "birthday":
                    if (ascending)
                        q = from u in q
                            orderby u.BirthMonth, u.BirthDay, u.LastName, u.FirstName
                            select u;
                    else
                        q = from u in q
                            orderby u.BirthMonth descending, u.BirthDay descending, u.LastName, u.FirstName
                            select u;
                    break;
                case "name":
                    if (ascending)
                        q = from u in q
                            orderby u.LastName, u.FirstName
                            select u;
                    else
                        q = from u in q
                            orderby u.LastName descending, u.FirstName descending
                            select u;
                    break;
            }
            return q.Take(1000);
        }

        public IEnumerable<dynamic> QuerySql(string sqlscript)
        {
            return QuerySql(sqlscript, null);
        }

        public IEnumerable<dynamic> QuerySql(string sqlscript, object p1)
        {
            return QuerySql(sqlscript, p1, null);
        }

        public IEnumerable<dynamic> QuerySql(string sql, object p1, Dictionary<string, string> d)
        {
            var p = new DynamicParameters();
            p.Add("@p1", p1 ?? "");
            if (d != null)
                foreach (var kv in d)
                    p.Add("@" + kv.Key, kv.Value);

            if (sql.Contains("@BlueToolbarTagId"))
                if (dictionary.ContainsKey("BlueToolbarGuid"))
                {
                    var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
                    if (!guid.HasValue)
                        throw new Exception("missing BlueToolbar Information");
                    var j = DbUtil.Db.PeopleQuery(guid.Value).Select(vv => vv.PeopleId).Take(1000);
                    var tag = DbUtil.Db.PopulateTemporaryTag(j);
                    p.Add("@BlueToolbarTagId", tag.Id);
                }

            var q = db.Connection.Query(sql, p, commandTimeout: 300);
            return q;
        }

        public int RegistrationCount(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in db.OrganizationMembers
                    where m.EnrollmentDate >= dt
                    where m.Organization.RegistrationTypeId > 0
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }

        public string SqlNameCountArray(string title, string sql)
        {
            //            var qb = db.PeopleQuery2(s);
            //            if (qb == null)
            //                return 0;
            var cs = db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            string declareqtagid = null;
            if (sql.Contains("@qtagid"))
            {
                var id = db.FetchLastQuery().Id;
                var tag = db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                declareqtagid = $"DECLARE @qtagid INT = {tag.Id}\n";
            }
            sql = $"{declareqtagid}{sql}";
            var q = cn.Query(sql);
            var list = q.Select(rr => new NameValuePair {Name = rr.Name, Value = rr.Cnt}).ToList();
            if (list.Count == 0)
                return @"[ ['No Data', 'Count'], ['Dummy Value 1', 1], ['Dummy Value 2', 2], ['Dummy Value 3', 3], ]";
            return $@"[
  ['{title}', 'Count'],
{string.Join(",\n", list)}
]";
        }

        public int StatusCount(string s)
        {
            if (s == "F00")
                return db.People.Count();
            var statusflags = s.Split(',').Where(ss => ss != "F00").ToArray();
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

        public int TagQueryList(object savedQuery)
        {
            var q = db.PeopleQuery2(savedQuery).Select(vv => vv.PeopleId);
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