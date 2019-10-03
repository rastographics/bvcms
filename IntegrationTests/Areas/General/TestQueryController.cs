using IntegrationTests.Support;
using Shouldly;
using Xunit;
using SharedTestFixtures;

namespace IntegrationTests.Areas.Manage
{
    [Collection(Collections.Webapp)]
    public class TestQueryController : AccountTestBase
    {
        [Fact]
        public void TestSearchBuilderOrgsDropdownOption()
        {
            const string finddivision = "input[type=radio][value$='First Division']";
            const string findorg = "input[type=radio][value$='Online Giving']";
            const string settingname = "ShowAllOrgsByDefaultInSearchBuilder";

            LoginAsAdmin();

            Open($"{rootUrl}SetSettingForLocalhost/{settingname}/DELETE");
            WaitForPageLoad();
            DisplayOrgDropdowns();
            IsElementPresent(finddivision).ShouldBeTrue();
            IsElementPresent(findorg).ShouldBeTrue();

            Open($"{rootUrl}SetSettingForLocalhost/{settingname}/false");
            WaitForPageLoad();
            DisplayOrgDropdowns();
            IsElementPresent(finddivision).ShouldBeFalse();
            IsElementPresent(findorg).ShouldBeFalse();
        }

        private void DisplayOrgDropdowns()
        {
            Open($"{rootUrl}QueryCode?code=IsMemberOf()=1");
            WaitForElementToDisappear(loadingUI);
            Find(css: "li.condition a[title^=IsMemberOf]").Click();
            WaitForElement("input[type=radio][value='0']", visible: false);
        }
        
        /// <summary>
        /// This verifies that when QueryModel m is bound in POST operation
        /// that it no longer needs to have m.Db = CurrentDatabase
        /// as this is now done in SmartBinder with an inherited class called DbBinder
        /// This is applicable wherever this pattern is found on any model binding that needs CurrentDatabase
        /// The POST ActionResult exercised is QueryController.Results(QueryModel m)
        /// </summary>
        [Fact, FeatureTest]
        public void DbInQueryModelShouldBeInstantiatedByModelBinding()
        {
            username = RandomString();
            password = RandomString();
            CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin" });
            Login();
            Open($"{rootUrl}QueryCode?code=Age>65");
            WaitForElement("input[id=totcnt]");
            PageSource.ShouldContain("David Carroll");
        }
    }
}
