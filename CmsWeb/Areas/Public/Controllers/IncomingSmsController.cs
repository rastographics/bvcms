using System.Web.Mvc;
using CmsWeb.Common.Extensions;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace CmsWeb.Areas.Public.Controllers
{
    public class IncomingSmsController : TwilioController
    {
        public TwiMLResult Index(SmsRequest incomingMessage)
        {
            var from = incomingMessage.From;
            var to = incomingMessage.To;
            var body = incomingMessage.Body;
            var messagingResponse = new MessagingResponse();
            switch (body.ToUpper())
            {
                case "JOIN":
                    break;
            }
            return TwiML(messagingResponse);
        }
    }
}
