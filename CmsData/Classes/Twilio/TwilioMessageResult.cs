using Twilio.Rest.Api.V2010.Account;

namespace CmsData.Classes.Twilio
{
    public class TwilioMessageResult
    {
        public string Status { get; set; }
        public int? ErrorCode {get; set; }
        public string ErrorMessage {get; set; }

        public TwilioMessageResult() { }
        public TwilioMessageResult(MessageResource messageResource)
        {
            Status = messageResource.Status.ToString();
            ErrorCode = messageResource.ErrorCode;
            ErrorMessage = messageResource.ErrorMessage;
        }
    }
}
