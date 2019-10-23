using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public bool TestEmail { get; set; }
        public int MaxEmails { get; set; }
        public bool Transactional { get; set; }

        public bool SmtpDebug
        {
            set { Util.SmtpDebug = value; }
        }

        public string ContentForDate(string contentName, object date)
        {
            var dtwanted = date.ToDate();
            if (!dtwanted.HasValue)
                return "no date";
            dtwanted = dtwanted.Value.Date;
            var c = db.ContentOfTypeHtml(contentName);
            var a = Regex.Split(c.Body, @"<h1>\s*(?<dt>\d{1,2}(?:/|-)\d{1,2}(?:/|-)\d{2,4})=+\s*</h1>", RegexOptions.ExplicitCapture);
            var i = 0;
            for (; i < a.Length; i++)
            {
                if (a[i].Length < 6 || a[i].Length > 10)
                    continue;
                var dt = a[i].ToDate();
                if (dt.HasValue && dt == dtwanted)
                    return a[i + 1];
            }
            return "cannot find email content";
        }

        public void Email(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body, string cclist = null, DateTime? dateWanted = null) 
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                if (q == null)
                    return;
                Email2(db2, q, queuedBy, fromAddr, fromName, subject, body, cclist, dateWanted);
            }
        }
        public void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            EmailContent(savedQuery, queuedBy, fromAddr, fromName, c.Title, contentName);
        }

        public void EmailContentWithSubject(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName, string cclist=null, DateTime? dateWanted = null)
        {
            EmailContent2(savedQuery, queuedBy, fromAddr, fromName, subject, contentName, cclist: cclist, dateWanted: dateWanted);
        }

        public void EmailReminders(object orgId)
        {
            using (var db2 = NewDataContext())
            {
                var oid = orgId.ToInt();
                var org = db2.LoadOrganizationById(oid);
                var m = new APIOrganization(db2);
                Util.IsInRoleEmailTest = TestEmail;
                if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                    m.SendVolunteerReminders(oid, false);
                else
                    m.SendEventReminders(oid);
            }
        }

        public void EmailReport(object savedquery, int queuedBy, string fromaddr, string fromname, string subject, string report)
        {
            using (var db2 = NewDataContext())
            {
                var from = new MailAddress(fromaddr, fromname);
                var q = db2.PeopleQuery2(savedquery);

                q = from p in q
                    where p.EmailAddress != null
                    where p.EmailAddress != ""
                    where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                    select p;
                var tag = db2.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

                var script = db2.ContentOfTypePythonScript(report);
                if (script == null)
                    return;

                var emailbody = RunScript(script);
                var emailqueue = db2.CreateQueue(queuedBy, from, subject, emailbody, null, tag.Id, false);

                emailqueue.Transactional = Transactional;
                Util.IsInRoleEmailTest = TestEmail;

                db2.SendPeopleEmail(emailqueue.Id);
            }
        }


        /// <summary>
        ///     Overloaded version of EmailReport adds variables in the function call for QueryName and QueryDescription. The
        ///     original version of EmailReport
        ///     required you to embed the query name in the Python Script.  This version of the function permits you to have a
        ///     generic Python script
        ///     and then call it multiple times with a different query and description each time.
        /// </summary>
        public void EmailReport(string savedquery, int queuedBy, string fromaddr, string fromname, string subject, string report, string queryname, string querydescription)
        {
            Data.QueryName = queryname;
            Data.QueryDescription = querydescription;
            EmailReport(savedquery, queuedBy, fromaddr, fromname, subject, report);
        }

        public string EmailStr(string body)
        {
            if (!Util.UserPeopleId.HasValue)
                return "no user";
            using (var db2 = NewDataContext())
            {
                db2.SetCurrentOrgId(CurrentOrgId);
                var p = db2.LoadPersonById(Util.UserPeopleId.Value);
                var m = new EmailReplacements(db2, body, new MailAddress(p.EmailAddress));
                return m.DoReplacements(db2, p);
            }
        }

        private void Email2(CMSDataContext db2, IQueryable<Person> q, int queuedBy, string fromAddr, string fromName, string subject,
            string body, string cclist = null, DateTime? dateWanted = null)
        {
            var from = new MailAddress(fromAddr, fromName);
            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            if (MaxEmails > 0)
                q = q.Take(MaxEmails);

            var tag = db2.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

            Util.IsInRoleEmailTest = TestEmail;
            var queueremail = db2.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).SingleOrDefault();
            if(!queueremail.HasValue())
                throw new Exception("QueuedBy PeopleId not found in model.Email");
            Util.UserEmail = queueremail;
            db2.SetCurrentOrgId(CurrentOrgId);

            var emailqueue = db2.CreateQueue(queuedBy, from, subject, body, dateWanted, tag.Id, false, cclist: cclist);
            emailqueue.Transactional = Transactional;
            if (dateWanted == null)
            {
                db2.SendPeopleEmail(emailqueue.Id);
            }
        }
        private void EmailContent2(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName, string cclist = null, DateTime? dateWanted = null)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                Email2(db2, q, queuedBy, fromAddr, fromName, subject, c.Body, cclist, dateWanted);
            }
        }

        public void EmailWithPythonData(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body, IEnumerable<dynamic> recipientData) 
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                if (q == null)
                    return;
                IQueryable<Person> q1 = q;
                var @from = new MailAddress(fromAddr, fromName);
                q1 = from p in q1
                    where p.EmailAddress != null
                    where p.EmailAddress != ""
                    where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                    select p;
                if (MaxEmails > 0)
                    q1 = q1.Take(MaxEmails);

                var tag = db2.PopulateSpecialTag(q1, DbUtil.TagTypeId_Emailer);

                Util.IsInRoleEmailTest = TestEmail;
                var queueremail = db2.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).SingleOrDefault();
                if(!queueremail.HasValue())
                    throw new Exception("QueuedBy PeopleId not found in model.Email");
                Util.UserEmail = queueremail;
                db2.SetCurrentOrgId(CurrentOrgId);

                var emailqueue = db2.CreateQueue(queuedBy, @from, subject, body, null, tag.Id, false, cclist: null);
                emailqueue.Transactional = Transactional;
                db2.SendPeopleEmailWithPython(emailqueue.Id, recipientData, Data);
            }
        }
        public void EmailContentWithPythonData(object savedQuery, int queuedBy, string fromAddr, string fromName, string contentName, IEnumerable<dynamic> recipientData)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                throw new Exception($"Cannot find content named \"{contentName.Truncate(50)}\"");
            EmailWithPythonData(savedQuery, queuedBy, fromAddr, fromName, c.Title, c.Body, recipientData);
        }
    }
}