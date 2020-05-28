using CmsData.Classes.Giving;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPageViewModel
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public int PageType { get; set; }
        public bool Enabled { get; set; }
        public FundsClass DefaultFund { get; set; }
        public FundsClass[] AvailableFunds { get; set; }
        public string DisabledRedirect { get; set; }
        public ContentFile SkinFile { get; set; }
        public string TopText { get; set; }
        public string ThankYouText { get; set; }
        public NotifyPerson[] OnlineNotifyPerson { get; set; }
        public ContentFile ConfirmEmailPledge { get; set; }
        public ContentFile ConfirmEmailOneTime { get; set; }
        public ContentFile ConfirmEmailRecurring { get; set; }
        public EntryPoint EntryPoint { get; set; }
        public int? CampusId { get; set; }
    }
}
