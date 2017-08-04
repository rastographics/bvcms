using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData.View;
using Dapper;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchSqlLookupRe = @"{sqllookup:(?<file>.*),\s*(?<col>.*)}";
        private static readonly Regex SqlLookupRe = new Regex(MatchSqlLookupRe, RegexOptions.Singleline);

        private Dictionary<string, List<Dictionary<string, object>>> sqlLookupinfos;
        private Dictionary<string, object> GetSqlInfo(string sqlfile, string column, EmailQueueTo emailqueueto)
        {
            var pid = emailqueueto.PeopleId as object;
            if (sqlLookupinfos == null)
                sqlLookupinfos = new Dictionary<string, List<Dictionary<string, object>>>();
            if (!sqlLookupinfos.ContainsKey(sqlfile))
            {
                var sql = db.ContentOfTypeSql(sqlfile);
                sqlLookupinfos[sqlfile] = db.Connection.Query(sql).Cast<Dictionary<string, object>>().ToList();
            }
            var list = sqlLookupinfos[sqlfile];
            return list.Single(vv => vv["PeopleId"] == pid);
        }

        public string SqlLookupReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = SqlLookupRe.Match(code);
            var sqlfile = match.Groups["file"].Value;
            var column = match.Groups["col"].Value;
            var i = GetSqlInfo(sqlfile, column, emailqueueto);
            return i?[column].ToString() ?? "";
        }
    }
}
