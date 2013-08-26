using System;
using System.Collections.Generic;
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
            return q.Sum(mm => mm.NumPresent);
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
        public static int Import(CMSDataContext Db, string text, string name)
        {
            var x = XDocument.Parse(text);
            QueryBuilderClause c = null;
            foreach (var xc in x.Root.Elements())
            {
                if (name.HasValue())
                    c = InsertClause(Db, xc, null, name);
                else
                    c = InsertClause(Db, xc, null, null);
            }
            return c.QueryId;
        }
        public string Export(int id, string name)
        {
            var clause = Db.LoadQueryById(id);
            var settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            using (var sw = new StringWriter())
            {
                using (var w = XmlWriter.Create(sw, settings))
                    WriteClause(clause, w);
                return sw.ToString();
            }
        }
        private void WriteClause(QueryBuilderClause clause, XmlWriter w)
        {
            w.WriteStartElement("Condition");
            w.WriteAttributeString("ClauseOrder", clause.ClauseOrder.ToString());
            w.WriteAttributeString("Field", clause.Field);
            if (clause.Description.HasValue())
                w.WriteAttributeString("Description", clause.Description);
            w.WriteAttributeString("Comparison", clause.Comparison);
            if (clause.TextValue.HasValue())
                w.WriteAttributeString("TextValue", clause.TextValue);
            if (clause.DateValue.HasValue)
                w.WriteAttributeString("DateValue", clause.DateValue.ToString());
            if (clause.CodeIdValue.HasValue())
                w.WriteAttributeString("CodeIdValue", clause.CodeIdValue);
            if (clause.StartDate.HasValue)
                w.WriteAttributeString("StartDate", clause.StartDate.ToString());
            if (clause.EndDate.HasValue)
                w.WriteAttributeString("EndDate", clause.EndDate.ToString());
            if (clause.Program > 0)
                w.WriteAttributeString("Program", clause.Program.ToString());
            if (clause.Division > 0)
                w.WriteAttributeString("Division", clause.Division.ToString());
            if (clause.Organization > 0)
                w.WriteAttributeString("Organization", clause.Organization.ToString());
            if (clause.Days > 0)
                w.WriteAttributeString("Days", clause.Days.ToString());
            if (clause.Quarters.HasValue())
                w.WriteAttributeString("Quarters", clause.Quarters);
            if (clause.Tags.HasValue())
                w.WriteAttributeString("Tags", clause.Tags);
            if (clause.Schedule > 0)
                w.WriteAttributeString("Schedule", clause.Schedule.ToString());
            if (clause.Age.HasValue)
                w.WriteAttributeString("Age", clause.Age.ToString());
            foreach (var qc in clause.Clauses)
                WriteClause(qc, w);
            w.WriteEndElement();
        }
        private static QueryBuilderClause InsertClause(CMSDataContext Db, XElement r, int? parent, string name = null)
        {
            var c = new QueryBuilderClause
            {
                Field = Attribute(r, "Field"),
                GroupId = parent,
                ClauseOrder = Attribute(r, "ClauseOrder").ToInt(),
                Comparison = Attribute(r, "Comparison"),
                TextValue = Attribute(r, "TextValue"),
                DateValue = AttributeDate(r, "DateValue"),
                CodeIdValue = Attribute(r, "CodeIdValue"),
                StartDate = AttributeDate(r, "StartDate"),
                EndDate = AttributeDate(r, "EndDate"),
                Program = Attribute(r, "Program").ToInt(),
                Division = Attribute(r, "Division").ToInt(),
                Organization = Attribute(r, "Organization").ToInt(),
                Days = Attribute(r, "Days").ToInt(),
                Quarters = Attribute(r, "Quarters"),
                Tags = Attribute(r, "Tags"),
                Schedule = Attribute(r, "Schedule").ToInt(),
                Age = Attribute(r, "Age").ToInt(),
                Description = name,
                SavedBy = Util.UserName
            };
            Db.QueryBuilderClauses.InsertOnSubmit(c);
            Db.SubmitChanges();
            if (c.Field == "Group")
                foreach (var rr in r.Elements())
                    InsertClause(Db, rr, c.QueryId);
            return c;
        }
        private static string Attribute(XElement r, string attr)
        {
            return Attribute(r, attr, null);
        }
        private static string Attribute(XElement r, string attr, string def)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return def;
            return a.Value;
        }
        private static DateTime? AttributeDate(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return null;
            return a.Value.ToDate();
        }
    }
}
