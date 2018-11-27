using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using Elmah;
using RestSharp;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage"), Route("~/Manage/Emails/{action}/{id?}")]
    public class EmailsController : CmsController
    {
        public EmailsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Emails")]
        public ActionResult Index()
        {
            var m = new EmailsModel();
            return View(m);
        }

        public ActionResult SentBy(int? id)
        {
            var m = new EmailsModel { senderid = id };
            return View("Index", m);
        }

        public ActionResult SentTo(int? id)
        {
            var m = new EmailsModel { peopleid = id };
            return View("Index", m);
        }

        [Route("~/Emails/Details/{id:int}")]
        [Route("~/Manage/Emails/Details/{id:int}")]
        public ActionResult Details(int id)
        {
            var m = new EmailModel(id);
            if (m.queue == null)
            {
                return Content("no email found");
            }

            var curruser = CurrentDatabase.LoadPersonById(Util.UserPeopleId ?? 0);
            if (curruser == null)
            {
                return Content("no user");
            }

            if (User.IsInRole("Admin")
                || User.IsInRole("ManageEmails")
                || User.IsInRole("Finance")
                || m.queue.FromAddr == curruser.EmailAddress
                || m.queue.QueuedBy == curruser.PeopleId
                || m.queue.EmailQueueTos.Any(et => et.PeopleId == curruser.PeopleId))
            {
                return View(m);
            }

            return Content("not authorized");
        }

        public ActionResult SeeHtml(int id)
        {
            var m = new EmailModel(id);
            if (m.queue == null)
            {
                return Content("no email found");
            }

            var curruser = CurrentDatabase.LoadPersonById(Util.UserPeopleId ?? 0);
            if (curruser == null)
            {
                return Content("no user");
            }

            if (User.IsInRole("Admin")
                || User.IsInRole("ManageEmails")
                || User.IsInRole("Finance")
                || m.queue.FromAddr == curruser.EmailAddress
                || m.queue.QueuedBy == curruser.PeopleId
                || m.queue.EmailQueueTos.Any(et => et.PeopleId == curruser.PeopleId))
            {
                return View(m);
            }

            return Content("not authorized");
        }

        public ActionResult GetEmailBody(int id)
        {
            var m = new EmailModel(id);
            if (m.queue == null)
            {
                return Content("no email found");
            }

            var curruser = CurrentDatabase.LoadPersonById(Util.UserPeopleId ?? 0);
            if (curruser == null)
            {
                return Content("no user");
            }

            if (User.IsInRole("Admin")
                || User.IsInRole("ManageEmails")
                || User.IsInRole("Finance")
                || m.queue.FromAddr == curruser.EmailAddress
                || m.queue.QueuedBy == curruser.PeopleId
                || m.queue.EmailQueueTos.Any(et => et.PeopleId == curruser.PeopleId))
            {
                return Content(m.queue.Body);
            }

            return Content("not authorized");
        }

        public ActionResult ConvertToSearch(int id)
        {
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            cc.AddNewClause(QueryType.EmailRecipient, CompareType.Equal, id);
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }

        public ActionResult Tracking(int id)
        {
            ViewBag.emailID = id;
            return View();
        }

        [Authorize(Roles = "Developer")]
        public ActionResult SendNow(int id)
        {
            string host = Util.Host;
            // save these from HttpContext to set again inside thread local storage
            var useremail = Util.UserEmail;
            var isinroleemailtest = User.IsInRole("EmailTest");

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    var db = DbUtil.Create(host);
                    var cul = CurrentDatabase.Setting("Culture", "en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
                    // set these again inside thread local storage
                    Util.UserEmail = useremail;
                    Util.IsInRoleEmailTest = isinroleemailtest;
                    db.SendPeopleEmail(id);
                }
                catch (Exception ex)
                {
                    var ex2 = new Exception("Emailing error for queueid " + id, ex);
                    ErrorLog errorLog = ErrorLog.GetDefault(null);
                    errorLog.Log(new Error(ex2));

                    var db = DbUtil.Create(host);
                    var equeue = db.EmailQueues.Single(ee => ee.Id == id);
                    equeue.Error = ex.Message.Truncate(200);
                    db.SubmitChanges();
                }
            });
            return Redirect("/Emails/Details/" + id);
        }

        public ActionResult DeleteQueued(int id)
        {
            var m = new EmailModel(id);
            if (m.queue == null)
            {
                return Redirect("/Emails");
            }

            if (m.queue.Sent.HasValue || !m.queue.SendWhen.HasValue || !m.CanDelete())
            {
                return Redirect("/");
            }

            DeleteEmail(id);
            return Redirect("/Emails");
        }

        private static void DeleteEmail(int id)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute(@"
                DELETE dbo.EmailLinks WHERE EmailID = @id;
                DELETE dbo.EmailResponses WHERE Id = @id;
                DELETE dbo.EmailQueueTo WHERE Id = @id;
                DELETE dbo.EmailQueue WHERE Id = @id;
                ", new { id });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var m = new EmailModel(id);
            if (!m.CanDelete())
            {
                return Redirect("/");
            }

            DeleteEmail(id);
            return Redirect("/Emails");
        }

        public ActionResult Resend(int id)
        {
            var email = (from e in CurrentDatabase.EmailQueues
                         where e.Id == id
                         select e).Single();
            var et = email.EmailQueueTos.First();
            var p = CurrentDatabase.LoadPersonById(et.PeopleId);

            if (email.FinanceOnly == true)
            {
                CurrentDatabase.EmailFinanceInformation(email.FromAddr, p, email.Subject, email.Body);
            }
            else
            {
                CurrentDatabase.Email(email.FromAddr, p, email.Subject, email.Body);
            }

            TempData["message"] = "Mail Resent";
            return RedirectToAction("Details", new { id });
        }

        public ActionResult MakePublic(int id)
        {
            var email = (from e in CurrentDatabase.EmailQueues
                         where e.Id == id
                         select e).Single();
            if (!User.IsInRole("Admin") && email.QueuedBy != Util.UserPeopleId)
            {
                return Redirect("/Emails/Details/" + id);
            }

            email.PublicX = true;
            CurrentDatabase.SubmitChanges();
            return Redirect("/EmailView/" + id);
        }
        public ActionResult MakePrivate(int id)
        {
            var email = (from e in CurrentDatabase.EmailQueues
                         where e.Id == id
                         select e).Single();
            if (!User.IsInRole("Admin") && email.QueuedBy != Util.UserPeopleId)
            {
                return Redirect("/Emails/Details/" + id);
            }

            email.PublicX = false;
            CurrentDatabase.SubmitChanges();
            return Redirect("/Emails/Details/" + id);
        }

        [HttpPost]
        public ActionResult Recipients(int id, FilterType filterType, int? page, int pageSize)
        {
            var m = new EmailModel(id, filterType, page, pageSize);
            return View(m);
        }

        public ActionResult List(EmailsModel m)
        {
            UpdateModel(m.Pager);
            return View(m);
        }

        public new ActionResult View(string id)
        {
            return Redirect("/EmailView/" + id);
        }

        [Route("~/Emails/Failed/{id?}")]
        public ActionResult Failed(int? id, string email)
        {
            var isadmin = User.IsInRole("Admin");
            var isdevel = User.IsInRole("Developer");
            var hasapikey = ConfigurationManager.AppSettings["SendGridApiKey"].HasValue();
            var q = from e in CurrentDatabase.EmailQueueToFails
                    where id == null || id == e.PeopleId
                    where email == null || email == e.Email
                    let et = CurrentDatabase.EmailQueueTos.SingleOrDefault(ef => ef.Id == e.Id && ef.PeopleId == e.PeopleId)
                    orderby e.Time descending
                    select new MailFail
                    {
                        time = e.Time,
                        eventx = e.EventX,
                        type = e.Bouncetype,
                        reason = e.Reason,
                        emailid = e.Id,
                        name = et != null ? et.Person.Name : "unknown",
                        subject = et != null ? et.EmailQueue.Subject : "unknown",
                        peopleid = e.PeopleId,
                        email = e.Email,
                        devel = isdevel,
                        admin = isadmin,
                        hasapikey = hasapikey,
                    };
            return View(q.Take(300));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Unblock(string email)
        {
            var apikey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new RestClient($"https://api.sendgrid.com/v3/suppression/bounces/{email}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("authorization", $"Bearer {apikey}");
            request.AddParameter("undefined", "null", ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                response.StatusDescription = "Email Unblocked!";
            }

            return Content(response.StatusDescription);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        public ActionResult Unspam(string email)
        {
            var apikey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new RestClient($"https://api.sendgrid.com/v3/suppression/spam_reports/{email}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("authorization", $"Bearer {apikey}");
            request.AddParameter("undefined", "null", ParameterType.RequestBody);
            var response = client.Execute(request);
            var okcodes = new[] { HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.NoContent, };
            if (okcodes.Contains(response.StatusCode))
            {
                CurrentDatabase.SpamReporterRemove(email);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                response.StatusDescription = "Email Unspammed!";
            }

            return Content(response.StatusDescription);
        }

        public class MailFail
        {
            public DateTime? time { get; set; }
            public string eventx { get; set; }
            public string type { get; set; }
            public string reason { get; set; }
            public string name { get; set; }
            public int? peopleid { get; set; }
            public int? emailid { get; set; }
            public string subject { get; set; }
            public string email { get; set; }
            public bool admin { get; set; }
            public bool devel { get; set; }
            public bool hasapikey { get; set; }
            public bool canunblock
            {
                get
                {
                    if (!admin || !email.HasValue())
                    {
                        return false;
                    }

                    if ((eventx != "bounce" || type != "blocked") && eventx != "dropped")
                    {
                        return false;
                    }

                    if (eventx == "dropped" && reason.Contains("spam", ignoreCase: true))
                    {
                        return false;
                    }

                    return hasapikey;
                }
            }
            public bool canunspam
            {
                get
                {
                    if (!devel || !email.HasValue())
                    {
                        return false;
                    }

                    if ((eventx != "bounce" || type != "blocked") && eventx != "dropped")
                    {
                        return false;
                    }

                    if (eventx == "dropped" && !reason.Contains("spam", ignoreCase: true))
                    {
                        return false;
                    }

                    return hasapikey;
                }
            }
        }
    }

    [RouteArea("Manage")]
    public class EmailViewController : CmsControllerNoHttps
    {
        public EmailViewController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/EmailView/{id}")]
        [Route("~/Emails/View/{id}")]
        public new ActionResult View(string id)
        {
            var iid = id.ToInt();
            var email = CurrentDatabase.EmailQueues.SingleOrDefault(ee => ee.Id == iid);
            if (email == null)
            {
                return Content("email document not found");
            }

            if ((email.PublicX ?? false) == false)
            {
                return Content("no email available");
            }

            var em = new EmailQueue
            {
                Subject = email.Subject,
                Body = Regex.Replace(email.Body, "({first}|{tracklinks}|{track})", "", RegexOptions.IgnoreCase)
            };
            return View(em);
        }
    }
}
