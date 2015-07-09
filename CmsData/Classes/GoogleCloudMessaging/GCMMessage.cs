using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMMessage
    {
        public List<string> registration_ids = null;
        public GCMData data = null;

        // Android: Unused, iOS: Wake app when in background
        public bool content_available = true;

        // Both: Used to test, will not deliver to device
        public bool dry_run = false;

        public GCMMessage(List<string> registration_ids, GCMData data)
        {
            this.registration_ids = registration_ids;
            this.data = data;
        }
    }
}
