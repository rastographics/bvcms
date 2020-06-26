using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Org.Views.Org
{
    [Collection(Collections.Webapp)]
    public class OrgSearchTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void Should_Get_Online_Giving_From_Org_Search()
        {
            LoginAsAdmin();
            Wait(5);
            Open($"{rootUrl}OrgSearch/");
            Wait(5);
            Find(id: "Name").SendKeys("Online Giving");
            Find(id: "search").Click();
            Wait(5);
            var orgName = Find(xpath: "//*[@id='resultsTable']/tbody/tr/td[2]/a").Text;
            orgName.ShouldBe("Online Giving");
        }
    }
}
