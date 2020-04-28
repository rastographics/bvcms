using Xunit;
using CmsWeb.Areas.Public.Controllers;
using SharedTestFixtures;
using CMSWebTests.Support;
using CmsWeb.Membership;
using CMSWebTests;

namespace CmsWeb.Areas.Public.ControllersTests
{
    [Collection(Collections.Database)]
    public class ExternalServicesControllerTests : ControllerTestBase
    {
        [Theory]
        [InlineData("NormalPassword")]
        [InlineData("Wei:rd Password1")]
        [InlineData("!@#$%^&*()_+-=~`{[}]|\\:;\"'<,>.?/")]
        public void ApiUserInfoTest(string password)
        {
            var username = RandomString();
            var apiKey = RandomString();
            var IP = "127.0.0.1";
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider();
            membershipProvider.ValidUsers.Add(new CmsData.User { Username = username, Password = password });
            var roleProvider = new MockCMSRoleProvider();
            db.SetSetting("ApiUserInfoKey", apiKey);
            db.SetSetting("ApiUserInfoIPList", IP);
            
            CreateUser(username, password);
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            requestManager.CurrentHttpContext.Request.Headers["ApiKey"] = apiKey;
            requestManager.CurrentHttpContext.Request.ServerVariables["REMOTE_ADDR"] = IP;
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);

            var controller = new ExternalServicesController(requestManager);
            controller.ApiUserInfo();
        }
    }
}
