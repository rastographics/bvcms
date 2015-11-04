using System.Collections.Generic;
using System.Linq;

namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMMessage
    {
        public List<string> registration_ids = null;
        public GCMData data = null;
        public GCMPayload notification = null;

        // Android: Unused, iOS: Wake app when in background
        public bool content_available = true;

        // Both: Used to set how quickly the message is delivered
        public string priority = "high"; // Can use "high" for immediate delivery

        // Both: Used to test, will not deliver to device
        public bool dry_run = false;

        public GCMMessage(List<string> registrationIDs, GCMData data)
        {
            this.registration_ids = registrationIDs;
            this.data = data;
        }

        public GCMMessage(List<int> peopleIDs, string exclude, GCMData data, GCMPayload notification)
        {
            this.data = data;
            this.notification = notification;

            this.registration_ids = (from r in DbUtil.Db.MobileAppPushRegistrations
                                     where peopleIDs.Contains(r.PeopleId)
                                     select r.RegistrationId).ToList();

            if (exclude != null && exclude.Length > 0)
            {
                this.registration_ids.Remove(exclude);
            }
        }

        public GCMMessage(int peopleID, string exclude, GCMData data, GCMPayload notification)
        {
            this.data = data;
            this.notification = notification;

            this.registration_ids = (from r in DbUtil.Db.MobileAppPushRegistrations
                                     where r.PeopleId == peopleID
                                     select r.RegistrationId).ToList();

            if (exclude != null && exclude.Length > 0)
            {
                this.registration_ids.Remove(exclude);
            }
        }
    }
}
