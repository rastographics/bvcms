using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using UtilityExtensions;
using CmsData;
using Elmah;
using System.Threading;
using Dapper;
using HtmlAgilityPack;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "Email"), Route("{action}/{id?}")]
    public class EmailController : CmsStaffController
    {
        [ValidateInput(false)]
        [Route("~/Email/{id:guid}")]
        public ActionResult Index(Guid id, int? templateID, bool? parents, string body, string subj, bool? ishtml, bool? ccparents, bool? nodups, int? orgid)
        {
            if (Util.SessionTimedOut()) return Redirect("/Errors/SessionTimeout.htm");
            if (!body.HasValue())
                body = TempData["body"] as string;

            if (!subj.HasValue() && templateID != 0)
            {
                if (templateID == null)
                    return View("SelectTemplate", new EmailTemplatesModel
                    {
                        WantParents = parents ?? false,
                        QueryId = id,
                    });

                DbUtil.LogActivity("Emailing people");

                var m = new MassEmailer(id, parents, ccparents, nodups);

                m.Host = Util.Host;

                if (body.HasValue())
                    templateID = SaveDraft(null, null, 0, null, body);

                ViewBag.templateID = templateID;
                m.OrgId = orgid;
                return View("Compose", m);
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
            if (c == null)
                return new EmptyResult();

            var doc = new HtmlDocument();
            doc.LoadHtml(c.Body);
            var bvedits = doc.DocumentNode.SelectNodes("//div[contains(@class,'bvedit') or @bvedit]");
            if (bvedits == null || !bvedits.Any())
                c.Body = $"<div bvedit='discardthis'>{c.Body}</div>";

            ViewBag.content = c;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveDraft(MassEmailer m, int saveid, string name, int roleid)
        {
            var id = SaveDraft(saveid, name, roleid, m.Subject, m.Body);

            System.Diagnostics.Debug.Print("Template ID: " + id);

            ViewBag.parents = m.wantParents;
            ViewBag.templateID = id;

            return View("Compose", m);
        }

        private static int SaveDraft(int? draftId, string name, int roleId, string draftSubject, string draftBody)
        {
            Content content = null;

            if (draftId.HasValue && draftId > 0)
                content = DbUtil.ContentFromID(draftId.Value);

            if (content == null)
            {
                content = new Content
                {
                    Name = name.HasValue()
                        ? name
                        : "new draft " + DateTime.Now.FormatDateTm(),
                    TypeID = ContentTypeCode.TypeSavedDraft,
                    RoleID = roleId,
                    OwnerID = Util.UserId
                };
            }

            content.Title = draftSubject;
            content.Body = GetBody(draftBody);

            content.DateCreated = DateTime.Now;

            if (!draftId.HasValue || draftId == 0)
                DbUtil.Db.Contents.InsertOnSubmit(content);

            DbUtil.Db.SubmitChanges();

            return content.Id;
        }

        private static string GetBody(string body)
        {
            if (body == null)
                body = "";
            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            var ele = doc.DocumentNode.SelectSingleNode("/div[@bvedit='discardthis']");
            if (ele != null)
                body = ele.InnerHtml;
            return body;
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
            m.Body = GetBody(m.Body);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new { id = 0, error = ex.Message });
            }

            var host = Util.Host;
            // save these from HttpContext to set again inside thread local storage
            var userEmail = Util.UserEmail;
            var isInRoleEmailTest = User.IsInRole("EmailTest");

            try
            {
                ValidateEmailReplacementCodes(DbUtil.Db, m.Body, new MailAddress(m.FromAddress));
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                try
                {
                    var db = DbUtil.Create(host);
                    var cul = db.Setting("Culture", "en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
                    // set these again inside thread local storage
                    Util.UserEmail = userEmail;
                    Util.IsInRoleEmailTest = isInRoleEmailTest;
                    db.SendPeopleEmail(id, m.CcAddresses);
                }
                catch (Exception ex)
                {
                    var ex2 = new Exception("Emailing error for queueid " + id, ex);
                    var errorLog = ErrorLog.GetDefault(null);
                    errorLog.Log(new Error(ex2));

                    var db = DbUtil.Create(host);
                    var equeue = db.EmailQueues.Single(ee => ee.Id == id);
                    equeue.Error = ex.Message.Truncate(200);
                    db.SubmitChanges();
                }
            });
            return Json(new { id = id });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TestEmail(MassEmailer m)
        {
            m.Body = GetBody(m.Body);
            if (Util.SessionTimedOut())
            {
                Session["massemailer"] = m;
                return Content("timeout");
            }

            if (m.EmailFroms().Count(ef => ef.Value == m.FromAddress) == 0)
                return Json(new { error = "No email address to send from." });

            m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;
            var from = Util.FirstAddress(m.FromAddress, m.FromName);
            var p = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);

            try
            {
                ValidateEmailReplacementCodes(DbUtil.Db, m.Body, from);

                DbUtil.Db.CopySession();
                DbUtil.Db.Email(from, p, null, m.Subject, m.Body, false);
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

            if ((bool)ViewData["finished"])
            {
                title = "Email has completed.";
            }
            else if (((string)ViewData["error"]).HasValue())
            {
                return Json(new { error = (string)ViewData["error"] });
            }
            else
            {
                title = "Your emails have been queued and will be sent.";
            }

            message = $"Queued: {ViewData["queued"]}\nStarted: {ViewData["started"]}\nTotal Emails: {ViewData["total"]}\nSent: {ViewData["sent"]}\nElapsed: {ViewData["elapsed"]}";

            return Json(new { title = title, message = message });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateVoteTag(int orgid, bool confirm, string smallgroup, string message, string text, string votetagcontent)
        {
            if (votetagcontent.HasValue())
                return Content($"<votetag id={orgid} confirm={confirm} smallgroup=\"{smallgroup}\" message=\"{message}\">{votetagcontent}</votetag>");

            return Content($"{{votelink id={orgid} confirm={confirm} smallgroup=\"{smallgroup}\" message=\"{message}\" text=\"{text}\"}}");
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

        private static void ValidateEmailReplacementCodes(CMSDataContext db, string emailText, MailAddress fromAddress)
        {
            var er = new EmailReplacements(db, emailText, fromAddress);
            er.DoReplacements(db, DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value));
        }
    }
}
