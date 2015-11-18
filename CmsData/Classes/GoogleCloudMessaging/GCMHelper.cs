using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UtilityExtensions;

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
            string gcmkey = DbUtil.Db.Setting("GCMKey", ConfigurationManager.AppSettings["GCMKey"]);

            if (message.registration_ids.Count == 0 || gcmkey.Length == 0) return;

            System.Threading.Tasks.Task.Factory.StartNew(() => {
                string json = JsonConvert.SerializeObject(message);
                string host = Util.Host;

                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Authorization", "key=" + gcmkey);
                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.Encoding = Encoding.UTF8;

                    string results = webClient.UploadString(GCM_URL, json);

                    GCMResponse response = JsonConvert.DeserializeObject<GCMResponse>(results);

                    var Db = DbUtil.Create(host);

                    for (int iX = 0; iX < message.registration_ids.Count; iX++)
                    {
                        if (response.results.Count > iX)
                        {
                            string registrationID = message.registration_ids[iX];
                            GCMResponseResult result = response.results[iX];

                            if (result.error != null && result.error.Length > 0)
                            {
                                switch (result.error)
                                {
                                    case "InvalidRegistration":
                                    case "NotRegistered":
                                    {
                                        var record = (from r in Db.MobileAppPushRegistrations
                                                      where r.RegistrationId == registrationID
                                                      select r).SingleOrDefault();

                                        if (record != null)
                                            Db.MobileAppPushRegistrations.DeleteOnSubmit(record);

                                        break;
                                    }
                                }
                            }
                            else if (result.error != null && result.registration_id.Length > 0)
                            {
                                var record = (from r in Db.MobileAppPushRegistrations
                                              where r.RegistrationId == registrationID
                                              select r).SingleOrDefault();

                                record.RegistrationId = result.registration_id;
                            }
                        }
                    }

                    Db.SubmitChanges();
                }
            });
        }

        public static void sendRefresh(int peopleID, int type)
        {
            GCMData data = new GCMData(type, ACTION_REFRESH, 0, "", "");
            GCMMessage msg = new GCMMessage(peopleID, null, data, null);
            send(msg);
        }

        public static void sendRefresh(List<int> peopleIDs, int type)
        {
            if (peopleIDs.Count == 0) return;

            GCMData data = new GCMData(type, ACTION_REFRESH, 0, "", "");
            GCMMessage msg = new GCMMessage(peopleIDs, null, data, null);
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
