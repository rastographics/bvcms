using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;
using CmsData;
using UtilityExtensions;
using System.Linq;
using OpenQA.Selenium.Support.UI;

namespace IntegrationTests.Areas.Figures.Views.Figures
{
    [Collection(Collections.Webapp)]
    public class RefineFundViewTests : AccountTestBase
    {
        [Fact]
        public void Should_Change_Years_In_Graph()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            List<int?> Years = CMSDataContext.Create(Util.Host).Contributions
                   .Where(x => (x.ContributionDate.HasValue ? ((DateTime)x.ContributionDate).Year : (int?)null) < DateTime.Now.Year)
                   .ToList()
                   .Select(x => x.ContributionDate.HasValue ? ((DateTime)x.ContributionDate).Year : (int?)null)
                   .Distinct()
                   .OrderByDescending(x => x)
                   .ToList();

            string YearToTest;

            if (Years.Count == 0)
            {
                YearToTest = DateTime.Now.Year.ToString();
            }
            else
            {
                YearToTest = Years[RandomNumber(0, Years.Count - 1)].ToString();
            }

            Open($"{rootUrl}Figures/Figures/Index");
            WaitForElement("div:nth-child(1) > .btn", 5);

            Find(css: "div:nth-child(1) > .btn").Click();
            WaitForElement("#year", 5);

            var YearDropdown = Find(id: "year");
            YearDropdown.SendKeys(YearToTest);

            Find(id: "DrawChart").Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            wait.Until(d => IsAlertPresent());
            driver.SwitchTo().Alert().Dismiss();

            WaitForElement("#Fund_chart_display svg > g:nth-child(4)", 5);
            driver.PageSource.ShouldContain(YearToTest);
        }
    }
}
