/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Net.Mail;
using UtilityExtensions;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace CmsData
{
    public partial class CMSDataContext
    {
        public string CmsHost
        {
            get
            {
                var h = ConfigurationManager.AppSettings["cmshost"];
                return h.Replace("{church}", Host, ignoreCase: true);
            }
        }
        public void Email(string from, Person p, string subject, string body)
        {
            Email(from, p, null, subject, body, false);
        }
        public void EmailRedacted(string from, Person p, string subject, string body)
        {
            Email(from, p, null, subject, body, true);
        }

        public void Email(string from, Person p, List<MailAddress> addmail, string subject, string body, bool redacted)
        {
            var From = Util.FirstAddress(from);
            Email(From, p, addmail, subject, body, redacted);
        }

        public void Email(MailAddress From, Person p, List<MailAddress> addmail, string subject, string body, bool redacted)
        {
            var emailqueue = new EmailQueue
            {
                Queued = DateTime.Now,
                FromAddr = From.Address,
                FromName = From.DisplayName,
                Subject = subject,
                Body = body,
                QueuedBy = Util.UserPeopleId ?? Util.UserId1,
                Redacted = redacted,
                Transactional = true
            };
            EmailQueues.InsertOnSubmit(emailqueue);
            string addmailstr = null;
            if (addmail != null)
                addmailstr = addmail.EmailAddressListToString();
            emailqueue.EmailQueueTos.Add(new EmailQueueTo
            {
                PeopleId = p.PeopleId,
                OrgId = CurrentOrgId,
                AddEmail = addmailstr,
                Guid = Guid.NewGuid(),
            });
            SubmitChanges();
            SendPersonEmail(emailqueue.Id, p.PeopleId);
        }

        public List<MailAddress> PersonListToMailAddressList(IEnumerable<Person> list)
        {
            var aa = new List<MailAddress>();
            foreach (var p in list)
                aa.AddRange(GetAddressList(p));
            return aa;
        }
        public void Email(string from, IEnumerable<Person> list, string subject, string body)
        {
            var li = list.ToList();
            if (!li.Any())
                return;
            var aa = PersonListToMailAddressList(li);
            Email(from, li[0], aa, subject, body, false);
        }
        public void EmailRedacted(string from, IEnumerable<Person> list, string subject, string body)
        {
            var li = list.ToList();
            if (!li.Any())
                return;
            var aa = PersonListToMailAddressList(li);
            Email(from, li[0], aa, subject, body, redacted: true);
        }
        public IEnumerable<Person> PeopleFromPidString(string pidstring)
        {
            var a = pidstring.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
            var q = from p in People
                    where a.Contains(p.PeopleId)
                    orderby p.PeopleId == a[0] descending
                    select p;
            return q;
        }
        public List<Person> AdminPeople()
        {
            var list = (from p in CMSRoleProvider.provider.GetAdmins()
                        where !p.EmailAddress.Contains("bvcms.com")
                        select p).ToList();
            if(list.Count == 0)
                list = (from p in CMSRoleProvider.provider.GetAdmins()
                        select p).ToList();
            return list;
        }
        public List<Person> FinancePeople()
        {
            var q = from u in Users
                    where u.UserRoles.Any(ur => ur.Role.RoleName == "Finance")
                    select u.Person;
            return q.ToList();
        }
        public string StaffEmailForOrg(int orgid)
        {
            var q = from o in Organizations
                    where o.OrganizationId == orgid
                    where o.NotifyIds != null && o.NotifyIds != ""
                    select o.NotifyIds;
            var pids = string.Join(",", q);
            var a = pids.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
            var q2 = from p in People
                     where p.PeopleId == a[0]
                     select p.FromEmail;
            if (!q2.Any())
                return Setting("AdminMail", "info@bvcms.com");
            return q2.SingleOrDefault();
        }
        public List<Person> StaffPeopleForOrg(int orgid, out bool usedAdmins)
        {
            usedAdmins = false;
            var org = LoadOrganizationById(orgid);
            var pids = org.NotifyIds ?? "";
            var a = pids.Split(',').Select(ss => ss.ToInt()).ToArray();
            var q2 = from p in People
                     where a.Contains(p.PeopleId)
                     orderby p.PeopleId == a.FirstOrDefault() descending
                     select p;
            var list = q2.ToList();
            // if we have notifids, return them
            if (list.Count > 0)
                return list;
            // no notifyids, check master org
            var masterOrgId = (from o in ViewMasterOrgs
                               where o.PickListOrgId == orgid
                               select o.OrganizationId).FirstOrDefault();
            // so if the master id has notifyids, return them 
            if (masterOrgId > 0)
                return StaffPeopleForOrg(masterOrgId);
            // there was no master notifyids either, so return admins
            usedAdmins = true;
            return AdminPeople();
        }
        public List<Person> StaffPeopleForOrg(int orgid)
        {
            bool usedAdmins;
            return StaffPeopleForOrg(orgid, out usedAdmins);
        }
        public Person UserPersonFromEmail(string email)
        {
            var q = from u in Users
                    where u.Person.EmailAddress == email || u.Person.EmailAddress2 == email
                    select u.Person;
            var p = q.FirstOrDefault() ?? CMSRoleProvider.provider.GetAdmins().First();
            return p;
        }
        public EmailQueue CreateQueue(MailAddress From, string subject, string body, DateTime? schedule, int tagId, bool publicViewable)
        {
            return CreateQueue(Util.UserPeopleId, From, subject, body, schedule, tagId, publicViewable);
        }
        public EmailQueue CreateQueue(int? queuedBy, MailAddress from, string subject, string body, DateTime? schedule, int tagId, bool publicViewable, int? goerSupporterId = null)
        {
            var tag = TagById(tagId);
            if (tag == null)
                return null;

            var emailqueue = new EmailQueue
            {
                Queued = DateTime.Now,
                FromAddr = from.Address,
                FromName = from.DisplayName,
                Subject = subject,
                Body = body,
                SendWhen = schedule,
                QueuedBy = queuedBy,
                Transactional = false,
                PublicX = publicViewable,
            };
            EmailQueues.InsertOnSubmit(emailqueue);
            SubmitChanges();

            if (body.Contains("{tracklinks}", true))
            {
                body = body.Replace("{tracklinks}", "", ignoreCase: true);
                emailqueue.Body = createClickTracking(emailqueue.Id, body);
                SubmitChanges();
            }

            if (body.Contains("http://publiclink", ignoreCase: true))
            {
                var link = Util.URLCombine(CmsHost, "/Manage/Emails/View/" + emailqueue.Id);
                var re = new Regex("http://publiclink", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                emailqueue.Body = re.Replace(body, link);
            }

            var q = tag.People(this);

            var q2 = from p in q.Distinct()
                     where p.EmailAddress != null
                     where p.EmailAddress != ""
                     where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                     where p.EmailOptOuts.All(oo => oo.FromEmail != emailqueue.FromAddr)
                     orderby p.PeopleId
                     select p.PeopleId;

            foreach (var pid in q2)
            {
                emailqueue.EmailQueueTos.Add(new EmailQueueTo
                {
                    PeopleId = pid,
                    OrgId = CurrentOrgId,
                    Guid = Guid.NewGuid(),
                    GoerSupportId = goerSupporterId,
                });
            }
            SubmitChanges();
            return emailqueue;
        }
        public void SendPersonEmail(int id, int pid)
        {
            var sysFromEmail = Util.SysFromEmail;
            var emailqueue = EmailQueues.Single(eq => eq.Id == id);
            var emailqueueto = EmailQueueTos.Single(eq => eq.Id == id && eq.PeopleId == pid);
            var fromname = emailqueue.FromName;
            fromname = !fromname.HasValue() ? emailqueue.FromAddr : emailqueue.FromName.Replace("\"", "");
            var from = Util.FirstAddress(emailqueue.FromAddr, fromname);

            try
            {
                var p = LoadPersonById(emailqueueto.PeopleId);
                var text = emailqueue.Body;
                var aa = DoReplacements(ref text, p, emailqueueto);

                var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl("{0}|{1}".Fmt(emailqueueto.PeopleId, from.Address));
                var url = Util.URLCombine(CmsHost, qs);
                var link = @"<a href=""{0}"">Unsubscribe</a>".Fmt(url);
                text = text.Replace("{unsubscribe}", link, ignoreCase: true);
                text = text.Replace("{Unsubscribe}", link, ignoreCase: true);
                if (aa.Count > 0)
                {
                    text = text.Replace("{toemail}", aa[0].Address, ignoreCase: true);
                    text = text.Replace("%7Btoemail%7D", aa[0].Address, ignoreCase: true);
                }
                text = text.Replace("{fromemail}", from.Address, ignoreCase: true);
                text = text.Replace("%7Bfromemail%7D", from.Address, ignoreCase: true);

                if (Setting("sendemail", "true") != "false")
                {
                    if (aa.Count > 0)
                        Util.SendMsg(sysFromEmail, CmsHost, from, emailqueue.Subject, text, aa, emailqueue.Id, pid);
                    else
                        Util.SendMsg(sysFromEmail, CmsHost, from,
                            "(no email address) " + emailqueue.Subject,
                            "<p style='color:red'>You are receiving this because there is no email address for {0}({1}). You should probably contact them since they were probably expecting this information.</p>\n{2}".Fmt(p.Name, p.PeopleId, text),
                            Util.ToMailAddressList(from),
                            emailqueue.Id, pid);
                    emailqueueto.Sent = DateTime.Now;
                    emailqueue.Sent = DateTime.Now;
                    if (emailqueue.Redacted ?? false)
                        emailqueue.Body = "redacted";
                    SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Util.SendMsg(sysFromEmail, CmsHost, from,
                    "sent emails - error", ex.ToString(),
                    Util.ToMailAddressList(from),
                    emailqueue.Id, null);
                throw;
            }
        }
        public List<MailAddress> GetAddressList(Person p)
        {
            return GetAddressList(p, null);
        }
        public List<MailAddress> GetAddressList(Person p, string regemail)
        {
            var aa = new List<MailAddress>();
            if (p.SendEmailAddress1 ?? true)
                Util.AddGoodAddress(aa, p.FromEmail);
            if (p.SendEmailAddress2 ?? false)
                Util.AddGoodAddress(aa, p.FromEmail2);
            if (regemail.HasValue())
                foreach (var ad in regemail.SplitStr(",;"))
                    Util.AddGoodAddress(aa, ad);
            return aa;
        }

        public void SendPeopleEmail(int queueid)
        {
            var emailqueue = EmailQueues.Single(ee => ee.Id == queueid);
            var sysFromEmail = Util.SysFromEmail;
            var from = Util.FirstAddress(emailqueue.FromAddr, emailqueue.FromName);
            if (!emailqueue.Subject.HasValue() || !emailqueue.Body.HasValue())
            {
                Util.SendMsg(sysFromEmail, CmsHost, from,
                    "sent emails - error", "no subject or body, no emails sent",
                    Util.ToMailAddressList(from),
                    emailqueue.Id, null);
                return;
            }

            emailqueue.Started = DateTime.Now;
            SubmitChanges();

            var q = from To in EmailQueueTos
                    where To.Id == emailqueue.Id
                    where To.Sent == null
                    orderby To.PeopleId
                    select To;
            foreach (var To in q)
            {
                try
                {
                    var p = LoadPersonById(To.PeopleId);
                    string text = emailqueue.Body;
                    var aa = DoReplacements(ref text, p, To);
                    var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl("{0}|{1}".Fmt(To.PeopleId, from.Address));
                    var url = Util.URLCombine(CmsHost, qs);
                    var link = @"<a href=""{0}"">Unsubscribe</a>".Fmt(url);
                    text = text.Replace("{unsubscribe}", link, ignoreCase: true);
                    if (aa.Count > 0)
                    {
                        text = text.Replace("{toemail}", aa[0].Address, ignoreCase: true);
                        text = text.Replace("%7Btoemail%7D", aa[0].Address);
                    }
                    text = text.Replace("{fromemail}", from.Address, ignoreCase: true);
                    text = text.Replace("%7Bfromemail%7D", from.Address);

                    if (Setting("sendemail", "true") != "false")
                    {
                        Util.SendMsg(sysFromEmail, CmsHost, from,
                            emailqueue.Subject, text, aa, emailqueue.Id, To.PeopleId);
                        To.Sent = DateTime.Now;

                        SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    Util.SendMsg(sysFromEmail, CmsHost, from,
                        "sent emails - error: {0}".Fmt(CmsHost), ex.Message,
                        Util.ToMailAddressList(from),
                        emailqueue.Id, null);
                    Util.SendMsg(sysFromEmail, CmsHost, from,
                        "sent emails - error: {0}".Fmt(CmsHost), ex.Message,
                        Util.SendErrorsTo(),
                        emailqueue.Id, null);
                }
            }
            emailqueue.Sent = DateTime.Now;
            if (emailqueue.Redacted ?? false)
                emailqueue.Body = "redacted";
            else if (emailqueue.Transactional == false)
            {
                var nitems = emailqueue.EmailQueueTos.Count();
                if (nitems > 1)
                    NotifySentEmails(from.Address, from.DisplayName,
                        emailqueue.Subject, nitems, emailqueue.Id);
            }
            SubmitChanges();
        }

        private void NotifySentEmails(string From, string FromName, string subject, int count, int id)
        {
            if (Setting("sendemail", "true") != "false")
            {
                var from = new MailAddress(From, FromName);
                string subj = "sent emails: " + subject;
                var uri = new Uri(new Uri(CmsHost), "/Manage/Emails/Details/" + id);
                string body = @"<a href=""{0}"">{1} emails sent</a>".Fmt(uri, count);
                var sysFromEmail = Util.SysFromEmail;

                Util.SendMsg(sysFromEmail, CmsHost, from,
                    subj, body, Util.ToMailAddressList(from), id, null);
                var host = uri.Host;
                Util.SendMsg(sysFromEmail, CmsHost, from,
                    host + " " + subj, body,
                    Util.SendErrorsTo(), id, null);
            }
        }

        private static string CLICK_TRACK = "https://{0}.bvcms.com/ExternalServices/ct?l={1}";

        private static string createClickTracking( int emailID, string input )
        {
            var doc = new HtmlDocument();
            doc.LoadHtml( input );
            int linkIndex = 0;

            using (var md5Hash = MD5.Create())
            {
                var linkList = doc.DocumentNode.SelectNodes("//a[@href]");
                if (linkList == null) return doc.DocumentNode.OuterHtml;

                foreach (HtmlNode link in linkList )
                {
                    var att = link.Attributes["href"];
                    if( IsSpecialLink( att.Value ) ) continue;

                    var hash = hashMD5Base64(md5Hash, att.Value + DateTime.Now.ToString("o") + linkIndex );

                    var emailLink = new EmailLink
                        {
                            Created = DateTime.Now,
                            EmailID = emailID,
                            Hash = hash,
                            Link = att.Value
                        };
                    DbUtil.Db.EmailLinks.InsertOnSubmit(emailLink);
                    DbUtil.Db.SubmitChanges();

                    att.Value = CLICK_TRACK.Fmt(Util.Host, HttpUtility.UrlEncode(hash));

                    linkIndex++;

                    //System.Diagnostics.Debug.WriteLine(att.Value);
                    //System.Diagnostics.Debug.WriteLine("Unhashed: " + att.Value + DateTime.Now.ToString("o"));
                    //System.Diagnostics.Debug.WriteLine("Hashed: " + hashMD5Base64( md5Hash, att.Value + DateTime.Now.ToString("o") ) );
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        private static string hashMD5Base64(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(data, 0, data.Length);
        }
    }
}
