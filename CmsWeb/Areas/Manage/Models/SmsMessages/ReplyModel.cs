using System;
using System.Linq;
using CmsData;
using CmsData.Classes.Twilio;
using Community.CsharpSqlite;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class ReplyModel
    {
        public int ReceivedId { get; set; }
        public string Message { get; set; }

        public string Send(CMSDataContext db)
        {
            var received = (from r in db.SmsReceiveds
                where r.Id == ReceivedId select r).Single();

            var tonumber = received.FromNumber;
            var person = received.Person;
            var fromnumber = (from n in db.SMSNumbers
                where n.GroupID == received.ToGroupId
                where n.Number == received.ToNumber
                select n).Single();

            var ret = TwilioHelper.SendSmsReplyIncoming(db,
                ReceivedId, person, tonumber, fromnumber,
                $"Reply To {person?.Name ?? "Unknown"}", Message);
            return ret;
        }
    }
}
