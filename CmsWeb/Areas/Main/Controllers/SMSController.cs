using System;
using System.Web.Mvc;
using CmsData.Classes.Twilio;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main")]
    public class SMSController : Controller
    {
        [Route("~/Sms/Options/{id:Guid}")]
        public ActionResult Options(Guid id )
        {
            return View("Options", id);
        }
        [Route("~/Sms/Send/{id:Guid}")]
        public ActionResult Send(Guid id, int iSendGroup, string sTitle, string sMessage)
        {
            TwilioHelper.QueueSMS(id, iSendGroup, sTitle, sMessage);
            ViewBag.sTitle = sTitle;
            ViewBag.sMessage = sMessage;
            return View("Send", id);
        }
    }
}
