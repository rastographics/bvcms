using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Net.Mail;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {
        public void EnrollAndConfirm()
        {
            if (masterorgid.HasValue)
            {
                EnrollAndConfirm2();
                return;
            }
            var Db = DbUtil.Db;
            var ti = Transaction;
            // make a list of email addresses
            var elist = new List<MailAddress>();
            if (UserPeopleId.HasValue)
            {
                if (user.SendEmailAddress1 ?? true)
                    Util.AddGoodAddress(elist, user.FromEmail);
                if (user.SendEmailAddress2 ?? false)
                    Util.AddGoodAddress(elist, user.FromEmail2);
            }
            if (registertag.HasValue())
            {
                var guid = registertag.ToGuid();
                var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                ot.Used = true;
            }
            var participants = new StringBuilder();
            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];
                if (p.IsNew)
                {
                    Person uperson = null;
                    switch (p.whatfamily)
                    {
                        case 1:
                            uperson = Db.LoadPersonById(UserPeopleId.Value);
                            break;
                        case 2:
                            if (i > 0)
                                uperson = List[i - 1].person;
                            break;
                    }
                    p.AddPerson(uperson, p.org.EntryPointId ?? 0);
                }

                Util.AddGoodAddress(elist, p.fromemail);
                participants.Append(p.ToString());
            }
            var p0 = List[0].person;
            if (this.user != null)
                p0 = user;

            //var emails = string.Join(",", elist.ToArray());
            string paylink = string.Empty;
            var amtpaid = ti.Amt ?? 0;
			var amtdue = ti.Amtdue;

            var pids2 = new List<TransactionPerson>();
            foreach (var p in List)
            {
                if (p.PeopleId == null)
                    return;
                if (pids2.Any(pp => pp.PeopleId == p.PeopleId))
                    continue;
                pids2.Add(new TransactionPerson
                {
                    PeopleId = p.PeopleId.Value,
                    Amt = p.TotalAmount(),
                    OrgId = orgid,
                });
            }

            ti.Emails = Util.EmailAddressListToString(elist);
            ti.Participants = participants.ToString();
            ti.TransactionDate = DateTime.Now;
            ti.TransactionPeople.AddRange(pids2);

            var estr = HttpUtility.UrlEncode(Util.Encrypt(ti.Id.ToString()));
            paylink = Util.ResolveServerUrl("/OnlineReg/PayAmtDue?q=" + estr);

            var pids = pids2.Select(pp => pp.PeopleId);

            var details = new StringBuilder("<table>");
            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];

                var q = from pp in Db.People
                        where pids.Contains(pp.PeopleId)
                        where pp.PeopleId != p.PeopleId
                        select pp.Name;
                var others = string.Join(",", q.ToArray());

                var om = p.Enroll(ti, paylink, testing, others);
                details.AppendFormat(@"
<tr><td colspan='2'><hr/></td></tr>
<tr><th valign='top'>{0}</th><td>
{1}
</td></tr>", i + 1, p.PrepareSummaryText(ti));

                om.RegisterEmail = p.email;
				om.TranId = ti.Id;
                if (p.setting.GiveOrgMembAccess == true)
                {
                    if (p.person.Users.Count() == 0)
                    {
                        p.IsNew = false;
                        var u = p.CreateAccount();
                        if (u != null)
                        {
                            var list = u.Roles.ToList();
                            if (!list.Contains("Access"))
                                list.Add("Access");
                            if (!list.Contains("OrgMembersOnly"))
                                list.Add("OrgMembersOnly");
                            u.SetRoles(Db, list.ToArray(), false);
                        }
                    }
                    Db.SubmitChanges();
                }
                int grouptojoin = p.setting.GroupToJoin.ToInt();
				if (grouptojoin > 0)
				{
					OrganizationMember.InsertOrgMembers(Db, grouptojoin, p.PeopleId.Value, 220, DateTime.Now, null, false);
					DbUtil.Db.UpdateMainFellowship(grouptojoin);
				}

                OnlineRegPersonModel.CheckNotifyDiffEmails(p.person,
                    Db.StaffEmailForOrg(p.org.OrganizationId),
                    p.fromemail,
                    p.org.OrganizationName,
                    p.org.PhoneNumber);
                if (p.CreatingAccount == true && p.setting.GiveOrgMembAccess == false)
                    p.CreateAccount();
            }
            details.Append("\n</table>\n");
            Db.SubmitChanges();

            var DivisionName = org.DivisionName;

            var os = settings[orgid.Value];
            var EmailSubject = Util.PickFirst(os.Subject, "no subject");
            var EmailMessage = Util.PickFirst(os.Body, "no body");

            bool usedAdmins;
            List<Person> NotifyIds = Db.StaffPeopleForOrg(org.OrganizationId, out usedAdmins);
            
            var Location = org.Location;

            var subject = Util.PickFirst(EmailSubject, "no subject");
            var message = Util.PickFirst(EmailMessage, "no message");

            message = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, p0, DivisionName, org.OrganizationName, Location, message);
            subject = subject.Replace("{org}", org.OrganizationName);

			message = message.Replace("{phone}", org.PhoneNumber.FmtFone7());
            message = message.Replace("{tickets}", List[0].ntickets.ToString());
            message = message.Replace("{details}", details.ToString());
            message = message.Replace("{paid}", amtpaid.ToString("c"));
            message = message.Replace("{sessiontotal}", amtpaid.ToString("c"));
            message = message.Replace("{participants}", details.ToString());
            if (amtdue > 0)
                message = message.Replace("{paylink}", "<a href='{0}'>Click this link to make a payment on your balance of {1:C}</a>.".Fmt(paylink, amtdue));
            else
                message = message.Replace("{paylink}", "You have a zero balance.");

            var re = new Regex(@"\{donation(?<text>.*)donation\}", RegexOptions.Singleline | RegexOptions.Multiline);
            if (ti.Donate > 0)
            {
                var p = List[donor.Value];
                ti.Fund = p.setting.DonationFund();
                var desc = "{0}; {1}; {2}, {3} {4}".Fmt(
                    p.person.Name,
                    p.person.PrimaryAddress,
                    p.person.PrimaryCity,
                    p.person.PrimaryState,
                    p.person.PrimaryZip);
                p.person.PostUnattendedContribution(DbUtil.Db,
                    ti.Donate.Value,
                    p.setting.DonationFundId,
                    desc);
                var ma = re.Match(message);
                if (ma.Success)
                {
                    var v = ma.Groups["text"].Value;
                    message = re.Replace(message, v);
                }
                message = message.Replace("{donation}", ti.Donate.ToString2("N2"));
                // send donation confirmations
                Db.Email(NotifyIds[0].FromEmail, NotifyIds, subject + "-donation",
                    "${0:N2} donation received from {1}".Fmt(ti.Donate, ti.FullName));
            }
            else
                message = re.Replace(message, "");

            // send confirmations
            Db.Email(NotifyIds[0].FromEmail, p0, elist,
                subject, message, false);
            // notify the staff
            foreach (var p in List)
            {
                Db.Email(Util.PickFirst(p.person.FromEmail, NotifyIds[0].FromEmail), NotifyIds, Header,
@"{6}{0} has registered for {1}<br/>
Feepaid for this registrant: {2:C}<br/>
Total Fee for this registration: {3:C}<br/>
AmountDue: {4:C}<br/>
<pre>{5}</pre>".Fmt(p.person.Name,
               Header,
               amtpaid,
               TotalAmount(),
               amtdue, // Amount Due
               p.PrepareSummaryText(ti),
               usedAdmins? @"<span style='color:red'>THERE ARE NO NOTIFY IDS ON THIS REGISTRATION!!</span><br/>
<a href='http://www.bvcms.com/Doc/MessagesSettings'>see documentation</a><br/>" : ""));
            }
        }


//---------------------------------------------------------------------------------------------------
        private void EnrollAndConfirm2()
        {
            var Db = DbUtil.Db;
            var ti = Transaction;
            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];
                if (p.IsNew)
                {
                    Person uperson = null;
                    switch (p.whatfamily)
                    {
                        case 1:
                            uperson = Db.LoadPersonById(UserPeopleId.Value);
                            break;
                        case 2:
                            if (i > 0)
                                uperson = List[i - 1].person;
                            break;
                    }
                    p.AddPerson(uperson, p.org.EntryPointId ?? 0);
                }
            }

            var amtpaid = ti.Amt ?? 0;

            var pids2 = new List<TransactionPerson>();
            foreach (var p in List)
            {
                if (p.PeopleId == null)
                    return;
                pids2.Add(new TransactionPerson
                {
                    PeopleId = p.PeopleId.Value,
                    Amt = p.TotalAmount(),
                    OrgId = orgid,
                });
            }

            ti.TransactionDate = DateTime.Now;

            var pids = pids2.Select(pp => pp.PeopleId);

            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i]; 
                
                var q = from pp in Db.People
                                         where pids.Contains(pp.PeopleId)
                                         where pp.PeopleId != p.PeopleId
                                         select pp.Name;
                var others = string.Join(",", q.ToArray());

                var om = p.Enroll(ti, null, testing, others);
                om.RegisterEmail = p.email;

                int grouptojoin = p.setting.GroupToJoin.ToInt();
                if (grouptojoin > 0)
                    OrganizationMember.InsertOrgMembers(Db, grouptojoin, p.PeopleId.Value, 220, DateTime.Now, null, false);

                OnlineRegPersonModel.CheckNotifyDiffEmails(p.person,
                    Db.StaffEmailForOrg(p.org.OrganizationId),
                    p.fromemail,
                    p.org.OrganizationName,
                    p.org.PhoneNumber);
                if (p.CreatingAccount == true)
                    p.CreateAccount();

                string DivisionName = masterorg.OrganizationName;
                string OrganizationName = p.org.OrganizationName;

                string EmailSubject = null;
                string message = null;

                if (p.setting.Body.HasValue())
                {
                    EmailSubject = Util.PickFirst(p.setting.Subject, "no subject");
                    message = p.setting.Body;
                }
                else
                {
                    try
                    {
                        var os = settings[masterorgid.Value];
                        EmailSubject = Util.PickFirst(os.Subject, "no subject");
                        message = Util.PickFirst(os.Body, "no body");
                    }
                    catch (Exception)
                    {
                        if (masterorgid == null)
                            throw new Exception("masterorgid was null");
                        if (settings == null)
                            throw new Exception("settings was null");
                        if (settings[masterorgid.Value] == null)
                            throw new Exception("setting not found for masterorgid " + masterorgid.Value);
                        throw;
                    }
                }
                var NotifyIds = Db.StaffPeopleForOrg(p.org.OrganizationId);

                var notify = NotifyIds[0];

                string Location = p.org.Location;
                if (!Location.HasValue())
                    Location = masterorg.Location;

                message = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, p.person, DivisionName, OrganizationName, Location, message);

                string details = p.PrepareSummaryText(ti);
                message = message.Replace("{phone}", p.org.PhoneNumber.FmtFone7());
                message = message.Replace("{tickets}", List[0].ntickets.ToString());
                message = message.Replace("{details}", details);
                message = message.Replace("{paid}", p.TotalAmount().ToString("c"));
                message = message.Replace("{sessiontotal}", amtpaid.ToString("c"));
                message = message.Replace("{participants}", details);

                // send confirmations
                Db.Email(notify.FromEmail, p.person, Util.EmailAddressListFromString(p.fromemail), EmailSubject, message, redacted:false);
                // notify the staff
                Db.Email(Util.PickFirst(p.person.FromEmail, notify.FromEmail),
                    NotifyIds, Header,
@"{0} has registered for {1}<br/>
Feepaid for this registrant: {2:C}<br/>
Others in this registration session: {3:C}<br/>
Total Fee paid for this registration session: {4:C}<br/>
<pre>{5}</pre>".Fmt(p.person.Name,
               Header,
               p.AmountToPay(),
               others,
               amtpaid,
               p.PrepareSummaryText(ti)));
            }
        }
        public void UseCoupon(string TransactionID, decimal AmtPaid)
        {
            string matchcoupon = @"Coupon\((?<coupon>[^)]*)\)";
            if (Regex.IsMatch(TransactionID, matchcoupon, RegexOptions.IgnoreCase))
            {
                var match = Regex.Match(TransactionID, matchcoupon, RegexOptions.IgnoreCase);
                var coup = match.Groups["coupon"];
                var coupon = "";
                if (coup != null)
                    coupon = coup.Value.Replace(" ", "");
                if (coupon != "Admin")
                {
                    var c = DbUtil.Db.Coupons.SingleOrDefault(cp => cp.Id == coupon);
                    if (c != null)
                    {
                        c.RegAmount = AmtPaid;
                        c.Used = DateTime.Now;
                        c.PeopleId = List[0].PeopleId;
                    }
                }
            }
        }
        public void ConfirmReregister()
        {
            var p = List[0];
            var message = DbUtil.Db.ContentHtml("ReregisterLinkEmail", @"Hi {name},
<p>Here is your <a href=""{url}"">MANAGE REGISTRATION</a> link to manage {orgname}. This link will work only once. Creating an account will allow you to do this again without having to email the link.</p>");
            message = message.Replace("{orgname}", Header);

            var Staff = DbUtil.Db.StaffPeopleForOrg(orgid.Value);
            p.SendOneTimeLink(Staff.First().FromEmail,
                Util.ServerLink("/OnlineReg/RegisterLink/"), "Manage Your Registration for " + Header, message);
        }
        public void ConfirmManageSubscriptions()
        {
            var p = List[0];
            if (p.IsNew)
                p.AddPerson(null, GetEntryPoint());
            if (p.CreatingAccount == true)
                p.CreateAccount();

            var c = DbUtil.Content("OneTimeConfirmation");
            if (c == null)
                c = new Content();

            var message = Util.PickFirst(c.Body,
                    @"Hi {name},
<p>Here is your <a href=""{url}"">link</a> to manage your subscriptions. (note: it will only work once for security reasons)</p> ");

            var Staff = DbUtil.Db.StaffPeopleForOrg(masterorgid.Value);
            p.SendOneTimeLink(
                Staff.First().FromEmail,
                Util.ServerLink("/OnlineReg/ManageSubscriptions/"), "Manage Your Subscriptions", message);
        }
        public void ConfirmPickSlots()
        {
            var p = List[0];
            if (p.IsNew)
                p.AddPerson(null, GetEntryPoint());
            if (p.CreatingAccount == true)
                p.CreateAccount();

            var c = DbUtil.Content("OneTimeConfirmationVolunteer");
            if (c == null)
                c = new Content();

            var message = Util.PickFirst(c.Body,
                    @"Hi {name},
<p>Here is your <a href=""{url}"">link</a> to manage your volunteer commitments. (note: it will only work once for security reasons)</p> ");

            List<Person> Staff = null;
            Staff = DbUtil.Db.StaffPeopleForOrg(orgid.Value);
            p.SendOneTimeLink(
                Staff.First().FromEmail,
                Util.ServerLink("/OnlineReg/ManageVolunteer/"), "Manage Your Volunteer Commitments", message);
        }
        public void ConfirmManagePledge()
        {
            var p = List[0];
            if (p.IsNew)
                p.AddPerson(null, p.org.EntryPointId ?? 0);
            if (p.CreatingAccount == true)
                p.CreateAccount();

            var c = DbUtil.Content("OneTimeConfirmationPledge");
            if (c == null)
            {
                c = new Content();
                c.Title = "Manage your pledge";
                c.Body = @"Hi {name},
<p>Here is your <a href=""{url}"">link</a> to manage your pledge. (note: it will only work once for security reasons)</p> ";
            }

            p.SendOneTimeLink(
                DbUtil.Db.StaffPeopleForOrg(orgid.Value).First().FromEmail,
                Util.ServerLink("/OnlineReg/ManagePledge/"), c.Title, c.Body);
        }
        public void ConfirmManageGiving()
        {
            var p = List[0];
            if (p.IsNew)
                p.AddPerson(null, p.org.EntryPointId ?? 0);
            if (p.CreatingAccount == true)
                p.CreateAccount();

            var c = DbUtil.Content("OneTimeManageGiving");
            if (c == null)
            {
                c = new Content();
                c.Title = "Manage your recurring giving";
                c.Body = @"Hi {name},
<p>Here is your <a href=""{url}"">link</a> to manage your recurring giving. (note: it will only work once for security reasons)</p> ";
            }

            p.SendOneTimeLink(
                DbUtil.Db.StaffPeopleForOrg(orgid.Value).First().FromEmail,
                Util.ServerLink("/OnlineReg/ManageGiving/"), c.Title, c.Body);
        }
        public int GetEntryPoint()
        {
            if (org != null && org.EntryPointId != null)
                return org.EntryPointId.Value;
            if (masterorg != null && masterorg.EntryPointId != null)
                return masterorg.EntryPointId.Value;
            return 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("orgid: {0}<br/>\n", this.orgid);
            sb.AppendFormat("masterorgid: {0}<br/>\n", this.masterorgid);
            sb.AppendFormat("userid: {0}<br/>\n", this.UserPeopleId);
            foreach (var li in List)
            {
                sb.AppendFormat("--------------------------------\nList: {0}<br/>\n", li.Index());
                sb.Append(li.ToString());
            }
            return sb.ToString();
        }
    }
}