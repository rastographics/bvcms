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
    }
}
