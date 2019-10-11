using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Areas.Search.Views.Query
{
    [Collection(Collections.Webapp)]
    public class EditConditionTests: AccountTestBase
    {
        [Fact]
        public void FundIds_Should_Appear_In_TopGiver_Search_Condition()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin" });
            Login();

            MaximizeWindow();

            Find(xpath: "//a[contains(text(),'People')]").Click();
            WaitForElement(".open .col-sm-6:nth-child(1) li:nth-child(5) > a", 5);
            Find(css: ".open .col-sm-6:nth-child(1) li:nth-child(5) > a").Click();

            WaitForElement(".input-group-lg > .searchConditions", 5);
            var InputSearchCondition = Find(css: ".input-group-lg > .searchConditions");
            InputSearchCondition.Clear();
            InputSearchCondition.SendKeys("IsTop");

            WaitForElement("#IsTopGiver", 5);
            Find(id: "IsTopGiver").Click();
            WaitForElement("#FundIds", 5);
            var FundIds = Find(id: "FundIds");
            FundIds.ShouldNotBeNull();
        }
    }
}
