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
            WaitForElementToDissappear("div.blockUI.blockOverlay");
            Find(css: "li.condition a[title^=IsMemberOf]").Click();
            WaitForElement("input[type=radio][value='0']", visible: false);
        }
    }
}
