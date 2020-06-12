using CmsData.Classes.Twilio;
using CmsWeb.Lifecycle;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Twilio.Rest.Api.V2010.Account;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class WebHookController : CMSBaseController
    {
        public WebHookController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [AllowAnonymous, HttpPost, Route("WebHook/Twilio/{Id}")]
        public ActionResult Twilio(int Id, string MessageSid, string SmsStatus)
        {
            if (MessageSid.HasValue())
            {
                var message = MessageResource.Fetch(MessageSid);
                var smsItem = CurrentDatabase.SMSItems.FirstOrDefault(m => m.Id == Id);

                if (smsItem != null && message != null)
                {
                    TwilioHelper.UpdateSMSItemStatus(CurrentDatabase, smsItem, new TwilioMessageResult(message));
                }
            }
            // No response needed
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}
