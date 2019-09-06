using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace IntegrationTests.Areas.Figures.Views.Figures
{
    [Collection(Collections.Webapp)]
    public class RefineFundViewTests : AccountTestBase
    {
        [Fact]
        public void Shoul_Change_Years_In_Graph()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            string YearToTest = (DateTime.Now.Year - RandomNumber(1, 4)).ToString();

            Open($"{rootUrl}Figures/Figures/Index");
            WaitForElement("div:nth-child(1) > .btn", 5);

            Find(css: "div:nth-child(1) > .btn").Click();
            WaitForElement("#year", 5);

            var YearDropdown = Find(id: "year");
            YearDropdown.SendKeys(YearToTest);

            Find(id: "DrawChart").Click();

            driver.SwitchTo().Alert().Dismiss();

            WaitForElement("#Fund_chart_display svg > g:nth-child(4)", 5);
            driver.PageSource.ShouldContain(YearToTest);
        }
    }
}
