using System;
using System.Web.Mvc;
using CmsData.Classes.Twilio;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main")]
    public class SMSController : Controller
    {
        [Route("~/Sms/Options/{id:Guid}")]
        public ActionResult Options(Guid id)
        {
            if (TempData.ContainsKey("_Error"))
            {
                ViewBag.Error = TempData["_Error"];
                ViewBag.Title = TempData["_Error_Title"];
                ViewBag.Message = TempData["_Error_Message"];
            }

            return View("Options", id);
        }

        [Route("~/Sms/Send/{id:Guid}")]
        public ActionResult Send(Guid id, int iSendGroup, string sTitle, string sMessage)
        {
            if (sMessage.Length > 1600)
            {
                TempData.Add("_Error", $"The message length was {sMessage.Length} cannot be over 1600.");
                TempData.Add("_Error_Title", sTitle);
                TempData.Add("_Error_Message", sMessage);
                return RedirectToAction("Options", new {id});
            }

            TwilioHelper.QueueSms(id, iSendGroup, sTitle, sMessage);
            ViewBag.sTitle = sTitle;
            ViewBag.sMessage = sMessage;
            return View("Send", id);
        }
    }
}
