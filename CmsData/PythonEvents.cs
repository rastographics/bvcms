using System.IO;
using System.Linq;
using System.Net.Mail;
using CmsData.Codes;
using Microsoft.Scripting.Hosting;
using UtilityExtensions;
using IronPython.Hosting;
using System;

namespace CmsData
{
	public class PythonEvents
	{
		private readonly CMSDataContext db;
		public dynamic instance { get; set; }

		public PythonEvents(CMSDataContext Db, string classname, string script)
		{
			this.db = Db;
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
			code.Execute(scope);
	        return ms.ToString();
	    }

	    // List of api functions to call from Python

		public void CreateTask(int forPeopleId, Person p, string description)
		{
			DbUtil.LogActivity("Adding Task about: {0}".Fmt(p.Name));
			var t = p.AddTaskAbout(db, forPeopleId, description);
			db.SubmitChanges();
            db.Email(DbUtil.SystemEmailAddress, DbUtil.Db.LoadPersonById(forPeopleId),
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

	    public void Email(string savedquery, int queuedBy, string fromaddr, string fromname, string subject, string body, bool transactional = false)
	    {
            var from = new MailAddress(fromaddr, fromname);
			var qB = db.QueryBuilderClauses.FirstOrDefault(c => c.Description == savedquery);
	        if (qB == null)
	            return;
            var q = db.PeopleQuery(qB.QueryId);
            if (qB.ParentsOf)
				q = db.PersonQueryParents(q);

            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);
	        var emailqueue = db.CreateQueue(queuedBy, from, subject, body, null, tag.Id, false);
            db.SendPeopleEmail(emailqueue.Id);
	    }
	    public void EmailContent(string savedquery, int queuedBy, string fromaddr, string fromname, string subject, string content)
	    {
            var from = new MailAddress(fromaddr, fromname);
			var qB = db.QueryBuilderClauses.FirstOrDefault(cc => cc.Description == savedquery);
	        if (qB == null)
	            return;
            var q = db.PeopleQuery(qB.QueryId);
            if (qB.ParentsOf)
				q = db.PersonQueryParents(q);

            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);
	        var c = db.Content(content);
	        if (c == null)
	            return;
#if DEBUG2
            var items = new string[] { "Purity", "Rite", "Launch", "Overview"};
            if(items.Any(ii => savedquery.Contains(ii)))
            {
    	        var emailqueue = db.CreateQueue(queuedBy, from, subject, c.Body, null, tag.Id, false);
                db.SendPeopleEmail(emailqueue.Id);
            }
#else
	        var emailqueue = db.CreateQueue(queuedBy, from, subject, c.Body, null, tag.Id, false);
            db.SendPeopleEmail(emailqueue.Id);
#endif
	    }
	}
}
