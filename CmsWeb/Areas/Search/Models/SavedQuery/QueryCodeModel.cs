using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using CmsData;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class QueryCodeModel
    {
        public List<dynamic> List;
        public int Count;
        public string Code;

        public QueryCodeModel(string queries, List<Guid> guids = null)
        {
#if DEBUG
            if (!DbUtil.Db.QueryAnalyses.Any())
                DbUtil.Db.ExecuteCommand(CodeSql.Populate);
#endif
            var all = DbUtil.Db.Connection.Query(queries).ToList();
            List = guids == null ? all : all.Where(vv => guids.Contains((Guid) vv.QueryId)).ToList();
            Count = List.Count;
            Debug.WriteLine($"{Util.Host} Count: {Count}");
        }


        public Guid? Existing;
        public Guid? Parsed;
        public string Error;
        public string Message;
        public string Xml;

        public void GetLinks(dynamic q)
        {
            Existing = q.QueryId as Guid?;
            Xml = q.text as string;
            Parsed = null;
            Error = null;
            if (Existing == null)
                return;
            var c = DbUtil.Db.LoadExistingQuery(Existing.Value);
            Code = c.ToCode();
        }
#if DEBUG
        public string Analyze(dynamic q)
        {
            Count--;
            Existing = q.QueryId as Guid?;
            var id = Existing;
            Xml = q.text as string;
            Parsed = null;
            Error = null;
            if (Existing == null)
                return null;
            var cnt1 = 0;
            var cnt2 = 0;
            var c = DbUtil.Db.LoadExistingQuery(Existing.Value);
            Code = c.ToCode();
            double? seconds = null;
            var sb = new StringBuilder();
            try
            {
                var dt = DateTime.Now;
                cnt1 = DbUtil.Db.PeopleQueryCondition(c).Count();
                seconds = DateTime.Now.Subtract(dt).TotalSeconds;
                Message = $"Count={Count} {seconds:N0} {Existing}";
                var s = $"{Util.Host} {Existing} {Count}  {seconds} seconds";
                sb.AppendLine(s);
                Debug.WriteLine(s);

            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            if (!Code.HasValue())
                return sb.ToString();
            try
            {
                cnt2 = DbUtil.Db.PeopleQueryCode(Code).Count();
                if (cnt2 != cnt1)
                {
                    Error = $"Original={cnt1:N0}  Parsed={cnt2:N0}";
                    var s = $"{Util.Host} {Existing} {Count}  Original={cnt1:N0}  Parsed={cnt2:N0}";
                    sb.AppendLine(s);
                    Debug.WriteLine(s);
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                var s = $"{Util.Host} {Existing} {Count}  {Error}";
                sb.AppendLine(s);
                Debug.WriteLine(s);
            }
            DbUtil.Db.Connection.Execute(@"
UPDATE QueryAnalysis 
set seconds = @seconds, Message = @Error, OriginalCount = @cnt1, ParsedCount=@cnt2 
where Id = @id", new { id, seconds, Error, cnt1, cnt2 });
            return sb.ToString();
        }
        public static string DoAnalysis(string host = null)
        {
            if (host.HasValue())
            {
                HttpRuntime.Cache["testhost"] = host;
                DbUtil.DbDispose();
            }
            var sb = new StringBuilder();
            var m = new QueryCodeModel(CodeSql.Analyze);
            foreach(var q in m.List)
                sb.Append(m.Analyze(q));
            return sb.ToString();
        }
        public static List<string> DatabaseList()
        {
            var cs = ConfigurationManager.ConnectionStrings["BlogData"].ConnectionString;
            var cn = new System.Data.SqlClient.SqlConnection(cs);
            cn.Open();
            var list = cn.Query<string>("select substring(name, 5, 40) from Databases order by name").ToList();
            return list;
        }

        public static void UpdateAll(string host)
        {
            if (host.HasValue())
            {
                HttpRuntime.Cache["testhost"] = host;
                DbUtil.DbDispose();
            }
            var list = DbUtil.Db.Connection.Query(CodeSql.SqlSavedqueries).ToList();
            foreach (var sq in list)
            {
                var g = sq.QueryId as Guid?;
                if (!g.HasValue)
                    continue;
                Debug.WriteLine($"{sq.name}");
                DbUtil.DbDispose();
                try
                {
                    var c = DbUtil.Db.LoadExistingQuery(g.Value);
                    var s = c.ToCode();
                    if (s.HasValue())
                    {
                        UpdateQueryConditions.Run(sq.QueryId);
                        DbUtil.Db.Connection.Execute(@"UPDATE QueryAnalysis set Updated = 1 where Id = @id", new { id=sq.QueryId });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
#endif
    }
}