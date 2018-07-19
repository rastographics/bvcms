/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Net.Mail;
using CmsData.Codes;
using UtilityExtensions;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CmsData.API;
using Elmah;
using HtmlAgilityPack;
using SendGrid;
using SendGrid.Helpers.Mail;
using MContent = SendGrid.Helpers.Mail.Content;

namespace CmsData
{
    public partial class CMSDataContext
    {
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
            Email(Util.FirstAddress(from), p, addmail, subject, body, redacted);
        }

        public void EmailFinanceInformation(string from, Person p, string subject, string body)
        {
            EmailFinanceInformation(Util.FirstAddress(from), p, null, subject, body);
        }

        public void EmailFinanceInformation(MailAddress from, Person p, string subject, string body)
        {
            EmailFinanceInformation(from, p, null, subject, body);
        }

        public void EmailFinanceInformation(string from, IEnumerable<Person> list, string subject, string body)
        {
            var li = list.ToList();
            if (!li.Any())
                return;
            var aa = PersonListToMailAddressList(li);
            EmailFinanceInformation(Util.FirstAddress(from), li[0], aa, subject, body);
        }

        public void EmailFinanceInformation(MailAddress fromaddress, Person p, List<MailAddress> list, string subject, string body)
        {
            var emailqueue = new EmailQueue
            {
                Queued = Util.Now,
                FromAddr = fromaddress.Address,
                FromName = fromaddress.DisplayName,
                Subject = subject,
                Body = body,
                QueuedBy = Util.UserPeopleId,
                Transactional = true,
                FinanceOnly = true
            };
            EmailQueues.InsertOnSubmit(emailqueue);
            string addmailstr = null;
            if (list != null)
                addmailstr = list.EmailAddressListToString();
            emailqueue.EmailQueueTos.Add(new EmailQueueTo
            {
                PeopleId = p.PeopleId,
                OrgId = Util2.CurrentOrgId,
                AddEmail = addmailstr,
                Guid = Guid.NewGuid(),
            });
            SubmitChanges();
            SendPersonEmail(emailqueue.Id, p.PeopleId);
        }

        public void Email(MailAddress fromAddress, Person p, List<MailAddress> addmail, string subject, string body, bool redacted = false)
        {
            var emailqueue = new EmailQueue
            {
                Queued = Util.Now,
                FromAddr = fromAddress.Address,
                FromName = fromAddress.DisplayName,
                Subject = subject,
                Body = body,
                QueuedBy = Util.UserPeopleId,
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
                OrgId = Util2.CurrentOrgId,
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

        public IEnumerable<Person> PeopleFromPidString(string pidstring, string role = null)
        {
            var a = pidstring.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
            var q = from p in People
                    where role == null || p.Users.Any(uu => uu.UserRoles.Any(ur => ur.Role.RoleName == role))
                    where a.Contains(p.PeopleId)
                    orderby p.PeopleId == a[0] descending
                    select p;
            return q;
        }

        public List<Person> AdminPeople()
        {
            var list = (from p in CMSRoleProvider.provider.GetAdmins()
                        where p.EmailAddress.HasValue()
                              && !p.EmailAddress.Contains("touchpointsoftware.com")
                        select p).ToList();
            if (list.Count == 0)
                list = (from p in CMSRoleProvider.provider.GetAdmins()
                        where p.EmailAddress.HasValue()
                        select p).ToList();
            return list;
        }
        public List<Person> AdminPeople2()
        {
            var list = (from p in CMSRoleProvider.provider.GetAdmins()
                        where p.EmailAddress.HasValue()
                        select p).ToList();
            if (list.Count == 0)
                list = (from p in CMSRoleProvider.provider.GetAdmins()
                        where p.EmailAddress.HasValue()
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

        public List<Person> RecurringGivingNotifyPersons()
        {
            var notifyids = (from o in Organizations
                             where o.RegistrationTypeId == RegistrationTypeCode.ManageGiving
                             select o.NotifyIds).FirstOrDefault();
            var people = new List<Person>();
            var toppid = 0;
            if (notifyids.HasValue())
            {
                var ppl = PeopleFromPidString(notifyids).ToList();
                if (Setting("SendRecurringGiftFailureNoticesToFinanceUsers", "false") == "false")
                    return ppl;
                toppid = ppl[0].PeopleId;
                people.Add(ppl[0]);
            }
            people.AddRange(from u in Users
                            where u.UserRoles.Any(ur => ur.Role.RoleName == "Finance")
                            where u.PeopleId != toppid
                            select u.Person);
            return people;
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
                return Setting("AdminMail", ConfigurationManager.AppSettings["supportemail"]);
            return q2.SingleOrDefault();
        }

        public List<MailAddress> StaffEmailsForOrg(int orgid)
        {
            var q = from o in Organizations
                    where o.OrganizationId == orgid
                    where o.NotifyIds != null && o.NotifyIds != ""
                    select o.NotifyIds;
            var pids = string.Join(",", q);
            var a = pids.SplitStr(",").Select(ss => ss.ToInt()).ToArray();
            var q2 = from p in People
                     where p.PeopleId == a[0]
                     select Util.TryGetMailAddress(p.FromEmail);
            if (!q2.Any())
                return Util.ToMailAddressList(Setting("AdminMail", ConfigurationManager.AppSettings["supportemail"]));
            return q2.ToList();
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

        public List<Person> NotifyIds(string ids)
        {
            var a = (ids ?? "").Split(',').Select(ss => ss.ToInt()).ToArray();
            var q2 = from p in People
                     where a.Contains(p.PeopleId)
                     orderby p.PeopleId == a.FirstOrDefault() descending
                     select p;
            var list = q2.ToList();
            if (list.Count == 0)
                return AdminPeople();
            return list;
        }

        public Person UserPersonFromEmail(string email)
        {
            var q = from u in Users
                    where u.Person.EmailAddress == email || u.Person.EmailAddress2 == email
                    select u.Person;
            var p = q.FirstOrDefault() ?? CMSRoleProvider.provider.GetAdmins().First();
            return p;
        }

        public EmailQueue CreateQueue(MailAddress From, string subject, string body, DateTime? schedule, int tagId, bool publicViewable, bool? ccParents = null, string cclist = null)
        {
            return CreateQueue(Util.UserPeopleId, From, subject, body, schedule, tagId, publicViewable, ccParents: ccParents, cclist: cclist);
        }

        public EmailQueue CreateQueueForOrg(MailAddress from, string subject, string body, DateTime? schedule, int orgid, bool publicViewable, string cclist = null)
        {
            var emailqueue = new EmailQueue
            {
                Queued = Util.Now,
                FromAddr = from.Address,
                FromName = from.DisplayName,
                Subject = subject,
                Body = body,
                SendWhen = schedule,
                QueuedBy = Util.UserPeopleId,
                Transactional = false,
                PublicX = publicViewable,
                SendFromOrgId = orgid,
                CClist = cclist
            };
            EmailQueues.InsertOnSubmit(emailqueue);
            SubmitChanges();

            if (body.Contains("http://publiclink", ignoreCase: true))
            {
                var link = ServerLink("/EmailView/" + emailqueue.Id);
                var re = new Regex("http://publiclink", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                emailqueue.Body = re.Replace(body, link);
            }
            SubmitChanges();
            return emailqueue;
        }

        public EmailQueue CreateQueue(int? queuedBy, MailAddress from, string subject, string body, DateTime? schedule, int tagId, bool publicViewable, int? goerSupporterId = null, bool? ccParents = null, string cclist = null)
        {
            var tag = TagById(tagId);
            if (tag == null)
                return null;

            var emailqueue = new EmailQueue
            {
                Queued = Util.Now,
                FromAddr = from.Address,
                FromName = from.DisplayName,
                Subject = subject,
                Body = body,
                SendWhen = schedule,
                QueuedBy = queuedBy,
                Transactional = false,
                PublicX = publicViewable,
                CCParents = ccParents,
                CClist = cclist,
                Testing = Util.IsInRoleEmailTest,
                ReadyToSend = false, // wait until all individual emailqueueto records are created.
            };
            EmailQueues.InsertOnSubmit(emailqueue);
            SubmitChanges();

            if (body.Contains("http://publiclink", ignoreCase: true))
            {
                var link = ServerLink("/EmailView/" + emailqueue.Id);
                var re = new Regex("http://publiclink",
                    RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                emailqueue.Body = re.Replace(body, link);
            }

            var q = tag.People(this);

            IQueryable<int> q2 = null;
            if (emailqueue.CCParents == true)
                q2 = from p in q.Distinct()
                     where (p.EmailAddress ?? "") != ""
                           || (p.Family.HeadOfHousehold.EmailAddress ?? "") != ""
                           || (p.Family.HeadOfHouseholdSpouse.EmailAddress ?? "") != ""
                     where (p.SendEmailAddress1 ?? true)
                           || (p.SendEmailAddress2 ?? false)
                           || (p.Family.HeadOfHousehold.SendEmailAddress1 ?? false)
                           || (p.Family.HeadOfHousehold.SendEmailAddress2 ?? false)
                           || (p.Family.HeadOfHouseholdSpouse.SendEmailAddress1 ?? false)
                           || (p.Family.HeadOfHouseholdSpouse.SendEmailAddress2 ?? false)
                     where p.EmailOptOuts.All(oo => oo.FromEmail != emailqueue.FromAddr)
                     orderby p.PeopleId
                     select p.PeopleId;
            else
                q2 = from p in q.Distinct()
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
                    OrgId = Util2.CurrentOrgId,
                    Guid = Guid.NewGuid(),
                    GoerSupportId = goerSupporterId,
                });
            }
            emailqueue.ReadyToSend = true;
            SubmitChanges();
            return emailqueue;
        }

        public EmailQueue CreateQueueForSupporters(int? queuedBy, MailAddress from, string subject, string body, DateTime? schedule, List<GoerSupporter> list, bool publicViewable)
        {
            var emailqueue = new EmailQueue
            {
                Queued = Util.Now,
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

            var q2 = from g in list
                     where g.SupporterId != null
                     where g.Supporter.EmailAddress != null
                     where g.Supporter.EmailAddress != ""
                     where (g.Supporter.SendEmailAddress1 ?? true) || (g.Supporter.SendEmailAddress2 ?? false)
                     where g.Supporter.EmailOptOuts.All(oo => oo.FromEmail != emailqueue.FromAddr)
                     orderby g.SupporterId
                     select g;

            foreach (var g in q2)
            {
                emailqueue.EmailQueueTos.Add(new EmailQueueTo
                {
                    PeopleId = g.SupporterId ?? 0,
                    OrgId = Util2.CurrentOrgId,
                    Guid = Guid.NewGuid(),
                    GoerSupportId = g.Id,
                });
            }
            SubmitChanges();
            return emailqueue;
        }

        public void SendPersonEmail(int id, int pid)
        {
            var emailqueue = EmailQueues.Single(eq => eq.Id == id);
            var emailqueueto = EmailQueueTos.Single(eq => eq.Id == id && eq.PeopleId == pid);
            var fromname = emailqueueto.EmailQueue.FromName;
            fromname = !fromname.HasValue() ? emailqueue.FromAddr : emailqueue.FromName.Replace("\"", "");
            var from = Util.FirstAddress(emailqueue.FromAddr, fromname);

            try
            {
                var p = LoadPersonById(emailqueueto.PeopleId);
                var body = DoClickTracking(emailqueue);
                var m = new EmailReplacements(this, body, from);
                var text = m.DoReplacements(emailqueueto.PeopleId, emailqueueto);
                var aa = m.ListAddresses;

                if (Setting("sendemail", "true") != "false")
                {
                    if (aa.Count > 0)
                        SendEmail(from, emailqueue.Subject, text, aa, emailqueue.Id, pid);
                    else
                        SendEmail(from,
                            $"(no email address) {emailqueue.Subject}",
                            $"<p style='color:red'>You are receiving this because there is no email address for {p.Name}({p.PeopleId}). You should probably contact them since they were probably expecting this information.</p>\n{text}",
                            Util.ToMailAddressList(from),
                            emailqueueto);
                    emailqueueto.Sent = Util.Now;
                    emailqueue.Sent = Util.Now;
                    if (emailqueue.Redacted ?? false)
                        emailqueue.Body = "redacted";
                    SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                SendEmail(from,
                    $"sent emails - error(emailid={emailqueue.Id})", ex.ToString(),
                    Util.ToMailAddressList(from),
                    emailqueue.Id);
                throw;
            }
        }

        public List<MailAddress> GetAddressList(Person p, string regemail = null)
        {
            var aa = new List<MailAddress>();
            if (p == null)
                return aa;
            if ((p.SendEmailAddress1 ?? true) && Util.ValidEmail(p.EmailAddress))
                Util.AddGoodAddress(aa, p.FromEmail);
            if ((p.SendEmailAddress2 ?? false) && Util.ValidEmail(p.EmailAddress2))
                Util.AddGoodAddress(aa, p.FromEmail2);
            if (regemail.HasValue())
                foreach (var ad in regemail.SplitStr(",;"))
                    if(Util.ValidEmail(ad))
                        Util.AddGoodAddress(aa, ad);
            return aa;
        }

        public void SendPeopleEmail(int queueid, bool onlyProspects = false)
        {
            var emailqueue = EmailQueues.Single(ee => ee.Id == queueid);
            var from = Util.FirstAddress(emailqueue.FromAddr, emailqueue.FromName);
            if (!emailqueue.Subject.HasValue() || !emailqueue.Body.HasValue())
            {
                SendEmail(from,
                    $"sent emails - error(emailid={emailqueue.Id})", "no subject or body, no emails sent",
                    Util.ToMailAddressList(from),
                    emailqueue.Id);
                return;
            }

            var body = DoClickTracking(emailqueue);
            var m = new EmailReplacements(this, body, from, queueid);
            emailqueue.Started = Util.Now;
            SubmitChanges();

            var cc = Util.ToMailAddressList(emailqueue.CClist);

            if (emailqueue.SendFromOrgId.HasValue)
            {
                if (!onlyProspects)
                {
                    var q2 = from om in OrganizationMembers
                             where om.OrganizationId == emailqueue.SendFromOrgId
                             where om.MemberTypeId != MemberTypeCode.InActive
                             where om.MemberTypeId != MemberTypeCode.Prospect
                             where (om.Pending ?? false) == false
                             let p = om.Person
                             where p.EmailAddress != null
                             where p.EmailAddress != ""
                             where (p.SendEmailAddress1 ?? true) || (p.SendEmailAddress2 ?? false)
                             where p.EmailOptOuts.All(oo => oo.FromEmail != emailqueue.FromAddr)
                             select om.PeopleId;
                    foreach (var pid in q2)
                    {
                        // Protect against duplicate PeopleIDs ending up in the queue
                        if (emailqueue.EmailQueueTos.Any(pp => pp.PeopleId == pid))
                            continue;
                        emailqueue.EmailQueueTos.Add(new EmailQueueTo
                        {
                            PeopleId = pid,
                            OrgId = emailqueue.SendFromOrgId,
                            Guid = Guid.NewGuid(),
                        });
                    }
                    SubmitChanges();
                }
            }

            var q = from to in EmailQueueTos
                    where to.Id == emailqueue.Id
                    where to.Sent == null
                    orderby to.PeopleId
                    select to;
            foreach (var to in q)
            {
                try
                {
                    if (m.OptOuts != null && m.OptOuts.Any(vv => vv.PeopleId == to.PeopleId && vv.OptOutX == true))
                        continue;
                    var text = m.DoReplacements(to.PeopleId, to);
                    var aa = m.ListAddresses;

                    if (Setting("sendemail", "true") != "false")
                    {
                        SendEmail(from, emailqueue.Subject, text, aa, to, cc);
                        to.Sent = Util.Now;
                        SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    var subject = $"sent emails - error:(emailid={emailqueue.Id}) {CmsHost}";
                    ErrorLog.GetDefault(null).Log(new Error(new Exception(subject, ex)));
                    SendEmail(from, subject, $"{ex.Message}\n{ex.StackTrace}", Util.ToMailAddressList(from), to);
                }
            }

            // Handle CC MailAddresses.  These do not get DoReplacement support.
            if (cc.Count > 0)
            {
                foreach (var ma in cc)
                {
                    try
                    {
                        if (Setting("sendemail", "true") != "false")
                        {
                            List<MailAddress> mal = new List<MailAddress> { ma };
                            SendEmail(from, emailqueue.Subject, body, mal, emailqueue.Id, cc: cc);
                        }
                    }
                    catch (Exception ex)
                    {
                        var subject = $"sent emails - error:(emailid={emailqueue.Id}) {CmsHost}";
                        ErrorLog.GetDefault(null).Log(new Error(new Exception(subject, ex)));
                        SendEmail(from, subject, ex.Message, Util.ToMailAddressList(from));
                    }
                }
            }

            emailqueue.Sent = Util.Now;
            if (emailqueue.Redacted ?? false)
                emailqueue.Body = "redacted";
            else if (emailqueue.Transactional == false)
            {
                var nitems = emailqueue.EmailQueueTos.Count();
                if (cc.Count > 0)
                {
                    nitems += cc.Count;
                }
                if (nitems > 1)
                    NotifySentEmails(from.Address, from.DisplayName,
                        emailqueue.Subject, nitems, emailqueue.Id);
            }
            SubmitChanges();
        }

        public void SendPeopleEmailWithPython(int queueid, IEnumerable<dynamic> recipientData, DynamicData pythonData)
        {
            var emailqueue = EmailQueues.Single(ee => ee.Id == queueid);
            var from = Util.FirstAddress(emailqueue.FromAddr, emailqueue.FromName);
            if (!emailqueue.Subject.HasValue() || !emailqueue.Body.HasValue())
            {
                SendEmail(from,
                    $"sent emails - error(emailid={emailqueue.Id})", "no subject or body, no emails sent",
                    Util.ToMailAddressList(from),
                    emailqueue.Id);
                return;
            }
            var dict = recipientData.ToDictionary(vv => (int)vv.PeopleId, vv => vv);

            var body = DoClickTracking(emailqueue);
            var m = new EmailReplacements(this, body, from, queueid, pythondata: pythonData);
            emailqueue.Started = Util.Now;
            SubmitChanges();

            var cc = Util.ToMailAddressList(emailqueue.CClist);

            var q = from to in EmailQueueTos
                    where to.Id == emailqueue.Id
                    where to.Sent == null
                    orderby to.PeopleId
                    select to;
            foreach (var to in q)
            {
                try
                {
                    if (m.OptOuts != null && m.OptOuts.Any(vv => vv.PeopleId == to.PeopleId && vv.OptOutX == true))
                        continue;
                    if (!dict.ContainsKey(to.PeopleId))
                        continue;

                    var text = m.DoReplacements(to.PeopleId, to);

                    text = RenderTemplate(text, dict[to.PeopleId]);
                    if(text.Contains("<!--SKIP-->"))
                        continue;
                    var re = new Regex("<!--SUBJECT:(?<subj>.*)-->");
                	var subj = re.Match(text).Groups["subj"].Value;

                    var aa = m.ListAddresses;

                    if (Setting("sendemail", "true") != "false")
                    {
                        SendEmail(from, Util.PickFirst(subj, emailqueue.Subject), text, aa, to, cc);
                        to.Sent = Util.Now;
                        SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    var subject = $"sent emails - error:(emailid={emailqueue.Id}) {CmsHost}";
                    ErrorLog.GetDefault(null).Log(new Error(new Exception(subject, ex)));
                    SendEmail(from, subject, $"{ex.Message}\n{ex.StackTrace}", Util.ToMailAddressList(from), to);
                }
            }

            emailqueue.Sent = Util.Now;
            var nitems = emailqueue.EmailQueueTos.Count();
            if (cc.Count > 0)
            {
                nitems += cc.Count;
            }
            if (nitems > 1)
                NotifySentEmails(from.Address, from.DisplayName,
                    emailqueue.Subject, nitems, emailqueue.Id);
            SubmitChanges();
        }
        private string DoClickTracking(EmailQueue emailqueue)
        {
            var body = emailqueue.Body;
            if (body.Contains("{tracklinks}", true))
            {
                body = body.Replace("{tracklinks}", "", ignoreCase: true);
                body = CreateClickTracking(emailqueue.Id, body);
            }
            return body;
        }

        private void NotifySentEmails(string fromemail, string fromName, string subject, int count, int id)
        {
            if (Setting("sendemail", "true") == "false")
                return;

            var from = new MailAddress(fromemail, fromName);
            string subj = "sent emails: " + subject;
            var link = ServerLink("/Emails/Details/" + id);
            string body = $@"<a href=""{link}"">{count} emails sent</a>";
#if DEBUG
#else
            if (Util.IsMyDataUser == false)
                SendEmail(from, subj, body, Util.ToMailAddressList(from), id);
#endif
        }

        private string CreateClickTracking(int emailId, string input)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            int linkIndex = 0;

            using (var md5Hash = MD5.Create())
            {
                var linkList = doc.DocumentNode.SelectNodes("//a[@href]");
                if (linkList == null)
                    return doc.DocumentNode.OuterHtml;

                foreach (HtmlNode link in linkList)
                {
                    var att = link.Attributes["href"];
                    var url = att.Value;
                    if (EmailReplacements.IsSpecialLink(url))
                        continue;
                    if (url.StartsWith("mailto:"))
                        continue;

                    if (EmailReplacements.SettingUrlRe.IsMatch(url))
                        url = EmailReplacements.SettingUrlReplacement(this, url);
                    var hash = HashMd5Base64(md5Hash, url + DateTime.Now.ToString("o") + linkIndex);

                    var emailLink = new EmailLink
                    {
                        Created = DateTime.Now,
                        EmailID = emailId,
                        Hash = hash,
                        Link = url
                    };
                    EmailLinks.InsertOnSubmit(emailLink);
                    SubmitChanges();

                    att.Value = ServerLink($"/ExternalServices/ct?l={HttpUtility.UrlEncode(hash)}");

                    linkIndex++;
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        private static string HashMd5Base64(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(data, 0, data.Length);
        }

        // Settings for SendGrid
        public string CustomSendGridApiKey => Setting("SendGridAPIKey", "");
        public bool UseSendGridApi => Setting("UseSendGridApi");
        public bool UseIpWarmup => Setting("UseIpWarmup");
        public string CustomFromDomain => Setting("sysfromemail", ConfigurationManager.AppSettings["sysfromemail"]);

        // Configuration for SendGrid
        public string DefaultSendGridApiKey => ConfigurationManager.AppSettings["SendGridAPIKey"];
        public string DefaultFromDomain => ConfigurationManager.AppSettings["sysfromemail"];

        public bool CanUseSendGrid => CustomSendGridApiKey.HasValue() && DefaultSendGridApiKey.HasValue();
        public bool UseCustomEmailDomain => CustomSendGridApiKey.HasValue() && CustomFromDomain.HasValue();
        public bool ShouldUseCustomEmailDomain => UseCustomEmailDomain && (!UseIpWarmup || TryIpWarmup() == 1);

        private void SendEmail(MailAddress fromAddress, string subject, string message, List<MailAddress> to, EmailQueueTo eqto, List<MailAddress> cc = null)
        {
            var domain = SendEmail(fromAddress, subject, message, to, eqto.Id, eqto.PeopleId, cc);
            eqto.DomainFrom = domain;
        }

        public string SendEmail(MailAddress fromAddress, string subject, string message, MailAddress to, int? id = null, int? pid = null, List<MailAddress> cc = null)
        {
            return SendEmail(fromAddress, subject, message, new[] { to }.ToList(), id, pid, cc);
        }

        public string SendEmail(MailAddress fromAddress, string subject, string message, List<MailAddress> to, int? id = null, int? pid = null, List<MailAddress> cc = null)
        {
            if (ConfigurationManager.AppSettings["sendemail"] == "false")
                return null;
#if DEBUG
#else
            if (UseSendGridApi && CanUseSendGrid)
                return SendGridMsg(fromAddress, subject, message, to, id, pid, cc);
#endif
            return SendSmtpMsg(fromAddress, subject, message, to, id, pid, cc: cc);
        }

        public string SendGridMsg(MailAddress from, string subject, string message, List<MailAddress> to, int? id, int? pid, List<MailAddress> cc = null)
        {
            var senderrorsto = ConfigurationManager.AppSettings["senderrorsto"];

            string fromDomain, apiKey;

            if (ShouldUseCustomEmailDomain)
            {
                fromDomain = CustomFromDomain;
                apiKey = CustomSendGridApiKey;
            }
            else
            {
                fromDomain = DefaultFromDomain;
                apiKey = DefaultSendGridApiKey;
            }
            var client = new SendGridClient(apiKey);

            if (from == null)
                from = Util.FirstAddress(senderrorsto);

            var mail = new SendGridMessage()
            {
                From = new EmailAddress(fromDomain, from.DisplayName),
                Subject = subject,
                ReplyTo = new EmailAddress(from.Address, from.DisplayName),
                PlainTextContent = "Hello, Email from the helper [SendSingleEmailAsync]!",
                HtmlContent = "<strong>Hello, Email from the helper! [SendSingleEmailAsync]</strong>"
            };
            var pe = new Personalization();
            foreach (var ma in to)
                if (ma.Host != "nowhere.name" || Util.IsInRoleEmailTest)
                    mail.AddTo(new EmailAddress(ma.Address, ma.DisplayName));

            if (cc?.Count > 0)
            {
                string cclist = string.Join(",", cc);
                if (!cc.Any(vv => vv.Address.Equal(from.Address)))
                    cclist = $"{from.Address},{cclist}";
                mail.ReplyTo = new EmailAddress(cclist);
            }

            pe.Headers.Add(XSmtpApi, XSmtpApiHeader(id, pid, fromDomain));
            pe.Headers.Add(XBvcms, XBvcmsHeader(id, pid));

            mail.Personalizations.Add(pe);

            if (pe.Tos.Count == 0 && pe.Tos.Any(tt => tt.Email.EndsWith("@nowhere.name")))
                return null;
            var badEmailLink = "";
            if (pe.Tos.Count == 0)
            {
                pe.Tos.Add(new EmailAddress(from.Address, from.DisplayName));
                pe.Tos.Add(new EmailAddress(Util.FirstAddress(senderrorsto).Address));
                mail.Subject += $"-- bad addr for {CmsHost}({pid})";
                badEmailLink = $"<p><a href='{CmsHost}/Person2/{pid}'>bad addr for</a></p>\n";
            }

            var regex = new Regex("</?([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            mail.PlainTextContent = regex.Replace(message, string.Empty);
            mail.HtmlContent = badEmailLink + message + CcMessage(cc);

            var response = client.SendEmailAsync(mail);
            return fromDomain;
        }

        public string SendSmtpMsg(MailAddress fromAddress, string subject, string message, List<MailAddress> to, int? id, int? pid, List<LinkedResource> attachments = null, List<MailAddress> cc = null)
        {
            var senderrorsto = ConfigurationManager.AppSettings["senderrorsto"];
            var msg = new MailMessage();
            if (fromAddress == null)
                fromAddress = Util.FirstAddress(senderrorsto);

            var fromDomain = DefaultFromDomain;
            msg.From = new MailAddress(fromDomain, fromAddress.DisplayName);
            msg.ReplyToList.Add(fromAddress);
            if (cc != null)
            {
                foreach (var a in cc)
                    msg.ReplyToList.Add(a);
                if (!msg.ReplyToList.Contains(msg.From) && msg.From.Address.NotEqual(fromDomain))
                    msg.ReplyToList.Add(msg.From);
            }

            msg.Headers.Add(XSmtpApi, XSmtpApiHeader(id, pid, fromDomain));
            msg.Headers.Add(XBvcms, XBvcmsHeader(id, pid));

            foreach (var ma in to)
                if (ma.Host != "nowhere.name" || Util.IsInRoleEmailTest)
                    msg.AddAddr(ma);

            msg.Subject = subject;
            var badEmailLink = "";
            if (msg.To.Count == 0 && to.Any(tt => tt.Host == "nowhere.name"))
                return null;
            if (msg.To.Count == 0)
            {
                msg.AddAddr(msg.From);
                msg.AddAddr(Util.FirstAddress(senderrorsto));
                msg.Subject += $"-- bad addr for {CmsHost}({pid})";
                badEmailLink = $"<p><a href='{CmsHost}/Person2/{pid}'>bad addr for</a></p>\n";
            }

            var regex = new Regex("</?([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var text = regex.Replace(message, string.Empty);
            var textview = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
            textview.TransferEncoding = TransferEncoding.Base64;
            msg.AlternateViews.Add(textview);

            var html = badEmailLink + message + CcMessage(cc);
            using (var htmlView = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html))
            {
                htmlView.TransferEncoding = TransferEncoding.Base64;
                if (attachments != null)
                    foreach (var a in attachments)
                        htmlView.LinkedResources.Add(a);
                msg.AlternateViews.Add(htmlView);

                var smtp = Smtp();
                smtp.Send(msg);
            }
            return fromDomain;
        }

        public SmtpClient Smtp()
        {
            var smtp = new SmtpClient();
            if (ConfigurationManager.AppSettings["requiresSsl"] == "true")
                smtp.EnableSsl = true;
            if (Util.SmtpDebug)
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = @"c:\email";
                smtp.Host = "localhost";
            }
            return smtp;
        }

        private static string CcMessage(List<MailAddress> cc)
        {
            if (cc != null && cc.Count > 0)
            {
                var cclist = string.Join(", ", cc);
                return $"<p align='center'><small><i>This email was CC\'d to the email addresses below and they are included in the Reply-To Field.</br>{cclist}</i></small></p>";
            }
            return "";
        }

        private const string XBvcms = "X-BVCMS";
        private string XBvcmsHeader(int? id, int? pid)
        {
            return $"host:{CmsHost}, mailid:{id}, pid:{pid}";
        }
        private const string XSmtpApi = "X-SMTPAPI";
        private string XSmtpApiHeader(int? id, int? pid, string fromEmail)
        {
            return $"{{\"unique_args\":{{\"host\":\"{CmsHost}\",\"mailid\":\"{id}\",\"pid\":\"{pid}\",\"domain\":\"{fromEmail}\"}}}}";
        }

    }
}
