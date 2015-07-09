using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.GoogleCloudMessaging
{
    public class GCMResponse
    {
        public long multicast_id = 0;
        public int success = 0;
        public int failure = 0;
        public int canonical_ids = 0;

        public List<GCMResponseResult> results;
    }
}
