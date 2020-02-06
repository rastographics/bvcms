using eSpace.Services;

namespace eSpace
{
    public class eSpaceClient
    {
        private eSpaceEventService _eSpaceEvent;

        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }

        public eSpaceEventService Event => _eSpaceEvent ?? (_eSpaceEvent = new eSpaceEventService() { Client = this });
    }
}
