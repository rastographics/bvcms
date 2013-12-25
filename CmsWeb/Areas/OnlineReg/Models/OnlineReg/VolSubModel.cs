using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Areas.Manage.Controllers;
using UtilityExtensions;
using CmsWeb.Areas.Main.Models;
using TaskAlias = System.Threading.Tasks.Task;
using System.Threading;
using Elmah;

namespace CmsWeb.Models
{
	public class VolSubModel
	{
		public long ticks { get; set; }
		public int sid { get; set; }

		public ICollection<int> pids { get; set; }
		public string subject { get; set; }
		public string message { get; set; }

		private CMSDataContext Db;

		public Attend attend { get; set; }
		public Person person { get; set; } 
		public Organization org { get; set; }

		private void FetchEntities(int aid, int? pid)
		{
			var q = from attend in Db.Attends
					where attend.AttendId == aid
					let p = Db.People.SingleOrDefault(pp => pp.PeopleId == pid)
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
			Db = DbUtil.Db;
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
				error = "bad link";
			var g = guid.ToGuid();
			if (g == null)
				error = "invalid link";
			var ot = Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == g.Value);
			if (ot == null)
				error = "invalid link";
			if (ot.Used)
				error = "link used";
			if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
				error = "link expired";
			if (error.HasValue())
				throw new Exception(error);
			ot.Used = true;
			Db.SubmitChanges();
			var a = ot.Querystring.Split(',');
			FetchEntities(a[0].ToInt(), a[1].ToInt());
			ticks = a[2].ToLong();
			sid = a[3].ToInt();
		}

		public void ComposeMessage()
		{
			var dt = DateTime.Now;
			ticks = dt.Ticks;
			var yeslink = @"<a href=""http://volsublink"" aid=""{0}"" pid=""{1}"" ticks=""{2}"" ans=""yes"">
Yes, I can sub for you.</a>".Fmt(attend.AttendId, person.PeopleId, ticks);
			var nolink = @"<a href=""http://volsublink"" aid=""{0}"" pid=""{1}"" ticks=""{2}"" ans=""no"">
Sorry, I cannot sub for you.</a>".Fmt(attend.AttendId, person.PeopleId, ticks);

			subject = "Volunteer substitute request for {0}".Fmt(org.OrganizationName);
            message = DbUtil.Db.ContentHtml("VolunteerSubRequest", Resource1.VolSubModel_ComposeMessage_Body);
		    message = message.Replace("{org}", org.OrganizationName);
            message = message.Replace("{meetingdate}", attend.MeetingDate.ToString("dddd, MMM d"));
            message = message.Replace("{meetingtime}", attend.MeetingDate.ToString("h:mm tt"));
            message = message.Replace("{yeslink}", yeslink);
            message = message.Replace("{nolink}", nolink);
            message = message.Replace("{sendername}", person.Name);
		}
		public string DisplayMessage { get; set; }
		public string Error { get; set; }

		public class SubInfo
		{
			public int PeopleId { get; set; }
			public string Name { get; set; }
			public string Email { get; set; }
		}

		public List<SubInfo> FetchPotentialSubs()
		{
			var q = from om in Db.OrganizationMembers
			        where om.OrganizationId == org.OrganizationId
			        where om.MemberTypeId != CmsData.Codes.MemberTypeCode.InActive
			        where om.Pending == false
			        where om.PeopleId != person.PeopleId
			        where !Db.Attends.Any(aa => aa.MeetingId == attend.MeetingId
			                                    && aa.Commitment != null && aa.PeopleId == om.PeopleId)
			        orderby om.Person.Name2
			        select new SubInfo()
			        {
				        PeopleId = om.PeopleId,
				        Name = om.Person.Name,
				        Email = om.Person.EmailAddress
			        };
			return q.ToList();
		}
		public void SendEmails()
		{
			var tag = Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, Db.NextTagId);
			Db.ExecuteCommand("delete TagPerson where Id = {0}", tag.Id);
			Db.TagAll(pids, tag);
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

			var qb = Db.ScratchPadCondition();
			qb.Reset(Db);
			qb.AddNewClause(QueryType.HasMyTag, CompareType.Equal, "{0},temp".Fmt(tag.Id));
			attend.Commitment = CmsData.Codes.AttendCommitmentCode.FindSub;
			Db.SubmitChanges();

			var reportlink = @"<a href=""{0}OnlineReg/VolSubReport/{1}/{2}/{3}"">Substitute Status Report</a>"
				.Fmt(Db.CmsHost, attend.AttendId, person.PeopleId, dt.Ticks);
			var list = Db.PeopleFromPidString(org.NotifyIds).ToList();
			//list.Insert(0, person);
			Db.Email(person.FromEmail, list,
				"Volunteer Substitute Commitment for " + org.OrganizationName,
				@"
<p>{0} has requested a substitute on {1:MMM d} at {1:h:mm tt}.</p>
<blockquote>
{2}
</blockquote>".Fmt(person.Name, attend.MeetingDate, reportlink));

			// Email subs
			var m = new MassEmailer(qb.Id, null);
			m.Subject = subject;
			m.Body = message;

			DbUtil.LogActivity("Emailing Vol Subs");
			m.FromName = person.Name;
			m.FromAddress = person.FromEmail;

			var eqid = m.CreateQueue(transactional: true);
			string host = Util.Host;
			// save these from HttpContext to set again inside thread local storage
			var useremail = Util.UserEmail;
			var isinroleemailtest = HttpContext.Current.User.IsInRole("EmailTest");

			TaskAlias.Factory.StartNew(() =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				try
				{
					var db = new CMSDataContext(Util.GetConnectionString(host));
					db.Host = host;
					// set these again inside thread local storage
					Util.UserEmail = useremail;
					Util.IsInRoleEmailTest = isinroleemailtest;
					db.SendPeopleEmail(eqid);
				}
				catch (Exception ex)
				{
					var ex2 = new Exception("Emailing error for queueid " + eqid, ex);
					ErrorLog errorLog = ErrorLog.GetDefault(null);
					errorLog.Log(new Error(ex2));

					var db = new CMSDataContext(Util.GetConnectionString(host));
					db.Host = host;
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
			if (attend.SubRequests.Any(ss => ss.CanSub == true))
			{
				DisplayMessage = "This substitute request has already been covered. Thank you so much for responding.";
				return;
			}
			var dt = new DateTime(ticks);
			var r = (from rr in Db.SubRequests
					 where rr.AttendId == attend.AttendId
					 where rr.RequestorId == person.PeopleId
					 where rr.Requested == dt
					 where rr.SubstituteId == sid
					 select rr).Single();
			r.Responded = DateTime.Now;
			if (ans != "yes")
			{
				DisplayMessage = "Thank you for responding";
				r.CanSub = false;
				Db.SubmitChanges();
				return;
			}
			r.CanSub = true;
			Attend.MarkRegistered(Db, r.Substitute.PeopleId, attend.MeetingId, CmsData.Codes.AttendCommitmentCode.Substitute);
			attend.Commitment = CmsData.Codes.AttendCommitmentCode.SubFound;
			Db.SubmitChanges();
			var body = @"
<p>{0},</p>
<p>Thank you so much.</p>
<p>You are now assigned to cover for {1}<br />
in the {2}<br />
on {3:MMM d, yyyy} at {3:t}.
See you there!</p>".Fmt(r.Substitute.Name, r.Requestor.Name,
				org.OrganizationName, attend.MeetingDate);

			// on screen message
			DisplayMessage = "<p>You have been sent the following email at {0}.</p>\n"
				.Fmt(Util.ObscureEmail(r.Substitute.EmailAddress)) + body;

			// email confirmation
			Db.Email(r.Requestor.FromEmail, r.Substitute,
				"Volunteer Substitute Committment for " + org.OrganizationName, body);

			// notify requestor and org notifyids
			var list = Db.PeopleFromPidString(org.NotifyIds).ToList();
			list.Insert(0, r.Requestor);
			Db.Email(r.Substitute.FromEmail, list,
				"Volunteer Substitute Committment for " + org.OrganizationName,
				@"
<p>The following email was sent to {0}.</p>
<blockquote>
{1}
</blockquote>".Fmt(r.Substitute.Name, body));
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
			var q = from r in Db.SubRequests
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
	}
}