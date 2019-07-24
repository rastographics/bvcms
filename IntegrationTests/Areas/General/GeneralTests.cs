using IntegrationTests.Support;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Manage
{
    [Collection("WebApp Collection")]
    public class GeneralTests : AccountTestBase
    {
        [Fact]
        public void Page_NotFound_Test()
        {
            username = RandomString();
            password = RandomString();
            
            var user = CreateUser();

            Login();

            Open($"{rootUrl}Invalid");

            PageSource.ShouldContain("Not Found");
        }

        [Fact]
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
