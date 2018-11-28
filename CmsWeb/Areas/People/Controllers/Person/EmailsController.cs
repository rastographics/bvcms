using CmsData;
using CmsWeb.Areas.People.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult Optouts(int id)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            return View("Emails/Optouts", p);
        }

        [HttpPost]
        public ActionResult DeleteOptout(int id, string email)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            var oo = CurrentDatabase.EmailOptOuts.SingleOrDefault(o => o.FromEmail == email && o.ToPeopleId == id);
            if (oo == null)
            {
                ViewBag.Error = $"Email not found ({email})";
                return View("Emails/Optouts", p);
            }
            CurrentDatabase.EmailOptOuts.DeleteOnSubmit(oo);
            CurrentDatabase.SubmitChanges();
            return View("Emails/Optouts", p);
        }

        [HttpPost]
        public ActionResult AddOptout(int id, string emailaddress)
        {
            var oo = CurrentDatabase.EmailOptOuts.SingleOrDefault(o => o.FromEmail == emailaddress && o.ToPeopleId == id);
            if (oo == null)
            {
                CurrentDatabase.EmailOptOuts.InsertOnSubmit(new EmailOptOut { FromEmail = emailaddress, ToPeopleId = id, DateX = DateTime.Now });
                CurrentDatabase.SubmitChanges();
            }
            var p = CurrentDatabase.LoadPersonById(id);
            return View("Emails/Optouts", p);
        }

        [HttpPost]
        public ActionResult FailedEmails(FailedMailModel m)
        {
            return View("Emails/Failed", m);
        }

        [HttpPost]
        public ActionResult ReceivedEmails(EmailReceivedModel m)
        {
            return View("Emails/Emails", m);
        }

        [HttpPost]
        public ActionResult SentEmails(EmailSentModel m)
        {
            return View("Emails/Emails", m);
        }

        [HttpPost]
        public ActionResult ScheduledEmails(EmailScheduledModel m)
        {
            return View("Emails/Emails", m);
        }

        [HttpPost]
        public ActionResult TransactionalEmails(EmailTransactionalModel m)
        {
            return View("Emails/Emails", m);
        }

        [HttpGet, Route("ViewEmail/{emailid:int}")]
        public ActionResult ViewEmail(int emailid)
        {
            var email = CurrentDatabase.EmailQueues.SingleOrDefault(ee => ee.Id == emailid);
            if (email == null)
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
                || email.FromAddr == curruser.EmailAddress
                || email.QueuedBy == curruser.PeopleId
                || email.EmailQueueTos.Any(et => et.PeopleId == curruser.PeopleId))
            {
                var em = new EmailQueue
                {
                    Subject = email.Subject,
                    Body = Regex.Replace(email.Body, "({first}|{tracklinks}|{track})", "", RegexOptions.IgnoreCase)
                };
                return View("Emails/View", em);
            }
            return Content("not authorized");
        }
    }
}
