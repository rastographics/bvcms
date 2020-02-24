using eSpace.Models;
using RestSharp;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace eSpace.Services
{
    public class eSpaceEventService : eSpaceServiceBase
    {
        public IEnumerable<eSpaceEvent> List(NameValueCollection filters)
        {
            var list = new List<eSpaceEvent>();

            var request = new RestRequest("event/list", Method.GET, DataFormat.Json);

            ExecuteGet(request, filters, out list);

            return list;
        }

        public IEnumerable<eSpaceOccurrence> Occurrences(long eventId, NameValueCollection filters)
        {
            var list = new List<eSpaceOccurrence>();

            filters.Add("eventId", eventId.ToString());

            var request = new RestRequest("event/occurrences", Method.GET, DataFormat.Json);

            ExecuteGet(request, filters, out list);

            return list;
        }
    }
}
