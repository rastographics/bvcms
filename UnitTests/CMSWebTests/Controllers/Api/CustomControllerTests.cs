using CmsWeb.Code;
using Xunit;
using UtilityExtensions;
using Shouldly;
using SharedTestFixtures;
using CMSWebTests.Support;
using CmsWeb.Controllers.Api;
using System.Linq;
using CmsWeb.Membership;
using System.Net.Http;

namespace CmsWeb.Controllers.ApiTests
{
    [Collection(Collections.Database)]
    public class CustomControllerTests : ControllerTestBase
    {
        [Theory]
        [InlineData("Developer", true)]
        [InlineData("APIWrite", false)]
        public void GetTest(string extraRole, bool valid)
        {
            var username = RandomString();
            var password = RandomString();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            var db = DatabaseFixture.NewDbContext();
            var context = ContextTestUtils.CreateMockHttpContext();
            roleProvider.UserRoles.AddRange(new[] { "APIOnly", extraRole });
            db.WriteContentSql("GetTest", "--API\r\nSELECT TOP 10 * FROM dbo.People");
            CreateUser(username, password, roles: new[] { "Access" });
            context.Headers["Authorization"] = BasicAuthenticationString(username, password);
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);

            var authenticated = AuthHelper.AuthenticateDeveloper(HttpContextFactory.Current, additionalRole: "APIOnly").IsAuthenticated;

            authenticated.ShouldBe(valid);
            if (valid)
            {
                var controller = new CustomController(new Lifecycle.RequestManager())
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/CustomAPI/GetTest")
                };
                var results = controller.Get("GetTest");

                results.Count().ShouldBeGreaterThan(0);
                (results.First().PeopleId as int?).ShouldNotBeNull();
            }
        }
    }
}
