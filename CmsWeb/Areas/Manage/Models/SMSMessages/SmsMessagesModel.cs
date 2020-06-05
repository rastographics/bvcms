using CmsData;

namespace CmsWeb.Areas.Manage.Models.SmsMessages
{
    public class SmsMessagesModel
    {
        public CMSDataContext CurrentDatabase { get; set; }

        public SmsMessagesModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        public SmsSentMessagesModel SentMessages()
        {
            return new SmsSentMessagesModel(CurrentDatabase);
        }
        public SmsReceivedMessagesModel ReceivedMessages()
        {
            return new SmsReceivedMessagesModel(CurrentDatabase);
        }
        public SmsReplyWordsModel ReplyWords()
        {
            return new SmsReplyWordsModel(CurrentDatabase);
        }
    }
}
