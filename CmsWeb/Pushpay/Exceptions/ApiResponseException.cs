using System;
using System.Net;
using System.Text;
using CmsWeb.Pushpay.ApiModels;

namespace CmsWeb.Pushpay.Exceptions
{
    public class ApiResponseException : Exception
    {
        public ApiResponseException(HttpStatusCode statusCode, string reasonPhrase, ErrorResponse response)
            : base(BuildMessage(statusCode, reasonPhrase, response))
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Response = response;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public ErrorResponse Response { get; set; }

        private static string BuildMessage(HttpStatusCode statusCode, string reasonPhrase, ErrorResponse response)
        {
            var message = new StringBuilder();

            message.AppendFormat("Non-successful response of status {0} {1}", statusCode, reasonPhrase);

            if (response != null)
            {
                if (response.Message != null)
                {
                    message.AppendFormat("\r\n\r\nError message: '{0}'", response.Message);
                }

                if (response.ResultCode != null)
                {
                    message.AppendFormat("\r\n\r\nResult Code: {0} ({1})", response.ResultCode.Key, response.ResultCode.Description);
                }

                if (response.ValidationFailures != null)
                {
                    message.Append("\r\n");
                    foreach (var validationFailure in response.ValidationFailures)
                    {
                        message.AppendFormat("\r\n    {0}: {1}", validationFailure.Key, string.Join(",", validationFailure.Value));
                    }
                }
            }

            return message.ToString();
        }
    }
}
