using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmsWeb.Areas.Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SharedTestFixtures;
using CMSWebTests.Support;
using CMSWebTests;
using CmsWeb.MobileAPI;
using Shouldly;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Web.Routing;
using CmsWeb.Membership;
using CmsWeb.Lifecycle;

namespace CmsWeb.Areas.Public.ControllersTests
{
    [Collection(Collections.Database)]
    public class MobileAPIControllerTests : ControllerTestBase
    {
        [Fact]
        public void FetchPeopleTest()
        {
            var requestManager = SetupRequestManager();
            var controller = new MobileAPIController(requestManager);
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext(requestManager.CurrentHttpContext, routeData, controller);
            var message = new BaseMessage
            {
                data = JsonConvert.SerializeObject(new MobilePostSearch { }),
                device = BaseMessage.API_DEVICE_ANDROID,
                version = BaseMessage.API_VERSION_2,
            };
            var data = message.ToString();
            var result = controller.FetchPeople(data) as BaseMessage;
            result.ShouldNotBeNull();
            result.error.ShouldBe(0);
            var people = JsonConvert.DeserializeObject<Dictionary<int, MobilePerson>>(result.data);
            people.Count.ShouldBeGreaterThan(0);
        }

        private IRequestManager SetupRequestManager()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            return requestManager;
        }
    }
}
