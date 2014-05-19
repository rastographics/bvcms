using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DDay.iCal;
using UtilityExtensions;
using System.IO;

namespace CmsData.Classes.MinistrESpace
{
    public class MinistrESpaceHelper
    {
        // Debug URLs
        
#if DEBUG
        public const string URL_EVENTS = "http://ezdtest01.easydraft.com/ministrespace/{0}/api/Events?start={1}&end={2}";
        public const string URL_SPACES = "http://ezdtest01.easydraft.com/ministrespace/{0}/api/Spaces";
        public const string URL_RESOURCES = "http://ezdtest01.easydraft.com/ministrespace/{0}/api/Resources";
        public const string URL_SERVICES = "http://ezdtest01.easydraft.com/ministrespace/{0}/api/Sevices";

        public const string URL_EVENT_PAGE = "http://ezdtest01.easydraft.com/ministrespace/{0}/Login/Index.rails?EventId={1}";
#else
        public const string URL_EVENTS = "https://my.ministrespace.com/{0}/api/Events?start={1}&end={2}";
        public const string URL_SPACES = "https://my.ministrespace.com/{0}/api/Spaces";
        public const string URL_RESOURCES = "https://my.ministrespace.com/{0}/api/Resources";
        public const string URL_SERVICES = "https://my.ministrespace.com/{0}/api/Sevices";
        
        public const string URL_EVENT_PAGE = "https://my.ministrespace.com/{0}/Login/Index.rails?EventId={1}";
#endif

        private string dateStart = "";
        private string dateEnd = "";
        private string church = "";
        private string key = "";

        public MinistrESpaceHelper()
        {
            church = DbUtil.Db.Setting("ministrEspaceID", "");
            key = DbUtil.Db.Setting("ministrEspaceKey", "");

            setDateRange(DateTime.Now.AddMonths(-2), DateTime.Now.AddDays(90));
        }

        public void setDateRange(DateTime dtStart, DateTime dtEnd)
        {
            dateStart = dtStart.ToString("yyyy-MM-ddThh:mm:ss");
            dateEnd = dtEnd.ToString("yyyy-MM-ddThh:mm:ss");
        }

        public string getRawEvents()
        {
            string url = URL_EVENTS.Fmt(church, dateStart, dateEnd);

            var response = createAuthClient().DownloadString(url);

            return response;
        }

        public IUniqueComponentList<IEvent> getEvents()
        {
            var entries = iCalendar.LoadFromStream(new StringReader(getRawEvents()));

            if (entries != null && entries.Count > 0)
                return entries[0].Events;
            else
                return null;

        }

        public Dictionary<string, string> getUniqueEvents()
        {
            var list = new Dictionary<string, string>();

            var events = getEvents();

            if (events == null)
            {
                list.Add("0", "-- No Events Found --");
                return list;
            }

            foreach (var item in events)
            {
                list[getEventID(item.UID)] = item.Summary;
            }

            return list;
        }

        public List<Event> getUniqueEventList()
        {
            var lEvents = new List<Event>();

            foreach (var item in getUniqueEvents())
            {
                var e = new Event { ID = item.Key, Name = item.Value };
                lEvents.Add(e);
            }

            return lEvents;
        }

        public string getEventURL(string sEventID)
        {
            return URL_EVENT_PAGE.Fmt(church, sEventID);
        }

        private WebClient createAuthClient()
        {
            var wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(key + ":"));
            wc.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            return wc;
        }

        public static string getEventID(string sUID)
        {
            string[] sBase = sUID.Split('@');
            string[] sIDs = sBase[0].Split('-');
            return sIDs[0];
        }
    }

    public class Event
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IDName { get { return ID + "|" + Name; } }
    }
}