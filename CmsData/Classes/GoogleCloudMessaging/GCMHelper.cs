using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMHelper
    {
        private const String GCM_URL = "https://gcm-http.googleapis.com/gcm/send";

        public static void send(GCMMessage message)
        {
            String json = JsonConvert.SerializeObject(message);

            using (var webClient = new WebClient())
            {
                //webClient.Headers.Add("Authorization", "key=" + System.Configuration.ConfigurationManager.AppSettings["GCMKey"]);
                webClient.Headers.Add("Authorization", "key=AIzaSyB8lydWFpJgyZriIsIzukaDjQV4iwM6GZw");
                webClient.Headers.Add("Content-Type", "application/json");

                webClient.Encoding = System.Text.Encoding.UTF8;

                string results = webClient.UploadString(GCM_URL, json);

                GCMResponse response = JsonConvert.DeserializeObject<GCMResponse>(results);
            }
        }
    }
}
