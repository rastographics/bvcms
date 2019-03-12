using CmsData;
using CmsData.Codes;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ScriptModel
    {
        public ScriptModel() { }
        internal static string RunScriptSql(string parameter, string body, DynamicParameters p, dynamic ViewBag)
        {
            if (!CanRunScript(body))
            {
                return "Not Authorized to run this script";
            }

            if (body.Contains("@qtagid", ignoreCase: true))
            {
                var id = DbUtil.Db.FetchLastQuery().Id;
                var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                int? qtagid = tag.Id;
                p.Add("@qtagid", qtagid);
                ViewBag.Type = "SqlReport";
            }
            if (body.Contains("@BlueToolbarTagId", ignoreCase: true))
            {
                var id = DbUtil.Db.FetchLastQuery().Id;
                var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                int? qtagid = tag.Id;
                p.Add("@BlueToolbarTagId", qtagid);
                ViewBag.Type = "SqlReport";
            }
            else if (body.Contains("@CurrentOrgId", ignoreCase: true))
            {
                var oid = DbUtil.Db.CurrentSessionOrgId;
                p.Add("@CurrentOrgId", oid);
                if (oid > 0)
                {
                    var name = DbUtil.Db.LoadOrganizationById(oid).FullName2;
                    ViewBag.Name2 = name;
                    ViewBag.Type = "SqlReport";
                }
            }
            else if (body.Contains("@OrgIds", ignoreCase: true))
            {
                var oid = DbUtil.Db.CurrentSessionOrgId;
                p.Add("@OrgIds", oid.ToString());
                ViewBag.Type = "OrgSearchSqlReport";
                if (body.Contains("--class=StartEndReport"))
                {
                    p.Add("@MeetingDate1", DateTime.Now.AddDays(-90));
                    p.Add("@MeetingDate2", DateTime.Now);
                }
            }
            else if (body.Contains("--class=TotalsByFund"))
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

            if (body.Contains("@StartDt"))
            {
                p.Add("@StartDt", new DateTime(DateTime.Now.Year, 1, 1));
            }

            if (body.Contains("@EndDt"))
            {
                p.Add("@EndDt", DateTime.Today);
            }

            if (body.Contains("@userid", ignoreCase: true))
            {
                p.Add("@userid", Util.UserId);
            }
#if DEBUG
            foreach (var name in p.ParameterNames)
            {
                body = QueryFunctions.RemoveDeclaration(name, body);
            }
#endif

            body = QueryFunctions.AddP1Parameter(body, parameter, p);

            return body;
        }

        internal static bool CanRunScript(string script)
        {
            if (!script.StartsWith("#Roles=", StringComparison.OrdinalIgnoreCase)
                    && !script.StartsWith("--Roles", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var re = new Regex("(--|#)Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim()).ToArray();
            if (roles.Length > 0)
            {
                return roles.Any(rr => HttpContextFactory.Current.User.IsInRole(rr));
            }

            return true;
        }

        public static Dictionary<string, string> CustomDepositImportMenuItems()
        {
            var q = from c in DbUtil.Db.Contents
                    where c.TypeID == ContentTypeCode.TypePythonScript
                    where c.Body.Contains("#class=UploadContributionsMenu")
                    select c;
            var list = new Dictionary<string, string>();
            foreach (var c in q)
            {
                list.Add(c.Name, ContributionsMenuTitle(c.Body));
            }
            return list;
        }

        public static string ContributionsMenuTitle(string body)
        {
            var re = new Regex(@"#class=UploadContributionsMenu,title=(?<title>.*)\r");
            return re.Match(body).Groups["title"].Value;
        }

        internal static void GetFilesContent(PythonModel pe)
        {
            var files = HttpContextFactory.Current.Request.Files;
            var a = files.AllKeys;
            for (var i = 0; i < a.Length; i++)
            {
                var file = files[i];
                var buffer = new byte[file.ContentLength];
                file.InputStream.Read(buffer, 0, file.ContentLength);
                System.Text.Encoding enc;
                string s = null;
                if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                {
                    enc = new System.Text.ASCIIEncoding();
                    s = enc.GetString(buffer, 3, buffer.Length - 3);
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    enc = new System.Text.UnicodeEncoding();
                    s = enc.GetString(buffer, 2, buffer.Length - 2);
                }
                else
                {
                    enc = new System.Text.ASCIIEncoding();
                    s = enc.GetString(buffer);
                }
                pe.DictionaryAdd(a[i], s);
            }
        }
        public static string Run(string name, PythonModel pe)
        {
            var script = DbUtil.Db.ContentOfTypePythonScript(name);
            if (pe.Dictionary("p1") != null)
            {
                script = script.Replace("@P1", pe.Dictionary("p1") ?? "NULL");
            }


            string runfromPath = null;
#if DEBUG
            var runfromRe = new Regex(@"#runfrom=(?<path>.*)\r");
            if (Regex.IsMatch(name, @"test\d*\.py"))
            {
                runfromPath = HttpContextFactory.Current.Server.MapPath($"~/{name}");
                script = System.IO.File.ReadAllText(HttpContextFactory.Current.Server.MapPath($"~/{name}"));
                var re1 = new Regex(@"#saveas=(?<saveas>.*)\r");
                var saveas = re1.Match(script).Groups["saveas"]?.Value;
                if (saveas.HasValue())
                {
                    SaveAsContent(saveas, script);
                }
            }
            else if (runfromRe.IsMatch(script))
            {
                runfromPath = runfromRe.Match(script).Groups["path"]?.Value;
                if (string.IsNullOrEmpty(runfromPath))
                {
                    throw new Exception($"no match for path");
                }

                script = System.IO.File.ReadAllText(runfromPath);
                SaveAsContent(name, script);
            }
            else
#endif

            if (!script.HasValue())
            {
                throw new Exception("no script named " + name);
            }

            pe.Data.Title = ContributionsMenuTitle(script);

#if DEBUG
            if (runfromPath.HasValue())
            {
                return PythonModel.ExecutePython(runfromPath, pe, fromFile: true);
            }
#endif
            return pe.RunScript(script);
        }

        private static void SaveAsContent(string saveas, string script)
        {
            var c = DbUtil.Db.Content(saveas, script, ContentTypeCode.TypePythonScript);
            c.Body = script;
            DbUtil.Db.SubmitChanges();
        }
    }

}
