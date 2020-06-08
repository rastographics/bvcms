using CmsData;
using System;
using System.Collections.Generic;

namespace CmsWeb.Areas.Giving.Models
{
    internal class ManageGivingConversionModel
    {
        public int? Day1 { get; set; }
        public int? Day2 { get; set; }
        public int? EveryN { get; set; }
        public DateTime? LastDate { get; set; }
        public DateTime? NextDate { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public Person Person { get; set; }
        public int PeopleId { get; set; }
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public string SemiEvery { get; set; }

        public IEnumerable<RecurringAmount> Amounts { get; set; }
    }
}
