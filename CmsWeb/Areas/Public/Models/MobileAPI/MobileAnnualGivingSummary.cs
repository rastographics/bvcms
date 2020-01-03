using CmsData.View;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
    internal static class Extensions
    {
        public static string ToMoney(this decimal value)
        {
            return "$" + value.ToString("N2");
        }
    }

    public class MobileAnnualGivingSummary
    {
        public string comment { get; set; }
        public string count { get; set; }
        public int loaded { get; set; }
        public string title { get; set; }
        public string total { get; set; }
        public List<AnnualSummary> summary { get; set; }
        public AnnualGivingDetails details { get; set; }

        public MobileAnnualGivingSummary() { } //for deserializing in tests
        public MobileAnnualGivingSummary(int y)
        {
            title = $"{y}";
        }

        public void Load(List<NormalContribution> contributions, List<UnitPledgeSummary> pledges, List<GiftsInKind> giftsinkind, List<NonTaxContribution> nontaxitems)
        {
            var empty = contributions.Count == 0;
            loaded = 1;
            count = contributions.Count.ToString();
            comment = empty ? "No items found" : "";
            total = contributions.Sum(c => c.ContributionAmount ?? 0).ToMoney();
            summary = new List<AnnualSummary>(new[] {
                new AnnualSummary(contributions),
                new AnnualSummary(nontaxitems),
                new AnnualSummary(pledges),
                new AnnualSummary(giftsinkind)
            });
            details = new AnnualGivingDetails(new GivingDetails(contributions), new GivingDetails(pledges), new GivingDetails(giftsinkind), new GivingDetails(nontaxitems));
        }
    }

    public class AnnualGivingDetails
    {
        public AnnualGivingDetails() { } //for deserializing in tests
        public AnnualGivingDetails(params GivingDetails[] givingDetails)
        {
        }
    }

    public class GivingDetails : List<GivingDetail>
    {
        public GivingDetails() { } //for deserializing in tests
        public GivingDetails(List<UnitPledgeSummary> pledges)
        {
            foreach(var pledge in pledges)
            {
                Add(new GivingDetail {
                    month = $"{pledge.PledgeDate?.Month}",
                    day = $"{pledge.PledgeDate?.Day}",
                    amount = pledge.Pledged.Value.ToMoney(),
                    fund = pledge.FundName,
                    origin = "",
                    giver = ""
                });
            }
        }

        public GivingDetails(List<NormalContribution> contributions)
        {
            foreach (var contribution in contributions)
            {
                Add(new GivingDetail
                {
                    month = $"{contribution.ContributionDate?.Month}",
                    day = $"{contribution.ContributionDate?.Day}",
                    amount = contribution.ContributionAmount?.ToMoney(),
                    fund = contribution.FundName,
                    origin = Util.PickFirst(contribution.CheckNo, contribution.Description),
                    giver = contribution.Name
                });
            }
        }

        public GivingDetails(List<GiftsInKind> giftsinkind)
        {
            foreach (var contribution in giftsinkind)
            {
                Add(new GivingDetail
                {
                    month = $"{contribution.ContributionDate?.Month}",
                    day = $"{contribution.ContributionDate?.Day}",
                    amount = "",
                    fund = contribution.FundName,
                    origin = contribution.Description,
                    giver = contribution.Name
                });
            }
        }

        public GivingDetails(List<NonTaxContribution> nontaxitems)
        {
            foreach (var contribution in nontaxitems)
            {
                Add(new GivingDetail
                {
                    month = $"{contribution.ContributionDate?.Month}",
                    day = $"{contribution.ContributionDate?.Day}",
                    amount = contribution.ContributionAmount?.ToMoney(),
                    fund = contribution.FundName,
                    origin = Util.PickFirst(contribution.CheckNo, contribution.Description),
                    giver = contribution.Name
                });
            }
        }
    }

    public class GivingDetail
    {
        public string month { get; set; }
        public string day { get; set; }
        public string fund { get; set; }
        public string origin { get; set; }
        public string giver { get; set; }
        public string amount { get; set; }
    }

    public class AnnualSummary
    {
        public string title { get; set; }
        public string comment { get; set; }
        public string count { get; set; }
        public string pledge { get; set; }
        public string total { get; set; }
        public int showAsPledge { get; set; }

        public AnnualSummary() { } //for deserializing in tests
        public AnnualSummary(List<NormalContribution> contributions)
        {
            title = "Contributions";
            var empty = contributions.Count == 0;
            count = contributions.Count.ToString();
            comment = empty ? "No items found" : "";
            pledge = ((decimal)0).ToMoney();
            total = contributions.Sum(c => c.ContributionAmount ?? 0).ToMoney();
            showAsPledge = 0;
        }

        public AnnualSummary(List<UnitPledgeSummary> pledges)
        {
            title = "Pledges";
            var empty = pledges.Count == 0;
            count = pledges.Count.ToString();
            comment = empty ? "No items found" : "";
            pledge = pledges.Sum(c => c.Pledged ?? 0).ToMoney();
            total = pledges.Sum(c => c.Given ?? 0).ToMoney();
            showAsPledge = 1;
        }

        public AnnualSummary(List<NonTaxContribution> contributions)
        {
            title = "Non-Tax Deductible";
            var empty = contributions.Count == 0;
            count = contributions.Count.ToString();
            comment = empty ? "No items found" : "";
            pledge = ((decimal)0).ToMoney();
            total = contributions.Sum(c => c.ContributionAmount ?? 0).ToMoney();
            showAsPledge = 0;
        }

        public AnnualSummary(List<GiftsInKind> contributions)
        {
            title = "Gifts In Kind";
            var empty = contributions.Count == 0;
            count = contributions.Count.ToString();
            comment = empty ? "No items found" : "";
            pledge = ((decimal)0).ToMoney();
            total = contributions.Sum(c => c.ContributionAmount ?? 0).ToMoney();
            showAsPledge = 0;
        }
    }
}
