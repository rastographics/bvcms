using System;
using UtilityExtensions;

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
        public int? FundId { get; set; }
        public int? TypeId { get; set; }
        public string Error { get; set; }

        public void AddError(string text)
        {
            if (!Error.HasValue())
                Error = "";
            else
                Error += "; ";
            Error += text;
            Valid = false;
        }

        public string RowError()
        {
            return $"Row {Row}: {Error}";
        }
    }
}
