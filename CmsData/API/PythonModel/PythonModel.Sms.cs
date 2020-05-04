using System;
using System.Configuration;
using CmsData.Classes.Twilio;
using RestSharp;

namespace CmsData
{
    public partial class PythonModel
    {
        /// <summary>
        /// Queue an SMS text message to be sent
        /// </summary>
        /// <param name="query">The people ID to send to, or the query that returns the people IDs to send to</param>
        /// <param name="iSendGroup">The ID of the SMS sending group, from SMSGroups table</param>
        /// <param name="sTitle">Kind of a subject.  Stored in the database, but not part of the actual text message.  Must not be over 150 characters.</param>
        /// <param name="sMessage">The text message content.  Must not be over 160 characters.</param>
        public void SendSms(object query, int iSendGroup, string sTitle, string sMessage)
        {
            if (sTitle.Length > 150)
            {
                throw new Exception($"The title length was {sTitle.Length} but cannot be over 150.");
            }
            if (sMessage.Length > 1600)
            {
                throw new Exception($"The message length was {sMessage.Length} but cannot be over 1600.");
            }
            TwilioHelper.QueueSms(db, query, iSendGroup, sTitle, sMessage);
        }
        public static string CreateTinyUrl(string url)
        {
            var serviceurl = ConfigurationManager.AppSettings["UrlShortenerService"];
            var token = ConfigurationManager.AppSettings["UrlShortenerServiceToken"];
            var shorturl = url; // default return value if no service is configured
            if (!string.IsNullOrEmpty(serviceurl) && !string.IsNullOrEmpty(token))
            {
                var client = new RestClient(serviceurl);
                var request = new RestRequest(Method.POST);
                request.AddParameter("token", token);
                request.AddParameter("url", url);
                shorturl = client.Execute(request).Content;
                // if the request fails, return the original url
                if (string.IsNullOrEmpty(shorturl))
                    return url;
            }
            return shorturl;
        }
    }
}
