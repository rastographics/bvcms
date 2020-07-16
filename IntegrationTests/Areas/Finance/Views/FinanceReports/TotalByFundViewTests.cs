using IntegrationTests.Support;
using Shouldly;
using System.Drawing;
using Xunit;
using SharedTestFixtures;
using System.Linq;
using OpenQA.Selenium;
using System;

namespace IntegrationTests.Areas.Finance.Views.FinanceReports
{
    [Collection(Collections.Webapp)]
    public class TotalByFundViewTests : AccountTestBase
    {
        [Fact]
        public void Should_Hide_Donor_Details_For_No_Finance_Rol()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "FinanceViewOnly" });
            Login();
            Wait(3);
            Open($"{rootUrl}FinanceReports/TotalsByFund");
            Find(xpath: "//button[contains(.,' Other Reports ')]").Click();
            var donorDetail = Find(id: "exportdonordetails");
            donorDetail.ShouldBeNull();
            var donorFundTotals = Find(id: "exportdonorfundtotals");
            donorFundTotals.ShouldBeNull();
            var donorTotals = Find(id: "exportdonortotals");
            donorTotals.ShouldBeNull();
        }

        [Fact]
        public void Should_Show_Donor_Details_For_Fund_Finance_Role()
        {
            username = RandomString();
            password = RandomString();
            var role = new CmsData.Role
            {
                RoleName = RandomString(5),                
            };
            db.Roles.InsertOnSubmit(role);
            db.SubmitChanges();
            var fund = new CmsData.ContributionFund
            {
                FundId = RandomNumber(500,1000),
                FundName = RandomString(5),
                FundManagerRoleId = role.RoleId,
                CreatedDate = DateTime.Now,
                CreatedBy = 1
            };
            db.ContributionFunds.InsertOnSubmit(fund);
            db.SubmitChanges();
            CreateUser(username, password, roles: new string[] { "Access", "FinanceViewOnly", "FundManager", role.RoleName });
            Login();
            Wait(3);
            Open($"{rootUrl}FinanceReports/TotalsByFund");
            Find(xpath: "//button[contains(.,' Other Reports ')]").Click();
            var donorDetail = Find(id: "exportdonordetails");
            donorDetail.ShouldNotBeNull();
            var donorFundTotals = Find(id: "exportdonorfundtotals");
            donorFundTotals.ShouldNotBeNull();
            var donorTotals = Find(id: "exportdonortotals");
            donorTotals.ShouldNotBeNull();
        }
    }
}
