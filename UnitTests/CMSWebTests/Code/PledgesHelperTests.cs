using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace CMSWebTests
{
    [Collection(Collections.Database)]
    public class PledgesHelperTests
    {
        [Theory]
        [MemberData(nameof(Data_GetFundIdListFromStringTest))]
        public void GetFundIdListFromStringTest(string stringFunds, List<int> expected)
        {
            var actual = PledgesHelper.GetFundIdListFromString(stringFunds);
            actual.ShouldBe(expected);
        }

        [Theory]
        [MemberData(nameof(Data_PledgesSummaryByFundListTest))]
        public void PledgesSummaryByFundListTest(List<PledgesSummary> pledgesSummary, List<int> fundIdList, List<PledgesSummary> expected)
        {
            var actual = PledgesHelper.PledgesSummaryByFundList(pledgesSummary, fundIdList);
            actual.ShouldBeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(200,0)]
        [InlineData(200,100)]
        public void Should_GetFilteredPledgesSummary(decimal pledged, decimal contributed)
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var fromDate = new DateTime(2019, 1, 1);
                var fund = MockFunds.CreateSaveFund(db, true);                
                var bundleHeader = MockContributions.CreateSaveBundle(db);
                var pledge = MockContributions.CreateSaveContribution(
                    db, bundleHeader, fromDate, pledged, peopleId: 1, fundId: fund.FundId, contributionType: ContributionTypeCode.Pledge);
                MockSettings.CreateSaveSetting(db, "PostContributionPledgeFunds", fund.FundId.ToString());
                var firstContribution = MockContributions.CreateSaveContribution(db, bundleHeader, fromDate, contributed, peopleId: 1, fundId: fund.FundId, contributionType: ContributionTypeCode.CheckCash);
                var secondContribution = MockContributions.CreateSaveContribution(db, bundleHeader, fromDate, 100, peopleId: 1, fundId: 2, contributionType: ContributionTypeCode.CheckCash);

                var expected = MockContributions.CustomFilteredPledgesSummary(fund.FundId, fund.FundName, contributed, pledged);
                var actual = PledgesHelper.GetFilteredPledgesSummary(db, 1);
                actual.ShouldBeEquivalentTo(expected);

                MockSettings.CreateSaveSetting(db, "PostContributionPledgeFunds", "1");
                MockContributions.DeleteAllFromBundle(db, bundleHeader);
                MockFunds.DeleteFund(db, fund.FundId);
            }         
        }

        public static IEnumerable<object[]> Data_GetFundIdListFromStringTest =>
            new List<object[]>
            {
                new object[] { "1,2,3,4,5", new List<int>() { 1, 2, 3, 4, 5 } },
                new object[] { "1", new List<int>() { 1 } },
                new object[] { "10", new List<int>() { 10 } },
                new object[] { "", new List<int>()},
                new object[] { "true", new List<int>()},  
                new object[] { "true,false", new List<int>()}
            };

        public static IEnumerable<object[]> Data_PledgesSummaryByFundListTest =>
            new List<object[]>
            {
                new object[] { new List<PledgesSummary>(), new List<int>() { 1, 2 }, new List<PledgesSummary>()},
                new object[] { MockContributions.CreatePledgesSummary(), new List<int>(), new List<PledgesSummary>()},
                new object[] { MockContributions.CreatePledgesSummary(), new List<int>() { 1 }, MockContributions.FilteredPledgesSummary()},
                new object[] { MockContributions.CreatePledgesSummary(), new List<int>() { 1, 2 }, MockContributions.CreatePledgesSummary() }
            };                       
    }
}
