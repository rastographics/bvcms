using Xunit;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSWebTests.Support;
using CMSWebTests;
using SharedTestFixtures;
using CmsWeb.Membership;
using Shouldly;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2Tests
{
    public class MobileAuthenticationTests : ControllerTestBase
    {
        [Theory]
        [InlineData("NormalPassword")]
        [InlineData("Wei:rd Password1")]
        [InlineData("!@#$%^&*()_+-=~`{[}]|\\:;\"'<,>.?/")]
        public void authenticateTest(string password)
        {
            var username = RandomString();
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider();
            membershipProvider.ValidUsers.Add(new CmsData.User { Username = username, Password = password });
            var roleProvider = new MockCMSRoleProvider();

            CreateUser(username, password);
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);

            var mobileAuth = new MobileAuthentication(db);
            mobileAuth.authenticate("");

            mobileAuth.hasError().ShouldBeFalse();
        }
    }
}
