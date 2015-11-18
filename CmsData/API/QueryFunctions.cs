using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using UtilityExtensions;
using IronPython.Hosting;
using System.IO;
using CmsData.Codes;
using HandlebarsDotNet;
using Microsoft.Scripting.Hosting;

namespace CmsData
{
    public class QueryFunctions
    {
        private readonly CMSDataContext db;
        private DateTime? lastSunday;

        public QueryFunctions(CMSDataContext Db)
        {
            db = Db;
        }

        public QueryFunctions(string dbname)
        {
            db = DbUtil.Create(dbname);
        }

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
                    dt = DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek);
                lastSunday = dt;
                return dt;
            }
        }

        public static string VitalStats(CMSDataContext db)
        {
            var script = db.Content("VitalStats");
            if (script == null)
                return "no VitalStats script";

            if (!script.Body.Contains("class VitalStats"))
                return RunScript(db, script.Body);

            var engine = Python.CreateEngine();
            var sc = engine.CreateScriptSourceFromString(script.Body);
            var qf = new QueryFunctions(db);

            try
            {
                var code = sc.Compile();
                var scope = engine.CreateScope();
                code.Execute(scope);

                dynamic vitalStats = scope.GetVariable("VitalStats");
                dynamic m = vitalStats();
                return m.Run(qf);
            }
            catch (Exception ex)
            {
                return "VitalStats script error: " + ex.Message;
            }
        }

        public string RenderTemplate(string source, object data)
        {
            CssStyle.RegisterHelpers(db);
            var template = Handlebars.Compile(source);
            var result = template(data);
            return result;
        }

        public static string RunScript(CMSDataContext db, string script)
        {
            if (!script.HasValue())
                return "no script";

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
                return "Python script error: " + ex.Message;
            }
        }

        public int AttendanceTypeCountDateRange(int progid, int divid, int orgid, string attendtype, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);
            var a = attendtype.Split(',');

            var qt = from t in db.AttendTypes
                select t;

            if (attendtype.NotEqual("All"))
                qt = from t in qt
                    where a.Contains(t.Description)
                    select t;

            var ids = string.Join(",", qt.Select(t => t.Id));
            var q = db.AttendanceTypeAsOf(start, enddt, progid, divid, orgid, 0, ids);
            return q.Count();
        }

        public int AttendMemberTypeCountAsOf(DateTime startdt, DateTime enddt, string membertypes, string notmembertypes,
            int progid, int divid, int orgid)
        {
            enddt = enddt.AddHours(24);
            return db.AttendMemberTypeAsOf(startdt, enddt, progid, divid, orgid, membertypes, notmembertypes).Count();
        }

        public int AttendCountAsOf(DateTime startdt, DateTime enddt, bool guestonly, int progid, int divid, int orgid)
        {
            enddt = enddt.AddHours(24);
            return db.AttendedAsOf(progid, divid, orgid, startdt, enddt, guestonly).Count();
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
            var typs = new[] {6, 7};
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
            var typs = new[] {6, 7};
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
            var typs = new[] {6, 7};
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

        public double ContributionTotals(int days1, int days2, int fundid)
        {
            return ContributionTotals(days1, days2, fundid.ToString());
        }

        public int ContributionCount(int days1, int days2, int fundid)
        {
            return ContributionCount(days1, days2, fundid.ToString());
        }

        public int ContributionCount2(int days1, int days2, int fundid)
        {
            return ContributionCount(days1, days2, fundid.ToString());
        }

        public int ContributionCount(int days, int fundid)
        {
            return ContributionCount(days, fundid.ToString());
        }

        public int DecisionCountDateRange(string decisiontype, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);
            var a = decisiontype.Split(',');
            var q = from p in db.People
                where p.DecisionDate >= start
                where p.DecisionDate < enddt
                where a.Contains(p.DecisionType.Description)
                select p;
            return q.Count();
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

        public int MeetingCountDateHours(int progid, int divid, int orgid, object startdt, int hours)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddHours(hours);
            var q = from m in db.Meetings
                where m.MeetingDate >= start
                where orgid == 0 || m.OrganizationId == orgid
                where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                select m;
            return q.Count();
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

        public int NumPresentDateRange(int progid, int divid, int orgid, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);

            var q = from m in db.Meetings
                where m.MeetingDate >= start
                where m.MeetingDate <= enddt
                where orgid == 0 || m.OrganizationId == orgid
                where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                select m;
            if (!q.Any())
                return 0;
            return q.Sum(mm => mm.MaxCount ?? 0);
        }

        public int QueryCount(string s)
        {
            var qb = db.PeopleQuery2(s);
            if (qb == null)
                return 0;
            return qb.Count();
        }

        public int QueryCountDivDateRange(string s, string division, object startdt, int days)
        {
            return QueryCountProgIdDivDateRange(s, null, division, startdt, days);
        }

        public int QueryCountProgIdDivDateRange(string s, int? progid, string division, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);
            var divid = (from dd in db.Divisions
                where dd.Name == division
                where progid == null || dd.ProgId == progid
                select dd.Id).SingleOrDefault();
            db.QbStartDateOverride = start;
            db.QbEndDateOverride = enddt;
            db.QbDivisionOverride = divid;

            try
            {
                var qb = db.PeopleQuery2(s);
                if (qb == null)
                    return 0;
                return qb.Count();
            }
            finally
            {
                db.QbStartDateOverride = null;
                db.QbEndDateOverride = null;
                db.QbDivisionOverride = null;
            }
        }

        public int QueryCountDivDate(string s, string division, object startdt)
        {
            return QueryCountDivDate(s, null, division, startdt);
        }

        public int QueryCountDivDate(string s, int? progid, string division, object startdt)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var divid = (from dd in db.Divisions
                where dd.Name == division
                where progid == null || dd.ProgId == progid
                select dd.Id).SingleOrDefault();
            db.QbStartDateOverride = start;
            db.QbEndDateOverride = start;
            db.QbDivisionOverride = divid;

            try
            {
                var qb = db.PeopleQuery2(s);
                if (qb == null)
                    return 0;
                return qb.Count();
            }
            finally
            {
                db.QbStartDateOverride = null;
                db.QbEndDateOverride = null;
                db.QbDivisionOverride = null;
            }
        }

        public int QueryCountDateRange(string s, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);
            db.QbStartDateOverride = start;
            db.QbEndDateOverride = enddt;

            try
            {
                var qb = db.PeopleQuery2(s);
                if (qb == null)
                    return 0;
                return qb.Count();
            }
            finally
            {
                db.QbStartDateOverride = null;
                db.QbEndDateOverride = null;
                db.QbDivisionOverride = null;
            }
        }

        public int QueryCountDate(string s, object startdt)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            db.QbStartDateOverride = start;
            db.QbEndDateOverride = start;

            try
            {
                var qb = db.PeopleQuery2(s);
                if (qb == null)
                    return 0;
                return qb.Count();
            }
            finally
            {
                db.QbStartDateOverride = null;
                db.QbEndDateOverride = null;
                db.QbDivisionOverride = null;
            }
        }

        /* QueryList is designed to run a pre-saved query referenced by name which is passed in as a string in the function call.
        * The resulting collection of people records (limited to 1000) is returned as an IEnumerable so that all attributes of the
        * Person record are accessible
        */
        public IEnumerable<Person> QueryList(object savedQuery)
        {
            return db.PeopleQuery2(savedQuery).Take(1000);
        }
        public int TagQueryList(object savedQuery)
        {
            //db.Log($"TagQueryList {savedQuery}");
            var q = db.PeopleQuery2(savedQuery).Select(vv => vv.PeopleId);
            var tag = db.PopulateTemporaryTag(q);
            return tag.Id;
        }
        public int TagCount(int tagid)
        {
            var n = db.TagPeople.Count(v => v.Id == tagid);
            //db.Log($"TagCount {tagid}={n}");
            return n;
        }

        public IEnumerable<Person> QueryList2(object savedQuery, string orderbyparam, bool ascending)
        {
            var q = db.PeopleQuery2(savedQuery);

            switch (orderbyparam.ToLower())
            {
                case "age":
                    if (ascending)
                        q = from u in q
                            orderby u.Age, u.LastName, u.FirstName
                            select u;
                    else
                        q = from u in q
                            orderby u.Age descending, u.LastName, u.FirstName
                            select u;
                    break;
                case "birthday":
                    if (ascending)
                        q = from u in q
                            orderby u.BirthMonth, u.BirthDay, u.LastName, u.FirstName
                            select u;
                    else
                        q = from u in q
                            orderby u.BirthMonth descending, u.BirthDay descending, u.LastName, u.FirstName
                            select u;
                    break;
                case "name":
                    if (ascending)
                        q = from u in q
                            orderby u.LastName, u.FirstName
                            select u;
                    else
                        q = from u in q
                            orderby u.LastName descending, u.FirstName descending
                            select u;
                    break;
            }
            return q.Take(1000);
        }

        public IEnumerable<dynamic> QuerySql(string sqlscript, object p1)
        {
            return QuerySql(sqlscript, p1, null);
        }
        public IEnumerable<dynamic> QuerySql(string sql, object p1, Dictionary<string, string> d)
        {
            var p = new DynamicParameters();
            p.Add("@p1", p1 ?? "");
            if (d != null)
                foreach (var kv in d)
                    p.Add("@" + kv.Key, kv.Value);

            var q = db.Connection.Query(sql, p, commandTimeout: 300);
            return q;
        }


        /// <summary>
        /// The QueryCustReportTag function returns the first 1000 people records contained 
        /// in a special tag called "LatestPeopleFromCustRepContext" which get populated based on
        /// the context when a Python Script being called from the Blue Toolbar
        /// </summary>
        public IEnumerable<Person> QueryCustReportTag()
        {
            var tag = DbUtil.Db.FetchOrCreateTag("LatestPeopleFromCustRepContext", Util.UserPeopleId, DbUtil.TagTypeId_System);
            var tagged = db.TaggedPeople(tag.Id);
            
            var q = from p in db.People
                    join t in tagged on p.PeopleId equals t.PeopleId
                    select p;
            
            return q.Take(1000);
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

        public string SqlNameCountArray(string title, string sql)
        {
            //            var qb = db.PeopleQuery2(s);
            //            if (qb == null)
            //                return 0;
            var cs = db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            string declareqtagid = null;
            if (sql.Contains("@qtagid"))
            {
                var id = db.FetchLastQuery().Id;
                var tag = db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                declareqtagid = $"DECLARE @qtagid INT = {tag.Id}\n";
            }
            sql = $"{declareqtagid}{sql}";
            var q = cn.Query(sql);
            var list = q.Select(rr => new NameValuePair {Name = rr.Name, Value = rr.Cnt}).ToList();
            if (list.Count == 0)
                return @"[ ['No Data', 'Count'], ['Dummy Value 1', 1], ['Dummy Value 2', 2], ['Dummy Value 3', 3], ]";
            return $@"[
  ['{title}', 'Count'],
{string.Join(",\n", list)}
]";
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

        public class NameValuePair
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return $"  ['{Name}', {Value}]";
            }
        }


    }
}