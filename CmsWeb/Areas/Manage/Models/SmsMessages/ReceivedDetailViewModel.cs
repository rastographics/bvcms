using CmsData;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class ReceivedDetailViewModel
    {
        public SmsReceived R { get; set; }
        public string PersonName { get; set; }
        public string GroupName { get; set; }
    }
}
