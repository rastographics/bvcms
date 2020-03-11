using System;
using System.Collections.Generic;
using CmsData.API;
using CmsData.View;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class StatementContext
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string header { get; set; }
        public DateTime now { get; set; }
        public string notice { get; set; }
        public string body { get; set; }
        public string footer { get; set; }
        public string envelopeNumber { get; set; }
        public ContributorInfo contributor { get; set; }
        public List<NormalContribution> contributions { get; set; }
        public List<GiftsInKind> giftsinkind { get; set; }
        public List<NonTaxContribution> nontaxitems { get; set; }
        public List<UnitPledgeSummary> pledges { get; set; }
        public ListOfNormalContributions taxSummary { get; set; }
        public ListOfNormalContributions nontaxSummary { get; set; }
        public decimal totalGiven { get; set; }
    }
}
