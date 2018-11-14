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
            int? errorCode = null;
            string errorMessage = null;

            if (MessageSid.HasValue())
            {
                if (MessageResource.StatusEnum.Failed.Equals(SmsStatus) ||
                    MessageResource.StatusEnum.Undelivered.Equals(SmsStatus))
                {
                    var message = MessageResource.Fetch(MessageSid);
                    errorCode = message.ErrorCode;
                    errorMessage = message.ErrorMessage;
                }
            }

            //var db = Db;

            var smsItem = CurrentDatabase.SMSItems.FirstOrDefault(m => m.Id == Id);

            if (smsItem != null)
            {
                if (errorMessage.IsNotNull() || errorCode.IsNotNull())
                {
                    smsItem.ErrorMessage = $"({errorCode}) {errorMessage}".MaxString(150);
                }
                smsItem.ResultStatus = SmsStatus;
                CurrentDatabase.SubmitChanges();
            }

            // No response needed
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}
