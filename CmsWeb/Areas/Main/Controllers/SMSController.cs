using System;
using System.Web.Mvc;
using CmsData.Classes.Twilio;
using CmsWeb.Lifecycle;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main")]
    public class SMSController : CMSBaseController
    {
        public SMSController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Sms/Options/{id:Guid}")]
        public ActionResult Options(Guid id)
        {
            return View("Options", id);
        }

        [ValidateInput(false)]
        [Route("~/Sms/Send/{id:Guid}")]
        public ActionResult Send(Guid id, int iSendGroup, string sTitle, string sMessage, DateTime? schedule)
        {
            ViewBag.Title = sTitle;
            ViewBag.Message = sMessage;
            if (sMessage.Length > 1600)
            {
                ViewBag.Error = $"The message length was {sMessage.Length} cannot be over 1600.";
                return View("Options", id);
            }
            if (schedule.HasValue)
            {
                ViewBag.Title = "SMS Messages Scheduled";
                ViewBag.PageHeader = "SMS Messages Scheduled";
                ViewBag.Message = $"Your SMS messages were scheduled to send at {schedule:t} on {schedule:M/d}.";
            }
            else
            {
                ViewBag.Title = "SMS Messages Sent";
                ViewBag.PageHeader = "SMS Messages Sent";
                ViewBag.Message = "Your SMS messages were sent.";
            }

            TwilioHelper.QueueSms(CurrentDatabase, id, iSendGroup, sTitle, sMessage, schedule);
            return View("Send", id);
        }
    }
}
