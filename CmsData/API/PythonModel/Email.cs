using System;
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

        public void Email(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body)
        {
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                if (q == null)
                    return;
                Email2(db2, q, queuedBy, fromAddr, fromName, subject, body);
            }
        }

        public void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            EmailContent(savedQuery, queuedBy, fromAddr, fromName, c.Title, contentName);
        }

        public void EmailContentWithSubject(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName)
        {
            EmailContent2(savedQuery, queuedBy, fromAddr, fromName, subject, contentName);
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




        private void Email2(CMSDataContext db2, IQueryable<Person> q, int queuedBy, string fromAddr, string fromName, string subject,
            string body)
        {
            var from = new MailAddress(fromAddr, fromName);
            q = from p in q
                where p.EmailAddress != null
                where p.EmailAddress != ""
                where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                select p;
            var tag = db2.PopulateSpecialTag(q, DbUtil.TagTypeId_Emailer);

            Util.IsInRoleEmailTest = TestEmail;
            var queueremail = db2.People.Where(pp => pp.PeopleId == queuedBy).Select(pp => pp.EmailAddress).Single();
            Util.UserEmail = queueremail;
            db2.SetCurrentOrgId(CurrentOrgId);

            var emailqueue = db2.CreateQueue(queuedBy, from, subject, body, null, tag.Id, false);
            emailqueue.Transactional = Transactional;
            db2.SendPeopleEmail(emailqueue.Id);
        }
        private void EmailContent2(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName)
        {
            var c = db.ContentOfTypeHtml(contentName);
            if (c == null)
                return;
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(savedQuery);
                Email2(db2, q, queuedBy, fromAddr, fromName, subject, c.Body);
            }
        }

    }
}