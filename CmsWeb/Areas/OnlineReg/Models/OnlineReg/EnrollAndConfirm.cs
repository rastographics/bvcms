using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using CmsData.Codes;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {
        private Regex donationtext = new Regex(@"\{donation(?<text>.*)donation\}", RegexOptions.Singleline | RegexOptions.Multiline);

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
                    OrgId = Orgid,
                });
            }

            if (SupportMissionTrip && GoerId == _list[0].PeopleId)
            {
                // reload transaction because it is not in this context
                var om = Db.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == GoerId && mm.OrganizationId == Orgid);
                if(om != null && om.TranId.HasValue)
                    ti.OriginalId = om.TranId;
            }
            else
            {
                ti.OriginalTrans.TransactionPeople.AddRange(pids2);
            }

            ti.Emails = Util.EmailAddressListToString(elist);
            ti.Participants = participants.ToString();
            ti.TransactionDate = DateTime.Now;

            if (org.IsMissionTrip == true)
            {
                paylink = DbUtil.Db.ServerLink("/OnlineReg/{0}?goerid={1}".Fmt(Orgid, p0.PeopleId));
            }
            else
            {
                var estr = HttpUtility.UrlEncode(Util.Encrypt(ti.OriginalId.ToString()));
                paylink = DbUtil.Db.ServerLink("/OnlineReg/PayAmtDue?q=" + estr);
            }
            var pids = pids2.Select(pp => pp.PeopleId);

            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];

                var q = from pp in Db.People
                        where pids.Contains(pp.PeopleId)
                        where pp.PeopleId != p.PeopleId
                        select pp.Name;
                var others = string.Join(",", q.ToArray());

                var om = p.Enroll(ti, paylink, testing, others);
                om.RegisterEmail = p.EmailAddress;

                if(om.TranId == null)
                    om.TranId = ti.OriginalId;

                int grouptojoin = p.setting.GroupToJoin.ToInt();
                if (grouptojoin > 0)
                {
                    OrganizationMember.InsertOrgMembers(Db, grouptojoin, p.PeopleId.Value, MemberTypeCode.Member, DateTime.Now, null, false);
                    DbUtil.Db.UpdateMainFellowship(grouptojoin);
                }

                OnlineRegPersonModel.CheckNotifyDiffEmails(p.person,
                    Db.StaffEmailForOrg(p.org.OrganizationId),
                    p.fromemail,
                    p.org.OrganizationName,
                    p.org.PhoneNumber);
                if (p.IsCreateAccount())
                    p.CreateAccount();
            }
            Db.SubmitChanges();

            var details = new StringBuilder("<table cellpadding='4'>");
            if (ti.Amt > 0)
                details.AppendFormat(@"
<tr><td colspan='2'>
    <table cellpadding='4'>
        <tr><td>Total Paid</td><td>Total Due</td></tr>
        <tr><td align='right'>{0:c}</td><td align='right'>{1:c}</td></tr>
    </table>
    </td>
</tr>
", amtpaid, amtdue);

            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];
                details.AppendFormat(@"
<tr><td colspan='2'><hr/></td></tr>
<tr><th valign='top'>{0}</th><td>
{1}
</td></tr>", i + 1, p.PrepareSummaryText(ti));
            }
            details.Append("\n</table>\n");

            var DivisionName = org.DivisionName;

            var os = settings[Orgid.Value];
            var EmailSubject = Util.PickFirst(os.Subject, "no subject");
            var EmailMessage = Util.PickFirst(os.Body, "no body");

            bool usedAdmins;
            var NotifyIds = Db.StaffPeopleForOrg(org.OrganizationId, out usedAdmins);

            var Location = org.Location;

            var subject = Util.PickFirst(EmailSubject, "no subject");
            var message = Util.PickFirst(EmailMessage, "no message");

            message = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, p0, DivisionName, org.OrganizationName, Location, message);
            subject = subject.Replace("{org}", Header);

            message = message.Replace("{phone}", org.PhoneNumber.FmtFone7())
                .Replace("{tickets}", List[0].ntickets.ToString())
                .Replace("{details}", details.ToString())
                .Replace("{paid}", amtpaid.ToString("c"))
                .Replace("{sessiontotal}", amtpaid.ToString("c"))
                .Replace("{participants}", details.ToString());
            if (amtdue > 0)
                message = message.Replace("{paylink}", "<a href='{0}'>Click this link to make a payment on your balance of {1:C}</a>.".Fmt(paylink, amtdue));
            else
                message = message.Replace("{paylink}", "You have a zero balance.");

            if (SupportMissionTrip && TotalAmount() > 0)
            {
                var p = List[0];
                ti.Fund = p.setting.DonationFund();
                var goerid = p.Parent.GoerId ?? p.MissionTripGoerId;
                if (p.MissionTripSupportGoer > 0)
                {
                    var gsa = new GoerSenderAmount
                    {
                        Amount = p.MissionTripSupportGoer.Value,
                        Created = DateTime.Now,
                        OrgId = p.orgid.Value,
                        SupporterId = p.PeopleId.Value,
                        NoNoticeToGoer = p.MissionTripNoNoticeToGoer,
                    };
                    if(goerid > 0)
                        gsa.GoerId = goerid;
                    DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(gsa);
                    if (p.Parent.GoerSupporterId.HasValue)
                    {
                        var gs = DbUtil.Db.GoerSupporters.Single(gg => gg.Id == p.Parent.GoerSupporterId);
                        if (!gs.SupporterId.HasValue)
                            gs.SupporterId = p.PeopleId;
                    }
                    if (!ti.TransactionId.StartsWith("Coupon"))
                    {
                        p.person.PostUnattendedContribution(DbUtil.Db,
                            p.MissionTripSupportGoer.Value, p.setting.DonationFundId,
                            "SupportMissionTrip: org={0}; goer={1}".Fmt(p.orgid, goerid), tranid: ti.Id);
                        // send notices
                        if (goerid > 0 && !p.MissionTripNoNoticeToGoer)
                        {
                            var goer = DbUtil.Db.LoadPersonById(goerid.Value);
                            Db.Email(NotifyIds[0].FromEmail, goer, org.OrganizationName + "-donation",
                                "{0:C} donation received from {1}".Fmt(p.MissionTripSupportGoer.Value,
                                    Transaction.FullName(ti)));
                        }
                    }
                }
                if (p.MissionTripSupportGeneral > 0)
                {
                    DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(
                        new GoerSenderAmount
                        {
                            Amount = p.MissionTripSupportGeneral.Value,
                            Created = DateTime.Now,
                            OrgId = p.orgid.Value,
                            SupporterId = p.PeopleId.Value
                        });
                    if (!ti.TransactionId.StartsWith("Coupon"))
                    {
                        p.person.PostUnattendedContribution(DbUtil.Db,
                            p.MissionTripSupportGeneral.Value, p.setting.DonationFundId,
                            "SupportMissionTrip: org={0}".Fmt(p.orgid), tranid: ti.Id);
                    }
                }
                var notifyids = Db.NotifyIds(org.GiftNotifyIds);
                Db.Email(NotifyIds[0].FromEmail, notifyids, org.OrganizationName + "-donation",
                    "${0:N2} donation received from {1}".Fmt(ti.Amt, Transaction.FullName(ti)));

                var senderSubject = os.SenderSubject ?? "NO SUBJECT SET";
                var senderBody = os.SenderBody ?? "NO SENDEREMAIL MESSAGE HAS BEEN SET";
                senderBody = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, p.person, DivisionName, org.OrganizationName, Location, senderBody);
                senderBody = senderBody.Replace("{phone}", org.PhoneNumber.FmtFone7());
                senderBody = senderBody.Replace("{paid}", ti.Amt.ToString2("c"));

                ti.Description = "Mission Trip Giving";
                Db.Email(notifyids[0].FromEmail, p.person, elist, senderSubject, senderBody, false);
                Db.SubmitChanges();
                return;
            }

            if (!SupportMissionTrip && org != null && org.IsMissionTrip == true && ti.Amt > 0)
            {
                var p = List[0];
                ti.Fund = p.setting.DonationFund();

                DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(
                    new GoerSenderAmount
                    {
                        Amount = ti.Amt,
                        GoerId = p.PeopleId,
                        Created = DateTime.Now,
                        OrgId = p.orgid.Value,
                        SupporterId = p.PeopleId.Value
                    });
                if (!ti.TransactionId.StartsWith("Coupon"))
                {
                    p.person.PostUnattendedContribution(DbUtil.Db,
                        ti.Amt.Value, p.setting.DonationFundId,
                        "MissionTrip: org={0}; goer={1}".Fmt(p.orgid, p.PeopleId), tranid: ti.Id);
                    ti.Description = "Mission Trip Giving";
                }
            }
            else if (ti.Donate > 0)
            {
                var p = List[donor.Value];
                ti.Fund = p.setting.DonationFund();
                var desc = "{0}; {1}; {2}, {3} {4}".Fmt(
                    p.person.Name,
                    p.person.PrimaryAddress,
                    p.person.PrimaryCity,
                    p.person.PrimaryState,
                    p.person.PrimaryZip);
                if (!ti.TransactionId.StartsWith("Coupon"))
                    p.person.PostUnattendedContribution(DbUtil.Db, ti.Donate.Value, p.setting.DonationFundId, desc, tranid: ti.Id);
                var ma = donationtext.Match(message);
                if (ma.Success)
                {
                    var v = ma.Groups["text"].Value;
                    message = donationtext.Replace(message, v);
                }
                message = message.Replace("{donation}", ti.Donate.ToString2("N2"));
                // send donation confirmations
                Db.Email(NotifyIds[0].FromEmail, NotifyIds, subject + "-donation",
                    "${0:N2} donation received from {1}".Fmt(ti.Donate, Transaction.FullName(ti)));
            }
            else
                message = donationtext.Replace(message, "");

            DbUtil.Db.SetCurrentOrgId(Orgid);
            // send confirmations
            if (subject != "DO NOT SEND")
                Db.Email(NotifyIds[0].FromEmail, p0, elist,
                    subject, message, false);

            Db.SubmitChanges();
            // notify the staff
            var n = 0;
            foreach (var p in List)
            {
                var tt = pids2.Single(vv => vv.PeopleId == p.PeopleId);
                Db.Email(Util.PickFirst(p.person.FromEmail, NotifyIds[0].FromEmail), NotifyIds, Header,
                    @"{7}{0} has registered for {1}<br/>
Total Fee for this registrant: {2:C}<br/>
Total Fee for this registration: {3:C}<br/>
Total Fee paid today: {4:C}<br/>
AmountDue: {5:C}<br/>
<pre>{6}</pre>".Fmt(p.person.Name,
                        Header,
                        p.TotalAmount(),
                        TotalAmount(),
                        amtpaid,
                        amtdue, // Amount Due
                        p.PrepareSummaryText(ti),
                        usedAdmins ? @"<span style='color:red'>THERE ARE NO NOTIFY IDS ON THIS REGISTRATION!!</span><br/>
<a href='http://docs.touchpointsoftware.com/OnlineRegistration/MessagesSettings.html'>see documentation</a><br/><br/>" : ""));
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
                    OrgId = p.orgid,
                });
            }

            ti.TransactionDate = DateTime.Now;
            var pids = pids2.Select(pp => pp.PeopleId);
            ti.OriginalTrans.TransactionPeople.AddRange(pids2);

            for (var i = 0; i < List.Count; i++)
            {
                var p = List[i];

                var q = from pp in Db.People
                        where pids.Contains(pp.PeopleId)
                        where pp.PeopleId != p.PeopleId
                        select pp.Name;
                var others = string.Join(",", q.ToArray());

                var om = p.Enroll(ti, null, testing, others);
                om.RegisterEmail = p.EmailAddress;
                om.TranId = ti.OriginalId;

                int grouptojoin = p.setting.GroupToJoin.ToInt();
                if (grouptojoin > 0)
                    OrganizationMember.InsertOrgMembers(Db, grouptojoin, p.PeopleId.Value, MemberTypeCode.Member, DateTime.Now, null, false);

                OnlineRegPersonModel.CheckNotifyDiffEmails(p.person,
                    Db.StaffEmailForOrg(p.org.OrganizationId),
                    p.fromemail,
                    p.org.OrganizationName,
                    p.org.PhoneNumber);
                if (p.CreatingAccount == true)
                    p.CreateAccount();

                string DivisionName = masterorg.OrganizationName;
                string OrganizationName = p.org.OrganizationName;

                string emailSubject = null;
                string message = null;

                if (p.setting.Body.HasValue())
                {
                    emailSubject = Util.PickFirst(p.setting.Subject, "no subject");
                    message = p.setting.Body;
                }
                else
                {
                    try
                    {
                        if (masterorgid.HasValue && !settings.ContainsKey(masterorgid.Value))
                            ParseSettings();
                        var os = settings[masterorgid.Value];
                        emailSubject = Util.PickFirst(os.Subject, "no subject");
                        message = Util.PickFirst(os.Body, "no body");
                    }
                    catch (Exception)
                    {
                        if (masterorgid == null)
                            throw new Exception("masterorgid was null");
                        if (settings == null)
                            throw new Exception("settings was null");
                        if (!settings.ContainsKey(masterorgid.Value))
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
                Db.SetCurrentOrgId(p.orgid);

                if (ti.Donate > 0)
                {
                    var pd = List[donor.Value];
                    ti.Fund = pd.setting.DonationFund();
                    var desc = "{0}; {1}; {2}, {3} {4}".Fmt(
                        pd.person.Name,
                        pd.person.PrimaryAddress,
                        pd.person.PrimaryCity,
                        pd.person.PrimaryState,
                        pd.person.PrimaryZip);
                    if (!ti.TransactionId.StartsWith("Coupon"))
                        pd.person.PostUnattendedContribution(DbUtil.Db, ti.Donate.Value, pd.setting.DonationFundId, desc, tranid: ti.Id);
                    var ma = donationtext.Match(message);
                    if (ma.Success)
                    {
                        var v = ma.Groups["text"].Value;
                        message = donationtext.Replace(message, v);
                    }
                    message = message.Replace("{donation}", ti.Donate.ToString2("N2"));
                    // send donation confirmations
                    Db.Email(NotifyIds[0].FromEmail, NotifyIds, emailSubject + "-donation",
                        "${0:N2} donation received from {1}".Fmt(ti.Donate, Transaction.FullName(ti)));
                }
                else
                    message = donationtext.Replace(message, "");

                // send confirmations
                if (emailSubject != "DO NOT SEND")
                    Db.Email(notify.FromEmail, p.person, Util.EmailAddressListFromString(p.fromemail),
                        emailSubject, message, redacted: false);
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
            message = message.Replace("{orgname}", Header).Replace("{org}", Header);

            var Staff = DbUtil.Db.StaffPeopleForOrg(Orgid.Value);
            p.SendOneTimeLink(Staff.First().FromEmail,
                DbUtil.Db.ServerLink("/OnlineReg/RegisterLink/"), "Manage Your Registration for " + Header, message);
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
                DbUtil.Db.ServerLink("/OnlineReg/ManageSubscriptions/"), "Manage Your Subscriptions", message);
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
            Staff = DbUtil.Db.StaffPeopleForOrg(Orgid.Value);
            p.SendOneTimeLink(
                Staff.First().FromEmail,
                DbUtil.Db.ServerLink("/OnlineReg/ManageVolunteer/"), "Manage Your Volunteer Commitments", message);
        }
        public void SendLinkForPledge()
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
                DbUtil.Db.StaffPeopleForOrg(Orgid.Value).First().FromEmail,
                DbUtil.Db.ServerLink("/OnlineReg/ManagePledge/"), c.Title, c.Body);
        }
        public void SendLinkToManageGiving()
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
                DbUtil.Db.StaffPeopleForOrg(Orgid.Value).First().FromEmail,
                DbUtil.Db.ServerLink("/OnlineReg/ManageGiving/"), c.Title, c.Body);
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
            sb.AppendFormat("orgid: {0}<br/>\n", this.Orgid);
            sb.AppendFormat("masterorgid: {0}<br/>\n", this.masterorgid);
            sb.AppendFormat("userid: {0}<br/>\n", this.UserPeopleId);
            foreach (var li in List)
            {
                sb.AppendFormat("--------------------------------\nList: {0}<br/>\n", li.Index);
                sb.Append(li.ToString());
            }
            return sb.ToString();
        }
    }
}