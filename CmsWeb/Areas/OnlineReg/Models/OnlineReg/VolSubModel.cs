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
    public class VolSubModel
    {
        public long ticks { get; set; }
        public int sid { get; set; }

        public ICollection<int> pids { get; set; }
        public string subject { get; set; }
        public string message { get; set; }

        //private readonly CMSDataContext Db;

        public Attend attend { get; set; }
        public Person person { get; set; }
        public Organization org { get; set; }

        private void FetchEntities(int aid, int? pid)
        {
            var q = from attend in DbUtil.Db.Attends
                    where attend.AttendId == aid
                    let p = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == pid)
                    select new
                    {
                        attend,
                        org = attend.Organization,
                        person = p,
                    };
            var i = q.SingleOrDefault();
            org = i.org;
            this.attend = i.attend;
            person = i.person;
        }

        public VolSubModel()
        {
            //Db = Db;
        }
        public VolSubModel(int aid, int pid, long ticks)
            : this(aid, pid)
        {
            this.ticks = ticks;
        }
        public VolSubModel(int aid, int pid)
            : this()
        {
            FetchEntities(aid, pid);
        }
        public VolSubModel(string guid)
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
            sid = a[3].ToInt();
        }

        public void ComposeMessage()
        {
            var dt = DateTime.Now;
            ticks = dt.Ticks;
            var yeslink = $@"<a href=""http://volsublink"" aid=""{attend.AttendId}"" pid=""{person.PeopleId}"" ticks=""{ticks}"" ans=""yes"">
Yes, I can sub for you.</a>";
            var nolink = $@"<a href=""http://volsublink"" aid=""{attend.AttendId}"" pid=""{person.PeopleId}"" ticks=""{ticks}"" ans=""no"">
Sorry, I cannot sub for you.</a>";

            subject = $"Volunteer substitute request for {org.OrganizationName}";
            message = DbUtil.Db.ContentHtml("VolunteerSubRequest", Resource1.VolSubModel_ComposeMessage_Body);
            message = message.Replace("{org}", org.OrganizationName)
                .Replace("{meetingdate}", attend.MeetingDate.ToString("dddd, MMM d"))
                .Replace("{meetingtime}", attend.MeetingDate.ToString("h:mm tt"))
                .Replace("{yeslink}", yeslink)
                .Replace("{nolink}", nolink)
                .Replace("{sendername}", person.Name);
        }
        public string DisplayMessage { get; set; }
        public string Error { get; set; }

        public class SubInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        private List<PotentialSubstitute> potentialSubs;

        private List<PotentialSubstitute> PotentialSubs()
        {
            return potentialSubs ??
                   (potentialSubs = (from ps in DbUtil.Db.PotentialSubstitutes(org.OrganizationId, attend.MeetingId)
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
        public void SendEmails()
        {
            var tag = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.Db.NextTagId);
            DbUtil.Db.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
            DbUtil.Db.TagAll(pids, tag);
            var dt = new DateTime(ticks);

            foreach (var id in pids)
            {
                var vr = new SubRequest
                {
                    AttendId = attend.AttendId,
                    RequestorId = person.PeopleId,
                    Requested = dt,
                    SubstituteId = id,
                };
                attend.SubRequests.Add(vr);
            }
            DbUtil.Db.SubmitChanges();

            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset();
            qb.AddNewClause(QueryType.HasMyTag, CompareType.Equal, $"{tag.Id},temp");
            attend.Commitment = CmsData.Codes.AttendCommitmentCode.FindSub;
            qb.Save(DbUtil.Db);

            var rurl = DbUtil.Db.ServerLink($"/OnlineReg/VolSubReport/{attend.AttendId}/{person.PeopleId}/{dt.Ticks}");
            var reportlink = $@"<a href=""{rurl}"">Substitute Status Report</a>";
            var list = DbUtil.Db.PeopleFromPidString(org.NotifyIds).ToList();
            //list.Insert(0, person);
            DbUtil.Db.Email(person.FromEmail, list,
                "Volunteer Substitute Commitment for " + org.OrganizationName,
                $@"
<p>{person.Name} has requested a substitute on {attend.MeetingDate:MMM d} at {attend.MeetingDate:h:mm tt}.</p>
<blockquote>
{reportlink}
</blockquote>");

            // Email subs
            var m = new MassEmailer(qb.Id);
            m.Subject = subject;
            m.Body = message;

            m.FromName = person.Name;
            m.FromAddress = person.FromEmail;

            var eqid = m.CreateQueue(transactional: true).Id;
            string host = Util.Host;
            // save these from HttpContext to set again inside thread local storage
            var useremail = Util.UserEmail;
            var isinroleemailtest = HttpContextFactory.Current.User.IsInRole("EmailTest");
            Log("Send Emails");

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
                    Log("Email Error");
                    var ex2 = new Exception("Emailing error for queueid " + eqid, ex);
                    ErrorLog errorLog = ErrorLog.GetDefault(null);
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

        public class VolSubLogInfo
        {
            public SubRequest rr { get; set; }
            public int oid { get; set; }
            public DateTime dt { get; set; }
        }

        public void PrepareToClaim(string ans, string guid)
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

            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
            {
                error = "link expired";
            }

            if (error.HasValue())
            {
                throw new Exception(error);
            }

            var a = ot.Querystring.Split(',');
            FetchEntities(a[0].ToInt(), a[1].ToInt());
            ticks = a[2].ToLong();
            sid = a[3].ToInt();
            var dt = new DateTime(ticks);
            var i = (from rr in DbUtil.Db.SubRequests
                     where rr.AttendId == attend.AttendId
                     where rr.RequestorId == person.PeopleId
                     where rr.Requested == dt
                     where rr.SubstituteId == sid
                     select rr).Single();
            Log("PrepareToClaim:" + ans, i.Requested, i.SubstituteId);
        }
        public void ProcessReply(string ans)
        {
            var dt = new DateTime(ticks);
            var i = (from rr in DbUtil.Db.SubRequests
                     where rr.AttendId == attend.AttendId
                     where rr.RequestorId == person.PeopleId
                     where rr.Requested == dt
                     where rr.SubstituteId == sid
                     select rr).Single();

            if (attend.Commitment == AttendCommitmentCode.SubFound || attend.SubRequests.Any(ss => ss.CanSub == true))
            {
                DisplayMessage = "This substitute request has already been covered. Thank you so much for responding.";
                Log("Covered", i.Requested, i.SubstituteId);
                return;
            }
            i.Responded = DateTime.Now;
            if (ans != "yes")
            {
                DisplayMessage = "Thank you for responding";
                i.CanSub = false;
                Log("Regrets", i.Requested, i.SubstituteId);
                DbUtil.Db.SubmitChanges();
                return;
            }
            i.CanSub = true;
            Attend.MarkRegistered(DbUtil.Db, i.Substitute.PeopleId, attend.MeetingId, AttendCommitmentCode.Substitute);
            attend.Commitment = AttendCommitmentCode.SubFound;
            Log("Claimed", i.Requested, i.SubstituteId);
            DbUtil.Db.SubmitChanges();

            message = DbUtil.Db.ContentHtml("VolunteerSubConfirm", Resource1.VolSubModel_VolunteerSubConfirm);

            var body = message
                .Replace("{substitute}", i.Substitute.Name)
                .Replace("{requestor}", i.Requestor.Name)
                .Replace("{org}", org.OrganizationName)
                .Replace("{meetingdate}", $"{attend.MeetingDate:MMM d, yyyy}")
                .Replace("{meetingtime}", $"{attend.MeetingDate:h:mm tt}");

            // on screen message
            DisplayMessage = $"<p>You have been sent the following email at {Util.ObscureEmail(i.Substitute.EmailAddress)}.</p>\n" + body;

            // email confirmation
            DbUtil.Db.Email(i.Requestor.FromEmail, i.Substitute,
                "Volunteer Substitute Commitment for " + org.OrganizationName, body);

            // notify requestor and org notifyids
            var list = DbUtil.Db.PeopleFromPidString(org.NotifyIds).ToList();
            list.Insert(0, i.Requestor);
            DbUtil.Db.Email(i.Substitute.FromEmail, list,
                "Volunteer Substitute Commitment for " + org.OrganizationName,
                $@"
<p>The following email was sent to {i.Substitute.Name}.</p>
<blockquote>
{body}
</blockquote>");
        }

        public void Log(string action, DateTime? reqtime, int? sub)
        {
            DbUtil.LogActivity($"VolSub {action} , mdt={attend.MeetingDate.FormatDateTm()}, rdt={reqtime.FormatDateTm()}, sub={sub}",
                attend.OrganizationId, attend.PeopleId);
        }
        public void Log(string action)
        {
            DbUtil.LogActivity($"VolSub {action}, mdt={attend.MeetingDate.FormatDateTm()}", attend.OrganizationId, attend.PeopleId);
        }

        public class SubStatusInfo
        {
            public string SubName { get; set; }
            public DateTime Requested { get; set; }
            public DateTime? Responded { get; set; }
            public bool? CanSub { get; set; }
            public HtmlString CanSubDisplay
            {
                get
                {
                    switch (CanSub)
                    {
                        case true: return new HtmlString("<span class=\"red\">Can Substitute</span>");
                        case false: return new HtmlString("Cannot Substitute");
                    }
                    return new HtmlString("");
                }
            }
        }
        public IEnumerable<SubStatusInfo> SubRequests()
        {
            var dt = new DateTime(ticks);
            var q = from r in DbUtil.Db.SubRequests
                    where r.AttendId == attend.AttendId
                    where r.RequestorId == person.PeopleId
                    where r.Requested == dt
                    orderby r.Responded descending, r.Substitute.Name2
                    select new SubStatusInfo
                    {
                        SubName = r.Substitute.Name,
                        Requested = r.Requested,
                        Responded = r.Responded,
                        CanSub = r.CanSub
                    };
            return q;
        }
        private Settings setting;
        public Settings Setting => setting ?? (setting = DbUtil.Db.CreateRegistrationSettings(org.OrganizationId));
    }
}
