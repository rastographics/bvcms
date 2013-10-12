using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Xml.Linq;
using System.Data.Linq.SqlClient;
using IronPython.Hosting;
using System.IO;
using CmsData.Codes;
using System.Web;
using CmsData.API;

namespace CmsData
{
    public class QueryFunctions
    {
        private CMSDataContext Db;

        public QueryFunctions()
        {
            Db = new CMSDataContext("Data Source=.;Initial Catalog=CMS_bellevue;Integrated Security=True");
        }
        public QueryFunctions(CMSDataContext Db)
        {
            this.Db = Db;
        }
        public static string VitalStats(CMSDataContext Db)
        {
            var qf = new QueryFunctions(Db);
            var script = Db.Content("VitalStats");
            if (script == null)
                return "no VitalStats script";
#if DEBUG2
            var options = new Dictionary<string, object>();
            options["Debug"] = true;
            var engine = Python.CreateEngine(options);
            var paths = engine.GetSearchPaths();
            paths.Add(path);
            engine.SetSearchPaths(paths);
            var sc = engine.CreateScriptSourceFromFile(HttpContext.Current.Server.MapPath("/MembershipAutomation2.py"));
#else
            var engine = Python.CreateEngine();
            var sc = engine.CreateScriptSourceFromString(script.Body);
#endif

            try
            {
                var code = sc.Compile();
                var scope = engine.CreateScope();
                code.Execute(scope);

                dynamic VitalStats = scope.GetVariable("VitalStats");
                dynamic m = VitalStats();
                return m.Run(qf);
            }
            catch (Exception ex)
            {
                return "VitalStats script error: " + ex.Message;
            }
        }
        public int MeetingCount(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in Db.Meetings
                    where m.MeetingDate >= dt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }
        public int NumPresent(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in Db.Meetings
                    where m.MeetingDate >= dt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            if (!q.Any())
                return 0;
            return q.Sum(mm => mm.MaxCount ?? 0);
        }

        public int RegistrationCount(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in Db.OrganizationMembers
                    where m.EnrollmentDate >= dt
                    where m.Organization.RegistrationTypeId > 0
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }
        public decimal ContributionTotals(int days1, int days2, int fundid)
        {
            return ContributionTotals(days1, days2, fundid.ToString());
        }

        public int ContributionCount(int days1, int days2, int fundid)
        {
            return ContributionCount(days1, days2, fundid.ToString());
        }

        public int ContributionCount(int days, int fundid)
        {
            return ContributionCount(days, fundid.ToString());
        }
        public int QueryCount(string s)
        {
            var qB = Db.QueryBuilderClauses.FirstOrDefault(c => c.Description == s);
            if (qB == null)
                return 0;
            var q = Db.People.Where(qB.Predicate(Db));
            return q.Count();
        }
        public decimal ContributionTotals(int days1, int days2, string funds)
        {
            var fundids = (from f in funds.Split(',')
                           let i = f.ToInt()
                           where i > 0
                           select i).ToArray();
            var exfundids = (from f in funds.Split(',')
                             let i = f.ToInt()
                             where i < 0
                             select -i).ToArray();

            var dt1 = DateTime.Now.AddDays(-days1);
            var dt2 = DateTime.Now.AddDays(-days2);
            var typs = new int[] { 6, 7 };
            var q = from c in Db.Contributions
                    where c.ContributionDate >= dt1
                    where days2 == 0 || c.ContributionDate <= dt2
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where fundids.Length == 0 || fundids.Contains(c.FundId)
                    where exfundids.Length == 0 || !exfundids.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Sum(c => c.ContributionAmount) ?? 0;
        }

        public int ContributionCount(int days1, int days2, string funds)
        {
            var fundids = (from f in funds.Split(',')
                           let i = f.ToInt()
                           where i > 0
                           select i).ToArray();
            var exfundids = (from f in funds.Split(',')
                             let i = f.ToInt()
                             where i < 0
                             select -i).ToArray();

            var dt1 = DateTime.Now.AddDays(-days1);
            var dt2 = DateTime.Now.AddDays(-days2);
            var typs = new int[] { 6, 7 };
            var q = from c in Db.Contributions
                    where c.ContributionDate >= dt1
                    where days2 == 0 || c.ContributionDate <= dt2
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where c.ContributionAmount > 0
                    where fundids.Length == 0 || fundids.Contains(c.FundId)
                    where exfundids.Length == 0 || !exfundids.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Count();
        }

        public int ContributionCount(int days, string funds)
        {
            var fundids = (from f in funds.Split(',')
                           let i = f.ToInt()
                           where i > 0
                           select i).ToArray();
            var exfundids = (from f in funds.Split(',')
                             let i = f.ToInt()
                             where i < 0
                             select -i).ToArray();

            var dt = DateTime.Now.AddDays(-days);
            var typs = new int[] { 6, 7 };
            var q = from c in Db.Contributions
                    where c.ContributionDate >= dt
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where c.ContributionAmount > 0
                    where fundids.Length == 0 || fundids.Contains(c.FundId)
                    where exfundids.Length == 0 || !exfundids.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Count();
        }

    }
}
