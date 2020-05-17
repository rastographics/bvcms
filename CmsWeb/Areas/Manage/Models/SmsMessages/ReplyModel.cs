﻿using System.Linq;
using CmsData;
using CmsData.Classes.Twilio;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class ReplyModel
    {
        public int ReceivedId { get; set; }
        public string Message { get; set; }

        public string Send(CMSDataContext db)
        {
            var m = (from r in db.SmsReceiveds
                     where r.Id == ReceivedId
                     select new
                     {
                         person = r.Person,
                         tonumber = r.FromNumber,
                         fromnumber = (from n in db.SMSNumbers
                                   where n.GroupID == r.ToGroupId
                                   where n.Number == r.ToNumber
                                   select n).Single()
                     }).Single();
            var ret = TwilioHelper.SendSMS(db, m.person, m.tonumber, m.fromnumber, "Reply To Incoming Message", Message);
            db.ExecuteCommand("update dbo.SmsReceived set RepliedTo = 1 where Id = {0}", ReceivedId);
            return ret;
        }
    }
}