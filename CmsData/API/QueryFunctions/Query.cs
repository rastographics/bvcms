using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
            var start = Util.Now;
            var count = qb.Count();
            ElapsedTime = Math.Round(Util.Now.Subtract(start).TotalSeconds).ToInt();
            return count;
        }

        public string SqlPeopleIdsToQuery(string sql)
        {
            var cn = GetReadonlyConnection();
            var pids = cn.Query<int>(sql).ToList();
            if (pids.Count == 0)
                return "";
            return $"peopleids='{string.Join(",", pids)}'";
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
        public IEnumerable<int> QueryPeopleIds(string query)
        {
            var q = db.PeopleQuery2(query).Select(vv => vv.PeopleId);
            return q;
        }
        public IEnumerable<int> QuerySqlPeopleIds(string query)
        {
            var cn = GetReadonlyConnection();
            return cn.Query<int>(query);
        }

        public int QuerySqlInt(string sql)
        {
            var cn = GetReadonlyConnection();
            return cn.ExecuteScalar<int>(sql);
        }
        public dynamic QuerySqlTop1(string sql)
        {
            return QuerySqlTop1(sql, null);
        }

        public dynamic QuerySqlTop1(string sql, object p1)
        {
            return QuerySqlTop1(sql, p1, null);
        }

        public dynamic QuerySqlTop1(string sql, object p1, Dictionary<string, string> d)
        {
            var q = QuerySql(sql, p1, d);
            return q.FirstOrDefault();
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

            if (sql.Contains("@UserPeopleId"))
                parameters.Add("@UserPeopleId", data.PeopleId ?? Util.UserPeopleId);
            if (sql.Contains("@CurrentOrgId"))
                parameters.Add("@CurrentOrgId", data.OrgId ?? db.CurrentOrgId0);

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


        /// <summary>
        /// Function takes a sql script and then places the results into an array.
        /// The first column in the SQL results should countain Dates.  
        /// This is followed by 1 to 10 columns value 
        /// Each row of SQL results is processed into a row of the array by creating a DateValueRow class
        /// 
        /// header: string containing the header row. (less the square brackets)
        /// numvalcols: number of columns containing values. (do not include date column)
        /// sql: string containing the sql script.
        /// 
        /// Example:
        /// SqlDateValueArray("{label: 'Date', type: 'date'},{label: 'Sum', type: 'number'},{label: 'Avg', type: 'number'}", 2, '''sql string''')
        /// 
        /// If there are no results from the SQL query, then a table is returned that prints "NULL" on the chart
        /// </summary>
        public string SqlDateValueArray(string header, int numvalcols, string sql)
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

            var list = q.Select(rr => new DateValueRow { SqlRow = rr, Columns = numvalcols }).ToList();

            if (list.Count == 0)
                return @"[[{label: 'Date', id: 'Date', type: 'date'}, 'Value'],[new Date(2000, 1, 1), 0], [new Date(2000, 1, 2), 10],[new Date(2000, 2, 1), 0],[new Date(2000, 2, 2), 10],[new Date(2000, 2, 3), 0],[new Date(2000, 3, 1), 0],[new Date(2000, 3, 2), 10],[new Date(2000, 3, 3), 1],[new Date(2000, 4, 1), 1],[new Date(2000, 4, 2), 10],[new Date(2000, 4, 3), 0],[new Date(2000, 5, 1), 0],[new Date(2000, 5, 2), 10],[new Date(2000, 5, 3), 1],[new Date(2000, 6, 1), 1],[new Date(2000, 6, 2), 0],[new Date(2000, 7, 1), 0],[new Date(2000, 7, 2), 10],[new Date(2000, 7, 3), 1],[new Date(2000, 8, 1), 1],[new Date(2000, 8, 2), 0]]";
            return $@"[ [{header}], {string.Join(",\n", list)} ]";
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

        public int TagSqlPeopleIds(string sql)
        {
            var q = db.Connection.Query<int>(sql);
            var tag = db.PopulateTempTag(q);
            return tag.Id;
        }

        public string GetWhereClause(string code)
        {
            var q = db.PeopleQuery2(code);
            return db.GetWhereClause(q);
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

        /// <summary>
        /// Takes a dynamic row resulting from executing SQL statement
        /// Expects one value to be labled "Date" 
        /// The remaining values are labeled "Val1", "Val2"... upto "Val10"
        /// Columns:  is used to initialize the class and determines the number of Value columns are assumed to be in the row.
        /// 
        /// NOTE: Month is Converted to 0-Base Index for JavaScript Date object 
        /// 
        /// Example:  (assuming Columns = 2)
        /// [new Date(2006, 02, 13), 45.23, 5006]
        /// </summary>
        public class DateValueRow
        {
            public DateTime Date { get; set; }
            public dynamic SqlRow { get; set; }
            public int Columns { get; set; }
            
            public override string ToString()
            {
                Date = SqlRow.Date;

                switch (Columns)
                {
                    case 10:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}, {SqlRow.Val5}, {SqlRow.Val6}, {SqlRow.Val7}, {SqlRow.Val8}, {SqlRow.Val9}, {SqlRow.Val10}]";
                    case 9:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}, {SqlRow.Val5}, {SqlRow.Val6}, {SqlRow.Val7}, {SqlRow.Val8}, {SqlRow.Val9}]";
                    case 8:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}, {SqlRow.Val5}, {SqlRow.Val6}, {SqlRow.Val7}, {SqlRow.Val8}]";
                    case 7:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}, {SqlRow.Val5}, {SqlRow.Val6}, {SqlRow.Val7}]";
                    case 6:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}, {SqlRow.Val5}, {SqlRow.Val6}]";
                    case 5:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}, {SqlRow.Val5}]";
                    case 4:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}, {SqlRow.Val4}]";
                    case 3:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}, {SqlRow.Val3}]";
                    case 2:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}, {SqlRow.Val2}]";
                    case 1:
                    default:
                        return $"  [new Date({Date.Year}, {Date.Month - 1}, {Date.Day}), {SqlRow.Val1}]";

                }


            }
        }
    }
}