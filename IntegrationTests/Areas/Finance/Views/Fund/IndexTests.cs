using IntegrationTests.Support;
using Shouldly;
using System.Drawing;
using Xunit;
using SharedTestFixtures;
using OpenQA.Selenium.Support.UI;
using TransactionGateway.ApiModels;
using CmsData.View;
using CmsData;
using System;

namespace IntegrationTests.Areas.Finance.Views.Fund
{
    [Collection(Collections.Webapp)]
    public class IndexTests : AccountTestBase
    {
        [Fact]
        public void Should_Edit_ShowList_Fund()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();

            Open($"{rootUrl}Funds/");

            Find(css: "#sl1").Click();
            Wait(2);
            var dropdown = Find(tag: "select");
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText("Secondary");
            Find(css: ".editable-submit").Click();
            Wait(2);
            var showList = Find(css: "#sl1").GetAttribute("innerHTML");
            showList.ShouldBe("Secondary");

            Find(css: "#sl1").Click();
            Wait(2);
            var dropdown1 = Find(tag: "select");
            var selectElement1 = new SelectElement(dropdown1);
            selectElement1.SelectByText("Primary");
            Find(css: ".editable-submit").Click();
            Wait(2);
            var showList1 = Find(css: "#sl1").GetAttribute("innerHTML");
            showList1.ShouldBe("Primary");
        }

        [Fact]
        public void Should_Show_Closed_Funds()
        {
            var fund = new ContributionFund
            {
                FundId = RandomNumber(500, 1000),
                CreatedBy = 1,
                CreatedDate = DateTime.Now,
                FundName = RandomString(),
                FundStatusId = 2,
                FundTypeId = 1,
                FundPledgeFlag = false
            };
            db.ContributionFunds.InsertOnSubmit(fund);
            db.SubmitChanges();

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();

            Open($"{rootUrl}Funds/");

            Find(xpath: "//a[contains(@href, '/Funds?status=2')]").Click();
            PageSource.ShouldContain(fund.FundName);

            db.ContributionFunds.DeleteOnSubmit(fund);
            db.SubmitChanges();
        }
    }
}
