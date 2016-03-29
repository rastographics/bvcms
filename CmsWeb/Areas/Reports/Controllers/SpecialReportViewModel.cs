using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Controllers
{
    public class SpecialReportViewModel
    {
        public string Report { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public Guid Id { get; set; }
        public string Results { get; set; }

        public SpecialReportViewModel(string report, Guid id)
        {
            Report = report;
            Name = report.SpaceCamelCase();
            Id = id;
        }

        public void RunSqlReport()
        {
            var content = DbUtil.Db.ContentOfTypeSql(Report);
            if (content == null)
                throw new Exception("no content");
            if (!CanRunScript(content.Body))
                throw new Exception("Not Authorized to run this script");

            var hasqtag = content.Body.Contains("@qtagid");
            var hascurrentorg = content.Body.Contains("@CurrentOrgId");
            if (!hasqtag && !hascurrentorg)
                throw new Exception("missing @qtagid or @CurrentOrgId");

            var p = new DynamicParameters();
            if (hasqtag)
            {
                var tag = DbUtil.Db.PopulateSpecialTag(Id, DbUtil.TagTypeId_Query);
                p.Add("@qtagid", tag.Id);
            }
            if (hascurrentorg)
            {
                var oid = DbUtil.Db.CurrentOrgId0;
                p.Add("@CurrentOrgId", oid);
                if (oid > 0)
                    Name2 = DbUtil.Db.LoadOrganizationById(oid).FullName2;
            }

            var cs = HttpContext.Current.User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                using (var rd = cn.ExecuteReader(content.Body, p))
                    Results = GridResult.Table(rd, Name2);
            }
        }

        public void RunPyScript()
        {
            var content = DbUtil.Db.ContentOfTypePythonScript(Report);
            if (content == null)
                throw new Exception("no script named " + Report);
            if (!CanRunScript(content))
                throw new Exception("Not Authorized to run this script");
            if (!content.Contains("BlueToolbarReport") && !content.Contains("@BlueToolbarTagId"))
                throw new Exception("Missing Call to Query Function 'BlueToolbarReport'");
            if (Id == Guid.Empty)
                throw new Exception("Must be run from the BlueToolbar");

            var pe = new PythonModel(Util.Host);

            pe.DictionaryAdd("BlueToolbarGuid", Id.ToCode());
            foreach (var key in HttpContext.Current.Request.QueryString.AllKeys)
                pe.DictionaryAdd(key, HttpContext.Current.Request.QueryString[key]);

            pe.RunScript(content);
            Results = pe.Output;
        }

        private bool CanRunScript(string script)
        {
            if (!script.StartsWith("#Roles=") && !script.StartsWith("--Roles"))
                return true;
            var re = new Regex("(--|#)Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim());
            if (!roles.Any(rr => HttpContext.Current.User.IsInRole(rr)))
                return false;
            return true;
        }
    }
}