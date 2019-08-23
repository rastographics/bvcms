using CmsData;
using CmsData.Codes;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class SqlScriptModel
    {
        private CMSDataContext Db;

        public SqlScriptModel(CMSDataContext db)
        {
            Db = db;
        }

        internal string AddParametersForSql(string parameter, string sql, DynamicParameters p, dynamic ViewBag)
        {
            if (sql.Contains("@qtagid", ignoreCase: true))
            {
                var id = Db.FetchLastQuery().Id;
                var tag = Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                int? qtagid = tag.Id;
                p.Add("@qtagid", qtagid);
                ViewBag.Type = "SqlReport";
            }

            if (sql.Contains("@BlueToolbarTagId", ignoreCase: true))
            {
                var id = Db.FetchLastQuery().Id;
                var tag = Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                int? qtagid = tag.Id;
                p.Add("@BlueToolbarTagId", qtagid);
                ViewBag.Type = "SqlReport";
            }
            else if (sql.Contains("@CurrentOrgId", ignoreCase: true))
            {
                var oid = Db.CurrentSessionOrgId;
                p.Add("@CurrentOrgId", oid);
                if (oid > 0)
                {
                    var name = Db.LoadOrganizationById(oid).FullName2;
                    ViewBag.Name2 = name;
                    ViewBag.Type = "SqlReport";
                }
            }
            else if (sql.Contains("@OrgIds", ignoreCase: true))
            {
                var oid = Db.CurrentSessionOrgId;
                if (oid != 0 || !p.Contains("@OrgIds"))
                {
                    p.Add("@OrgIds", oid.ToString());
                }

                ViewBag.Type = "OrgSearchSqlReport";
                if (sql.Contains("--class=StartEndReport"))
                {
                    p.Add("@MeetingDate1", DateTime.Now.AddDays(-90));
                    p.Add("@MeetingDate2", DateTime.Now);
                }
            }
            else if (sql.Contains("--class=TotalsByFund"))
            {
                ViewBag.Type = "TotalsByFundSqlReport";
                p.Add("@StartDate", dbType: DbType.DateTime);
                p.Add("@EndDate", dbType: DbType.DateTime);
                p.Add("@CampusId", dbType: DbType.Int32);
                p.Add("@Online", dbType: DbType.Boolean);
                p.Add("@TaxNonTax", dbType: DbType.Boolean);
                p.Add("@FundSet", dbType: DbType.String);
                p.Add("@IncludeUnclosedBundles", dbType: DbType.Boolean);
                p.Add("@ActiveTagFilter", dbType: DbType.Int64);
            }
            else
            {
                ViewBag.Type = "SqlReport";
            }

            if (sql.Contains("@StartDt"))
            {
                p.Add("@StartDt", new DateTime(DateTime.Now.Year, 1, 1));
            }

            if (sql.Contains("@EndDt"))
            {
                p.Add("@EndDt", DateTime.Today);
            }

            if (sql.Contains("@userid", ignoreCase: true))
            {
                p.Add("@userid", Util.UserId);
            }
#if DEBUG
            foreach (var name in p.ParameterNames)
            {
                sql = QueryFunctions.RemoveDeclaration(name, sql);
            }
#endif

            sql = QueryFunctions.AddP1Parameter(sql, parameter, p);

            return sql;
        }

        internal static bool CanRunScript(string script)
        {
            if (!script.StartsWith("--Roles", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var re = new Regex("--Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim()).ToArray();
            if (roles.Length > 0)
            {
                return roles.Any(rr => HttpContextFactory.Current.User.IsInRole(rr));
            }

            return true;
        }

        public string FetchScript(string name)
        {
#if DEBUG
            name = ParseDebuggingName(name);
#endif
            var script = Db.ContentOfTypeSql(name);
            return script;
        }
        public DynamicParameters FetchParameters()
        {
            var request = HttpContextFactory.Current.Request;
            var d = request.QueryString.AllKeys.ToDictionary(key => key, key => request.QueryString[key]);
            var p = new DynamicParameters();
            foreach (var kv in d)
            {
                p.Add("@" + kv.Key, kv.Value);
            }
            return p;
        }
#if DEBUG
        public string ParseDebuggingName(string name)
        {
            var runfromUrlRe = new Regex(@"([c-e]![\w-]*-)(\w*)\.sql(-kw-([^/]*)){0,1}");
            if (runfromUrlRe.IsMatch(name))
            {
                var match = runfromUrlRe.Match(name);
                var debuggingName = match.Groups[0].Value;
                var contentName = match.Groups[2].Value;
                var runFromPath = match.Groups[1].Value
                                  .Replace("!", ":\\")
                                  .Replace("-", "\\")
                              + contentName + ".sql";
                var keyword = match.Groups[4].Value;
                var script = System.IO.File.ReadAllText(runFromPath);

                Db.WriteContentSql(contentName, script, keyword);
                return contentName;
            }

            return name;
        }
#endif
    }
}
