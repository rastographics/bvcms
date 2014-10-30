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
        }

        public PythonEvents(string dbname, string classname, string script)
        {
            var cs = Util.GetConnectionString(dbname);
            db = new CMSDataContext(cs);
            db.Host = dbname;
            var engine = Python.CreateEngine();
            var sc = engine.CreateScriptSourceFromString(script);

            var code = sc.Compile();
            var scope = engine.CreateScope();
            scope.SetVariable("model", this);
            code.Execute(scope);
            db.SubmitChanges();

            dynamic Event = scope.GetVariable(classname);
            instance = Event();
        }

        public PythonEvents(string dbname)
        {
            db = new CMSDataContext(Util.GetConnectionString(dbname));
            db.Host = dbname;
        }

        public string Script { get; set; } // set this in the python code for javascript on the output page
        public string Header { get; set; } // set this in the python code for output page
        public string Output { get; set; } // this is set automatically for the output page

        public PythonEvents(string dbname, string script)
        {
            var engine = Python.CreateEngine();
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            engine.Runtime.IO.SetOutput(ms, sw);
            engine.Runtime.IO.SetErrorOutput(ms, sw);
            var sc = engine.CreateScriptSourceFromString(script);
            var code = sc.Compile();
            var scope = engine.CreateScope();
            db = new CMSDataContext(Util.GetConnectionString(dbname));
            db.Host = dbname;
            scope.SetVariable("model", this);
            var qf = new QueryFunctions(db);
            scope.SetVariable("q", qf);
            code.Execute(scope);
            db.SubmitChanges();
            ms.Position = 0;
            var sr = new StreamReader(ms);
            Output = sr.ReadToEnd();
        }

        public static string RunScript(string dbname, string script)
        {
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
                var pe = new PythonEvents(dbname);
                scope.SetVariable("model", pe);
                var qf = new QueryFunctions(pe.db);
                scope.SetVariable("q", qf);
                code.Execute(scope);
                pe.db.SubmitChanges();
                ms.Position = 0;
                var sr = new StreamReader(ms);
                var s = sr.ReadToEnd();
                return s;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string RunScript(string script)
        {
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
                scope.SetVariable("model", this);
                var qf = new QueryFunctions(db);
                scope.SetVariable("q", qf);
                code.Execute(scope);
                db.SubmitChanges();
                ms.Position = 0;
                var sr = new StreamReader(ms);
                var s = sr.ReadToEnd();
                return s;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CallScript(string scriptname)
        {
            var script = db.Content(scriptname);
            var engine = Python.CreateEngine();
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            engine.Runtime.IO.SetOutput(ms, sw);
            engine.Runtime.IO.SetErrorOutput(ms, sw);
            var sc = engine.CreateScriptSourceFromString(script.Body);
            var code = sc.Compile();
            var scope = engine.CreateScope();
            var pe = new PythonEvents(db.Host);
            scope.SetVariable("model", pe);
            var qf = new QueryFunctions(pe.db);
            scope.SetVariable("q", qf);
            code.Execute(scope);
            pe.db.SubmitChanges();
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

        public int DayOfWeek
        {
            get { return DateTime.Today.DayOfWeek.ToInt(); }
        }

        public DateTime DateTime
        {
            get { return DateTime.Now; }
        }

        public DateTime DateAddDays(object dt, int days)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.AddDays(days);
        }

        public int WeekNumber(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.GetWeekNumber();
        }

        public DateTime SundayForDate(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.Sunday();
        }

        public DateTime SundayForWeek(int year, int week)
        {
            return Util.SundayForWeek(year, week);
        }

        public DateTime MostRecentAttendedSunday(int progid)
        {
            var q = from m in db.Meetings
                where m.MeetingDate.Value.Date.DayOfWeek == 0
                where m.MaxCount > 0
                where
                    progid == 0 || m.Organization.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                where m.MeetingDate < Util.Now
                orderby m.MeetingDate descending
                select m.MeetingDate.Value.Date;
            var dt = q.FirstOrDefault();
            if (dt == DateTime.MinValue) //Sunday Date equal/before today
                dt = DateTime.Today.AddDays(-(int) DateTime.Today.DayOfWeek);
            return dt;
        }

        public bool TestEmail { get; set; }
        public bool Transactional { get; set; }
        public int CurrentOrgId { get; set; }

        public bool SmtpDebug
        {
            set { Util.SmtpDebug = value; }
        }

        public void Email2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            var q = db.PeopleQuery(qid);
            Email2(q, queuedBy, fromAddr, fromName, subject, body);
        }

        private void Email2(IQueryable<Person> q, int queuedBy, string fromAddr, string fromName, string subject,
            string body)
        {
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

        public void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject,
            string contentName)
        {
            var c = db.Content(contentName);
            if (c == null)
                return;
            Email2(qid, queuedBy, fromAddr, fromAddr, subject, c.Body);
        }

        public void Email(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            var q = db.PeopleQuery2(savedQuery);
            if (q == null)
                return;
            Email2(q, queuedBy, fromAddr, fromName, subject, body);
        }

        public void EmailContent(string savedQuery, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.Content(contentName);
            if (c == null)
                return;
            EmailContent(savedQuery, queuedBy, fromAddr, fromName, c.Title, contentName);
        }

        public void EmailContent(string savedQuery, int queuedBy, string fromAddr, string fromName, string subject,
            string contentName)
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
                where progid == 0 || o.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                select o.OrganizationId;
            return q.ToList();
        }

        public void AddExtraValueCode(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraValue(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueText(object savedQuery, string name, string text)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraData(db, pid, name, text);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueDate(object savedQuery, string name, object dt)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            var dt2 = dt.ToDate();
            foreach (var pid in list)
            {
                Person.AddEditExtraDate(db, pid, name, dt2);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueInt(object savedQuery, string name, int n)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraInt(db, pid, name, n);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void AddExtraValueBool(object savedQuery, string name, bool b)
        {
            var list = db.PeopleQuery2(savedQuery).Select(ii => ii.PeopleId).ToList();
            foreach (var pid in list)
            {
                Person.AddEditExtraBool(db, pid, name, b);
                db.SubmitChanges();
                ResetDb();
            }
        }

        public void UpdateCampus(object savedQuery, string campus)
        {
            var id = db.FetchOrCreateCampusId(campus);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("CampusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateMemberStatus(object savedQuery, string status)
        {
            var id = Person.FetchOrCreateMemberStatus(db, status);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("MemberStatusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassStatus(object savedQuery, string status)
        {
            var id = Person.FetchOrCreateNewMemberClassStatus(db, status);
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("NewMemberClassStatusId", id);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void UpdateNewMemberClassDate(object savedQuery, object dt)
        {
            var q = db.PeopleQuery2(savedQuery);
            foreach (var p in q)
            {
                p.UpdateValue("NewMemberClassDate", dt);
                p.LogChanges(db);
                db.SubmitChanges();
            }
        }

        public void AddMembersToOrg(object savedQuery, int OrgId)
        {
            var q = db.PeopleQuery2(savedQuery);
            var dt = DateTime.Now;
            foreach (var p in q)
            {
                OrganizationMember.InsertOrgMembers(db, OrgId, p.PeopleId, MemberTypeCode.Member, dt, null, false);
                db.SubmitChanges();
            }
        }

        public void AddMemberToOrg(int pid, int OrgId)
        {
            AddMembersToOrg("peopleid=" + pid, OrgId);
        }
    }
}