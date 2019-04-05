using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using CmsWeb.Lifecycle;
using Dapper;
using Elmah;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web.Hosting;
using System.Web.Http.Validation;
using System.Web.Mvc;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "Email"), Route("{action}/{id?}")]
    public class EmailController : CmsStaffController
    {
        [ValidateInput(false)]
        [Route("~/Email/{id:guid}")]
        public ActionResult Index(Guid id, int? templateID, bool? parents, string body, string subj, 
                                  bool? ishtml, bool? ccparents, bool? nodups, int? orgid, int? personid, 
                                  bool? recover, bool? onlyProspects, bool? membersAndProspects, bool? useUnlayer)
        {
            if (Util.SessionTimedOut())
            {
                return Redirect("/Errors/SessionTimeout.htm");
            }

            if (!body.HasValue())
            {
                body = TempData["body"] as string;
            }

            if (!subj.HasValue() && templateID != 0)
            {
                if (templateID == null)
                {
                    return View("SelectTemplate", new EmailTemplatesModel
                    {
                        WantParents = parents ?? false,
                        QueryId = id,
                    });
                }

                DbUtil.LogActivity("Emailing people");

                var m = new MassEmailer(id, parents, ccparents, nodups);

                if (User.IsInRole("EmailBuilder"))
                    m.UseUnlayer = useUnlayer;

                m.Host = Util.Host;

                if (body.HasValue())
                {
                    templateID = SaveDraft(null, null, 0, null, body, null, null);
                }

                ViewBag.templateID = templateID;
                m.OrgId = orgid;
                m.guid = id;
                if (recover == true)
                {
                    m.recovery = true;
                }

                return View("Compose", m);
            }

            // using no templates

            DbUtil.LogActivity("Emailing people");

            var me = new MassEmailer(id, parents, ccparents, nodups);

            me.Host = Util.Host;
            me.OnlyProspects = onlyProspects.GetValueOrDefault();

            // Unless UX-AllowMyDataUserEmails is true, CmsController.OnActionExecuting() will filter them
            if (!User.IsInRole("Access"))
            {
                if (templateID != 0 || (!personid.HasValue && !orgid.HasValue))
                {
                    return Redirect($"/Person2/{Util.UserPeopleId}");
                }
            }

            if (personid.HasValue)
            {
                me.AdditionalRecipients = new List<int> { personid.Value };

                var person = CurrentDatabase.LoadPersonById(personid.Value);
                ViewBag.ToName = person?.Name;
            }
            else if (orgid.HasValue)
            {
                var org = CurrentDatabase.LoadOrganizationById(orgid.Value);
                GetRecipientsFromOrg(me, orgid.Value, onlyProspects, membersAndProspects);
                me.Count = me.Recipients.Count();
                ViewBag.ToName = org?.OrganizationName;
            }

            if (body.HasValue())
            {
                me.Body = Server.UrlDecode(body);
            }

            if (subj.HasValue())
            {
                me.Subject = Server.UrlDecode(subj);
            }

            ViewData["oldemailer"] = "/EmailPeople.aspx?id=" + id
                 + "&subj=" + subj + "&body=" + body + "&ishtml=" + ishtml
                 + (parents == true ? "&parents=true" : "");

            if (parents == true)
            {
                ViewData["parentsof"] = "with ParentsOf option";
            }

            return View("Index", me);
        }

        private void GetRecipientsFromOrg(MassEmailer me, int orgId, bool? onlyProspects, bool? membersAndProspects)
        {
            //todo: static ref, use injection
            var members = CurrentDatabase.OrgPeopleCurrent(orgId).Select(x => CurrentDatabase.LoadPersonById(x.PeopleId));
            var prospects = CurrentDatabase.OrgPeopleProspects(orgId, false).Select(x => CurrentDatabase.LoadPersonById(x.PeopleId));

            me.Recipients = new List<string>();
            me.RecipientIds = new List<int>();

            var recipients = new List<Person>();

            if (onlyProspects.GetValueOrDefault())
            {
                recipients = prospects.ToList();
            }
            else if (membersAndProspects.GetValueOrDefault())
            {
                recipients = members.Union(prospects).ToList();
            }
            else
            {
                recipients = members.ToList();
            }

            foreach (var r in recipients)
            {
                me.Recipients.Add(r.ToString());
                me.RecipientIds.Add(r.PeopleId);
            }
        }

        public ActionResult EmailBody(string id, bool? useUnlayer)
        {
            var i = id.ToInt();
            var c = ViewExtensions2.GetContent(i);
            if (c == null)
            {
                return new EmptyResult();
            }

            var design = string.Empty;
            var body = string.Empty;

            if (c.TypeID == ContentTypeCode.TypeUnlayerSavedDraft)
            {
                dynamic payload = JsonConvert.DeserializeObject(c.Body);
                design = payload.design;
                body = payload.rawHtml;
            }
            else
                body = c.Body;
            
            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            var bvedits = doc.DocumentNode.SelectNodes("//div[contains(@class,'bvedit') or @bvedit]");
            if (bvedits == null || !bvedits.Any())
            {
                body = $"<div bvedit='discardthis'>{body}</div>";
            }

            ViewBag.body = body;
            ViewBag.design = design;
            ViewBag.useUnlayer = useUnlayer;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveDraft(MassEmailer m, int saveid, string name, int roleid)
        {
            var id = SaveDraft(saveid, name, roleid, m.Subject, m.Body, m.UnlayerDesign, m.UseUnlayer);

            System.Diagnostics.Debug.Print("Template ID: " + id);

            ViewBag.parents = m.wantParents;
            ViewBag.templateID = id;

            return View("Compose", m);
        }

        private int SaveDraft(int? draftId, string name, int roleId, string draftSubject, 
                              string draftBody, string draftDesign, bool? useUnlayer)
        {
            Content content = null;

            if (draftId.HasValue && draftId > 0)
            {
                content = DbUtil.ContentFromID(draftId.Value);
            }

            if (content != null)
            {
                CurrentDatabase.ArchiveContent(draftId);
            }
            else
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

            if (useUnlayer.GetValueOrDefault())
            {
                var body = new { design = draftDesign, rawHtml = GetBody(draftBody) };
                content.Body = JsonConvert.SerializeObject(body);
                content.TypeID = ContentTypeCode.TypeUnlayerSavedDraft;
            }
            else
            {
                content.Body = GetBody(draftBody);
            }
            
            content.Archived = null;
            content.ArchivedFromId = null;

            content.DateCreated = DateTime.Now;

            if (!draftId.HasValue || draftId == 0)
            {
                CurrentDatabase.Contents.InsertOnSubmit(content);
            }

            CurrentDatabase.SubmitChanges();

            return content.Id;
        }

        [HttpPost]
        public ActionResult CloneDraft(int id, string name, Guid queryId)
        {
            var content = ViewExtensions2.GetContent(id);
            var clone = new Content
            {
                Name = name.HasValue()
                    ? name
                    : "new draft " + DateTime.Now.FormatDateTm(),
                TypeID = content.TypeID,
                RoleID = content.RoleID,
                OwnerID = Util.UserId,
                Title = content.Title,
                Body = content.Body,
                Archived = null,
                ArchivedFromId = null,
                DateCreated = DateTime.Now
            };

            CurrentDatabase.Contents.InsertOnSubmit(clone);
            CurrentDatabase.SubmitChanges();
            return RedirectToAction("Index", new {id = queryId});
        }

        private string GetBody(string body)
        {
            if (body == null)
            {
                body = "";
            }

            body = body.RemoveGrammarly();
            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            var ele = doc.DocumentNode.SelectSingleNode("/div[@bvedit='discardthis']");
            if (ele != null)
            {
                body = ele.InnerHtml;
            }

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
            if (UsesGrammarly(m))
            {
                return Json(new { error = GrammarlyNotAllowed });
            }

            if (TooLarge(m))
            {
                return Json(new { error = TooLargeError });
            }

            if (!m.Subject.HasValue() || !m.Body.HasValue())
            {
                return Json(new { id = 0, error = "Both subject and body need some text." });
            }

            if (!User.IsInRole("Admin") && m.Body.Contains("{createaccount}", ignoreCase: true))
            {
                return Json(new { id = 0, error = "Only Admin can use {createaccount}." });
            }

            if (Util.SessionTimedOut())
            {
                Session["massemailer"] = m;
                return Content("timeout");
            }

            DbUtil.LogActivity("Emailing people");

            if (m.EmailFroms().Count(ef => ef.Value == m.FromAddress) == 0)
            {
                return Json(new { id = 0, error = "No email address to send from." });
            }

            m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;

            int id;
            try
            {
                var eq = m.CreateQueue();
                if (eq == null)
                {
                    throw new Exception("No Emails to send (tag does not exist)");
                }

                id = eq.Id;

                // If there are additional recipients, add them to the queue
                if (m.AdditionalRecipients != null)
                {
                    foreach (var pid in m.AdditionalRecipients)
                    {
                        // Protect against duplicate PeopleIDs ending up in the queue
                        var q3 = from eqt in CurrentDatabase.EmailQueueTos
                                 where eqt.Id == eq.Id
                                 where eqt.PeopleId == pid
                                 select eqt;
                        if (q3.Any())
                        {
                            continue;
                        }
                        eq.EmailQueueTos.Add(new EmailQueueTo
                        {
                            PeopleId = pid,
                            OrgId = eq.SendFromOrgId,
                            Guid = Guid.NewGuid(),
                        });
                    }
                    CurrentDatabase.SubmitChanges();
                }

                if (m.RecipientIds != null)
                {
                    foreach (var pid in m.RecipientIds)
                    {
                        // Protect against duplicate PeopleIDs ending up in the queue
                        var q3 = from eqt in CurrentDatabase.EmailQueueTos
                                 where eqt.Id == eq.Id
                                 where eqt.PeopleId == pid
                                 select eqt;
                        if (q3.Any())
                        {
                            continue;
                        }
                        eq.EmailQueueTos.Add(new EmailQueueTo
                        {
                            PeopleId = pid,
                            OrgId = eq.SendFromOrgId,
                            Guid = Guid.NewGuid(),
                        });
                    }
                    CurrentDatabase.SubmitChanges();
                }

                if (eq.SendWhen.HasValue)
                {
                    return Json(new { id = 0, content = "Emails queued to be sent." });
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new { id = 0, error = ex.Message });
            }

            var host = Util.Host;
            var currorgid = CurrentDatabase.CurrentSessionOrgId;
            // save these from HttpContext to set again inside thread local storage
            var userEmail = Util.UserEmail;
            var isInRoleEmailTest = User.IsInRole("EmailTest");
            var isMyDataUser = User.IsInRole("Access") == false;

            try
            {
                ValidateEmailReplacementCodes(CurrentDatabase, m.Body, new MailAddress(m.FromAddress));
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            var onlyProspects = m.OnlyProspects;

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    var db = DbUtil.Create(host);
                    var cul = CurrentDatabase.Setting("Culture", "en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
                    // set these again inside thread local storage
                    db.SetCurrentOrgId(currorgid);
                    Util.UserEmail = userEmail;
                    Util.IsInRoleEmailTest = isInRoleEmailTest;
                    Util.IsMyDataUser = isMyDataUser;
                    db.SendPeopleEmail(id, onlyProspects);
                }
                catch (Exception ex)
                {
                    var ex2 = new Exception($"Emailing error for queueid {id} on {host}\n{ex.Message}", ex);
                    var cb = new SqlConnectionStringBuilder(Util.ConnectionString) { InitialCatalog = "ELMAH" };
                    var errorLog = new SqlErrorLog(cb.ConnectionString) { ApplicationName = "BVCMS" };
                    errorLog.Log(new Error(ex2));

                    var db = DbUtil.Create(host);
                    var equeue = db.EmailQueues.Single(ee => ee.Id == id);
                    equeue.Error = ex.Message.Truncate(200);
                    db.SubmitChanges();
                }
            });

            return Json(new { id = id });
        }

        private const string TooLargeError = "This email is too large. It appears that it may contain an embedded image. Please see <b><a href='https://docs.touchpointsoftware.com/EmailTexting/EmailFileUpload.html' target='_blank'>this document</a></b> for instructions on how to send an image.";
        private static bool TooLarge(MassEmailer m)
        {
            return (m.Body.Contains("data:image") && m.Body.Length > 100000) || m.Body.Length > 400000;
        }
        private const string GrammarlyNotAllowed = "It appears that you are using Grammarly. Please see <b><a href='https://blog.touchpointsoftware.com/2016/06/29/warning-re-grammarly-and-ck-editor/' target='_blank'>this document</a></b> for important information about why we cannot allow Grammarly";

        public EmailController(IRequestManager requestManager) : base(requestManager)
        {
        }

        private static bool UsesGrammarly(MassEmailer m)
        {
            return m.Body.Contains("btn_grammarly");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TestEmail(MassEmailer m)
        {
            m.Body = GetBody(m.Body);
            if (UsesGrammarly(m))
            {
                return Json(new { error = GrammarlyNotAllowed });
            }

            if (TooLarge(m))
            {
                return Json(new { error = TooLargeError });
            }

            if (Util.SessionTimedOut())
            {
                Session["massemailer"] = m;
                return Content("timeout");
            }

            if (m.EmailFroms().Count(ef => ef.Value == m.FromAddress) == 0)
            {
                return Json(new { error = "No email address to send from." });
            }

            m.FromName = m.EmailFroms().First(ef => ef.Value == m.FromAddress).Text;
            var from = Util.FirstAddress(m.FromAddress, m.FromName);
            var p = CurrentDatabase.LoadPersonById(Util.UserPeopleId ?? 0);

            try
            {
                ValidateEmailReplacementCodes(CurrentDatabase, m.Body, from);

                CurrentDatabase.CopySession();
                CurrentDatabase.Email(from, p, null, m.Subject, m.Body, false);
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
            {
                return Json(new { error = "No queue." });
            }

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
            {
                return Content($"<votetag id={orgid} confirm={confirm} smallgroup=\"{smallgroup}\" message=\"{message}\">{votetagcontent}</votetag>");
            }

            return Content($"{{votelink id={orgid} confirm={confirm} smallgroup=\"{smallgroup}\" message=\"{message}\" text=\"{text}\"}}");
        }

        public ActionResult CheckQueued()
        {
            var q = from e in CurrentDatabase.EmailQueues
                    where e.SendWhen < DateTime.Now
                    where e.Sent == null
                    select e;

            foreach (var emailqueue in q)
            {
                CurrentDatabase.SendPeopleEmail(emailqueue.Id);
            }

            return Content("done");
        }

        private EmailQueue SetProgressInfo(int id)
        {
            var emailqueue = CurrentDatabase.EmailQueues.SingleOrDefault(e => e.Id == id);
            if (emailqueue != null)
            {
                var q = from et in CurrentDatabase.EmailQueueTos
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
                    {
                        ViewData["completed"] = "running";
                    }
                    else
                    {
                        ViewData["completed"] = max;
                        if (emailqueue.Error.HasValue())
                        {
                            ViewData["Error"] = emailqueue.Error;
                        }
                        else
                        {
                            ViewData["finished"] = true;
                        }
                    }
                    ViewData["elapsed"] = max.Value.Subtract(emailqueue.Started.Value).ToString(@"h\:mm\:ss");
                }
            }
            return emailqueue;
        }

        private void ValidateEmailReplacementCodes(CMSDataContext db, string emailText, MailAddress fromAddress)
        {
            var er = new EmailReplacements(db, emailText, fromAddress);
            er.DoReplacements(db, db.LoadPersonById(Util.UserPeopleId ?? 0));
        }
    }
}
