using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using CmsWeb.Areas.Manage.Controllers;
using UtilityExtensions;
using CmsData;
using Elmah;
using System.Threading;
using Dapper;

namespace CmsWeb.Areas.Main.Controllers
{
	public class EmailController : CmsStaffController
	{
		[ValidateInput(false)]
		public ActionResult Index(int? id, int? templateID, bool? parents, string body, string subj, bool? ishtml)
		{
			if (!id.HasValue) return Content("no id");
			if (Util.SessionTimedOut()) return Redirect("/Errors/SessionTimeout.htm");
			if (!body.HasValue())
				body = TempData["body"] as string;

			if (!subj.HasValue() && templateID != 0 && DbUtil.Db.Setting("UseEmailTemplates", "false") == "true")
			{
				if (templateID == null)
					return View("SelectTemplate", new EmailTemplatesModel() { wantparents = parents ?? false, queryid = id.Value });
				else
				{
					DbUtil.LogActivity("Emailing people");

					var m = new MassEmailer(id.Value, parents);
					m.CmsHost = DbUtil.Db.CmsHost;
					m.Host = Util.Host;

					ViewBag.templateID = templateID;
					return View("Compose", m);
				}
			}

			// using no templates

			DbUtil.LogActivity("Emailing people");

			var me = new MassEmailer(id.Value, parents);
			me.CmsHost = DbUtil.Db.CmsHost;
			me.Host = Util.Host;

			if (body.HasValue())
				me.Body = Server.UrlDecode(body);

			if (subj.HasValue())
				me.Subject = Server.UrlDecode(subj);

			ViewData["oldemailer"] = "/EmailPeople.aspx?id=" + id
				 + "&subj=" + subj + "&body=" + body + "&ishtml=" + ishtml
				 + (parents == true ? "&parents=true" : "");

			if (parents == true)
				ViewData["parentsof"] = "with ParentsOf option";

			return View(me);
		}
		[ValidateInput(false)]
		public ActionResult Index2(Guid id, int? templateID, bool? parents, string body, string subj, bool? ishtml)
		{
			if (Util.SessionTimedOut()) return Redirect("/Errors/SessionTimeout.htm");
			if (!body.HasValue())
				body = TempData["body"] as string;

			if (!subj.HasValue() && templateID != 0 && DbUtil.Db.Setting("UseEmailTemplates", "false") == "true")
			{
				if (templateID == null)
					return View("SelectTemplate", new EmailTemplatesModel() { wantparents = parents ?? false, queryid = id });
				else
				{
					DbUtil.LogActivity("Emailing people");

					var m = new MassEmailer(id, parents);
					m.CmsHost = DbUtil.Db.CmsHost;
					m.Host = Util.Host;

					ViewBag.templateID = templateID;
					return View("Compose", m);
				}
			}

			// using no templates

			DbUtil.LogActivity("Emailing people");

			var me = new MassEmailer(id, parents);
			me.CmsHost = DbUtil.Db.CmsHost;
			me.Host = Util.Host;

			if (body.HasValue())
				me.Body = Server.UrlDecode(body);

			if (subj.HasValue())
				me.Subject = Server.UrlDecode(subj);

			ViewData["oldemailer"] = "/EmailPeople.aspx?id=" + id
				 + "&subj=" + subj + "&body=" + body + "&ishtml=" + ishtml
				 + (parents == true ? "&parents=true" : "");

			if (parents == true)
				ViewData["parentsof"] = "with ParentsOf option";

			return View("Index", me);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult SaveDraft(int tagId, bool wantParents, int saveid, string name, string subject, string body, int roleid)
		{
			Content content;

			if (saveid > 0)
			{
				content = DbUtil.ContentFromID(saveid);
			}
			else
			{
				content = new Content
				{
				    Name = name.HasValue() ? name 
                        : "new draft " + DateTime.Now.FormatDateTm(), 
                    TypeID = ContentTypeCode.TypeSavedDraft, 
                    RoleID = roleid
				};
				content.OwnerID = Util.UserId;
			}

			content.Title = subject;
			content.Body = body;

			content.DateCreated = DateTime.Now;

			if (saveid == 0) DbUtil.Db.Contents.InsertOnSubmit(content);
			DbUtil.Db.SubmitChanges();

			var m = new MassEmailer
							{
								TagId = tagId,
								wantParents = wantParents,
								CmsHost = DbUtil.Db.CmsHost,
								Host = Util.Host,
								Subject = subject,
							};

			System.Diagnostics.Debug.Print("Template ID: " + content.Id);

			ViewBag.parents = wantParents;
			ViewBag.templateID = content.Id;
			return View("Compose", m);
		}

		[HttpPost]
		public ActionResult ContentDeleteDrafts(int queryid, bool parents, int[] draftId)
		{
			using (var cn = new SqlConnection(Util.ConnectionString))
			{
				cn.Open();
				cn.Execute("delete from dbo.Content where id in @ids", new { ids = draftId });
			}
			return RedirectToAction("Index", new { id = queryid, parents });
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult QueueEmails(MassEmailer m)
		{
			if (!m.Subject.HasValue() || !m.Body.HasValue())
				return Json(new { id = 0, content = "<h2>Both Subject and Body need some text</h2>" });
			if (!User.IsInRole("Admin") && m.Body.Contains("{createaccount}"))
				return Json(new { id = 0, content = "<h2>Only Admin can use {createaccount}</h2>" });

			if (Util.SessionTimedOut())
			{
				Session["massemailer"] = m;
				return Content("timeout");
			}

			DbUtil.LogActivity("Emailing people");

			if (m.EmailFroms().Count(ef => ef.Value == m.FromAddress) == 0)
				return Json(new { id = 0, content = "No email address to send from" });
			m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;

			int id;
			try
			{
				id = m.CreateQueue();
				if (id == 0)
					throw new Exception("No Emails to send (tag does not exist)");
				if (m.Schedule.HasValue)
					return Json(new { id = 0, content = "<h2>Emails Queued</h2>" });
			}
			catch (Exception ex)
			{
				Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
				return Json(new { id = 0, content = "<h2>Error</h2><p>{0}</p>".Fmt(ex.Message) });
			}

			string host = Util.Host;
			// save these from HttpContext to set again inside thread local storage
			var useremail = Util.UserEmail;
			var isinroleemailtest = User.IsInRole("EmailTest");

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				try
				{
					var Db = new CMSDataContext(Util.GetConnectionString(host));
					Db.Host = host;
					var cul = Db.Setting("Culture", "en-US");
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
					// set these again inside thread local storage
					Util.UserEmail = useremail;
					Util.IsInRoleEmailTest = isinroleemailtest;
					Db.SendPeopleEmail(id);
				}
				catch (Exception ex)
				{
					var ex2 = new Exception("Emailing error for queueid " + id, ex);
					ErrorLog errorLog = ErrorLog.GetDefault(null);
					errorLog.Log(new Error(ex2));

					var Db = new CMSDataContext(Util.GetConnectionString(host));
					Db.Host = host;
					var equeue = Db.EmailQueues.Single(ee => ee.Id == id);
					equeue.Error = ex.Message.Truncate(200);
					Db.SubmitChanges();
				}
			});
			string keepdraft = Request["keepdraft"];
			int saveid = Request["saveid"].ToInt();

			System.Diagnostics.Debug.Print("Keep: " + keepdraft + " - Save ID: " + saveid);
			if (keepdraft != "on" && saveid > 0) DbUtil.ContentDeleteFromID(saveid);
			return Json(new { id = id });
		}
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult TestEmail(MassEmailer m)
		{
			if (Util.SessionTimedOut())
			{
				Session["massemailer"] = m;
				return Content("timeout");
			}
			if (m.EmailFroms().Count(ef => ef.Value == m.FromAddress) == 0)
				return Content("No email address to send from");
			m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;
			var From = Util.FirstAddress(m.FromAddress, m.FromName);
			var p = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
			try
			{
				DbUtil.Db.Email(From, p, null, m.Subject, m.Body, false);
			}
			catch (Exception ex)
			{
				return Content("<h2>Error Email Sent</h2>" + ex.Message);
			}
			return Content("<h2>Test Email Sent</h2>");
		}
		[HttpPost]
		public ActionResult TaskProgress(int id)
		{
			var queue = SetProgressInfo(id);
			if (queue == null)
				return Content("no queue");
			return View();
		}
		private EmailQueue SetProgressInfo(int id)
		{
			var emailqueue = DbUtil.Db.EmailQueues.SingleOrDefault(e => e.Id == id);
			if (emailqueue != null)
			{
				var q = from et in DbUtil.Db.EmailQueueTos
						  where et.Id == id
						  select et;
				ViewData["queued"] = emailqueue.Queued.ToString("g");
				ViewData["total"] = q.Count();
				ViewData["sent"] = q.Count(e => e.Sent != null);
				ViewData["finished"] = false;
				if (emailqueue.Started == null)
				{
					ViewData["started"] = "not started";
					ViewData["completed"] = "not started";
					ViewData["elapsed"] = "not started";
				}
				else
				{
					ViewData["started"] = emailqueue.Started.Value.ToString("g");
					var max = q.Max(et => et.Sent);
					max = max ?? DateTime.Now;

					if (emailqueue.Sent == null && !emailqueue.Error.HasValue())
						ViewData["completed"] = "running";
					else
					{
						ViewData["completed"] = max;
						if (emailqueue.Error.HasValue())
							ViewData["Error"] = emailqueue.Error;
						else
							ViewData["finished"] = true;
					}
					ViewData["elapsed"] = max.Value.Subtract(emailqueue.Started.Value).ToString(@"h\:mm\:ss");
				}
			}
			return emailqueue;
		}

		private bool Authenticate()
		{
			string username, password;
			var auth = Request.Headers["Authorization"];
			if (auth.HasValue())
			{
				var cred = System.Text.ASCIIEncoding.ASCII.GetString(
					 Convert.FromBase64String(auth.Substring(6))).Split(':');
				username = cred[0];
				password = cred[1];
			}
			else
			{
				username = Request.Headers["username"];
				password = Request.Headers["password"];
			}
			return CMSMembershipProvider.provider.ValidateUser(username, password);
		}
		public ActionResult CheckQueued()
		{
			var q = from e in DbUtil.Db.EmailQueues
					  where e.SendWhen < DateTime.Now
					  where e.Sent == null
					  select e;

			foreach (var emailqueue in q)
				DbUtil.Db.SendPeopleEmail(emailqueue.Id);
			return Content("done");
		}
		public ActionResult Timeout()
		{
			var m = Session["massemailer"] as MassEmailer;
			if (m == null)
				Response.Redirect("/");
			return View(m);
		}
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CreateVoteTag(int orgid, bool confirm, string smallgroup, string message, string text, string votetagcontent)
		{
			if (votetagcontent.HasValue())
				return Content("<votetag id={0} confirm={1} smallgroup=\"{2}\" message=\"{3}\">{4}</votetag>".Fmt(
					 orgid, confirm, smallgroup, message, votetagcontent));

			return Content("{{votelink id={0} confirm={1} smallgroup=\"{2}\" message=\"{3}\" text=\"{4}\"}}".Fmt(
				 orgid, confirm, smallgroup, message, text));
		}
	}
}
