using System;

namespace CmsWeb.Areas.People.Models
{
    public class EmailRow
    {
        public int Id { get; set; }
        public DateTime? Sent { get; set; }
        public DateTime? SendWhen { get; set; }
        public DateTime? Queued { get; set; }
        public string From { get; set; }
        public string FromAddr { get; set; }
        public int Count { get; set; }
        public string Subject { get; set; }
    }
}