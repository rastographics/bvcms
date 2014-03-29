using System.IO;
using System.Linq;
using System.Net.Mail;
using CmsData.Codes;
using DevDefined.OAuth.Utility;
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
        }

        public PythonEvents(CMSDataContext db, string classname, string script)
        {
            this.db = db;
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
        }
        public PythonEvents()
        {
            db = new CMSDataContext("Data Source=.;Initial Catalog=CMS_bellevue;Integrated Security=True");
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
            var s = ms.ReadToEnd();
            return s;
        }

        // List of api functions to call from Python

        public void CreateTask(int forPeopleId, Person p, string description)
        {
            DbUtil.LogActivity("Adding Task about: {0}".Fmt(p.Name));
            var t = p.AddTaskAbout(db, forPeopleId, description);
            db.SubmitChanges();
            db.Email(DbUtil.SystemEmailAddress, db.LoadPersonById(forPeopleId),
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

        public void Email(string savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body, bool transactional = false)
        {
            var from = new MailAddress(fromAddr, fromName);
            var qB = db.Queries.FirstOrDefault(c => c.Name == savedQuery);
            if (qB == null)
                return;
            var q = db.PeopleQuery(qB.QueryId);

            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

            Util.IsInRoleEmailTest = TestEmail;
            var queueremail = db.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).Single();
            Util.UserEmail = queueremail;
            var emailqueue = db.CreateQueue(queuedBy, from, subject, body, null, tag.Id, false);
            db.SendPeopleEmail(emailqueue.Id);
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
            var from = new MailAddress(fromAddr, fromName);
            var qB = db.Queries.FirstOrDefault(cc => cc.Name == savedQuery);
            if (qB == null)
                return;
            var q = db.PeopleQuery(qB.QueryId);

            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);
            var c = db.Content(contentName);
            if (c == null)
                return;

            Util.IsInRoleEmailTest = TestEmail;
            var queueremail = db.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).Single();
            Util.UserEmail = queueremail;
            var emailqueue = db.CreateQueue(queuedBy, from, subject, c.Body, null, tag.Id, false);
            db.SendPeopleEmail(emailqueue.Id);
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
