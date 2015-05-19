using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using UtilityExtensions;
using CmsData;
using Elmah;
using System.Threading;
using Dapper;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix="Email"), Route("{action}/{id?}")]
	public class EmailController : CmsStaffController
	{
		[ValidateInput(false)]
        [Route("~/Email/{id:guid}")]
		public ActionResult Index(Guid id, int? templateID, bool? parents, string body, string subj, bool? ishtml, bool? ccparents, bool? nodups, int? orgid)
		{
			if (Util.SessionTimedOut()) return Redirect("/Error/SessionTimeout");
			if (!body.HasValue())
				body = TempData["body"] as string;

            if (!subj.HasValue() && templateID != 0)
			{
				if (templateID == null)
					return View("SelectTemplate", new EmailTemplatesModel()
					{
					    wantparents = parents ?? false, 
                        queryid = id,
					});
				else
				{
					DbUtil.LogActivity("Emailing people");

					var m = new MassEmailer(id, parents, ccparents, nodups);

					m.Host = Util.Host;

					ViewBag.templateID = templateID;
					return View("Compose", m);
				}
			}

			// using no templates

			DbUtil.LogActivity("Emailing people");

			var me = new MassEmailer(id, parents, ccparents, nodups);
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

        public ActionResult EmailBody(string id)
        {
            var i = id.ToInt();
            var c = ViewExtensions2.GetContent(i);
            if(c == null)
                return new EmptyResult();

            ViewBag.content = c;
            return View();
        }
        
        [HttpPost]
		[ValidateInput(false)]
		public ActionResult SaveDraft(MassEmailer m, int saveid, string name, int roleid)
		{
			Content content = null;

			if (saveid > 0)
				content = DbUtil.ContentFromID(saveid);
            if(content == null)
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

			content.Title = m.Subject;
			content.Body = m.Body;
			content.DateCreated = DateTime.Now;

			if (saveid == 0) 
                DbUtil.Db.Contents.InsertOnSubmit(content);

			DbUtil.Db.SubmitChanges();

			System.Diagnostics.Debug.Print("Template ID: " + content.Id);

			ViewBag.parents = m.wantParents;
			ViewBag.templateID = content.Id;
            
			return View("Compose", m);
		}

		[HttpPost]
		public ActionResult ContentDeleteDrafts(Guid queryid, bool parents, int[] draftId)
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
                return Json(new { id = 0, error = "Both subject and body need some text." });
			if (!User.IsInRole("Admin") && m.Body.Contains("{createaccount}", ignoreCase: true))
                return Json(new { id = 0, error = "Only Admin can use {createaccount}." });

			if (Util.SessionTimedOut())
			{
				Session["massemailer"] = m;
				return Content("timeout");
			}

			DbUtil.LogActivity("Emailing people");

			if (m.EmailFroms().Count(ef => ef.Value == m.FromAddress) == 0)
                return Json(new { id = 0, error = "No email address to send from." });

			m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;

			int id;
			try
			{
				var eq = m.CreateQueue();
				if (eq == null)
					throw new Exception("No Emails to send (tag does not exist)");
			    id = eq.Id;
				if (eq.SendWhen.HasValue)
					return Json(new { id = 0, content = "Emails queued to be sent." });
			}
			catch (Exception ex)
			{
				Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
				return Json(new { id = 0, error = ex.Message });
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
					var Db = DbUtil.Create(host);
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

					var Db = DbUtil.Create(host);
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
                return Json(new { error = "No email address to send from." });
			m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;
			var From = Util.FirstAddress(m.FromAddress, m.FromName);
			var p = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
			try
			{
                DbUtil.Db.CopySession();
				DbUtil.Db.Email(From, p, null, m.Subject, m.Body, false);
			}
			catch (Exception ex)
			{
                return Json(new { error = ex.Message });
			}
			return Content("Test email sent.");
		}

		[HttpPost]
		public ActionResult TaskProgress(string id)
		{
		    var idi = id.ToInt();
			var queue = SetProgressInfo(idi);
			if (queue == null)
                return Json(new { error = "No queue." });

		    var title = string.Empty;
		    var message = string.Empty;

		    if ((bool) ViewData["finished"])
		    {
		        title = "Email has completed.";
		    }
            else if (((string) ViewData["error"]).HasValue())
            {
                return Json(new {error = (string) ViewData["error"]});
            }
            else
            {
                title = "Your emails have been queued and will be sent.";
            }

		    message = "Queued: {0}\nStarted: {1}\nTotal Emails: {2}\nSent: {3}\nElapsed: {4}".Fmt(ViewData["queued"],
		        ViewData["started"], ViewData["total"], ViewData["sent"], ViewData["elapsed"]);

            return Json(new {title = title, message = message});
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
