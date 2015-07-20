using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        public class LinkInfo
        {
            internal string error;
            internal OneTimeLink ot;
            internal string[] a;
            internal int? oid;
            internal int? pid;
            private string link;
            private string from;

            public LinkInfo(string link, string from, string id, bool hasorg = true)
            {
                this.link = link;
                this.from = from;
                try
                {
                    if (!id.HasValue())
                        throw LinkException("missing id");
                    var guid = id.ToGuid();
                    if (guid == null)
                        throw LinkException("invalid id");
                    ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
                    if (ot == null)
                        throw LinkException("missing link");

                    a = ot.Querystring.SplitStr(",", 5);
                    if (hasorg)
                        oid = a[0].ToInt();
                    pid = a[1].ToInt();
                    if (ot.Used)
                        throw LinkException("link used");
                    if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                        throw LinkException("link expired");
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            internal Exception LinkException(string msg)
            {
                DbUtil.LogActivity("{0}{1}Error: {2}".Fmt(link, from, msg), oid, pid);
                return new Exception(msg);
            }
        }

        private const string registerlinkSTR = "RegisterLink";
        private const string votelinkSTR = "VoteLink";
        private const string rsvplinkSTR = "RsvpLink";
        private const string sendlinkSTR = "SendLink";

        private const string landingSTR = "Landing";
        private const string confirmSTR = "Confirm";

        public ActionResult VoteLinkSg(string id, string message, bool? confirm)
        {
            var li = new LinkInfo(votelinkSTR, landingSTR, id);
            if (li.error.HasValue())
                return Message(li.error);

            ViewBag.Id = id;
            ViewBag.Message = message;
            ViewBag.Confirm = confirm.GetValueOrDefault().ToString();

            var smallgroup = li.a[4];
            DbUtil.LogActivity("{0}{1}: {2}".Fmt(votelinkSTR, landingSTR, smallgroup), li.oid, li.pid);
            return View();
        }

        [HttpPost]
        public ActionResult VoteLinkSg(string id, string message, bool? confirm, FormCollection formCollection)
        {
            var li = new LinkInfo(votelinkSTR, confirmSTR, id);
            if (li.error.HasValue())
                return Message(li.error);

            try
            {
                var smallgroup = li.a[4];

                if (!li.oid.HasValue)
                    throw new Exception("orgid missing");

                if (!li.pid.HasValue)
                    throw new Exception("peopleid missing");

                var q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == li.oid)
                         let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == li.oid && oo.PeopleId == li.pid)
                         select new { p = pp, org = org, om = om }).Single();

                if (q.org == null && DbUtil.Db.Host == "trialdb")
                {
                    var oid = li.oid + Util.TrialDbOffset;
                    q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
                         let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == li.pid)
                         select new { p = pp, org = org, om = om }).Single();
                }

                if (q.org == null)
                {
                    throw new Exception("org missing, bad link");
                }
                if ((q.org.RegistrationTypeId ?? RegistrationTypeCode.None) == RegistrationTypeCode.None)
                    throw new Exception("votelink is no longer active");

                if (q.om == null && q.org.Limit <= q.org.RegLimitCount(DbUtil.Db))
                    throw new Exception("sorry, maximum limit has been reached");

                if (q.om == null &&
                    (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
                    throw new Exception("sorry, registration has been closed");

                var setting = new Settings(q.org.RegSetting, DbUtil.Db, li.oid.Value);
                if (IsSmallGroupFilled(setting, li.oid.Value, smallgroup))
                    throw new Exception("sorry, maximum limit has been reached for " + smallgroup);

                var omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                     li.oid.Value, li.pid.Value, MemberTypeCode.Member, DateTime.Now, null, false);

                if (q.org.AddToSmallGroupScript.HasValue())
                {
                    var script = DbUtil.Db.Content(q.org.AddToSmallGroupScript);
                    if (script != null && script.Body.HasValue())
                    {
                        try
                        {
                            var pe = new PythonEvents(Util.Host, "RegisterEvent", script.Body);
                            pe.instance.AddToSmallGroup(smallgroup, omb);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                omb.AddToGroup(DbUtil.Db, smallgroup);
                li.ot.Used = true;
                DbUtil.Db.SubmitChanges();

                DbUtil.LogActivity("{0}{1}: {2}".Fmt(votelinkSTR, confirmSTR, smallgroup), li.oid, li.pid);

                if (confirm == true)
                {
                    var subject = Util.PickFirst(setting.Subject, "no subject");
                    var msg = Util.PickFirst(setting.Body, "no message");
                    msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationId, q.org.OrganizationName, q.org.Location, msg);
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
            }
            catch (Exception ex)
            {
                DbUtil.LogActivity("{0}{1}Error: {2}".Fmt(votelinkSTR, confirmSTR, ex.Message), li.oid, li.pid);
                return Message(ex.Message);
            }

            return Message(message);
        }

        public ActionResult RsvpLinkSg(string id, string message, bool? confirm, bool regrets = false)
        {
            var li = new LinkInfo(rsvplinkSTR, landingSTR, id, hasorg: false);
            if (li.error.HasValue())
                return Message(li.error);

            ViewBag.Id = id;
            ViewBag.Message = message;
            ViewBag.Confirm = confirm.GetValueOrDefault().ToString();
            ViewBag.Regrets = regrets.ToString();

            DbUtil.LogActivity("{0}{1}: {2}".Fmt(rsvplinkSTR, landingSTR, regrets), li.oid, li.pid);
            return View();
        }

        [HttpPost]
        public ActionResult RsvpLinkSg(string id, string message, bool? confirm, FormCollection formCollection, bool regrets = false)
        {
            var li = new LinkInfo(rsvplinkSTR, landingSTR, id, hasorg: false);
            if (li.error.HasValue())
                return Message(li.error);

            try
            {
                if (!li.pid.HasValue)
                    throw new Exception("missing peopleid");

                var meetingid = li.a[0].ToInt();
                var emailid = li.a[2].ToInt();
                var smallgroup = li.a[3];
                if (meetingid == 0 && li.a[0].EndsWith(".next"))
                {
                    var orgid = li.a[0].Split('.')[0].ToInt();
                    var nextmeet = (from mm in DbUtil.Db.Meetings
                                    where mm.OrganizationId == orgid
                                    where mm.MeetingDate > DateTime.Now
                                    orderby mm.MeetingDate
                                    select mm).FirstOrDefault();
                    if (nextmeet == null)
                        return Message("no meeting");
                    meetingid = nextmeet.MeetingId;
                }
                var q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let meeting = DbUtil.Db.Meetings.SingleOrDefault(mm => mm.MeetingId == meetingid)
                         let org = meeting.Organization
                         select new { p = pp, org, meeting }).Single();

                if (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive)
                    throw new Exception("sorry, registration has been closed");

                if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
                    throw new Exception("rsvp is no longer available");

                if (q.org.Limit <= q.meeting.Attends.Count(aa => aa.Commitment == 1))
                    throw new Exception("sorry, maximum limit has been reached");
                var omb = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                                                  q.meeting.OrganizationId, li.pid.Value, MemberTypeCode.Member, DateTime.Now, null, false);
                if (smallgroup.HasValue())
                    omb.AddToGroup(DbUtil.Db, smallgroup);


                li.ot.Used = true;
                DbUtil.Db.SubmitChanges();
                Attend.MarkRegistered(DbUtil.Db, li.pid.Value, meetingid, regrets ? AttendCommitmentCode.Regrets : AttendCommitmentCode.Attending);
                DbUtil.LogActivity("{0}{1}: {2}".Fmt(rsvplinkSTR, confirmSTR, regrets), q.org.OrganizationId, li.pid);
                var setting = new Settings(q.org.RegSetting, DbUtil.Db, q.meeting.OrganizationId);

                if (confirm == true)
                {
                    var subject = Util.PickFirst(setting.Subject, "no subject");
                    var msg = Util.PickFirst(setting.Body, "no message");
                    msg = CmsData.API.APIOrganization.MessageReplacements(DbUtil.Db, q.p, q.org.DivisionName, q.org.OrganizationId, q.org.OrganizationName, q.org.Location, msg);
                    msg = msg.Replace("{details}", q.meeting.MeetingDate.ToString2("f"));
                    var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);

                    DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation
                    DbUtil.Db.Email(q.p.FromEmail, NotifyIds,
                              q.org.OrganizationName,
                              "{0} has registered for {1}<br>{2}".Fmt(q.p.Name, q.org.OrganizationName, q.meeting.MeetingDate.ToString2("f")));
                }
            }
            catch (Exception ex)
            {
                DbUtil.LogActivity("{0}{1}Error: {2}".Fmt(rsvplinkSTR, confirmSTR, regrets), peopleid: li.pid);
                return Message(ex.Message);
            }
            return Message(message);
        }

        [ValidateInput(false)]
        // ReSharper disable once FunctionComplexityOverflow
        public ActionResult RegisterLink(string id, bool? showfamily, string source)
        {
            var li = new LinkInfo(registerlinkSTR, landingSTR, id);
            if (li.error.HasValue())
                return Message(li.error);

            try
            {
                if (!li.pid.HasValue)
                    throw new Exception("missing peopleid");

                if (!li.oid.HasValue)
                    throw new Exception("missing orgid");

                var linktype = li.a.Length > 3 ? li.a[3].Split(':') : "".Split(':');
                int? gsid = null;
                if (linktype[0].Equal("supportlink"))
                    gsid = linktype.Length > 1 ? linktype[1].ToInt() : 0;

                var q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == li.oid)
                         let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == li.oid && oo.PeopleId == li.pid)
                         select new { p = pp, org = org, om = om }).Single();

                if (q.org == null && DbUtil.Db.Host == "trialdb")
                {
                    var oid = li.oid + Util.TrialDbOffset;
                    q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let org = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == oid)
                         let om = DbUtil.Db.OrganizationMembers.SingleOrDefault(oo => oo.OrganizationId == oid && oo.PeopleId == li.pid)
                         select new { p = pp, org = org, om = om }).Single();
                }

                if (q.org == null)
                    throw new Exception("org missing, bad link");

                if (q.om == null && !gsid.HasValue && q.org.Limit <= q.org.RegLimitCount(DbUtil.Db))
                    throw new Exception("sorry, maximum limit has been reached");

                if (q.om == null && (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive))
                    throw new Exception("sorry, registration has been closed");

                DbUtil.LogActivity("{0}{1}".Fmt(registerlinkSTR, landingSTR), li.oid, li.pid);

                var url = string.IsNullOrWhiteSpace(source)
                    ? "/OnlineReg/{0}?registertag={1}".Fmt(li.oid, id)
                    : "/OnlineReg/{0}?registertag={1}&source={2}".Fmt(li.oid, id, source);
                if (gsid.HasValue)
                    url += "&gsid=" + gsid;
                if (showfamily == true)
                    url += "&showfamily=true";
                return Redirect(url);
            }
            catch (Exception ex)
            {
                DbUtil.LogActivity("{0}{1}Error: {2}".Fmt(registerlinkSTR, landingSTR, ex.Message), li.oid, li.pid);
                return Message(ex.Message);
            }
        }

        [ValidateInput(false)]
        public ActionResult SendLink(string id)
        {
            var li = new LinkInfo(sendlinkSTR, landingSTR, id);
            if (li.error.HasValue())
                return Message(li.error);

            ViewBag.Id = id;
            DbUtil.LogActivity("{0}{1}".Fmt(sendlinkSTR, landingSTR), li.oid, li.pid);
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendLink(string id, FormCollection formCollection)
        {
            var li = new LinkInfo(sendlinkSTR, landingSTR, id);
            if (li.error.HasValue())
                return Message(li.error);

            try
            {
                if (!li.pid.HasValue)
                    throw new Exception("missing peopleid");

                if (!li.oid.HasValue)
                    throw new Exception("missing orgid");

                var queueid = li.a[2].ToInt();
                var linktype = li.a[3]; // for supportlink, this will also have the goerid
                var q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let org = DbUtil.Db.LoadOrganizationById(li.oid)
                         select new { p = pp, org }).Single();

                if (q.org == null && DbUtil.Db.Host == "trialdb")
                {
                    var oid = li.oid + Util.TrialDbOffset;
                    q = (from pp in DbUtil.Db.People
                         where pp.PeopleId == li.pid
                         let org = DbUtil.Db.LoadOrganizationById(oid)
                         select new { p = pp, org }).Single();
                }

                if (q.org.RegistrationClosed == true || q.org.OrganizationStatusId == OrgStatusCode.Inactive)
                    throw new Exception("sorry, registration has been closed");

                if (q.org.RegistrationTypeId == RegistrationTypeCode.None)
                    throw new Exception("sorry, registration is no longer available");

                DbUtil.LogActivity("{0}{1}".Fmt(sendlinkSTR, confirmSTR), li.oid, li.pid);

                var expires = DateTime.Now.AddMinutes(DbUtil.Db.Setting("SendlinkExpireMintues", "30").ToInt());
                var c = DbUtil.Content("SendLinkMessage");
                if (c == null)
                {
                    c = new Content 
                    {
                        Name = "SendLinkMessage", 
                        Title = "Your Link for {org}", 
                        Body = @"
<p>Here is your temporary <a href='{url}'>LINK</a> to register for {org}.</p>

<p>This link will expire at {time} (30 minutes).
You may request another link by clicking the link in the original email you received.</p>

<p>Note: If you did not request this link, please ignore this email,
or contact the church if you need help.</p>
"
                    };
                    DbUtil.Db.Contents.InsertOnSubmit(c);
                    DbUtil.Db.SubmitChanges();
                }
                var url = EmailReplacements.RegisterLinkUrl(DbUtil.Db,
                    li.oid.Value, li.pid.Value, queueid, linktype, expires);
                var subject = c.Title.Replace("{org}", q.org.OrganizationName);
                var msg = c.Body.Replace("{org}", q.org.OrganizationName)
                    .Replace("{time}", expires.ToString("f"))
                    .Replace("{url}", url)
                    .Replace("%7Burl%7D", url);

                var NotifyIds = DbUtil.Db.StaffPeopleForOrg(q.org.OrganizationId);
                DbUtil.Db.Email(NotifyIds[0].FromEmail, q.p, subject, msg); // send confirmation

                return Message("Thank you, {0}, we just sent an email to {1} with your link...".Fmt(q.p.PreferredName, Util.ObscureEmail(q.p.EmailAddress)));
            }
            catch (Exception ex)
            {
                DbUtil.LogActivity("{0}{1}Error: {2}".Fmt(sendlinkSTR, confirmSTR, ex.Message), li.oid, li.pid);
                return Message(ex.Message);
            }
        }

        private bool IsSmallGroupFilled(Settings setting, int orgid, string sg)
        {
            var GroupTags = (from mt in DbUtil.Db.OrgMemMemTags
                             where mt.OrgId == orgid
                             select mt.MemberTag.Name).ToList();
            return setting.AskItems.Where(aa => aa.Type == "AskDropdown").Any(aa => ((AskDropdown)aa).IsSmallGroupFilled(GroupTags, sg))
                 || setting.AskItems.Where(aa => aa.Type == "AskCheckboxes").Any(aa => ((AskCheckboxes)aa).IsSmallGroupFilled(GroupTags, sg));
        }

        public ActionResult RegisterLinkMaster(int id)
        {
			var pid = TempData["PeopleId"] as int?;
            TempData["PeopleId"] = pid;
            if (pid == null)
                return Message("Must start with a registerlink");
            var m = new OnlineRegModel() { masterorgid = id };
            ViewBag.Token = TempData["token"];
			SetHeaders(id.ToInt());
            return View(m);
        }
    }
}
