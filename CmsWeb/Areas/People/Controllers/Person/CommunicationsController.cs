using CmsData;
using CmsWeb.Areas.People.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CmsWeb.Areas.People.Models.Communications;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [HttpPost]
        public ActionResult Sms(SmsModel m)
        {
            return View("Communications/Sms", m);
        }
        [HttpPost]
        public ActionResult Optouts(OptOutsModel m)
        {
            return View("Communications/Optouts", m);
        }

        [HttpPost]
        public ActionResult DeleteOptout(int id, string from, bool istext)
        {
            var m = new OptOutsModel(CurrentDatabase, id);
            var p = CurrentDatabase.LoadPersonById(id);
            if (istext)
            {
                var regexObj = new Regex(@".*\((?<group>\d*)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var groupid = regexObj.Match(from).Groups["group"].Value.ToInt();
                var oo = CurrentDatabase.SmsGroupOptOuts.SingleOrDefault(o => o.FromGroup == groupid && o.ToPeopleId == id);
                if (oo == null)
                {
                    ViewBag.Error = $"Group not found ({from})";
                    return View("Communications/Optouts", m);
                }
                CurrentDatabase.SmsGroupOptOuts.DeleteOnSubmit(oo);
            }
            else
            {
                var oo = CurrentDatabase.EmailOptOuts.SingleOrDefault(o => o.FromEmail == from && o.ToPeopleId == id);
                if (oo == null)
                {
                    ViewBag.Error = $"Email not found ({from})";
                    return View("Communications/Optouts", m);
                }
                CurrentDatabase.EmailOptOuts.DeleteOnSubmit(oo);
            }
            CurrentDatabase.SubmitChanges();
            return View("Communications/Optouts", m);
        }

        [HttpPost]
        public ActionResult AddOptoutText(int id, int groupid)
        {
            var oo = CurrentDatabase.SmsGroupOptOuts.SingleOrDefault(o => o.FromGroup == groupid && o.ToPeopleId == id);
            if (oo == null)
            {
                CurrentDatabase.SmsGroupOptOuts.InsertOnSubmit(new SmsGroupOptOut { FromGroup = groupid, ToPeopleId = id, DateX = DateTime.Now });
                CurrentDatabase.SubmitChanges();
            }
            var m = new OptOutsModel(CurrentDatabase, id);
            return View("Communications/Optouts", m);
        }
        [HttpPost]
        public ActionResult AddOptoutEmail(int id, string email)
        {
            var oo = CurrentDatabase.EmailOptOuts.SingleOrDefault(o => o.FromEmail == email && o.ToPeopleId == id);
            if (oo == null)
            {
                CurrentDatabase.EmailOptOuts.InsertOnSubmit(new EmailOptOut
                { FromEmail = email, ToPeopleId = id, DateX = DateTime.Now });
                CurrentDatabase.SubmitChanges();
            }
            var m = new OptOutsModel(CurrentDatabase, id);
            return View("Communications/Optouts", m);
        }


        [HttpPost]
        public ActionResult FailedEmails(FailedMailModel m)
        {
            return View("Communications/Failed", m);
        }

        [HttpPost]
        public ActionResult ReceivedEmails(EmailReceivedModel m)
        {
            return View("Communications/Emails", m);
        }

        [HttpPost]
        public ActionResult SentEmails(EmailSentModel m)
        {
            return View("Communications/Emails", m);
        }

        [HttpPost]
        public ActionResult ScheduledEmails(EmailScheduledModel m)
        {
            return View("Communications/Emails", m);
        }

        [HttpPost]
        public ActionResult TransactionalEmails(EmailTransactionalModel m)
        {
            return View("Communications/Emails", m);
        }

        [HttpGet, Route("ViewEmail/{emailid:int}")]
        public ActionResult ViewEmail(int emailid)
        {
            var email = CurrentDatabase.EmailQueues.SingleOrDefault(ee => ee.Id == emailid);
            if (email == null)
            {
                return Content("no email found");
            }

            var curruser = CurrentDatabase.LoadPersonById(CurrentDatabase.UserPeopleId ?? 0);
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
                return View("Communications/View", em);
            }
            return Content("not authorized");
        }
    }
}
