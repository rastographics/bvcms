using CmsData;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class QueryCodeModel
    {
        public List<dynamic> List;
        public int Count;
        public string Code;
        public string Sql;
        internal CMSDataContext Db;

        public QueryCodeModel() { }

        public QueryCodeModel(CMSDataContext db, string queries, List<Guid> guids = null)
        {
            Db = db;
            var all = Db.Connection.Query(queries).ToList();
            List = guids == null ? all : all.Where(vv => guids.Contains((Guid)vv.QueryId)).ToList();
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
            {
                return;
            }

            var c = Db.LoadExistingQuery(Existing.Value);
            Code = c.ToCode();
            Sql = c.ToSql(Db);
        }

        public string GetPythonCode(dynamic q)
        {
            Existing = q.QueryId as Guid?;
            if (Existing == null)
            {
                return string.Empty;
            }

            var c = Db.LoadExistingQuery(Existing.Value);
            var s = c.ToCode();
            var lines = s.SplitLines();
            string ret = null;
            string name = Regex.Replace(q.name, @"^F\d\d:", "", RegexOptions.IgnoreCase);
            string nameid = name.ToSuitableId();
            ret = lines.Length == 1
                ? $"model.CreateQueryTag(\"{nameid}\", \"{s}\")\n\n"
                : $"model.CreateQueryTag(\"{nameid}\", '''\t{string.Join("\n\t", lines)}\n''')\n\n";
            return ret;
        }
        public string GetSqlCode(dynamic q)
        {
            Existing = q.QueryId as Guid?;
            if (Existing == null)
            {
                return string.Empty;
            }

            string name = Regex.Replace(q.name, @"^F\d\d:", "", RegexOptions.IgnoreCase);
            string nameid = name.ToSuitableId();
            return $"\t\t,{nameid} = IIF(EXISTS(SELECT NULL FROM dbo.TagPerson tp JOIN dbo.Tag t ON t.Name = '{name}' AND t.TypeId = 99 AND t.Id = tp.Id WHERE tp.PeopleId = p.PeopleId), 1, 0)\n";
        }

        public string ServerLink(string path)
        {
            return Db.ServerLink(path);
        }
    }
}

