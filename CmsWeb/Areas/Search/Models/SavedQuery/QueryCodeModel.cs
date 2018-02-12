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
        public string Sql;

        public QueryCodeModel(string queries, List<Guid> guids = null)
        {
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
            Sql = c.ToSql();
        }
    }
}
