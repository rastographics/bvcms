using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
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

            Open($"{rootUrl}NewQuery");

            WaitForElement(".input-group-lg > .searchConditions");
            var InputSearchCondition = Find(css: ".input-group-lg > .searchConditions");
            InputSearchCondition.Clear();
            InputSearchCondition.SendKeys("IsTop");

            WaitForElement("#IsTopGiver");
            Find(id: "IsTopGiver").Click();
            WaitForElement("#FundIds");
            
            var FundIds = Find(id: "FundIds");
            FundIds.ShouldNotBeNull();
        }
    }
}
