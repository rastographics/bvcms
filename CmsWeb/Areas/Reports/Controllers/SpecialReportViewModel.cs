using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Models;
using Dapper;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Controllers
{
    public class SpecialReportViewModel
    {
        public string Report { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public Guid Id { get; set; }
        public int? Qtagid { get; set; }
        public string Results { get; set; }
        public CMSDataContext CurrentDatabase { get; set; }
        public string ExcelUrl => $"/Reports/SqlReportExcel/{Report}/{Id}";

        public SpecialReportViewModel(CMSDataContext db, string report, Guid id)
        {
            CurrentDatabase = db;
            Report = report;
            Name = report.SpaceCamelCase();
            Id = id;
        }

        public void RunSqlReport()
        {
            DynamicParameters p;
            var content = GetParameters(out p);

            using (var cn = CurrentDatabase.ReadonlyConnection())
            {
                cn.Open();
                using (var rd = cn.ExecuteReader(content, p))
                {
                    Results = GridResult.Table(rd, Name2);
                }
            }
        }
        public EpplusResult RunSqlExcel()
        {
            DynamicParameters p;
            var content = GetParameters(out p);
            using (var cn = CurrentDatabase.ReadonlyConnection())
            {
                cn.Open();
                return cn.ExecuteReader(content, p).ToExcel($"{Report.Replace(" ", "")}.xlsx", fromSql: true);
            }
        }

        private string GetParameters(out DynamicParameters p)
        {
            var content = CurrentDatabase.ContentOfTypeSql(Report);
            if (!content.HasValue())
            {
                throw new Exception("no content");
            }

            if (!CanRunScript(content))
            {
                throw new Exception("Not Authorized to run this script");
            }

            var hasqtag = content.Contains("@qtagid");
            var hasbtbtag = content.Contains("@BlueToolbarTagId");
            var hascurrentorg = content.Contains("@CurrentOrgId");
            if (!hasqtag && !hascurrentorg && !hasbtbtag)
            {
                throw new Exception("missing @qtagid or @CurrentOrgId or @BlueToolbarTagId");
            }

            p = new DynamicParameters();
            if (hasqtag)
            {
                var tag = CurrentDatabase.PopulateSpecialTag(Id, DbUtil.TagTypeId_Query);
                p.Add("@qtagid", tag.Id);
                Qtagid = tag.Id;
            }
            if (hasbtbtag)
            {
                var tag = CurrentDatabase.PopulateSpecialTag(Id, DbUtil.TagTypeId_Query);
                p.Add("@BlueToolbarTagId", tag.Id);
                Qtagid = tag.Id;
            }
            if (hascurrentorg)
            {
                var oid = CurrentDatabase.CurrentSessionOrgId;
                p.Add("@CurrentOrgId", oid);
                if (oid > 0)
                {
                    Name2 = CurrentDatabase.LoadOrganizationById(oid).FullName2;
                }
            }
            if (content.Contains("@userid"))
            {
                p.Add("@userid", CurrentDatabase.CurrentUser.UserId);
            }

            return content;
        }


        public void RunPyScript()
        {
            var content = CurrentDatabase.ContentOfTypePythonScript(Report);
            if (content == null)
            {
                throw new Exception("no script named " + Report);
            }

            if (!CanRunScript(content))
            {
                throw new Exception("Not Authorized to run this script");
            }

            if (!content.Contains("BlueToolbarReport") && !content.Contains("@BlueToolbarTagId"))
            {
                throw new Exception("Missing Call to Query Function 'BlueToolbarReport'");
            }

            if (Id == Guid.Empty)
            {
                throw new Exception("Must be run from the BlueToolbar");
            }

            var pe = new PythonModel(CurrentDatabase.Host);

            pe.DictionaryAdd("BlueToolbarGuid", Id.ToCode());
            foreach (var key in HttpContextFactory.Current.Request.QueryString.AllKeys)
            {
                pe.DictionaryAdd(key, HttpContextFactory.Current.Request.QueryString[key]);
            }

            pe.RunScript(content);
            Results = pe.Output;
        }

        public static bool CanRunScript(string script)
        {
            if (script.StartsWith("#Roles=") || script.StartsWith("--Roles"))
            {
                var re = new Regex("(--|#)Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
                var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim()).ToArray();
                if (roles.Length > 0)
                {
                    return HttpContextFactory.Current == null || roles.Any(rr => HttpContextFactory.Current.User.IsInRole(rr));
                }
            }
            return true;
        }
    }
}
