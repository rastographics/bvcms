using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Classes.Twilio;

namespace CmsWeb.Areas.Main.Controllers
{
    public class SMSController : Controller
    {
        public ActionResult Options(int id )
        {
            return View(id);
        }
        public ActionResult Send(int iQBID, int iSendGroup, string sTitle, string sMessage)
        {
            TwilioHelper.QueueSMS(iQBID, iSendGroup, sTitle, sMessage);
            ViewBag.sTitle = sTitle;
            ViewBag.sMessage = sMessage;
            return View(iQBID);
        }


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
