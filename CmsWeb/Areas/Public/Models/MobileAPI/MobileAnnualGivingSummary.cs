using CmsData.View;
using System;
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
        public string statement { get; set; }
        public string total { get; set; }
        public List<AnnualSummary> summary { get; set; }
        public AnnualGivingDetails details { get; set; }

        public MobileAnnualGivingSummary() { } //for deserializing in tests
        public MobileAnnualGivingSummary(int y)
        {
            title = $"{y}";
        }

        public void Load(int peopleId, List<NormalContribution> contributions, List<UnitPledgeSummary> pledges, List<GiftsInKind> giftsinkind, List<NonTaxContribution> nontaxitems)
        {
            statement = $"/Person2/ContributionStatement/{peopleId}/{title}-01-01/{title}-12-31";
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
        public List<FundSummary> funds { get; set; }
        public int showAsPledge { get; set; }

        public AnnualSummary() { } //for deserializing in tests
        public AnnualSummary(List<NormalContribution> contributions)
        {
            title = "Contributions";
            var empty = contributions.Count == 0;
            count = contributions.Count.ToString();
            comment = empty ? "No items found" : "";
            funds = FundTotals(contributions);
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
            funds = FundTotals(pledges);
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
            funds = FundTotals(contributions);
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
            funds = FundTotals(contributions);
            pledge = ((decimal)0).ToMoney();
            total = contributions.Sum(c => c.ContributionAmount ?? 0).ToMoney();
            showAsPledge = 0;
        }

        private List<FundSummary> FundTotals(List<NormalContribution> contributions)
        {
            var dict = new Dictionary<string, string>();
            foreach (var fund in contributions.Select(m => m.FundName).Distinct())
            {
                dict[fund] = contributions.Where(c => c.FundName == fund).Sum(c => c.ContributionAmount ?? 0).ToMoney();
            }
            return dict.ToList().Select(v => new FundSummary { name = v.Key, given = v.Value }).ToList();
        }

        private List<FundSummary> FundTotals(List<NonTaxContribution> contributions)
        {
            var dict = new Dictionary<string, string>();
            foreach (var fund in contributions.Select(m => m.FundName).Distinct())
            {
                dict[fund] = contributions.Where(c => c.FundName == fund).Sum(c => c.ContributionAmount ?? 0).ToMoney();
            }
            return dict.ToList().Select(v => new FundSummary { name = v.Key, given = v.Value }).ToList();
        }

        private List<FundSummary> FundTotals(List<GiftsInKind> contributions)
        {
            var dict = new Dictionary<string, string>();
            foreach (var fund in contributions.Select(m => m.FundName).Distinct())
            {
                dict[fund] = contributions.Where(c => c.FundName == fund).Sum(c => c.ContributionAmount ?? 0).ToMoney();
            }
            return dict.ToList().Select(v => new FundSummary { name = v.Key, given = v.Value }).ToList();
        }

        private List<FundSummary> FundTotals(List<UnitPledgeSummary> pledges)
        {
            var dict = new Dictionary<string, (string, string)>();
            foreach (var fund in pledges.Select(m => m.FundName).Distinct())
            {
                var g = pledges.Where(c => c.FundName == fund);
                dict[fund] = (
                    g.Sum(c => c.Given ?? 0).ToMoney(),
                    g.Sum(c => c.Pledged ?? 0).ToMoney()
                );
            }
            return dict.ToList().Select(v => new FundSummary { name = v.Key, given = v.Value.Item1, pledge = v.Value.Item2 }).ToList();
        }
    }

    public class FundSummary
    {
        public string name { get; set; }
        public string pledge { get; set; }
        public string given { get; set; }
    }
}
