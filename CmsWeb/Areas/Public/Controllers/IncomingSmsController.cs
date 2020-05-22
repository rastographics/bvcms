using System.Diagnostics;
using CmsData;
using CmsWeb.Areas.Public.Models;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class IncomingSmsController : TwilioController
    {
        public TwiMLResult Index(SmsRequest incomingMessage)
        {
            var db = CMSDataContext.Create(HttpContext);
            var model = new IncomingSmsModel(db, incomingMessage);
            var returnMessage = model.ProcessAndRespond();
            var messagingResponse = new MessagingResponse();
            if(returnMessage.HasValue())
                messagingResponse.Message(returnMessage);
            return TwiML(messagingResponse);
        }
    }
}
