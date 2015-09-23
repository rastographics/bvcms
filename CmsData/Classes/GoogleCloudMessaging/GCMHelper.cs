using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMHelper
    {
        public static int TYPE_TASK = 1;

        public static int ACTION_REFRESH = 1;
        public static int ACTION_REFRESH_AND_NOTIFY = 2;

        private const string GCM_URL = "https://gcm-http.googleapis.com/gcm/send";

        private static void send(GCMMessage message)
        {
            if (message.registration_ids.Count == 0) return;

            String json = JsonConvert.SerializeObject(message);

            System.Threading.Tasks.Task.Factory.StartNew(() => {
                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Authorization", "key=" + System.Configuration.ConfigurationManager.AppSettings["GCMKey"]);
                    webClient.Headers.Add("Content-Type", "application/json");

                    webClient.Encoding = Encoding.UTF8;

                    string results = webClient.UploadString(GCM_URL, json);

                    GCMResponse response = JsonConvert.DeserializeObject<GCMResponse>(results);
                }
            });
        }

        public static void sendRefresh(int peopleID, int type)
        {
            GCMPayload notification = new GCMPayload( "Refresh", "Refresh" );
            GCMData data = new GCMData(type, ACTION_REFRESH, 0, "", "");
            GCMMessage msg = new GCMMessage(peopleID, null, data, notification);
            send(msg);
        }

        public static void sendRefresh(List<int> peopleIDs, int type)
        {
            if (peopleIDs.Count == 0) return;

            GCMPayload notification = new GCMPayload("Refresh", "Refresh");
            GCMData data = new GCMData(type, ACTION_REFRESH, 0, "", "");
            GCMMessage msg = new GCMMessage(peopleIDs, null, data, notification);
            send(msg);
        }

        public static void sendNotification(int peopleID, int type, int id, string title, string message)
        {
            GCMPayload notification = new GCMPayload(title, message);
            GCMData data = new GCMData(type, ACTION_REFRESH_AND_NOTIFY, id, title, message);
            GCMMessage msg = new GCMMessage(peopleID, null, data, notification);
            send(msg);
        }

        public static void sendNotification(List<int> peopleIDs, int type, int id, string title, string message)
        {
            if (peopleIDs.Count == 0) return;

            GCMPayload notification = new GCMPayload(title, message);
            GCMData data = new GCMData(type, ACTION_REFRESH_AND_NOTIFY, id, title, message);
            GCMMessage msg = new GCMMessage(peopleIDs, null, data, notification);
            send(msg);
        }
    }
}
