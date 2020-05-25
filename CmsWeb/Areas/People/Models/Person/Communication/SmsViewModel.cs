using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.People.Models
{
    public class SmsViewModel
    {
        public int Id { get; set; }
        public bool IsIncoming { get; set; }
        public int? ReplyToIncomingId { get; set; }
        public DateTime? Date { get; set; }
        public string Sender { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
    }
}
