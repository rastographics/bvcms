using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Hosting;
using UtilityExtensions;

namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMHelper
    {
        private string _host;
        private readonly CMSDataContext _dataContext;

        public const int ACTION_REFRESH = 1;
        public const int ACTION_REFRESH_AND_NOTIFY = 2;
        public const int TYPE_TASK = 1;
        private const string GCM_URL = "https://fcm.googleapis.com/fcm/send";

        public GCMHelper(string host, CMSDataContext db)
        {
            if (!host.HasValue())
            {
                throw new ArgumentException("Host must be supplied.", nameof(host));
            }

            _host = host;
            _dataContext = db ?? throw new ArgumentException("Db must be supplied.", nameof(db));
        }

        private void send(GCMMessage message)
        {
            if (!_host.HasValue())
            {
                return;
            }

            string gcmkey = _dataContext.Setting("GCMKey", ConfigurationManager.AppSettings["GCMKey"]);

            if (message.registration_ids.Count == 0 || gcmkey.Length == 0)
            {
                return;
            }

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var threadDb = DbUtil.Create(_host);

                string json = JsonConvert.SerializeObject(message);

                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Authorization", "key=" + gcmkey);
                    webClient.Headers.Add("Content-Type", "application/json");
                    webClient.Encoding = Encoding.UTF8;

                    string results = webClient.UploadString(GCM_URL, json);

                    GCMResponse response = JsonConvert.DeserializeObject<GCMResponse>(results);

                    for (int iX = 0; iX < message.registration_ids.Count; iX++)
                    {
                        if (response.results.Count <= iX)
                        {
                            continue;
                        }

                        string registrationId = message.registration_ids[iX];
                        GCMResponseResult result = response.results[iX];

                        if (!string.IsNullOrEmpty(result.error))
                        {
                            switch (result.error)
                            {
                                case "InvalidRegistration":
                                case "NotRegistered":
                                    {
                                        var record = (from r in threadDb.MobileAppPushRegistrations
                                                      where r.RegistrationId == registrationId
                                                      select r).SingleOrDefault();

                                        if (record != null)
                                        {
                                            threadDb.MobileAppPushRegistrations.DeleteOnSubmit(record);
                                        }

                                        break;
                                    }
                            }
                        }
                        else if (result.error != null && result.registration_id.Length > 0)
                        {
                            var record = (from r in threadDb.MobileAppPushRegistrations
                                          where r.RegistrationId == registrationId
                                          select r).SingleOrDefault();

                            if (record != null)
                            {
                                record.RegistrationId = result.registration_id;
                            }
                        }
                    }

                    threadDb.SubmitChanges();
                }
            });
        }

        public void sendRefresh(int peopleID, int type)
        {
            GCMData data = new GCMData(type, ACTION_REFRESH, 0, "", "");
            GCMMessage msg = new GCMMessage(peopleID, null, data, null, _host, _dataContext);
            send(msg);
        }

        public void sendRefresh(List<int> peopleIDs, int type)
        {
            if (peopleIDs.Count == 0)
            {
                return;
            }

            GCMData data = new GCMData(type, ACTION_REFRESH, 0, "", "");
            GCMMessage msg = new GCMMessage(peopleIDs, null, data, null, _host, _dataContext);
            send(msg);
        }

        public void sendNotification(int peopleID, int type, int id, string title, string message)
        {
            GCMPayload notification = new GCMPayload(title, message);
            GCMData data = new GCMData(type, ACTION_REFRESH_AND_NOTIFY, id, title, message);
            GCMMessage msg = new GCMMessage(peopleID, null, data, notification, _host, _dataContext);
            send(msg);
        }

        public void sendNotification(List<int> peopleIDs, int type, int id, string title, string message)
        {
            if (peopleIDs.Count == 0)
            {
                return;
            }

            GCMPayload notification = new GCMPayload(title, message);
            GCMData data = new GCMData(type, ACTION_REFRESH_AND_NOTIFY, id, title, message);
            GCMMessage msg = new GCMMessage(peopleIDs, null, data, notification, _host, _dataContext);
            send(msg);
        }
    }
}
