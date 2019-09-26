using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Manage
{
    [Collection(Collections.Webapp)]
    public class GeneralTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void Page_NotFound_Test()
        {
            username = RandomString();
            password = RandomString();
            
            var user = CreateUser();

            Login();

            Open($"{rootUrl}Invalid");

            PageSource.ShouldContain("Not Found");
        }

        [Fact, FeatureTest]
        public void Page_NotAuthorized_Test()
        {
            username = RandomString();
            password = RandomString();
            
            var user = CreateUser();

            Login();

            Open($"{rootUrl}Bundles");

            PageSource.ShouldContain("Sign In");
        }
    }
}
