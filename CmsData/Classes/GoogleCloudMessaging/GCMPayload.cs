namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMPayload
    {
        public string title;
        public string body;

        public string sound = "default";
        public string icon = "app_icon_notification";

        public int badge = 0;

        public GCMPayload(string title, string body)
        {
            this.title = title;
            this.body = body;
        }
    }
}
