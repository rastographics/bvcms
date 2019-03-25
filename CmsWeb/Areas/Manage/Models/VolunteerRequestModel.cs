using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Areas.Main.Models;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class VolunteerRequestModel
    {
        //private readonly CMSDataContext Db;
        private List<PotentialSubstitute> potentialSubs;
        private Settings setting;

        public VolunteerRequestModel()
        {
            //Db = Db;
        }

        public VolunteerRequestModel(int mid, int pid, long ticks)
            : this(mid, pid)
        {
            this.ticks = ticks;
        }

        public VolunteerRequestModel(int mid, int pid)
            : this()
        {
            FetchEntities(mid, pid);
        }

        public VolunteerRequestModel(string guid)
            : this()
        {
            var error = "";
            if (!guid.HasValue())
            {
                error = "bad link";
            }

            var g = guid.ToGuid();
            if (g == null)
            {
                error = "invalid link";
            }

            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == g.Value);
            if (ot == null)
            {
                error = "invalid link";
            }

            if (ot.Used)
            {
                error = "link used";
            }

            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
            {
                error = "link expired";
            }

            if (error.HasValue())
            {
                throw new Exception(error);
            }

            ot.Used = true;
            DbUtil.Db.SubmitChanges();
            var a = ot.Querystring.Split(',');
            FetchEntities(a[0].ToInt(), a[1].ToInt());
            ticks = a[2].ToLong();
            vid = a[3].ToInt();
        }

        public long ticks { get; set; } // this is the time the request was composed
        public int vid { get; set; }
        public ICollection<int> pids { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public Meeting meeting { get; set; }
        public Person person { get; set; }
        public Organization org { get; set; }
        public int count { get; set; }
        public int limit { get; set; }
        public string DisplayMessage { get; set; }
        public string Error { get; set; }
        public Settings Setting => setting ?? (setting = DbUtil.Db.CreateRegistrationSettings(org.OrganizationId));

        private void FetchEntities(int mid, int pid)
        {
            var q = from m in DbUtil.Db.Meetings
                    where m.MeetingId == mid
                    let c = m.Attends.Count(aa => AttendCommitmentCode.committed.Contains(aa.Commitment ?? 0))
                    let p = DbUtil.Db.People.Single(pp => pp.PeopleId == pid)
                    select new
                    {
                        m,
                        org = m.Organization,
                        p,
                        c
                    };
            var i = q.Single();
            org = i.org;
            meeting = i.m;
            person = i.p;
            count = i.c;
        }

        public void ComposeMessage()
        {
            var dt = DateTime.Now;
            ticks = dt.Ticks;
            var yeslink = $@"<a href=""http://volreqlink"" mid=""{meeting.MeetingId}"" pid=""{person.PeopleId}"" ticks=""{ticks}"" ans=""yes"">
Yes, I will be there.</a>";
            var nolink = $@"<a href=""http://volreqlink"" mid=""{meeting.MeetingId}"" pid=""{person.PeopleId}"" ticks=""{ticks}"" ans=""no"">
Sorry, I cannot be there.</a>";

            subject = $"Volunteer request for {org.OrganizationName}";
            message = DbUtil.Db.ContentHtml("VolunteerRequest", Resource1.VolunteerRequestModel_ComposeMessage_Body);
            message = message.Replace("{org}", org.OrganizationName)
                .Replace("{meetingdate}", meeting.MeetingDate.ToString2("dddd, MMM d"))
                .Replace("{meetingtime}", meeting.MeetingDate.ToString2("h:mm tt"))
                .Replace("{yeslink}", yeslink)
                .Replace("{nolink}", nolink)
                .Replace("{sendername}", person.Name);
        }

        public List<VolInfo> FetchPotentialVolunteers()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.OrganizationId == org.OrganizationId
                    where om.MemberTypeId != MemberTypeCode.InActive
                    where om.Pending == false
                    where om.PeopleId != person.PeopleId
                    where !DbUtil.Db.Attends.Any(aa => aa.MeetingId == meeting.MeetingId
                                                && aa.Commitment != null && aa.PeopleId == om.PeopleId)
                    orderby om.Person.Name2
                    select new VolInfo
                    {
                        PeopleId = om.PeopleId,
                        Name = om.Person.Name,
                        Email = om.Person.EmailAddress
                    };
            return q.ToList();
        }

        private List<PotentialSubstitute> PotentialSubs()
        {
            return potentialSubs ??
                   (potentialSubs = (from ps in DbUtil.Db.PotentialSubstitutes(org.OrganizationId, meeting.MeetingId)
                                     select ps).ToList());
        }

        public IEnumerable<PotentialSubstitute> CommittedThisMeeting()
        {
            return from ps in PotentialSubs()
                   where ps.Committed != null
                   orderby ps.Name2
                   select ps;
        }

        public IEnumerable<PotentialSubstitute> ThisSchedule()
        {
            return from ps in PotentialSubs()
                   where ps.Committed == null
                   where ps.SameSchedule != null
                   orderby ps.Name2
                   select ps;
        }

        public IEnumerable<PotentialSubstitute> OtherVolunteers()
        {
            return from ps in PotentialSubs()
                   where ps.Committed == null
                   where ps.SameSchedule == null
                   orderby ps.Name2
                   select ps;
        }

        public IEnumerable<SelectListItem> SmallGroups()
        {
            var list = (from t in DbUtil.Db.MemberTags
                        where t.OrgId == org.OrganizationId
                        where t.Name.StartsWith("SG:")
                        orderby t.Name
                        select new SelectListItem { Text = t.Name.Substring(3), Value = ".sg-" + t.Id }).ToList();
            list.Insert(0, new SelectListItem { Text = "(no filter)", Value = "0" });
            return list;
        }

        public void SendEmails(int? additional)
        {
            var tag = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.Db.NextTagId);
            DbUtil.Db.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            DbUtil.Db.TagAll(pids, tag);
            var dt = new DateTime(ticks);

            foreach (var id in pids)
            {
                var vr = new VolRequest
                {
                    MeetingId = meeting.MeetingId,
                    RequestorId = person.PeopleId,
                    Requested = dt,
                    VolunteerId = id
                };
                meeting.VolRequests.Add(vr);
            }

            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause(QueryType.HasMyTag, CompareType.Equal, $"{tag.Id},temp");
            meeting.AddEditExtra(DbUtil.Db, "TotalVolunteersNeeded", ((additional ?? 0) + limit).ToString());
            qb.Save(DbUtil.Db);

            var rurl = DbUtil.Db.ServerLink($"/OnlineReg/VolRequestReport/{meeting.MeetingId}/{person.PeopleId}/{dt.Ticks}");
            var reportlink = $@"<a href=""{rurl}"">Volunteer Request Status Report</a>";
            var list = DbUtil.Db.PeopleFromPidString(org.NotifyIds).ToList();
            //list.Insert(0, person);
            DbUtil.Db.Email(person.FromEmail, list,
                "Volunteer Commitment for " + org.OrganizationName,
                $@"
<p>{person.Name} has requested volunteers on {meeting.MeetingDate:MMM d} for {meeting.MeetingDate:h:mm tt}.</p>
<blockquote>
{reportlink}
</blockquote>");

            // Email subs
            var m = new MassEmailer(qb.Id);
            m.Subject = subject;
            m.Body = message;

            DbUtil.LogActivity("Emailing Vol Subs");
            m.FromName = person.Name;
            m.FromAddress = person.FromEmail;

            var eqid = m.CreateQueue(true).Id;
            var host = Util.Host;
            // save these from HttpContext to set again inside thread local storage
            var useremail = Util.UserEmail;
            var isinroleemailtest = HttpContextFactory.Current.User.IsInRole("EmailTest");

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    var db = DbUtil.Create(host);
                    // set these again inside thread local storage
                    Util.UserEmail = useremail;
                    Util.IsInRoleEmailTest = isinroleemailtest;
                    db.SendPeopleEmail(eqid);
                }
                catch (Exception ex)
                {
                    var ex2 = new Exception("Emailing error for queueid " + eqid, ex);
                    var errorLog = ErrorLog.GetDefault(null);
                    errorLog.Log(new Error(ex2));

                    var db = DbUtil.Create(host);
                    // set these again inside thread local storage
                    Util.UserEmail = useremail;
                    Util.IsInRoleEmailTest = isinroleemailtest;
                    var equeue = db.EmailQueues.Single(ee => ee.Id == eqid);
                    equeue.Error = ex.Message.Truncate(200);
                    db.SubmitChanges();
                }
            });
        }

        public void ProcessReply(string ans)
        {
            var dt = new DateTime(ticks);

            var i = (from rr in DbUtil.Db.VolRequests
                     where rr.MeetingId == meeting.MeetingId
                     where rr.RequestorId == person.PeopleId
                     where rr.Requested == dt
                     where rr.VolunteerId == vid
                     let commits = (from a in rr.Meeting.Attends
                                    where AttendCommitmentCode.committed.Contains(a.Commitment ?? 0)
                                    select a).Count()
                     let needed = (from e in rr.Meeting.MeetingExtras
                                   where e.Field == "TotalVolunteersNeeded"
                                   select e.Data).SingleOrDefault()
                     select new
                     {
                         volunteer = rr.Volunteer,
                         r = rr,
                         commits,
                         needed
                     }).Single();
            if (i.commits >= i.needed.ToInt())
            {
                DisplayMessage = "This volunteer request has already been covered. Thank you so much for responding.";
                return;
            }
            i.r.Responded = DateTime.Now;
            if (ans != "yes")
            {
                DisplayMessage = "Thank you for responding";
                i.r.CanVol = false;
                DbUtil.Db.SubmitChanges();
                return;
            }
            i.r.CanVol = true;
            Attend.MarkRegistered(DbUtil.Db, i.r.VolunteerId, meeting.MeetingId, AttendCommitmentCode.Attending);
            DbUtil.Db.SubmitChanges();
            var body = $@"
<p>{i.volunteer.Name},</p>
<p>Thank you so much.</p>
<p>You are now assigned to volunteer on {meeting.MeetingDate:MMM d, yyyy} at {meeting.MeetingDate:t}.
in {org.OrganizationName}<br />
<p><a href=""http://registerlink"" lang=""{org.OrganizationId}"">Click here</a> to manage your commitments.</p>
<p>See you there!</p>";
            DbUtil.Db.Email(person.FromEmail, i.volunteer, "Thank you for responding and serving", body);

            // on screen message
            DisplayMessage = $@"<p>You have been sent the following email at {Util.ObscureEmail(i.volunteer.EmailAddress)}.</p>
<p>{i.volunteer.Name},</p>
<p>Thank you so much.</p>
<p>You are now assigned to volunteer on {meeting.MeetingDate:MMM d, yyyy} at {meeting.MeetingDate:t}.
in {org.OrganizationName}<br />
<p>See you there!</p>
<p>You will be able to manage your commitments with a link in the email you were just sent.</p>";

            // notify requestor and org notifyids
            var list = DbUtil.Db.PeopleFromPidString(org.NotifyIds).ToList();
            list.Insert(0, person);
            DbUtil.Db.Email(i.volunteer.FromEmail, list,
                "Volunteer Commitment for " + org.OrganizationName,
                $@"
<p>The following email was sent to {i.volunteer.Name}.</p>
<blockquote>
{body}
</blockquote>");
        }

        public IEnumerable<VolStatusInfo> VolRequests()
        {
            var dt = new DateTime(ticks);
            var q = from r in DbUtil.Db.VolRequests
                    where r.MeetingId == meeting.MeetingId
                    where r.RequestorId == person.PeopleId
                    where r.Requested == dt
                    orderby r.Responded descending, r.Volunteer.Name2
                    select new VolStatusInfo
                    {
                        VolName = r.Volunteer.Name,
                        Requested = r.Requested,
                        Responded = r.Responded,
                        CanVol = r.CanVol
                    };
            return q;
        }

        public class VolInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class VolStatusInfo
        {
            public string VolName { get; set; }
            public DateTime Requested { get; set; }
            public DateTime? Responded { get; set; }
            public bool? CanVol { get; set; }

            public HtmlString CanVolDisplay
            {
                get
                {
                    switch (CanVol)
                    {
                        case true:
                            return new HtmlString("<span class=\"red\">Can Volunteer</span>");
                        case false:
                            return new HtmlString("Cannot Volunteer");
                    }
                    return new HtmlString("");
                }
            }
        }
    }
}
