using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.Linq;
using CmsData;
using UtilityExtensions;
using System.Web.Mvc;
using System.Text;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Web.Security;

namespace CmsWeb.Models
{
    public class EmailsModel
    {
        public int? peopleid { get; set; }
        public string name
        {
            get
            {
                var nam = (from p in DbUtil.Db.People
                          where p.PeopleId == peopleid
                          select p.Name).SingleOrDefault();
                return nam;
            }
        }
        public int? senderid { get; set; }
        public Person sender
        {
            get
            {
                return DbUtil.Db.LoadPersonById(senderid ?? 0);
            }
        }
        public string subject { get; set; }
        public string body { get; set; }
        public string from { get; set; }
        public DateTime? startdt { get; set; }
        public DateTime? enddt { get; set; }
        public bool transactional { get; set; }
        public bool scheduled { get; set; }
        public PagerModel2 Pager { get; set; }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchEmails().Count();
            return _count.Value;
        }
        public EmailsModel()
        {
            Pager = new PagerModel2(Count);
            Pager.Sort = "Sent/Scheduled";
            Pager.Direction = "desc";
        }
        public IEnumerable<EmailQueueInfo> Emails()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
            var q2 = from e in q
                     select new EmailQueueInfo
                     {
                         queue = e,
                         count = e.EmailQueueTos.Count(),
                         nopens = e.EmailResponses.Count(),
                         nuopens = e.EmailResponses.Select(er => er.PeopleId).Distinct().Count()
                     };
            return q2;
        }

        private IQueryable<EmailQueue> FilterOutFinanceOnly(IQueryable<EmailQueue> q)
        {
            var user = HttpContextFactory.Current.User;
            if (!user.IsInRole("Finance") && !user.IsInRole("FinanceAdmin"))
                q = from e in q
                    where (e.FinanceOnly ?? false) == false
                    select e;
            return q;
        }
        private IQueryable<EmailQueue> emails;
        private IQueryable<EmailQueue> FetchEmails()
        {
            if (emails != null)
                return emails;
            var sndr = sender;
            if (sndr == null)
                sndr = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
            emails
               = from t in DbUtil.Db.EmailQueues
                 where t.Sent >= startdt || startdt == null
                 where subject == null || t.Subject.Contains(subject)
                 where body == null || t.Body.Contains(body)
                 where @from == null || t.FromName.Contains(@from) || t.FromAddr.Contains(@from)
                 where peopleid == null || t.EmailQueueTos.Any(et => et.PeopleId == peopleid)
                 where senderid == null || t.QueuedBy == senderid || t.FromAddr == sndr.EmailAddress
                 where (t.Transactional ?? false) == transactional
                 where scheduled == (t.SendWhen != null && t.Sent == null) 
                 select t;
            var edt = enddt;
            if (!edt.HasValue && startdt.HasValue)
                 edt = startdt.Value.AddHours(24);
            if (edt.HasValue)
                emails = emails.Where(t => t.Sent < edt);
            emails = FilterOutFinanceOnly(emails);

            var roles = new [] { "Admin", "ManageEmails", "Finance" };
            if (DbUtil.Db.CurrentUser.Roles.Any(uu => roles.Contains(uu)))
                return emails;

            // show only user's emails sent or received
            var u = DbUtil.Db.LoadPersonById(Util.UserPeopleId ?? 0);
            emails = from t in emails
                     where t.FromAddr == u.EmailAddress
                           || t.QueuedBy == u.PeopleId
                           || t.EmailQueueTos.Any(et => et.PeopleId == u.PeopleId)
                     select t;
            return emails;
        }
        public IQueryable<EmailQueue> ApplySort()
        {
            var q = FetchEmails();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Sent/Scheduled":
                        q = from t in q
                            orderby (t.SendWhen ?? t.Sent) ?? t.Queued
                            select t;
                        break;
                    case "From":
                        q = from t in q
                            orderby t.FromAddr, t.Sent
                            select t;
                        break;
                    case "Name":
                        q = from t in q
                            orderby t.FromName, t.Sent
                            select t;
                        break;
                    case "Subject":
                        q = from t in q
                            orderby t.Subject, t.Sent
                            select t;
                        break;
                    case "Count":
                        q = from t in q
                            orderby t.EmailQueueTos.Count()
                            select t;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Sent/Scheduled":
                        q = from t in q
                            orderby ((t.SendWhen ?? t.Sent) ?? t.Queued) descending
                            select t;
                        break;
                    case "From":
                        q = from t in q
                            orderby t.FromAddr descending, t.Sent descending
                            select t;
                        break;
                    case "Name":
                        q = from t in q
                            orderby t.FromName, t.Sent descending
                            select t;
                        break;
                    case "Subject":
                        q = from t in q
                            orderby t.Subject, t.Sent descending
                            select t;
                        break;
                    case "Count":
                        q = from t in q
                            orderby t.EmailQueueTos.Count() descending
                            select t;
                        break;
                }
            return q;
        }
    }
    public class EmailQueueInfo
    {
        public EmailQueue queue { get; set; }
        public int count { get; set; }
        public int nopens { get; set; }
        public int nuopens { get; set; }
    }
}
