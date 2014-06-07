using System;
using System.Linq;
using UtilityExtensions;
using IronPython.Hosting;
using System.IO;
using CmsData.Codes;

namespace CmsData
{
    public class QueryFunctions
    {
        private CMSDataContext db;

        public QueryFunctions(string dbname = "bellevue")
        {
            db = new CMSDataContext("Data Source=.;Initial Catalog=CMS_{0};Integrated Security=True".Fmt(dbname));
        }

        public QueryFunctions(CMSDataContext Db)
        {
            this.db = Db;
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

        public static string RunScript(CMSDataContext db, string script)
        {
            if (!script.HasValue())
                return "no VitalStats script";

            var qf = new QueryFunctions(db);
            var engine = Python.CreateEngine();
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            engine.Runtime.IO.SetOutput(ms, sw);
            engine.Runtime.IO.SetErrorOutput(ms, sw);
            var sc = engine.CreateScriptSourceFromString(script);
            try
            {
                var code = sc.Compile();
                var scope = engine.CreateScope();
                scope.SetVariable("q", qf);
                scope.SetVariable("db", db);
                code.Execute(scope);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "VitalStats script error: " + ex.Message;
            }
        }

        public int MeetingCount(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in db.Meetings
                    where m.MeetingDate >= dt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }

        public int AttendMemberTypeCountAsOf(DateTime startdt, DateTime enddt, string membertypes, int progid, int divid, int orgid)
        {
            enddt = enddt.AddHours(24);
            return db.AttendMemberTypeAsOf(startdt, enddt, progid, divid, orgid, membertypes).Count();
        }
        public int AttendCountAsOf(DateTime startdt, DateTime enddt, bool guestonly, int progid, int divid, int orgid)
        {
            enddt = enddt.AddHours(24);
            return db.AttendedAsOf(progid, divid, orgid, startdt, enddt, guestonly).Count();
        }

        public int NumPresent(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in db.Meetings
                    where m.MeetingDate >= dt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            if (!q.Any())
                return 0;
            return q.Sum(mm => mm.MaxCount ?? 0);
        }

        public int LastWeekAttendance(int progid, int divid, int starthour, int endhour)
        {
            var dt = LastSunday;
            var dt1 = dt.AddHours(starthour);
            var dt2 = dt.AddHours(endhour);

            var q = from p in db.Programs
                where progid == 0 || p.Id == progid
                from pd in p.ProgDivs
                where divid == 0 || pd.DivId == divid
                select (from dg in pd.Division.DivOrgs
                    from m in dg.Organization.Meetings
                    where m.MeetingDate >= dt1
                    where m.MeetingDate < dt2
                    select m.MaxCount).Sum() ?? 0;
            return q.Sum();
        }

        private DateTime? lastSunday;
        public DateTime LastSunday
        {
            get
            {
                if (lastSunday.HasValue)
                    return lastSunday.Value;
                var q = from m in db.Meetings
                        where m.MeetingDate.Value.Date.DayOfWeek == 0
                        where m.MaxCount > 0
                        where m.MeetingDate < Util.Now
                        orderby m.MeetingDate descending
                        select m.MeetingDate.Value.Date;
                var dt = q.FirstOrDefault();
                if (dt == DateTime.MinValue) //Sunday Date equal/before today
                    dt = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                lastSunday = dt;
                return dt;
            }
        }

        public int RegistrationCount(int days, int progid, int divid, int orgid)
        {
            var dt = DateTime.Now.AddDays(-days);
            var q = from m in db.OrganizationMembers
                    where m.EnrollmentDate >= dt
                    where m.Organization.RegistrationTypeId > 0
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }
        public double ContributionTotals(int days1, int days2, int fundid)
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
            var qb = db.PeopleQuery2(s);
            if (qb == null)
                return 0;
            return qb.Count();
        }

        public int StatusCount(string s)
        {
            var statusflags = s.Split(',');
            var q = from p in db.People
                    let ac = p.Tags.Count(tt => statusflags.Contains(tt.Tag.Name))
                    where ac == statusflags.Length
                    select p;
            return q.Count();
        }
        public double ContributionTotals(int days1, int days2, string funds)
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
            var q = from c in db.Contributions
                    where c.ContributionDate >= dt1
                    where days2 == 0 || c.ContributionDate <= dt2
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where fundids.Length == 0 || fundids.Contains(c.FundId)
                    where exfundids.Length == 0 || !exfundids.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return Convert.ToDouble(q.Sum(c => c.ContributionAmount) ?? 0);
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
            var q = from c in db.Contributions
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
            var q = from c in db.Contributions
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
