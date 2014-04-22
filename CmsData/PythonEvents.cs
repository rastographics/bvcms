using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using CmsData.Codes;
using UtilityExtensions;
using IronPython.Hosting;
using System;

namespace CmsData
{
    public class PythonEvents
    {
        private CMSDataContext db;
        public dynamic instance { get; set; }

        private void ResetDb()
        {
            if (db == null)
                return;
            var cs = db.Connection.ConnectionString;
            db.Dispose();
            db = new CMSDataContext(cs);
            if (!db.CmsHost.HasValue())
                db.CmsHost = Util.ServerLink();
        }

        public PythonEvents(CMSDataContext db, string classname, string script)
        {
            this.db = db;
            if (!db.CmsHost.HasValue())
                db.CmsHost = Util.ServerLink();
            var engine = Python.CreateEngine();
            var sc = engine.CreateScriptSourceFromString(script);

            var code = sc.Compile();
            var scope = engine.CreateScope();
            scope.SetVariable("model", this);
            code.Execute(scope);

            dynamic Event = scope.GetVariable(classname);
            instance = Event();
        }
        public PythonEvents(CMSDataContext db)
        {
            this.db = db;
            if (!db.CmsHost.HasValue())
                db.CmsHost = Util.ServerLink();
        }
        public PythonEvents(string dbname = "bellevue")
        {
            db = new CMSDataContext("Data Source=.;Initial Catalog=CMS_{0};Integrated Security=True".Fmt(dbname));
            db.CmsHost = Util.ServerLink();
        }

        public static string RunScript(CMSDataContext db, string script)
        {
            var engine = Python.CreateEngine();
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            engine.Runtime.IO.SetOutput(ms, sw);
            engine.Runtime.IO.SetErrorOutput(ms, sw);
            var sc = engine.CreateScriptSourceFromString(script);
            var code = sc.Compile();
            var scope = engine.CreateScope();
            var pe = new PythonEvents(db);
            scope.SetVariable("model", pe);
            var qf = new QueryFunctions(db);
            scope.SetVariable("q", qf);
            code.Execute(scope);
            ms.Position = 0;
            var sr = new StreamReader(ms);
            var s = sr.ReadToEnd();
            return s;
        }

        // List of api functions to call from Python

        public void CreateTask(int forPeopleId, Person p, string description)
        {
            var t = p.AddTaskAbout(db, forPeopleId, description);
            db.SubmitChanges();
            db.Email(db.Setting("AdminMail", "support@bvcms.com"), db.LoadPersonById(forPeopleId),
                "TASK: " + description,
                Task.TaskLink(db, description, t.Id) + "<br/>" + p.Name);
        }
        public void JoinOrg(int orgId, Person p)
        {
            OrganizationMember.InsertOrgMembers(db, orgId, p.PeopleId, 220, DateTime.Now, null, false);
        }
        public void UpdateField(Person p, string field, object value)
        {
            p.UpdateValue(field, value);
        }
        public void EmailReminders(int orgId)
        {
            var org = db.LoadOrganizationById(orgId);
            var m = new API.APIOrganization(db);
            if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                m.SendVolunteerReminders(orgId, false);
            else
                m.SendEventReminders(orgId);
        }

        public int DayOfWeek { get { return DateTime.Today.DayOfWeek.ToInt(); } }
        public DateTime DateTime { get { return DateTime.Now; } }

        public bool TestEmail { get; set; }
        public bool Transactional { get; set; }
        public int CurrentOrgId { get; set; }
        public string CmsHost { set { db.CmsHost = value; } }
        public bool SmtpDebug { set { Util.SmtpDebug = value; } }

        public void Email2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            var q = db.PeopleQuery(qid);

            var from = new MailAddress(fromAddr, fromName);
            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

            Util.IsInRoleEmailTest = TestEmail;
            var queueremail = db.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).Single();
            Util.UserEmail = queueremail;
            db.CurrentOrgId = CurrentOrgId;
            var emailqueue = db.CreateQueue(queuedBy, from, subject, body, null, tag.Id, false);
            emailqueue.Transactional = Transactional;
            db.SendPeopleEmail(emailqueue.Id);
        }
        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.Content(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromName, c.Title, c.Body);
        }
        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string contentName)
        {
            var c = db.Content(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromAddr, subject, c.Body);
        }

        public void Email(string savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            var qB = db.Queries.FirstOrDefault(c => c.Name == savedQuery);
            if (qB == null)
                return;
            Email2(qB.QueryId, queuedBy, fromAddr, fromName, subject, body);
        }
        public void EmailContent(string savedQuery, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.Content(contentName);
            if (c == null)
                return;
            EmailContent(savedQuery, queuedBy, fromAddr, fromName, c.Title, contentName);
        }
        public void EmailContent(string savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName)
        {
            var c = db.Content(contentName);
            if (c == null)
                return;
            var qB = db.Queries.FirstOrDefault(cc => cc.Name == savedQuery);
            if (qB == null)
                return;
            Email2(qB.QueryId, queuedBy, fromAddr, fromName, subject, c.Body);
        }

        public Guid OrgMembersQuery(int progid, int divid, int orgid, string memberTypes)
        {
            var c = db.ScratchPadCondition();
            c.Reset(db);
            var mtlist = memberTypes.Split(',');
            var mts = string.Join(";", from mt in db.MemberTypes
                where mtlist.Contains(mt.Description)
                select "{0},{1}".Fmt(mt.Id, mt.Code));
            var clause = c.AddNewClause(QueryType.MemberTypeCodes, CompareType.OneOf, mts);
            clause.Program = progid;
            clause.Division = divid;
            clause.Organization = orgid;
            c.Save(db);
            return c.Id;
        }

        public List<int> OrganizationIds(int progid, int divid)
        {
            var q = from o in db.Organizations
                where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                select o.OrganizationId;
            return q.ToList();
        }

        public void AddxtraValueCode(string savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraValue(db, pid, name, text);
                db.SubmitChanges();
                db.Dispose();
            }
        }
        public void AddExtraValueText(string savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraData(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
            }
        }
        public void AddExtraValueDate(string savedQuery, string name, DateTime dt)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraDate(db, pid, name, dt);
                db.SubmitChanges();
                ResetDb();
            }
        }
        public void AddExtraValueInt(string savedQuery, string name, int n)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraInt(db, pid, name, n);
                db.SubmitChanges();
                ResetDb();
            }
        }
        public void AddExtraValueBool(string savedQuery, string name, bool b)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraBool(db, pid, name, b);
                db.SubmitChanges();
                ResetDb();
            }
        }
    }
}
