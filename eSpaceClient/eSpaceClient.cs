using eSpace.Services;
using System.Configuration;

namespace eSpace
{
    public class eSpaceClient
    {
        private eSpaceEventService _eSpaceEvent;

        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; } = ConfigurationManager.AppSettings["eSpaceEventService.BaseUrl"];

        public eSpaceEventService Event => _eSpaceEvent ?? (_eSpaceEvent = new eSpaceEventService() { Client = this });
    }
}
