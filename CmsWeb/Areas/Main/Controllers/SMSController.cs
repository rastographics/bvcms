using System;
using System.Web.Mvc;
using CmsData.Classes.Twilio;

namespace CmsWeb.Areas.Main.Controllers
{
    public class SMSController : Controller
    {
        public ActionResult Options2(Guid id )
        {
            return View("Options", id);
        }
        public ActionResult Send2(Guid id, int iSendGroup, string sTitle, string sMessage)
        {
            TwilioHelper.QueueSMS(id, iSendGroup, sTitle, sMessage);
            ViewBag.sTitle = sTitle;
            ViewBag.sMessage = sMessage;
            return View("Send", id);
        }
    }
}
