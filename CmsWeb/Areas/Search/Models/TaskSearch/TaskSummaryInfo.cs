namespace CmsWeb.Areas.Search.Models
{
    public class TaskSummaryInfo
    {
        public int Count { get; set; }
        public string ContactType { get; set; }
        public string ReasonType { get; set; }
        public string Ministry { get; set; }
        public string HasComments { get; set; }
        public string HasDate { get; set; }
        public string HasContactor { get; set; }
    }
}