using IntegrationTests.Support;
using Shouldly;
using System.Drawing;
using Xunit;
using SharedTestFixtures;
using System.Linq;
using OpenQA.Selenium;

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
            donorFundTotals.ShouldBeNull();
        }
    }
}
