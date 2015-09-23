namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMData
    {
        public int type = 0;
        public int action = 0;
        public int id = 0;

        public string title = "";
        public string message = "";

        public GCMData(int type, int action, int id, string title, string message)
        {
            this.type = type;
            this.action = action;
            this.id = id;
            this.title = title;
            this.message = message;
        }
    }
}
