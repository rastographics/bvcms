using CmsData;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Twilio.Rest.Api.V2010.Account;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class WebHookController : Controller
    {
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

            var db = DbUtil.Db;

            var smsItem = db.SMSItems.FirstOrDefault(m => m.Id == Id);

            if (smsItem != null)
            {
                if (errorMessage.IsNotNull() || errorCode.IsNotNull())
                {
                    smsItem.ErrorMessage = $"({errorCode}) {errorMessage}";
                }
                smsItem.ResultStatus = SmsStatus;
                db.SubmitChanges();
            }

            // No response needed
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}
