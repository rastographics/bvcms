using CmsData;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace IntegrationTests.Areas.Figures.Views.Figures
{
    [Collection(Collections.Webapp)]
    public class RefineFundViewTests : AccountTestBase
    {
        [Fact, FeatureTest]
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
            MaximizeWindow();
            WaitForElement("div:nth-child(1) > .btn", 5);

            Find(css: "div:nth-child(1) > .btn").Click();
            WaitForElement("#year", 5);

            var YearDropdown = Find(id: "year");
            YearDropdown.SendKeys(YearToTest);

            RepeatUntil(() => Find(id: "DrawChart").Click(),
                condition: () => Find(css: "#Fund_chart_display svg > g:nth-child(5)") != null);
            PageSource.ShouldContain(YearToTest);
        }
    }
}
