using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Models;
using UtilityExtensions;
using System.Collections.Generic;
using CmsData.Codes;
using CmsWeb.Models.OrganizationPage;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        public ActionResult VoteLinkSg(string id, string message, bool? confirm)
        {
            if (!id.HasValue())
                return Content("bad link");

            var guid = id.ToGuid();
            if (guid == null)
                return Content("invalid link");
            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
                return Content("invalid link");
            if (ot.Used)
                return Content("link used");
            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                return Content("link expired");
            var a = ot.Querystring.SplitStr(",", 5);
            var oid = a[0].ToInt();
            var pid = a[1].ToInt();
            var emailid = a[2].ToInt();
            var pre = a[3];
            var smallgroup = a[4];
            var q = (from pp in DbUtil.Db.People
                     where pp.PeopleId == pid
                     let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
                     let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
                     select new { p = pp, org = org, om = om }).Single();

            if (q.org == null)
                return Content("org missing, bad link");

            if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
                return Content("votelink is no longer active");

            if (q.om == null && q.org.Limit <= q.org.MemberCount)
                return Content("sorry, maximum limit has been reached");

            if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
                return Content("sorry, registration has been closed");

            var setting = new Settings(q.org.RegSetting, DbUtil.Db, oid);
            if (IsSmallGroupFilled(setting, oid, smallgroup))
                return Content("sorry, maximum limit has been reached for " + smallgroup);

            var omb = q.om;
            omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                 oid, pid, 220, DateTime.Now, null, false);
            //DbUtil.Db.UpdateMainFellowship(oid);

            if (q.org.AddToSmallGroupScript.HasValue())
            {
                var script = DbUtil.Db.Content(q.org.AddToSmallGroupScript);
                if (script != null && script.Body.HasValue())
                {
                    try
                    {
                        var pe = new PythonEvents(DbUtil.Db, "RegisterEvent", script.Body);
                        pe.instance.AddToSmallGroup(smallgroup, omb);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            omb.AddToGroup(DbUtil.Db, smallgroup);
            omb.AddToGroup(DbUtil.Db, "emailid:" + emailid);
            ot.Used = true;
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Votelink: {0}".Fmt(q.org.OrganizationName));

            if (confirm == true)
            {
                var subject = Util.PickFirst(setting.Subject, "no subject");
                var msg = Util.PickFirst(setting.Body, "no message");
                msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationName, q.org.Location, msg);
                msg = msg.Replace("{details}", smallgroup);
                var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

                try
                {
                    DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
                }
                catch (Exception ex)
                {
                    DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
                                         q.org.OrganizationName,
                                         "There was a problem sending confirmation from org: " + ex.Message);
                }
                DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
                          q.org.OrganizationName,
                          "{0} has registered for {1}<br>{2}<br>(from votelink)".Fmt(q.p.Name, q.org.OrganizationName, smallgroup));
            }

            return Content(message);
        }

        public ActionResult TestVoteLink(string id, string smallgroup, string message, bool? confirm)
        {
            if (!id.HasValue())
                return Content("bad link");

            var guid = id.ToGuid();
            if (guid == null)
                return Content("not a guid");
            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
                return Content("cannot find link");
            if (ot.Used)
                return Content("link used");
            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                return Content("link expired");
            var a = ot.Querystring.SplitStr(",", 5);
            var oid = a[0].ToInt();
            var pid = a[1].ToInt();
            var emailid = a[2].ToInt();
            var pre = a[3];
            if (a.Length == 5)
                smallgroup = a[4];
            var q = (from pp in DbUtil.Db.People
                     where pp.PeopleId == pid
                     let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
                     let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
                     select new { p = pp, org, om }).SingleOrDefault();
            if (q == null)
                return Content("peopleid {0} not found".Fmt(pid));

            if (q.org == null)
                return Content("no org " + oid);

            if (q.om == null && q.org.Limit <= q.org.MemberCount)
                return Content("sorry, maximum limit has been reached");

            if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
                return Content("sorry, registration has been closed");

            var setting = new Settings(q.org.RegSetting, DbUtil.Db, oid);
            if (IsSmallGroupFilled(setting, oid, smallgroup))
                return Content("sorry, maximum limit has been reached for " + smallgroup);

            return Content(@"<pre>
looks ok
oid={0}
pid={1}
emailid={2}
</pre>".Fmt(oid, pid, emailid));
        }

        public ActionResult RsvpLinkSg(string id, string message, bool? confirm)
        {
            if (!id.HasValue())
                return Content("bad link");

            var guid = id.ToGuid();
            if (guid == null)
                return Content("invalid link");
            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
                return Content("invalid link");
            if (ot.Used)
                return Content("link used");
            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                return Content("link expired");
            var a = ot.Querystring.SplitStr(",", 4);
            var meetingid = a[0].ToInt();
            var pid = a[1].ToInt();
            var emailid = a[2].ToInt();
            var smallgroup = a[3];
            var q = (from pp in DbUtil.Db.People
                     where pp.PeopleId == pid
                     let meeting = DbUtil.Db.Meetings.SingleOrDefault(mm => mm.MeetingId == meetingid)
                     let org = meeting.Organization
                     select new { p = pp, org, meeting }).Single();

            if (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive)
                return Content("sorry, registration has been closed");

            if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
                return Content("rsvp is no longer available");

            if (q.org.Limit <= q.meeting.Attends.Count(aa => aa.Commitment == 1))
                return Content("sorry, maximum limit has been reached");
            var omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                                              q.meeting.OrganizationId, pid, 220, DateTime.Now, null, false);
            if (smallgroup.HasValue())
                omb.AddToGroup(DbUtil.Db, smallgroup);
            omb.AddToGroup(DbUtil.Db, "emailid:" + emailid);


            ot.Used = true;
            DbUtil.Db.SubmitChanges();
            Attend.MarkRegistered(DbUtil.Db, pid, meetingid, 1);
            DbUtil.LogActivity("Rsvplink: {0}".Fmt(q.org.OrganizationName));
            var setting = new Settings(q.org.RegSetting, DbUtil.Db, q.meeting.OrganizationId);

            if (confirm == true)
            {
                var subject = Util.PickFirst(setting.Subject, "no subject");
                var msg = Util.PickFirst(setting.Body, "no message");
                msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationName, q.org.Location, msg);
                msg = msg.Replace("{details}", q.meeting.MeetingDate.ToString2("f"));
                var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

                DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
                DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
                          q.org.OrganizationName,
                          "{0} has registered for {1}<br>{2}".Fmt(q.p.Name, q.org.OrganizationName, q.meeting.MeetingDate.ToString2("f")));
            }
            return Content(message);

        }

        [ValidateInput(false)]
        public ActionResult RegisterLink(string id, bool? showfamily)
        {
            if (!id.HasValue())
                return Content("bad link");
            if (!Request.Browser.Cookies)
                return Content(Request.UserAgent + "<br>Your browser must support cookies");

            var guid = id.ToGuid();
            if (guid == null)
                return Content("invalid link");
            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
                return Content("invalid link");
            if (ot.Used)
                return Content("link used");
            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                return Content("link expired");
            var a = ot.Querystring.Split(',');
            var oid = a[0].ToInt();
            var pid = a[1].ToInt();
            var q = (from pp in DbUtil.Db.People
                     where pp.PeopleId == pid
                     let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
                     let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == pid)
                     select new { p = pp, org = org, om = om }).Single();

            if (q.org == null)
                return Content("org missing, bad link");

            if (q.om == null && q.org.Limit <= q.org.MemberCount)
                return Content("sorry, maximum limit has been reached");

            if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
                return Content("sorry, registration has been closed");

            var url = "/OnlineReg/Index/{0}?registertag={1}".Fmt(oid, id);
            if (showfamily == true)
                url += "&showfamily=true";
            return Redirect(url);
        }

        public ActionResult SendLink(string id)
        {
            if (!id.HasValue())
                return Content("bad link");

            var guid = id.ToGuid();
            if (guid == null)
                return Content("invalid link");
            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
                return Content("invalid link");
            if (ot.Used)
                return Content("link used");
            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                return Content("link expired");
            var a = ot.Querystring.SplitStr(",", 4);
            var orgid = a[0].ToInt();
            var pid = a[1].ToInt();
            var queueid = a[2].ToInt();
            var linktype = a[3];
            var q = (from pp in DbUtil.Db.People
                     where pp.PeopleId == pid
                     let org = DbUtil.Db.LoadOrganizationById(orgid)
                     select new { p = pp, org }).Single();

            if (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive)
                return Content("sorry, registration has been closed");

            if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
                return Content("sorry, registration is no longer available");

            ot.Used = true;
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Sendlink: {0}".Fmt(q.org.OrganizationName));

            var subject = "Your link for " + q.org.OrganizationName;
            var msg = @"<p>Here is your <a href=""{0}"">LINK</a></p>
<p>Note: If you did not request this link, please ignore this email,
or contact the church if you need help.</p>"
                .Fmt(DbUtil.Db.RegisterLinkUrl(orgid, pid, queueid, showfamily: linktype == "registerlink2"));
            var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

            DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation

            return Content("Thank you, {0}, we just sent an email to {1} with your link...".Fmt(q.p.PreferredName, Util.ObscureEmail(q.p.EmailAddress)));
        }

        private bool IsSmallGroupFilled(Settings setting, int orgid, string sg)
        {
            var GroupTags = (from mt in DbUtil.Db.OrgMemMemTags
                             where mt.OrgId == orgid
                             select mt.MemberTag.Name).ToList();
            return setting.AskItems.Where(aa => aa.Type == "AskDropdown").Any(aa => ((AskDropdown)aa).IsSmallGroupFilled(GroupTags, sg))
                 || setting.AskItems.Where(aa => aa.Type == "AskCheckboxes").Any(aa => ((AskCheckboxes)aa).IsSmallGroupFilled(GroupTags, sg));
        }
    }
}
