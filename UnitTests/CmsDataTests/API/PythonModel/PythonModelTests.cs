using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmsData;
using CmsData.Codes;
using CmsDataTests.Properties;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace CmsDataTests
{
    [TestClass()]
    [Collection(Collections.Database)]
    public class PythonModelTests : IDisposable
    {
        public PythonModelTests()
        {
            MockAppSettings.Apply(
                ("PublicKey", "testin"),
                ("PublicSalt", "85 85 85 85 85 85 85 85")
            );
        }

        [Theory]
        [InlineData("2019-07-25", "7/22/2019 1:29:32 PM", 1, "12.00", "3214", "Slush Fund", 1)]
        [InlineData("2019-07-25", "7/22/2019 13:30:00", 1, "50", "1080", "Slush Fund", 1)]
        public void AddContributionTest(string date, string cDate, int fundid, string amount, string checkno, string description, int peopleid)
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            var model = new PythonModel(db);
            var dateValue = DateTime.Parse(date);
            var bundleHeader = model.GetBundleHeader(dateValue, DateTime.Now);
            var contributionDate = model.ParseDate(cDate).Value;
            var detail = model.AddContribution(contributionDate, fundid, amount, checkno, description, peopleid);
            bundleHeader.BundleDetails.Add(detail);
            model.FinishBundle(bundleHeader);

            detail.ContributionId.ShouldNotBe(0);
            detail.BundleHeader.ContributionDate.ShouldBe(dateValue);
            detail.Contribution.ContributionDate.ShouldBe(contributionDate);
            detail.Contribution.FundId.ShouldBe(fundid);
            detail.Contribution.ContributionAmount.ShouldBe(decimal.Parse(amount));
            detail.Contribution.CheckNo.ShouldBe(checkno);
            detail.Contribution.ContributionDesc.ShouldBe(description);
            detail.Contribution.PeopleId.ShouldBe(peopleid);
            MockContributions.DeleteAllFromBundle(db, detail.BundleHeader);
        }

        [Theory]
        [InlineData("2019-10-17", "10/17/2019 3:30:00", 1, "540.00", "", "1234567890", "852258", ContributionTypeCode.Stock)]
        [InlineData("2019-10-18", "10/17/2019 18:00:00", 1, "120.00", "", "1234567890", "789654", ContributionTypeCode.Online)]
        [InlineData("2019-10-19", "10/17/2019 23:01:00", 1, "-120.00", "1030", "1234567890", "741963", ContributionTypeCode.ReturnedCheck)]
        [InlineData("2019-10-19", "10/17/2019 23:11:00", 1, "20.00", "1596", "1234567890", "123789", ContributionTypeCode.NonTaxDed)]
        [InlineData("2019-10-19", "10/17/2019 23:11:00", 1, "60.00", "", "1234567890", "951753", ContributionTypeCode.CheckCash)]
        public void AddContributionDetailTest(string date, string cDate, int fundid, string amount, string checkno, string routing, string account, int cType)
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            var model = new PythonModel(db);
            var dateValue = DateTime.Parse(date);
            var bundleHeader = model.GetBundleHeader(dateValue, DateTime.Now);
            var contributionDate = model.ParseDate(cDate).Value;
            var detail = model.AddContributionDetail(contributionDate, fundid, amount, checkno, routing, account, cType);
            bundleHeader.BundleDetails.Add(detail);
            model.FinishBundle(bundleHeader);

            detail.ContributionId.ShouldNotBe(0);
            detail.BundleHeader.ContributionDate.ShouldBe(dateValue);
            detail.Contribution.ContributionDate.ShouldBe(contributionDate);
            detail.Contribution.ContributionTypeId.ShouldBe(cType);
            detail.Contribution.FundId.ShouldBe(fundid);
            detail.Contribution.ContributionAmount.ShouldBe(decimal.Parse(amount));
            detail.Contribution.CheckNo.ShouldBe(checkno);
            detail.Contribution.BankAccount.ShouldNotBeNull();
            MockContributions.DeleteAllFromBundle(db, detail.BundleHeader);
        }

        [Fact]
        public void DocusignApiTest()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            var model = new PythonModel(db);
            var result = model.RunScript(Resources.DocusignApiTest);

            result.TrimEnd().ShouldBe("[False, False, False, False, False, False, False, False, False, False, False, False, False, False, False]");
        }

        [Fact]
        public void RenderTemplateTest()
        {
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            var model = new PythonModel(db);
            model.Data.header = new { DateFrom = "10/17/2019", DateTo = "11/27/2019" };
            model.Data.results = new dynamic[] {
                new { trans_date = new DateTime(2019, 11, 13),
                    Payment_Type = "CC",
                    OrganizationName = "Test Template Org",
                    account_code = "1234-567890",
                    Amount = 99.95,
                    total_amt = 102938.67
                },
                new { trans_date = new DateTime(2019, 11, 14),
                    Payment_Type = "ACH",
                    OrganizationName = "Some Other Org",
                    account_code = "1234-XXXXXX",
                    Amount = 109,
                    total_amt = 360
                }
            }; 
            var result = model.RenderTemplate(Resources.RenderTemplateTest);
            result.ShouldBe(Resources.RenderTemplateResults);
        }

        public void Dispose()
        {
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
