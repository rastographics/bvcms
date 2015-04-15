using System;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class DepositRecord
    {
        public DateTime? Date { get; set; }
        public string Batch { get; set; }
        public int PeopleId { get; set; }
        public string Routing { get; set; }
        public string Account { get; set; }
        public string CheckNo { get; set; }
        public string Amount { get; set; }
        public string Type { get; set; }
        public int Row { get; set; }
        public bool Valid { get; set; }
        public string Description { get; set; }
    }
}