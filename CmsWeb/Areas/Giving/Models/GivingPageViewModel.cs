using CmsData.Classes.Giving;

namespace CmsWeb.Areas.Giving.Models
{
    public class GivingPageViewModel
    {
        public int pageId { get; set; }
        public string pageName { get; set; }
        public string pageUrl { get; set; }
        public int pageType { get; set; }
        public bool enabled { get; set; }
        public FundsClass defaultFund { get; set; }
        public FundsClass[] availFundsArray { get; set; }
        public string disRedirect { get; set; }
        public ContentFile skinFile { get; set; }
        public string topText { get; set; }
        public string thankYouText { get; set; }
        public NotifyPerson[] onlineNotifyPerson { get; set; }
        public ContentFile confirmEmailPledge { get; set; }
        public ContentFile confirmEmailOneTime { get; set; }
        public ContentFile confirmEmailRecurring { get; set; }
        public int? campusId { get; set; }
        public int? entryPointId { get; set; }
        public int? currentIndex { get; set; }
    }
}
